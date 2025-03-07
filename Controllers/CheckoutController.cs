using Microsoft.AspNetCore.Mvc;
using Shop_HTH.Models;
using Shop_HTH.Repository;
using System.Security.Claims;

namespace Shop_HTH.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly DataContext _dataContext;
		public IActionResult Index()
		{
			return View();
		}
		public CheckoutController(DataContext context)
		{
			_dataContext = context;
		}
		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			var ordercode = Guid.NewGuid().ToString();
			var orderItem = new OrderModel();
			orderItem.OrderCode = ordercode;
			orderItem.UserName = userEmail;
			orderItem.Status = 1;
			orderItem.CreatedDate = DateTime.Now;
			_dataContext.Add(orderItem);
			_dataContext.SaveChanges();
			List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			foreach(var cart in cartItems)
			{
				var orderdetails = new OrderDetails();
				orderdetails.UserName = userEmail;
				orderdetails.OrderCode = ordercode;
				orderdetails.ProductId = (int)cart.ProductId;
				orderdetails.Price = cart.Price;
				orderdetails.Quantity = cart.Quantity;
				_dataContext.Add(orderdetails);
				_dataContext.SaveChanges();
			}
			HttpContext.Session.Remove("Cart");
			TempData["Success"] = "Check out thanh cong";
			return RedirectToAction("Index","Cart");
		}
	}
}
