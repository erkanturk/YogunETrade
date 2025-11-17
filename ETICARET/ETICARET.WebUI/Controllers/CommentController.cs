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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommentService _commentService;
        private readonly IProductService _productService;

        public CommentController(UserManager<ApplicationUser> userManager, ICommentService commentService, IProductService productService)
        {
            _userManager = userManager;
            _commentService = commentService;
            _productService = productService;
        }
        public IActionResult ShowProductComments(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = _productService.GetProductDetail(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            var users = new Dictionary<string, string>();

            foreach (var comment in product.Comments)
            {
                if (!users.ContainsKey(comment.UserId))
                {
                    var user = _userManager.FindByIdAsync(comment.UserId).Result;
                    users[comment.UserId] = user?.UserName;
                }
            }

            ViewBag.UserNames = users;

            return PartialView("_PartialComments", product.Comments);
        }


        public IActionResult Create(CommentModel model, int? productId)
        {
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                if (productId is null)
                {
                    return BadRequest();
                }

                Product product = _productService.GetById(productId.Value);

                if (product is null)
                {
                    return NotFound();
                }

                Comment comment = new Comment()
                {
                    ProductId = productId.Value,
                    CreateOn = DateTime.Now,
                    UserId = _userManager.GetUserId(User) ?? "0",
                    Text = model.Text.Trim('\n').Trim(' ')
                };

                _commentService.Create(comment);

                return Json(new { result = true });
            }

            return View(model);
        }

        public IActionResult Edit(int? id, string text)
        {
            if (id is null)
            {
                return BadRequest();
            }

            Comment comment = _commentService.GetById(id.Value);

            if (comment is null)
            {
                return NotFound();
            }

            comment.Text = text;
            comment.CreateOn = DateTime.Now;

            _commentService.Update(comment);

            return Json(new { result = true });
        }

        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            Comment comment = _commentService.GetById(id.Value);


            if (comment is null)
            {
                return NotFound();
            }

            _commentService.Delete(comment);

            return Json(new { result = true });
        }
    }
}
