using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Data;
using TamVaxti.Helpers;
using TamVaxti.Middleware;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

EmailHelper.Configure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.MaxDepth = 64; // Increase the max depth if needed
});



builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
                                                     .AddDefaultTokenProviders();
// Add Antiforgery services
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "data-protection-keys")));
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn"; // Set your login URL here
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});
builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredUniqueChars = 1;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireDigit = true;
    opt.Password.RequireNonAlphanumeric = true;

    opt.User.RequireUniqueEmail = true;
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpertService, ExpertService>();
builder.Services.AddScoped<ISliderInstaService, SliderInstaService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ISubcategoryService, SubCategoryService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IAttributeOptionService, AttributeOptionService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<IAboutUsService, AboutUsService>();
builder.Services.AddScoped<IUserWishList, UserWishListService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IBrandService, BrandService>();

var app = builder.Build();
app.UseMiddleware<UnAuthorizedRedirect>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
          new ExceptionHandlerOptions()
          {
              AllowStatusCode404Response = true, // important!
              ExceptionHandlingPath = "/Account/Error"
          }
      );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}


// Middleware order
//app.UseHttpsRedirection(); // Ensure HTTPS for all requests
app.UseStaticFiles();      // Serve static files
app.UseRouting();          // Setup routing
app.UseAuthentication();   // Authenticate users
app.UseAuthorization();    // Authorize users





app.MapControllerRoute(
  name: "areas",
  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
