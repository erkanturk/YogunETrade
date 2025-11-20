using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETICARET.WebUI.Controllers
{
    public class AdminController : Controller
    {
        // Ürün işlemleri (Create, Update, Delete vs.) için servis
        private IProductService _productService;

        // Kategori işlemleri (Create, Update, Delete vs.) için servis
        private ICategoryService _categoryService;

        // Kullanıcı yönetimi işlemleri için UserManager
        private UserManager<ApplicationUser> _userManager;

        // Rol yönetimi işlemleri için RoleManager
        private RoleManager<IdentityRole> _roleManager;

        // Constructor içerisinde servislerin ve yöneticilerin (manager) dependency injection ile alınması
        public AdminController(IProductService productService, ICategoryService categoryService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        /// <summary>
        /// admin/products route'una gelen istekleri karşılar.
        /// Tüm ürünleri listeler.
        /// </summary>
        /// <returns>ProductList view'ını döndürür</returns>
        [Route("admin/products")]
        public IActionResult ProductList()
        {
            // Ürünlerin tümünü alıp view modeline koyar ve ProductList view'ına gönderir
            return View(
                new ProductListModel()
                {
                    Products = _productService.GetAll()
                }
             );
        }

        /// <summary>
        /// Yeni ürün oluşturmak için form sayfası.
        /// Kategori listesini dropdown olarak gösterebilmek için ViewBag içerisine eklenir.
        /// </summary>
        /// <returns>CreateProduct view'ı</returns>
        public IActionResult CreateProduct()
        {
            // Tüm kategorileri alır
            var category = _categoryService.GetAll();

            // Kategorilerin adları ve id değerleri dropdown olarak ViewBag'e eklenir
            ViewBag.Category = category.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            // Yeni bir ProductModel nesnesi oluşturarak view'a gönderir
            return View(new ProductModel());
        }

        /// <summary>
        /// Yeni ürün oluşturmak için formdan POST isteği.
        /// Dosyalar (resimler) ile birlikte ürünü kaydeder.
        /// </summary>
        /// <param name="model">Formdan gelen ProductModel</param>
        /// <param name="files">Yüklenen resimleri tutan liste</param>
        /// <returns>Eğer başarılıysa ProductList sayfasına yönlendirir</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, List<IFormFile> files)
        {
            // ModelState üzerinden "SelectedCategories" alanını kaldırıyoruz çünkü bu alana ihtiyacımız yok
            // veya validate edilmesi gereken bir alan değil
            ModelState.Remove("SelectedCategories");

            // Model validasyonu (örneğin required alanlar dolu mu vs.)
            if (ModelState.IsValid)
            {
                // Kullanıcı kategori seçmezse (id -1 ise), hata mesajı verir
                if (int.Parse(model.CategoryId) == -1)
                {
                    ModelState.AddModelError("", "Lütfen bir kategori seçiniz.");

                    // Kategorileri tekrar yükleyip dropdown'a doldurmak için
                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                    // Hata ile birlikte sayfayı yeniden döndürür
                    return View(model);
                }

                // Ürün nesnesi oluşturulur
                var entity = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price
                };

                // Dosyalar (resimler) kontrol edilir, en az 4 tane yüklenmesi istenir
                if (files.Count < 4 || files == null)
                {
                    ModelState.AddModelError("", "Lütfen en az 4 resim yükleyin.");
                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    return View(model);
                }

                // Her bir dosyanın sunucuya kaydedilmesi ve veritabanına eklenmesi
                foreach (var item in files)
                {
                    // Resim tablosu için yeni bir Image nesnesi
                    Image image = new Image();
                    image.ImageUrl = item.FileName;  // Resmin yolunu kaydediyoruz

                    // Ürünün resim listesine ekleniyor
                    entity.Images.Add(image);

                    // Resmin fiziki olarak kaydedileceği dizin
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", item.FileName);

                    // Dosyayı ilgili dizine kopyalıyoruz
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                }

                // Ürünle kategori arasındaki ilişkiyi tanımlar
                entity.ProductCategories = new List<ProductCategory> { new ProductCategory { CategoryId = int.Parse(model.CategoryId), ProductId = entity.Id } };

                // Ürünü veritabanına kaydeder
                _productService.Create(entity);

                // İşlem başarılıysa ürün listesi sayfasına yönlendirir
                return RedirectToAction("ProductList");
            }

            // Eğer model valid değilse, kategorileri yeniden yükler ve formu tekrar gösterir
            ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            return View(model);
        }

        /// <summary>
        /// Ürün güncelleme formunu döndürür.
        /// </summary>
        /// <param name="id">Güncellenecek ürünün id'si</param>
        /// <returns>EditProduct view'ı</returns>
        public IActionResult EditProduct(int id)
        {
            // Eğer id null gönderilirse sayfa bulunamadı hatası döndür
            if (id == null)
            {
                return NotFound();
            }

            // İlgili id'ye sahip ürünü detaylarıyla birlikte (ilişkili kategoriler vs.) alır
            var entity = _productService.GetProductDetail(id);

            // Ürün yoksa NotFound döndür
            if (entity == null)
            {
                return NotFound();
            }

            // Edit sayfasında gösterilecek model
            var model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Images = entity.Images,
                SelectedCategories = entity.ProductCategories.Select(i => i.Category).ToList()
            };

            // Sayfada kategorileri gösterebilmek için
            ViewBag.Categories = _categoryService.GetAll();

            // EditProduct sayfasını model ile döndürür
            return View(model);
        }

        /// <summary>
        /// Ürün güncelleme işleminin POST metodudur.
        /// Yeni eklenen resimler ve kategori güncellemeleri de bu metodta yapılır.
        /// </summary>
        /// <param name="model">Güncellenen ürün bilgisi</param>
        /// <param name="files">Yüklenen yeni resimler</param>
        /// <param name="categoryIds">Yeni seçilen kategorilerin id'leri</param>
        /// <returns>Başarılı olursa ProductList sayfasına yönlendirir</returns>
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, List<IFormFile> files, int[] categoryIds)
        {
            // Veritabanından güncellenecek ürünü çekiyoruz
            var entity = _productService.GetById(model.Id);

            // Ürün yoksa NotFound döndür
            if (entity == null)
            {
                return NotFound();
            }

            // Ürün bilgilerini güncelle
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.Images = model.Images;

            // Eğer yeni resimler yüklenmişse ekler
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Image image = new Image();
                    image.ImageUrl = file.FileName;

                    entity.Images.Add(image);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            // Ürün servisindeki Update metodunda kategori id'leri de güncellenir
            _productService.Update(entity, categoryIds);

            // Güncellenmiş ürünleri liste sayfasına yönlendirir
            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Ürünü silmek için kullanılır.
        /// </summary>
        /// <param name="productId">Silinecek ürünün id'si</param>
        /// <returns>Ürün listesi sayfasına yönlendirir</returns>
        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            // Veritabanından silinecek ürünü çek
            var product = _productService.GetById(productId);

            // Ürün varsa sil
            if (product != null)
            {
                _productService.Delete(product);
            }

            // Ürün listesine geri dön
            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Tüm kategorileri listeler.
        /// </summary>
        /// <returns>CategoryList view'ını döndürür</returns>
        public IActionResult CategoryList()
        {
            // Kategori listesini alır ve CategoryListModel üzerinden View'a gönderir
            return View(new CategoryListModel() { Categories = _categoryService.GetAll() });
        }

        /// <summary>
        /// Kategori düzenlemek için form sayfasını döndürür.
        /// </summary>
        /// <param name="id">Düzenlenecek kategorinin id'si</param>
        /// <returns>EditCategory view'ı</returns>
        public IActionResult EditCategory(int? id)
        {
            // İlgili kategori ve o kategorideki ürünler çekilir
            var entity = _categoryService.GetByWithProducts(id.Value);

            // Kategori modelini doldurup view'a gönderir
            return View(
                    new CategoryModel()
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        Products = entity.ProductCategories.Select(i => i.Product).ToList()
                    }
                );
        }

        /// <summary>
        /// Kategori güncelleme işlemi.
        /// </summary>
        /// <param name="model">Düzenlenen kategori bilgisi</param>
        /// <returns>Başarılı olursa kategori listesi sayfasına yönlendirir</returns>
        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            // Mevcut kategori çekilir
            var entity = _categoryService.GetById(model.Id);

            // Kategori yoksa NotFound döndür
            if (entity == null)
            {
                return NotFound();
            }

            // Kategori bilgilerini güncelle
            entity.Name = model.Name;
            _categoryService.Update(entity);

            // Kategori listesine yönlendir
            return RedirectToAction("CategoryList");
        }

        /// <summary>
        /// Kategori silme işlemi.
        /// </summary>
        /// <param name="categoryId">Silinecek kategorinin id'si</param>
        /// <returns>Kategori listesi sayfasına yönlendirir</returns>
        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)
        {
            // Silinecek kategoriyi veritabanından çek
            var entity = _categoryService.GetById(categoryId);

            // Kategoriyi sil
            _categoryService.Delete(entity);

            // Kategori listesine dön
            return RedirectToAction("CategoryList");
        }

        /// <summary>
        /// Yeni kategori oluşturma formunu döndürür.
        /// </summary>
        /// <returns>CreateCategory view'ı</returns>
        public IActionResult CreateCategory()
        {
            // Yeni bir CategoryModel nesnesi oluşturur
            return View(new CategoryModel());
        }

        /// <summary>
        /// Yeni kategori oluşturma işlemi
        /// </summary>
        /// <param name="model">Oluşturulan kategori bilgisi</param>
        /// <returns>Kategori listesi sayfasına yönlendirir</returns>
        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            // Yeni bir Category nesnesi oluşturulur
            var entity = new Category()
            {
                Name = model.Name
            };

            // Yeni kategori veritabanına eklenir
            _categoryService.Create(entity);

            // Kategori listesine yönlendir
            return RedirectToAction("CategoryList");
        }

    }
}
