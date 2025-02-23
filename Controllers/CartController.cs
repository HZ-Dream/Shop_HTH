using Microsoft.AspNetCore.Mvc;

namespace Shop_HTH.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Cart/Index.cshtml");
        }

        public IActionResult Checkout()
        {
            return View("~/Views/Checkout/Index.cshtml");
        }
    }
}
