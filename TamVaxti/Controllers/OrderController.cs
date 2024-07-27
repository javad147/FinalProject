using System.ComponentModel.DataAnnotations;
using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Baskets;
using TamVaxti.ViewModels.Orders;
using TamVaxti.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TamVaxti.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IHttpContextAccessor _accessor;
    private readonly AppDbContext _context;
    private readonly IProductService _productService;
    private readonly UserManager<AppUser> _userManager;

    public OrderController(AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor accessor,
        IProductService productService)
    {
        _context = context;
        _userManager = userManager;
        _accessor = accessor;
        _productService = productService;
    }

    public async Task<IActionResult> OrderHistory()
    {
        AppUser user = new();

        user = await _userManager.FindByNameAsync(User.Identity.Name);

        var orders = await _context.Orders
            .Where(o => o.CustomerId == user.Id)
            .Include(o => o.OrderProductDetails)
            .ThenInclude(op => op.Product)
            .ToListAsync();

        // Calculate ratings in-memory, filtering published reviews
        var orderHistory = orders.OrderByDescending(o=>o.OrderId).Select(o => new OrderHistoryVM
        {
            OrderNumber = o.OrderNumber,
            TotalAmount = o.TotalAmount,
            ActualDeliveryDate = o.ActualDeliveryDate,
            Products = o.OrderProductDetails.Select(op => new ProductVM
            {
                Name = op.Product.Name,
                Image = $"/img/product/{op.Product.MainImage}",
                Rating = RoundRating(_context.Product_Reviews
                    .Where(pr => pr.ProductId == op.ProductId && pr.Status)
                    .Average(pr => (double?)pr.Rating) ?? 0),
                Quantity = op.Quantity,
                Amount = op.Amount
            }).ToList()
        }).ToList();

        return View(orderHistory);
    }

    private static int RoundRating(double rating)
    {
        return rating % 1 < 0.5 ? (int)Math.Floor(rating) : (int)Math.Ceiling(rating);
    }

    public async Task<IActionResult> CheckOut()
    {
        var basketProducts = await GetOrderProducts();

        if(basketProducts.Count == 0) return RedirectToAction("Cart", "Basket");

        var user = await GetUser();

        var userShippingAddress = await _context.UserShippingAddress.FirstOrDefaultAsync(x => x.UserId == user.Id);
        userShippingAddress.User = user;


        var checkoutVM = new CheckoutVM
        {
            Products = basketProducts,
            UserShippingAddress = userShippingAddress
        };
        return View(checkoutVM);
    }


    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromBody] OrderCreateViewModel orderVM)
    {
        // Validate the orderViewModel data
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var cart = await GetOrderProducts();

        var user = await GetUser();

        decimal discountAmount = 0;
        if (!string.IsNullOrEmpty(orderVM.CouponCode))
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == orderVM.CouponCode && c.IsEnabled);
            if (coupon != null)
            {
                discountAmount = cart.Sum(x => x.SubTotal) * (coupon.DiscountValue / 100);
                orderVM.CouponDiscountAmount = discountAmount;
                orderVM.CouponDiscountPercentage = coupon.DiscountValue;
            }
            else
            {
                orderVM.CouponCode = string.Empty;
            }
        }

        var order = new Order
        {
            CustomerId = user.Id,
            OrderDate = DateTime.Now,
            Subtotal = cart.Sum(x => x.SubTotal) - discountAmount,
            ShippingCharges = 0,
            CouponCode = orderVM.CouponCode,
            CouponDiscountAmount = orderVM.CouponDiscountAmount,
            CouponDiscountPercentage = orderVM.CouponDiscountPercentage,
            Tax = cart.Sum(x => x.SubTotal) * 0.05m,
            ShippingAddress = GetShippingAddress(orderVM),
            DeliveryAddress = GetShippingAddress(orderVM),
            DeliveryStatus = "Pending"
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var payment = new Payment()
        {
            OrderId = order.OrderId,
            PaymentMode = orderVM.PaymentMode,
            PaymentAmount = order.TotalAmount,
            PaymentDate = order.OrderDate,
            PaymentStatus = "Paid"
        };
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        foreach (var orderProductDetail in cart.Select(item => new OrderProductDetail
        {
            OrderId = order.OrderId,
            ProductId = item.Product.Id,
            Quantity = item.Quantity,
            Amount = item.SubTotal,
            
        }))
        {
            _context.OrderProductDetails.Add(orderProductDetail);
        }

        await _context.SaveChangesAsync();

        // Clear the cart cookie
        _accessor.HttpContext?.Response.Cookies.Delete("cart");

        // Return a success response
        return Ok(order);
    }

    private async Task<List<BasketVM>> GetOrderProducts()
    {
        var basketProducts = new Dictionary<int, int>();

        if (_accessor.HttpContext?.Request.Cookies["cart"] is not null)
        {
            var cartJson = _accessor.HttpContext.Request.Cookies["cart"];
            basketProducts = JsonConvert.DeserializeObject<Dictionary<int, int>>(cartJson);
        }

        var products = await _productService.GetAllWithImagesAsync();

        return (from basketProduct in basketProducts
                let product = products.FirstOrDefault(m => m.Id == basketProduct.Key)
                where product != null
                let subtotal = basketProduct.Value * product.Price
                select new BasketVM { Product = product, Quantity = basketProduct.Value, SubTotal = subtotal }).ToList();
    }


    private async Task<AppUser> GetUser()
    {
        AppUser user = new();
        if (User.Identity is { IsAuthenticated: true }) user = await _userManager.FindByNameAsync(User.Identity.Name);
        return user;
    }

    private string GetShippingAddress(OrderCreateViewModel enteredAddress)
    {
        return
            enteredAddress.Flat ?? "" + ", " + enteredAddress.Address + ", " + enteredAddress.City + ", " +
               enteredAddress.Region + ", " + enteredAddress.Country + ", " + enteredAddress.ZipCode;
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon([FromBody] string couponCode)
    {
        // Validate the coupon code
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode && c.IsEnabled);
        if (coupon == null)
        {
            return BadRequest("Invalid or expired coupon code.");
        }

        // Calculate the discount
        var discount = coupon.DiscountValue;

        // Return the discount percentage
        return Ok(discount);
    }
}

public class OrderCreateViewModel
{
    [Required][StringLength(255)] public string Name { get; set; }

    [Required][StringLength(255)] public string Phone { get; set; }

    [Required][StringLength(255)] public string Email { get; set; }

    public string Flat { get; set; }

    [Required][StringLength(255)] public string Address { get; set; }

    [Required][StringLength(20)] public string ZipCode { get; set; }

    [Required][StringLength(100)] public string Country { get; set; }

    [Required][StringLength(100)] public string City { get; set; }

    [Required][StringLength(100)] public string Region { get; set; }

    public string PaymentMode { get; set; }

    public string CouponCode { get; set; } // Added CouponCode property
    public decimal? CouponDiscountAmount { get; set; }
    public decimal? CouponDiscountPercentage { get; set; }
}