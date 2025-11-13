using ETICARET.Business.Abstract;
using ETICARET.WebUI.EmailService;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICartService _cartService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // genarete mail code
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                string siteUrl = "https://localhost:7220";
                string activeUrl = $"{siteUrl}{callbackUrl}";

                string body = $"Hesabınızı Onaylayınız. Hesabınızı aktifleştirmek için <a href='{activeUrl}'>tıklayınız</a>";

                // Email Service
                MailHelper.SendMail(body, user.Email, "ETICARET Hesabınızı Onaylayınız");

                return RedirectToAction("Login", "Account");

            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Geçersiz Token",
                    Message = "Hesap onay bilgileri geçersiz",
                    Css = "danger"
                });
                return Redirect("~");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token); // user email onaylama => EmailConfirmed = true
                if (result.Succeeded)
                {
                    // create cart
                    _cartService.InitialCart(user.Id);
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Hesap Onayı",
                        Message = "Hesabınız onaylanmıştır",
                        Css = "success"
                    });
                    return RedirectToAction("Login", "Account");
                }
            }

            TempData.Put("message", new ResultModel()
            {
                Title = "Hesap Onayı",
                Message = "Hesabınız onaylanmamıştır",
                Css = "danger"
            });

            return Redirect("~");
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(
                new LoginModel()
                {
                    ReturnUrl = returnUrl
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ModelState.Remove("ReturnUrl");
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Giriş Bilgileri",
                    Message = "Giriş Bilgileriniz Hatalıdır!",
                    Css = "danger"
                });

                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                ModelState.AddModelError("", "Bu mail adresiyle bir kullanıcı bulunamadı.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }

            if (result.IsLockedOut)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Kilitlendi",
                    Message = "Hesabınız geçici olarak kilitlenmişir. 5 dk sonra tekrar deneyiniz!",
                    Css = "danger"
                });
                return View(model);
            }

            ModelState.AddModelError("", "Email veya parola yanlış");
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new ResultModel()
            {
                Title = "Oturum Kapatıldı",
                Message = "Hesabınız güvenli bir şekilde sonlandırıldı",
                Css = "success"
            });
            return Redirect("~/");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Lütfen Email adresini boş bırakmayın",
                    Css = "danger"
                });
                return View();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "İlgili kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token = code
            });
            string siteUrl = "https://localhost:7220";
            string resetUrl = $"{siteUrl}{callbackUrl}";
            string body = $"Parolanızı yenilemek için <a href='{resetUrl}'>tıklayınız</a>";
            // Email Service
            MailHelper.SendMail(body, email, "ETICARET Parola Sıfırlama");
            TempData.Put("message", new ResultModel()
            {
                Title = "Parola Sıfırlama",
                Message = "Parola sıfırlama linki mail adresinize gönderilmiştir.",
                Css = "success"
            });
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Geçersiz Token",
                    Message = "Parola sıfırlama bilgileri geçersiz",
                    Css = "danger"
                });
                return Redirect("~");
            }

            var model = new ResetPasswordModel() { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Parola Sıfırlama",
                    Message = "İlgili kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Parola Sıfırlama",
                    Message = "Parolanız başarıyla sıfırlanmıştır.",
                    Css = "success"
                });
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Parola Sıfırlama",
                    Message = "Parolanız uygun değildir.",
                    Css = "danger"
                });

                return View(model);

            }

        }

        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user  == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Yönetimi",
                    Message = "Kullanıcı bulunamadı",
                    Css = "danger"
                });

                return View();
            }

            var model = new AccountModel()
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(AccountModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Yönetimi",
                    Message = "Lütfen bilgilerinizi kontrol ediniz",
                    Css = "danger"
                });
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Yönetimi",
                    Message = "Kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View(model);
            }

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Yönetimi",
                    Message = "Hesap bilgileriniz güncellenmiştir",
                    Css = "success"
                });

                return RedirectToAction("Index", "Home");
            }

            TempData.Put("message", new ResultModel()
            {
                Title = "Hesap Yönetimi",
                Message = "Hesap bilgilerinizi güncellenemedi, lütfen tekrar deneyiniz",
                Css = "danger"
            });

            return View(model);
        }
    }
}
