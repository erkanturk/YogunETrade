using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class CommentController : Controller
    {
        // Kullanıcı yönetimi işlemleri için UserManager
        private UserManager<ApplicationUser> _userManager;

        // Yorumlarla ilgili veritabanı işlemleri için servis
        private ICommentService _commentService;

        // Ürünlerle ilgili işlemler için servis
        private IProductService _productService;

        /// <summary>
        /// Constructor ile servisleri ve UserManager'ı dependency injection ile alır.
        /// </summary>
        /// <param name="userManager">Kullanıcı yönetimi</param>
        /// <param name="productService">Ürün servisi</param>
        /// <param name="commentService">Yorum servisi</param>
        public CommentController(UserManager<ApplicationUser> userManager, IProductService productService, ICommentService commentService)
        {
            _userManager = userManager;
            _productService = productService;
            _commentService = commentService;
        }

        /// <summary>
        /// Belirtilen ürünün yorumlarını getirir ve "_PartialComments" kısmı görünümünde döndürür.
        /// </summary>
        /// <param name="id">Ürünün ID değeri</param>
        /// <returns>Ürüne ait yorumların partial view'ı</returns>
        public IActionResult ShowProductComments(int? id)
        {
            // Ürün ID null ise hata döndür
            if (id == null)
            {
                return BadRequest();
            }

            // İlgili ürünü detaylarıyla al
            Product product = _productService.GetProductDetail(id.Value);

            // Ürün bulunamazsa 404 döndür
            if (product == null)
            {
                return NotFound();
            }

            // Yorum yapan kullanıcıların UserName bilgilerini almak için sözlük oluşturur
            var users = new Dictionary<string, string>();
            foreach (var comment in product.Comments)
            {
                // Eğer bu kullanıcının bilgileri sözlükte yoksa, veritabanından çek
                if (!users.ContainsKey(comment.UserId))
                {
                    var user = _userManager.FindByIdAsync(comment.UserId).Result;
                    users[comment.UserId] = user?.UserName;
                }
            }

            // ViewBag'e kullanıcıların verileri konulur
            ViewBag.Usernames = users;

            // Ürüne ait yorumları "_PartialComments" partial view'ına gönderir
            return PartialView("_PartialComments", product.Comments);
        }

        /// <summary>
        /// Yeni yorum oluşturur (POST metodu).
        /// </summary>
        /// <param name="model">Yorum model bilgileri (metin vs.)</param>
        /// <param name="productId">Yorumun ekleneceği ürünün ID'si</param>
        /// <returns>Eğer başarılı ise JSON sonucu true döndürür.</returns>
        public IActionResult Create(CommentModel model, int? productId)
        {
            // ModelState üzerinden "UserId" alanını kaldırıyoruz (çünkü bu alan burada set ediliyor)
            ModelState.Remove("UserId");

            // Model validasyonu (örneğin Text alanı dolu mu vs.)
            if (ModelState.IsValid)
            {
                // Ürün ID'si yoksa 400 döndür
                if (productId is null)
                {
                    return BadRequest();
                }

                // Verilen ID'ye sahip ürünü alır
                Product product = _productService.GetById(productId.Value);

                // Ürün bulunamazsa 404 döndür
                if (product is null)
                {
                    return NotFound();
                }

                // Yeni yorum nesnesi oluşturur
                Comment comment = new Comment()
                {
                    ProductId = productId.Value,
                    CreateOn = DateTime.Now,
                    // Oturum açmış kullanıcının ID'sini alır, yoksa "0" olarak geçer
                    UserId = _userManager.GetUserId(User) ?? "0",
                    // Gelen metni gereksiz boşluk ve satır sonlarından arındırır
                    Text = model.Text.Trim('\n').Trim(' ')
                };

                // Yorum veritabanına eklenir
                _commentService.Create(comment);

                // İşlem başarılı
                return Json(new { result = true });
            }

            // Model valid değilse aynı sayfaya döner
            return View(model);
        }

        /// <summary>
        /// Var olan bir yorumu düzenler (text günceller).
        /// </summary>
        /// <param name="id">Yorumun ID değeri</param>
        /// <param name="text">Yeni yorum metni</param>
        /// <returns>JSON result ile true döndürür.</returns>
        public IActionResult Edit(int? id, string text)
        {
            // Eğer ID null ise 400 döndür
            if (id is null)
            {
                return BadRequest();
            }

            // Veritabanından ilgili yorumu alır
            Comment comment = _commentService.GetById(id.Value);

            // Yorum yoksa 404 döndür
            if (comment is null)
            {
                return NotFound();
            }

            // Yorumun metnini güncelle
            comment.Text = text;
            comment.CreateOn = DateTime.Now;

            // Yorum servisi üzerinden veritabanında güncelle
            _commentService.Update(comment);

            return Json(new { result = true });
        }

        /// <summary>
        /// Yorum silme işlemi.
        /// </summary>
        /// <param name="id">Silinecek yorumun ID değeri</param>
        /// <returns>JSON result ile true döndürür.</returns>
        public IActionResult Delete(int? id)
        {
            // ID null ise 400 döndür
            if (id is null)
            {
                return BadRequest();
            }

            // İlgili yorum veritabanından çekilir
            Comment comment = _commentService.GetById(id.Value);

            // Yorum yoksa 404 döndür
            if (comment is null)
            {
                return NotFound();
            }

            // Yorum veritabanından silinir
            _commentService.Delete(comment);

            return Json(new { result = true });
        }
    }
}
