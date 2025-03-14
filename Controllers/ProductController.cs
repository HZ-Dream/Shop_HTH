using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_HTH.Models;
using Shop_HTH.Models.ViewModel;
using Shop_HTH.Repository;

namespace Shop_HTH.Controllers
{
    public class ProductController : Controller
    {
		private readonly DataContext _dataContext;
		public IActionResult Index()
        {
            return View();
        }

		public ProductController(DataContext context)
		{
			_dataContext = context;
		}

		public async Task<IActionResult> Details(int Id)
        {
			if (Id == null) return RedirectToAction("Index");

			var productsById = _dataContext.Products
				.Include(p => p.Ratings)
				.Where(p => p.Id == Id).FirstOrDefault();
			var relatedProducts = await _dataContext.Products
			.Where(p => p.CategoryId == productsById.CategoryId && p.Id != productsById.Id)
			.Take(3)
			.ToListAsync();
			ViewBag.RelatedProducts = relatedProducts;

			var viewModel = new ProductDetailsViewModel
			{
				ProductDetails = productsById,
			};
			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> CommentProduct(RatingModel rating)
		{
			if (ModelState.IsValid)
			{
				var ratingEntity = new RatingModel
				{
					ProductId = rating.ProductId,
					Name = rating.Name,
					Email = rating.Email,
					Comment = rating.Comment,
					Star = rating.Star

				};
				_dataContext.Ratings.Add(ratingEntity);
				await _dataContext.SaveChangesAsync();

				TempData["success"] = "Thêm đánh giá thành công";

				return Redirect(Request.Headers["Referer"]);
			}
			else
			{
				TempData["error"] = "Model có một vài thứ đang lỗi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);

				return RedirectToAction("Detail", new { id = rating.ProductId });
			}

			return Redirect(Request.Headers["Referer"]);
		}
		public async Task<IActionResult> Search(string searchTerm)
		{
			var products = await _dataContext.Products
			.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
			.ToListAsync();

			ViewBag.Keyword = searchTerm;

			return View(products);
		}
	}
}
