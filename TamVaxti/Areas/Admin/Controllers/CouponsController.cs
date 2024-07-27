using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Data;
using TamVaxti.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly AppDbContext _context;

        public CouponsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Coupons
        public async Task<IActionResult> List(string searchString)
        {
            var coupons = from c in _context.Coupons
                          select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                coupons = coupons.Where(s => s.CouponTitle.Contains(searchString) || s.CouponCode.Contains(searchString));
            }

            return View(await coupons.ToListAsync());
        }

        // GET: Admin/Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Coupons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CouponTitle,CouponCode,StartDate,EndDate,AllowFreeShipping,Quantity,DiscountType,DiscountValue,IsEnabled,Products,Category,MinimumSpend,MaximumSpend,PerLimit,PerCustomer")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(coupon);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var coupon = _context.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        public IActionResult Edit(Coupon model)
        {
            if (ModelState.IsValid)
            {
                // Update the model in the database
                _context.Coupons.Update(model);
                _context.SaveChanges();
                return RedirectToAction("List"); // or wherever you want to redirect after editing
            }
            return View(model);
        }

        // POST: Admin/Coupons/DeleteSelected
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int[] ids)
        {
            foreach (var id in ids)
            {
                var coupon = await _context.Coupons.FindAsync(id);
                if (coupon != null)
                {
                    _context.Coupons.Remove(coupon);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }
    }

}


