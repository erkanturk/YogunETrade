using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ETICARET.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // Ürünlerle ilgili veritabaný iþlemleri için servis
        private IProductService _productService;

        /// <summary>
        /// Constructor üzerinden IProductService dependency injection ile alýnýr.
        /// </summary>
        /// <param name="productService">Ürün servisi</param>
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Anasayfa (Index) aksiyonudur. Tüm ürünleri getirerek kullanýcýya gösterir.
        /// </summary>
        /// <returns>Ürün listesini içeren bir View döndürür.</returns>
        public IActionResult Index()
        {
            // Tüm ürünleri veritabanýndan getirme
            var products = _productService.GetAll();

            // Eðer ürün listesi boþ ya da null ise, yeni bir ürün listesi oluþturulur
            if (products == null || !products.Any())
            {
                products = new List<Product>();
            }

            // Ürünleri ProductListModel içine yerleþtirerek View'a gönderiyoruz
            return View(new ProductListModel()
            {
                Products = products
            });
        }
    }
}
