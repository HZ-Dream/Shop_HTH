using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop_HTH.Models;
using Shop_HTH.Repository;
using Shop_HTH.Services.Vnpay;
using Shopping_HTH.Areas.Admin.Repository;
using System.Security.Claims;

namespace Shop_HTH.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private readonly IVnPayService _vnPayService;
        public IActionResult Index()
        {
            return View();
        }
        public CheckoutController(IEmailSender emailSender, DataContext context, IVnPayService vnPayService)
        {
            _dataContext = context;
            _emailSender = emailSender;
            _vnPayService = vnPayService;
        }
        public async Task<IActionResult> Checkout(string PaymentMethod, string PaymentId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;
                var coupon_code = Request.Cookies["CouponTitle"];

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                } 
                else
                {
                    shippingPrice = 0;
                }

                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;
                orderItem.ShippingCost = shippingPrice;
                orderItem.CouponCode = coupon_code;   
                orderItem.UserName = userEmail;

                if(PaymentMethod == "VnPay")
                {
                    orderItem.PaymentMethod = "VnPay" + " " + PaymentId;
                }
                else if(PaymentMethod == "MOMO")
                {
                    orderItem.PaymentMethod = "MOMO" + " " + PaymentId;
                }
                else
                {
                    orderItem.PaymentMethod = "COD" + " " + PaymentId;
                }

                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderdetails = new OrderDetails();
                    orderdetails.UserName = userEmail;
                    orderdetails.OrderCode = ordercode;
                    orderdetails.ProductId = (int)cart.ProductId;
                    orderdetails.Price = cart.Price;
                    orderdetails.Quantity = cart.Quantity;

                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                    product.Quantity -= cart.Quantity;
                    product.Sold += cart.Quantity;
                    _dataContext.Update(product);

                    _dataContext.Add(orderdetails);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                //Send mail order when success
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé.";

                await _emailSender.SendEmailAsync(receiver, subject, message);
                TempData["Success"] = "Check out thanh cong";
                return RedirectToAction("History", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if(response.VnPayResponseCode == "00")
            {
                var newVnpayInsert = new VnpayModel
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId,
                    DateCreated = DateTime.Now,
                };
                _dataContext.Add(newVnpayInsert);
                await _dataContext.SaveChangesAsync();
                var PaymentMethod = response.PaymentMethod;
                var PaymentId = response.PaymentId;
                await Checkout(PaymentMethod, PaymentId);
            }
            else
            {
                TempData["Error"] = "Thanh toán thất bại";
                return RedirectToAction("Index", "Cart");
            }
            return View(response);
        }
    }
}
