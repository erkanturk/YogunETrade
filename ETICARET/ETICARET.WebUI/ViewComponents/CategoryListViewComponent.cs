using ETICARET.Business.Abstract;      // ICategoryService interface'i için
using ETICARET.WebUI.Models;          // CategoryListViewModel için
using Microsoft.AspNetCore.Mvc;       // ViewComponent base class için

namespace ETICARET.WebUI.ViewComponents
{
    /// <summary>
    /// Kategori listesini görüntüleyen ViewComponent
    /// ViewComponent'ler partial view'lar gibi çalışır ancak daha güçlü mantık içerebilir
    /// Layout'ta veya view'larda @await Component.InvokeAsync("CategoryList") ile çağrılır
    /// </summary>
    public class CategoryListViewComponent : ViewComponent
    {
        // Dependency Injection ile kategori servisini al
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor - Dependency Injection Container tarafından çağrılır
        /// </summary>
        /// <param name="categoryService">Kategori işlemleri için servis interface'i</param>
        public CategoryListViewComponent(ICategoryService categoryService)
        {
            // Readonly field'ı constructor'da initialize et
            _categoryService = categoryService;
        }

        /// <summary>
        /// ViewComponent'in ana metodu - çağrıldığında otomatik olarak çalışır
        /// Kategori listesi ve seçili kategori bilgisini hazırlar
        /// </summary>
        /// <returns>View ile birlikte model döndürür</returns>
        public IViewComponentResult Invoke()
        {
            // View'a gönderilecek model oluştur
            var model = new CategoryListViewModel()
            {
                // RouteData'dan mevcut kategoriyi al
                // URL'de /products/elektronik gibi bir route varsa "elektronik" değerini alır
                // Null-conditional operator (?.) ve null-coalescing ile güvenli erişim
                SelectedCategory = RouteData.Values["category"]?.ToString(),

                // Tüm kategorileri servis katmanından al
                // Business layer'dan gelen kategori listesi
                Categories = _categoryService.GetAll()
            };

            // Default view'ı (CategoryList.cshtml) model ile birlikte döndür
            // Views/Shared/Components/CategoryList/Default.cshtml dosyasını arar
            return View(model);
        }
    }
}
