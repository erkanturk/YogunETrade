using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class ShopController : Controller
    {
        // Ürünlerle (Product) ilgili veritabanı işlemleri için servis
        private IProductService _productService;

        /// <summary>
        /// Constructor üzerinden IProductService dependency injection ile alınır.
        /// </summary>
        /// <param name="productService">Ürün servisi</param>
        public ShopController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Kategoriye göre ürün listeleme sayfası (sayfalama desteğiyle).
        /// </summary>
        /// <param name="category">Filtrelenecek kategori adı</param>
        /// <param name="page">Şu anki sayfa numarası</param>
        /// <returns>Filtrelenmiş ya da tüm ürünleri sayfalar halinde döndürür</returns>
        [Route("products/{category?}")]
        public IActionResult List(string category, int page = 1)
        {
            // Sayfalama için sayfada gösterilecek ürün sayısı
            const int pageSize = 5;

            // View model oluşturarak, sayfa bilgilerini ve ürün listesini ekliyoruz
            var products = new ProductListModel()
            {
                // Sayfalama ile ilgili bilgileri PageInfo içerisinde tutuyoruz
                PageInfo = new PageInfo()
                {
                    // Kategoriye göre toplam ürün sayısını çeker
                    TotalItems = _productService.GetCountByCategory(category),
                    ItemsPerPage = pageSize,   // Sayfada gösterilecek ürün sayısı
                    CurrentCategory = category, // Şu anki kategori
                    CurrentPage = page         // Şu anki sayfa numarası
                },
                // Kategoriye ve sayfa numarasına göre ürünleri veritabanından çeker
                Products = _productService.GetProductByCategory(category, page, pageSize)
            };

            // Oluşturulan ürün listesini View'a gönderir
            return View(products);
        }

        /// <summary>
        /// Belirli bir ürünün detay sayfası.
        /// </summary>
        /// <param name="id">Detayına bakılacak ürünün ID değeri</param>
        /// <returns>Seçilen ürünün bilgilerini, kategorilerini ve yorumlarını içerir</returns>
        public IActionResult Details(int? id)
        {
            // Ürün ID'si null gelmişse 404 döner
            if (id == null)
            {
                return NotFound();
            }

            // Ürünü detaylarıyla birlikte veritabanından alır
            Product product = _productService.GetProductDetail(id.Value);

            // Ürün bulunamazsa 404 döner
            if (product == null)
            {
                return NotFound();
            }

            // Ürünün detaylarını view'a ProductDetailsModel üzerinden gönderir
            return View(new ProductDetailsModel()
            {
                Product = product,
                // Ürünün ait olduğu kategorileri alır
                Categories = product.ProductCategories.Select(i => i.Category).ToList(),
                // Ürüne ait yorumları ekler
                Comments = product.Comments
            });
        }
    }
}
