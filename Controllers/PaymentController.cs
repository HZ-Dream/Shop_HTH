using Microsoft.AspNetCore.Mvc;
using Shop_HTH.Models;
using Shop_HTH.Models.Vnpay;
using Shop_HTH.Services.Vnpay;
//using Shop_HTH.Services.Momo;


namespace Shop_HTH.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        //private IMomoService _momoService;
        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        //[HttpPost]
        //public async Task<IActionResult> CreatePaymentMomo(OrderInfo model)
        //{
        //    var response = await _momoService.CreatePaymentAsync(model);
        //    return Redirect(response.PayUrl);

        //}


        //[HttpGet]
        //public IActionResult PaymentCallBack()
        //{
        //    var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
        //    return View(response);
        //}

        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
    }
}
