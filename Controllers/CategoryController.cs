using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_HTH.Models;
using Shop_HTH.Repository;


namespace Shop_HTH.Controllers
{
	public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;
		public CategoryController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{
			CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
			if (category == null)
				return RedirectToAction("Index");
			var productsByCategory = _dataContext.Products.Where(c => c.CategoryId == category.Id);

			return View(await productsByCategory.OrderByDescending(p => p.Id).ToListAsync());
		}
	}
}
