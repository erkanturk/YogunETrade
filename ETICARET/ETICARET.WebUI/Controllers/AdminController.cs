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
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ICategoryService categoryService, IProductService productService)
        {
            _roleManager=roleManager;
            _userManager=userManager;
            _categoryService=categoryService;
            _productService=productService;
        }


        [Route("admin/products")]
        public IActionResult ProductList()
        {
            return View(
                new ProductListModel()
                {
                    Products = _productService.GetAll()
                }
                );
        }

        public IActionResult CreateProduct()
        {
            var category = _categoryService.GetAll();
            ViewBag.Category = category.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            return View(new ProductModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, List<IFormFile> files)
        {
            ModelState.Remove("SelectedCategories");
            if (ModelState.IsValid)
            {
                if (int.Parse(model.CategoryId) == -1)
                {
                    ModelState.AddModelError("", "Lütfen bir kategori seçiniz");
                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem
                    { Text = x.Name, Value = x.Id.ToString() });

                    return View(model);
                }

                var entity = new Product()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price
                };

                if (files == null || files.Count < 4)
                {
                    ModelState.AddModelError("", "Lütfen en az 4 resim yükleyin");
                    ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

                    return View(model);

                }

                foreach (var item in files)
                {
                    Image image = new Image();
                    image.ImageUrl = item.FileName;

                    entity.Images.Add(image);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", item.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                }

                entity.ProductCategories = new List<ProductCategory>()
                {
                    new ProductCategory()
                    {
                        CategoryId = int.Parse(model.CategoryId),
                        ProductId = entity.Id
                    }
                };

                _productService.Create(entity);

                return RedirectToAction("ProductList");
            }

            ViewBag.Category = _categoryService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });

            return View(model);

        }

        public IActionResult EditProduct(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = _productService.GetProductDetail(id);

            if (entity == null)
            {
                return NotFound();
            }

            var model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                Images = entity.Images,
                SelectedCategories = entity.ProductCategories.Select(i => new Category()
                {
                    Id = i.Category.Id,
                    Name = i.Category.Name
                }).ToList()
            };

            ViewBag.Categories = _categoryService.GetAll();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, List<IFormFile> files, int[] categoryIds)
        {

            var entity = _productService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.Images = model.Images;

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
            _productService.Update(entity, categoryIds);
            return RedirectToAction("ProductList");

        }

        [HttpPost]
        public IActionResult DeleteProduct(int productId)
        {
            var product = _productService.GetById(productId);
            if (product != null)
            {
                _productService.Delete(product);
            }
            return RedirectToAction("ProductList");
        }

        public IActionResult CategoryList()
        {
            var entity = _categoryService.GetAll();
            return View(
                new CategoryListModel()
                {
                    Categories = entity
                }
            );
        }

        public IActionResult EditCategory(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = _categoryService.GetByIdWithProducts(id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new CategoryModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Products = entity.ProductCategories.Select(i => i.Product).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var entity = _categoryService.GetById(model.Id);
            if (entity == null)
            {
                return NotFound();
            }
            entity.Name = model.Name;
            _categoryService.Update(entity);
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteCategory(int categoryId)
        {
            var category = _categoryService.GetById(categoryId);
            if (category != null)
            {
                _categoryService.Delete(category);
            }
            return RedirectToAction("CategoryList");
        }

        public IActionResult CreateCategory()
        {
            return View(new CategoryModel());
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            var entity = new Category()
            {
                Name = model.Name
            };
            _categoryService.Create(entity);
            return RedirectToAction("CategoryList");
        }
    }
}
