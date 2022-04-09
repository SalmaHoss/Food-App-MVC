using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodApp.Models
{
    /*
     * we change here to IdentityDBcontext to create user, roles,claims
     * that exsit in identityUser 
     * if we not pass our application user the EF will use IdentityUser
     * as defualt.
     * when update datebase the tables (users,Roles,userClaims,...)
     */

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Restaurant> Restaurants{ get; set; }
        public DbSet<Item> Items { get; set; }


        //Orders related tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

    }
}
