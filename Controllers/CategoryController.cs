using Microsoft.AspNetCore.Mvc;


namespace Shop_HTH.Controllers 
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
