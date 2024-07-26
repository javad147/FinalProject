using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TamVaxti.Models;
using TamVaxti.ViewModels;

namespace TamVaxti.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Company> Company { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<About> About { get; set; }
        public DbSet<AboutUsHistory> AboutUsHistory { get; set; }
        public DbSet<AboutUsTeam> AboutUsTeam { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
