using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop_HTH.Repository;

namespace Shop_HTH.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Order")]
    [Authorize(Roles = "Publisher,Author,Admin")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync());
        }
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder = await _dataContext.OrderDetails.Include(p => p.Product).Where(od => od.OrderCode==ordercode).ToListAsync();
            return View(DetailsOrder);
        }

    }
}
