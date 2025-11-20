using ETICARET.Business.Abstract;
using ETICARET.WebUI.EmailService;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ETICARET.WebUI.Controllers
{
    public class AccountController : Controller
    {
        // Kullanıcı yönetimi işlemleri için UserManager
        private UserManager<ApplicationUser> _userManager;

        // Oturum işlemleri (login, logout vs.) için SignInManager
        private SignInManager<ApplicationUser> _signInManager;

        // Sepet servisinin kullanılması için
        private ICartService _cartService;

        // Constructor içerisinde servislerin ve yöneticilerin dependency injection ile alınması
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        /// <summary>
        /// Kayıt olma (Register) formunu görüntüleyen GET metodudur.
        /// </summary>
        /// <returns>Register sayfasını döndürür.</returns>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Kayıt olma formundan gelen bilgileri işleyen POST metodudur.
        /// Yeni kullanıcı oluşturulur ve email onayı için mail gönderilir.
        /// </summary>
        /// <param name="model">Kayıt formundan gelen RegisterModel</param>
        /// <returns>Eğer kayıt başarılıysa Login sayfasına yönlendirir, aksi halde tekrar Register sayfasını gösterir.</returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            // Model validasyonu (örneğin required alanlar dolu mu vs.)
            if (!ModelState.IsValid)
            {
                // Model valid değilse form tekrar gösterilir
                return View(model);
            }

            // Yeni bir kullanıcı nesnesi oluşturulur
            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };

            // UserManager aracılığıyla kullanıcı oluşturulur
            var result = await _userManager.CreateAsync(user, model.Password);

            // Eğer kullanıcı oluşturma başarılıysa
            if (result.Succeeded)
            {
                // Kullanıcıya email onayı için token üretilir
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Email onaylaması için callback URL'si oluşturulur
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });

                // Site adresi (lokalhost üstünden)
                string siteUrl = "https://localhost:5174";

                // Tıklanacak linki oluşturur
                string activeUrl = $"{siteUrl}{callbackUrl}";

                // Gönderilecek e-postanın içeriği
                string body = $"Hesabınızı onaylayınız. <br> <br> Lütfen email hesabını onaylamak için linke <a href='{activeUrl}'> tıklayınız.</a>";

                // Email gönderme işlemi
                MailHelper.SendMail(body, model.Email, "ETİCARET Hesap Aktifleştirme Onayı");

                // Başarılıysa Login sayfasına yönlendirir
                return RedirectToAction("Login", "Account");
            }

            // Eğer kullanıcı oluşturma başarısızsa, aynı sayfayı gösterir
            return View(model);
        }

        /// <summary>
        /// Email onayı için tıklanan link aracılığıyla gelen isteği karşılar.
        /// </summary>
        /// <param name="userId">Onaylanacak kullanıcının Id'si</param>
        /// <param name="token">Email onayı için üretilen token</param>
        /// <returns>Kullanıcı onaylandıysa Login sayfasına yönlendirir, aksi takdirde anasayfaya yönlendirir.</returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            // Gerekli parametreler gelmediyse
            if (userId == null || token == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Geçersiz Token",
                    Message = "Hesap onay bilgileri yanlış",
                    Css = "danger"
                });

                // Anasayfaya yönlendirir
                return Redirect("~");
            }

            // UserManager aracılığıyla kullanıcı bulunur
            var user = await _userManager.FindByIdAsync(userId);

            // Kullanıcı varsa
            if (user != null)
            {
                // Email onayı gerçekleştirilir
                var result = await _userManager.ConfirmEmailAsync(user, token);

                // Email onayı başarılıysa
                if (result.Succeeded)
                {
                    // Kullanıcı için sepet başlatılır
                    _cartService.InitialCart(userId);

                    // Bilgi mesajı kullanıcıya verilir
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Hesap Onayı",
                        Message = "Hesabınız onaylanmıştır",
                        Css = "success"
                    });

                    // Kullanıcı Login sayfasına yönlendirilir
                    return RedirectToAction("Login", "Account");
                }
            }

            // Buraya geldiyse email onayı başarısız demektir
            TempData.Put("message", new ResultModel()
            {
                Title = "Hesap Onayı",
                Message = "Hesabınız onaylanmamıştır",
                Css = "danger"
            });

            // Anasayfaya yönlendirir
            return Redirect("~");
        }

        /// <summary>
        /// Login formunu döndürür. returnUrl, başarılı giriş sonrası yönlendirme yapılacak sayfadır.
        /// </summary>
        /// <param name="returnUrl">Giriş sonrası yönlendirme adresi</param>
        /// <returns>Login sayfası</returns>
        public IActionResult Login(string returnUrl = null)
        {
            return View(
                new LoginModel()
                {
                    ReturnUrl = returnUrl
                }
            );
        }

        /// <summary>
        /// Login formundan gelen bilgileri işleyen POST metodudur.
        /// </summary>
        /// <param name="model">Kullanıcının girdiği email ve şifre bilgilerini barındırır</param>
        /// <returns>Başarılı giriş yapıldıysa ilgili sayfaya yönlendirir, aksi halde Login sayfasını tekrar gösterir</returns>
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            // ReturnUrl'u kontrol etmek veya valid etmek istemediğimiz için ModelState üzerinden kaldırıyoruz
            ModelState.Remove("ReturnUrl");

            // Model validasyonu (örneğin required alanlar)
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Giriş Bilgileri",
                    Message = "Bilgileriniz Hatalıdır",
                    Css = "danger"
                });

                return View(model);
            }

            // Email adresine göre kullanıcı bulunur
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Eğer böyle bir kullanıcı yoksa
            if (user is null)
            {
                ModelState.AddModelError("", "Bu email adresi ile kayıtlı kullanıcı bulunamadı");
                return View(model);
            }

            // Parola ve kullanıcı bilgileri ile oturum açılır
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);

            // Eğer başarılıysa
            if (result.Succeeded)
            {
                // Eğer ReturnUrl varsa oraya yönlendir, yoksa anasayfaya yönlendir
                return Redirect(model.ReturnUrl ?? "~/");
            }
            // Kullanıcı hesabı kilitlenmişse
            if (result.IsLockedOut)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Kilitlendi",
                    Message = "Hesabınız geçici olarak kilitlenmiştir. Lütfen biraz sonra tekrar deneyin.",
                    Css = "danger"
                });
                return View(model);
            }

            // Buraya gelindiyse email veya şifre hatalı demektir
            ModelState.AddModelError("", "Email veya şifre hatalı");

            return View(model);
        }

        /// <summary>
        /// Oturumu sonlandırır (Logout) ve kullanıcıyı anasayfaya yönlendirir
        /// </summary>
        /// <returns>Anasayfaya yönlendirir</returns>
        public async Task<IActionResult> Logout()
        {
            // SignInManager aracılığıyla oturumu sonlandırır
            await _signInManager.SignOutAsync();

            // Kullanıcıya bilgi mesajı verilir
            TempData.Put("message", new ResultModel()
            {
                Title = "Oturum Hesabı Kapatıldı",
                Message = "Hesabınız güvenli bir şekilde sonlandırıldı",
                Css = "success"
            });

            // Anasayfaya yönlendirir
            return Redirect("~/");
        }

        /// <summary>
        /// Şifremi Unuttum sayfasını döndürür.
        /// </summary>
        /// <returns>ForgotPassword view</returns>
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Şifremi unuttum işleminde email girilir ve o emaile şifre sıfırlama linki gönderilir.
        /// </summary>
        /// <param name="email">Şifresi unutulan kullanıcının emaili</param>
        /// <returns>Login sayfasına yönlendirir veya hata durumunda ForgotPassword sayfasını gösterir</returns>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // Email boş gelmişse uyarı verilir
            if (string.IsNullOrEmpty(email))
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Lütfen Email adresini boş bırakmayınız",
                    Css = "danger"
                });
                return View();
            }

            // Email adresi ile kullanıcı bulunur
            var user = await _userManager.FindByEmailAsync(email);

            // Kullanıcı yoksa uyarı verilir
            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Bu Email adresiyle bir kullanıcı bulunamadı",
                    Css = "danger"
                });

                return View();
            }

            // Şifre yenileme token üretilir
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Callback URL'si oluşturulur
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token = code
            });

            string siteUrl = "https://localhost:5174";
            string activeUrl = $"{siteUrl}{callbackUrl}";

            // Gönderilecek e-postanın içeriği
            string body = $"Parolanızı yenilemek için linke <a href='{activeUrl}'> tıklayınız.</a>";

            // Email gönderme işlemi
            MailHelper.SendMail(body, email, "ETİCARET Parola Yenileme");

            // Kullanıcıya bilgi mesajı
            TempData.Put("message", new ResultModel()
            {
                Title = "Şifremi Unuttum",
                Message = "Email adresinize şifre yenileme bağlantısı gönderilmiştir.",
                Css = "success"
            });

            // İşlem sonrası Login sayfasına yönlendirir
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Şifre yenileme sayfasını döndürür.
        /// </summary>
        /// <param name="token">Şifre yenileme için gerekli token</param>
        /// <returns>ResetPassword view</returns>
        public IActionResult ResetPassword(string token)
        {
            // Token kontrolü yapılır
            if (token == null)
            {
                return RedirectToAction("Home", "Index");
            }

            // ResetPasswordModel'e token eklenir
            var model = new ResetPasswordModel { Token = token };

            return View(model);
        }

        /// <summary>
        /// Şifre yenileme formundan gelen POST isteğidir.
        /// </summary>
        /// <param name="model">Email, Token ve Yeni Şifre bilgilerini içerir</param>
        /// <returns>Eğer başarılı olursa Login sayfasına, aksi takdirde tekrar ResetPassword sayfasına yönlendirir.</returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            // Model validasyonu
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kullanıcı email aracılığıyla bulunur
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Kullanıcı yoksa hata mesajıyla anasayfaya yönlendirir
            if (user is null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Bu Email adresi ile kullanıcı bulunamadı.",
                    Css = "danger"
                });
                return RedirectToAction("Home", "Index");
            }

            // Kullanıcının şifresi resetlenir
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            // Şifre yenileme başarılıysa Login sayfasına yönlendirilir
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            else
            {
                // Hata mesajıyla tekrar ResetPassword sayfasına dönülür
                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifremi Unuttum",
                    Message = "Şifreniz uygun değildir.",
                    Css = "danger"
                });

                return View(model);
            }
        }

        /// <summary>
        /// Kullanıcının hesap yönetimi sayfasını döndürür (profil bilgileri vs.).
        /// </summary>
        /// <returns>Manage view</returns>
        public async Task<IActionResult> Manage()
        {
            // Şu anki oturumdaki kullanıcıyı alır
            var user = await _userManager.GetUserAsync(User);

            // Eğer kullanıcı yoksa mesaj gösterir
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Bağlantı Hatası",
                    Message = "Kullanıcı bilgileri bulunamadı tekrar deneyin.",
                    Css = "danger"
                });
                return View();
            }

            // Mevcut kullanıcı bilgilerini AccountModel'e dolduruyoruz
            var model = new AccountModel
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email
            };

            // Sayfaya model ile döndürür
            return View(model);
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelleme formundan gelen POST isteğidir.
        /// </summary>
        /// <param name="model">Kullanıcı tarafından girilen yeni bilgileri içerir</param>
        /// <returns>Güncelleme başarılıysa anasayfaya yönlendirir, aksi halde tekrar Manage sayfasına döndürür.</returns>
        [HttpPost]
        public async Task<IActionResult> Manage(AccountModel model)
        {
            // Model validasyonu
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Giriş Bilgileri",
                    Message = "Bilgileriniz Hatalıdır",
                    Css = "danger"
                });

                return View(model);
            }

            // Şu anki oturumdaki kullanıcıyı alır
            var user = await _userManager.GetUserAsync(User);

            // Kullanıcı yoksa Login sayfasına yönlendirilir
            if (user == null)
            {
                TempData["message"] = new ResultModel()
                {
                    Title = "Bağlantı Hatası",
                    Message = "Kullanıcı bilgileri bulunamadı, lütfen tekrar deneyin.",
                    Css = "danger"
                };
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcı bilgilerini güncelle
            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            // Eğer email değişmişse, yeni email adresine şifre sıfırlama bağlantısı gönderilir
            if (model.Email != user.Email)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                string siteUrl = "https://localhost:5174";
                string resetUrl = $"{siteUrl}{callbackUrl}";

                // Kullanıcıya email gönderilir
                string body = $"Şifrenizi yenilemek için linke <a href='{resetUrl}'> tıklayınız.</a>";
                MailHelper.SendMail(body, model.Email, "ETRADE Şifre Sıfırlama");

                TempData.Put("message", new ResultModel()
                {
                    Title = "Şifre Sıfırlama",
                    Message = "Şifre sıfırlama linki email adresinize gönderilmiştir.",
                    Css = "success"
                });

                // Sonrasında Login sayfasına yönlendirir
                return RedirectToAction("Login");
            }

            // Kullanıcı bilgileri (email değişmemişse) direkt güncellenir
            var result = await _userManager.UpdateAsync(user);

            // Güncelleme başarılıysa
            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesap Bilgileri Güncellendi",
                    Message = "Bilgileriniz başarıyla güncellenmiştir.",
                    Css = "success"
                });
                return RedirectToAction("Index", "Home");
            }

            // Güncelleme başarısızsa
            TempData.Put("message", new ResultModel()
            {
                Title = "Hata",
                Message = "Bilgileriniz güncellenemedi. Lütfen tekrar deneyin.",
                Css = "danger"
            });
            return View(model);
        }
    }
}
