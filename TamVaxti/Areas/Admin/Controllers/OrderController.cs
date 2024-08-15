using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TamVaxti.ViewModels.Orders;
using TamVaxti.Helpers;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class OrderController : Controller
    {
        private AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public IOrderService _orderService;
        private readonly ICompanyService _companyService;
        public OrderController(AppDbContext context, UserManager<AppUser> userManager, IOrderService orderService, ICompanyService companyService)
        {
            _context = context;
            _userManager = userManager;
            _orderService = orderService;
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string searchString = null)
        {
            List<Order> orders = await _orderService.GetAllOrdersPaginatedAsync(page, 100);


            var userIds = orders.Select(o => o.CustomerId).ToList();

            var users = await _userManager.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName })
            .ToListAsync();

            var orderViewModels = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = users.FirstOrDefault(u => u.Id == o.CustomerId)?.FullName,
                OrderDate = o.OrderDate,
                Payments = o.Payments,
                TotalAmount = o.TotalAmount,
                DeliveryStatus = o.DeliveryStatus
            }).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                orderViewModels = orderViewModels.Where(o => o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }


            int pageCount = await GetPageCountAsync(4);

            //Paginate<Order> model = new(orders, pageCount, page);
            Paginate<OrderViewModel> model = new Paginate<OrderViewModel>(orderViewModels, pageCount, page);

            /*orders = from order in _context.Orders
                        join user in _userManager.Users.ToList()
                        on order.CustomerId equals user.Id
                        select new
                        {
                            Order = order,
                            UserName = user.UserName,
                            Payments = order.Payments.ToList()
                        };
            var res = await orders.ToListAsync();*/


            ViewData["CurrentFilter"] = searchString;
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            return View(model);

            //return View(orders);
        }



        private async Task<int> GetPageCountAsync(int take)
        {
            int count = await _orderService.GetCountAsync();

            return (int)Math.Ceiling((decimal)count / take);
        }

        [HttpGet]
        public async Task<IActionResult> Tracking(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderTracking = await _orderService.GetTrackingDetails(id);
            var DeliveryStatusItems = _orderService.GetDeliveryStatusItems();
            DeliveryStatusItems.Remove("Returned");
            DeliveryStatusItems.Remove("Cancelled");
            ViewData["DeliveryStatusItems"] = DeliveryStatusItems;
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            return View(order);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["DeliveryStatusItems"] = _orderService.GetDeliveryStatusItems();
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            return View(order);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderEditVM
            {
                OrderId = order.OrderId,
                DeliveryStatus = order.DeliveryStatus,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                ActualDeliveryDate = order.ActualDeliveryDate,
                ExpectedDeliveryDate = order.ExpectedDeliveryDate
            };

            ViewData["DeliveryStatusItems"] = _orderService.GetDeliveryStatusItems();
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderEditVM model)
        {
            if (ModelState.IsValid)
            {
                var result = _orderService.UpdateOrder(model);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Unable to update order.");
            }
            ViewBag.CurrencySymbol = _companyService.GetCurrencySymbol();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var result = _orderService.DeleteOrder(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpGet]
        public IActionResult AddTracking(int id)
        {
            var model = new OrderTrackingVM { OrderId = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTracking(OrderTrackingVM model)
        {
            if (ModelState.IsValid)
            {
                var order = new OrderTracking
                {
                    OrderId = model.OrderId,
                    Location = model.Location,
                    Notes = model.Notes,
                    Timestamp = model.Timestamp
                };
                var res = _orderService.AddTracking(order);
                if (res)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));

        }

    }
}
