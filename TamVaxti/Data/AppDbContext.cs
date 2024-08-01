using TamVaxti.Models;
using TamVaxti.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<About> About { get; set; }
        public DbSet<AboutUsHistory> AboutUsHistory { get; set; }
        public DbSet<AboutUsTeam> AboutUsTeam { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<UserWishList> UserWishList { get; set; }
        public DbSet<UserShippingAddress> UserShippingAddress { get; set; }
        public DbSet<ProductReview> Product_Reviews { get; set; }
        public DbSet<SliderInfo> SliderInfos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<About> AboutParts { get; set; }
        public DbSet<Expert> Experts { get; set; }
        public DbSet<ExpertImage> ExpertImages { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<InstaSlider> InstaSliders { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OrderProductDetail> OrderProductDetails { get; set; }
        public DbSet<OrderTracking> OrderTracking { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<NewsletterSubscriptions> NewsletterSubscriptions { get; set; }
        public DbSet<Attributes> Attributes { get; set; }   
        public DbSet<AttributeOption> AttributeOptions { get; set; }
        public DbSet<SKU> SKUs { get; set; }
        public DbSet<AttributeOptionSKU> AttributeOptionSKUs { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<SkuStock> SkuStocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Brand> Brand { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
              .Property(b => b.Uname)
              .HasDefaultValue("")
              .IsRequired();
            modelBuilder.Entity<ProductReview>()
            .HasKey(pr => pr.ReviewId);

            modelBuilder.Entity<Blog>()
            .Property(b => b.Date)
            .HasColumnType("datetime2");

            modelBuilder.Entity<Blog>()

           .HasMany(b => b.Comments)
           .WithOne(c => c.Blog)
           .HasForeignKey(c => c.BlogId)
           
           .OnDelete(DeleteBehavior.Cascade);
        

        modelBuilder.Entity<ProductReview>()
                .Property(pr => pr.Status)
                .HasColumnName("Status");

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Subcategories)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
            .HasOne(o => o.Users)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<OrderProductDetail>()
            .HasOne(o => o.Order)
            .WithMany(u => u.OrderProductDetails)
            .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderProductDetail>()
            .HasOne(o => o.SKU)
            .WithMany(u => u.OrderProductDetail)
            .HasForeignKey(o => o.SkuId);

            modelBuilder.Entity<Product>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<AttributeOption>()
            .HasKey(ao => ao.Id);

            modelBuilder.Entity<AttributeOption>()
            .HasOne(ao => ao.Attribute)
            .WithMany(a => a.AttributeOptions)
            .HasForeignKey(ao => ao.AttributeId);

            modelBuilder.Entity<SKU>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<SKU>()
                .HasOne(s => s.Product)
                .WithMany(p => p.SKUs)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<SKU>()
                .Ignore(p => p.Quantity)
                .Ignore(p => p.TotalQuantity)
                .Ignore(p => p.ImageUrl1File)
                .Ignore(p => p.ImageUrl2File)
                .Ignore(p => p.ImageUrl3File)
                .Ignore(p => p.ImageUrl4File);

            modelBuilder.Entity<SkuStock>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<SkuStock>()
                .HasOne(s => s.SKU)
                .WithMany(p => p.SkuStock)
                .HasForeignKey(s => s.SkuId);

            modelBuilder.Entity<AttributeOptionSKU>()
            .HasKey(aos => new { aos.SkuId, aos.AttributeOptionId });

            modelBuilder.Entity<AttributeOptionSKU>()
                .HasOne(aos => aos.SKU)
                .WithMany(sku => sku.AttributeOptionSKUs)
                .HasForeignKey(aos => aos.SkuId);

            modelBuilder.Entity<AttributeOptionSKU>()
                .HasOne(aos => aos.AttributeOption)
                .WithMany(ao => ao.AttributeOptionSKUs)
                .HasForeignKey(aos => aos.AttributeOptionId);

            modelBuilder.Entity<AttributeOptionSKU>()
            .ToTable("AttributeOptionSKU");

            modelBuilder.Entity<SkuStock>()
            .ToTable("SkuStock");
        }
    }

}
