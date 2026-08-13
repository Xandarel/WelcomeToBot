using Microsoft.EntityFrameworkCore;
using WelcomeTo.DAL.Entities;

namespace WelcomeTo.DAL.Contexts
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Quest> Quest { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }
    }
}
