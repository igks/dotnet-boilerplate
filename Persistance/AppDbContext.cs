using dotnet.boilerplate.Models;
using dotnet.boilerplate.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;

namespace dotnet.boilerplate.Persistance
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfig());
        }
    }
}