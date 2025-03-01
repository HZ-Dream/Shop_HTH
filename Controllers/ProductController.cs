using Microsoft.AspNetCore.Mvc;
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
            if (Id == null)
                return  RedirectToAction("Index");
			var productsById = _dataContext.Products.Where(c => c.Id == Id).FirstOrDefault();
			return View(productsById);
        }
    }
}
