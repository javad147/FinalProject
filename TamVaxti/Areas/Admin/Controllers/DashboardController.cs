using TamVaxti.Data;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashboardController : Controller
    {
        private AppDbContext _context;
        public IDashboardService _dashboardService;
        private readonly ICompanyService _companyService;
        public DashboardController(AppDbContext context, IDashboardService dashboardService, ICompanyService companyService)
        {
            _context = context;
            _dashboardService = dashboardService;
            _companyService = companyService;
        }

        public async Task<IActionResult> Index()
        {

            var OrdersInMonth = await _dashboardService.GetAllProductsPurchasedInMonthAsync(DateTime.Now);
            //return Ok(OrdersInMonth);
            ViewBag.Earnings = OrdersInMonth.Sum(o => o.TotalAmount);
            ViewBag.TotalSku = OrdersInMonth.Sum(o => o.OrderProductDetails.Sum(od => od.Quantity));
            ViewBag.OrderCnt = OrdersInMonth.Count;

            var LatestOrders = await _dashboardService.GetLatestOrdersInMonthAsync(DateTime.Now, 5);
            if (LatestOrders != null)
            {
                ViewBag.LatestOrders = LatestOrders.Select(o => new
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    DeliveryStatus = o.DeliveryStatus,
                    PaymentMode = o.Payments?.FirstOrDefault().PaymentMode
                });
            }

            var LastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
            var OrdersInLastMonth = await _dashboardService.GetAllProductsPurchasedInMonthAsync(LastMonth);

            ViewBag.LastMonth = LastMonth.ToString("MMMM");
            ViewBag.LastMonthEarnings = OrdersInLastMonth.Sum(o => o.TotalAmount);

            var CashTransactions = OrdersInMonth.Select(o => new
            { TotalAmount = o.TotalAmount, PaymentMode = o.Payments.FirstOrDefault().PaymentMode });

            ViewBag.CashTransactions = CashTransactions.Where(p => p.PaymentMode == "Cash").Sum(o => o.TotalAmount);
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            //return Ok(ViewBag.LatestOrders);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLastMonthRevenue()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);

            var orders = await _dashboardService.GetAllProductsPurchasedInMonthAsync(startDate);

            var ordernw = orders.GroupBy(o => o.OrderDate).Select(g => new
            {
                Date = g.Key,
                TotalAmount = g.Sum(o => o.TotalAmount)
            }).OrderBy(o => o.Date).ToList();

            var labels = ordernw.Select(o => o.Date.Day).ToList();
            var series = ordernw.Select(o => o.TotalAmount).ToList();

            var nestedList = new List<List<decimal>> { series };

            var model = new
            {
                Labels = labels,
                Series = nestedList
            };

            return Ok(JsonSerializer.Serialize(model));
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
