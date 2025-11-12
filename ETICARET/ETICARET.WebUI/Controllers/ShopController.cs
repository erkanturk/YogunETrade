using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        public ShopController(IProductService productService)
        {
            _productService = productService;
        }
        [Route("products/{category?}")]

        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 5;
            var products = new ProductListModel()
            {
                Products=_productService.GetProductByCategory(category, page, pageSize),
                PageInfo=new PageInfo()
                {
                    CurrentPage=page,
                    ItemsPerPage=pageSize,
                    TotalItems=_productService.GetCountByCategory(category),
                    CurrentCategory=category
                }
            };
            return View(products);
        }
        public IActionResult Details(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetail(id.Value);
            if (product==null)
            {
                return NotFound();
            }
            return View(new ProductDetailsModel()
            {
                Product = product,
                Categories=product.ProductCategories.Select(i => i.Category).ToList(),
                Comments=product.Comments
            });

        }
    }
}
