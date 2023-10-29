using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Models;

namespace ShoppingCart.Infrastructure
{   
        public class DataContext : IdentityDbContext<AppUser>
        {
                public DataContext(DbContextOptions<DataContext> options) : base(options)
                { }
                public DbSet<Product> Products { get; set; }
                public DbSet<Category> Categories { get; set; }
                
                public DbSet<Order> Orders { get; set; }

                public DbSet<Review> Reviews { get; set; }
                
                public DbSet<CartItem> CartItems { get; set; }


                protected override void OnModelCreating(ModelBuilder modelBuilder)
                {
                        
                        modelBuilder.Entity<Review>()
                                .HasOne(r => r.AppUser)
                                .WithMany() // or WithOne() depending on your model
                                .HasForeignKey(r => r.UserId);
                        
                        base.OnModelCreating(modelBuilder);

                        modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
                                { Name = "Admin", NormalizedName = "ADMIN", Id = "admin-role" });
                        
                        
                        modelBuilder.Entity<AppUser>().HasData(
                                new AppUser
                                {
                                        Id = "admin-user",
                                        Email = "admin@mail.com",
                                        NormalizedEmail = "ADMIN@MAIL.COM",
                                        NormalizedUserName = "ADMIN@MAIL.COM",
                                        UserName = "admin@mail.com",
                                        PasswordHash =
                                                new PasswordHasher<AppUser>().HashPassword(
                                                        new AppUser { Email = "admin@mail.com", UserName = "admin" },
                                                        "admin")
                                }
                        );
                        
                        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                                new IdentityUserRole<string> { UserId = "admin-user", RoleId = "admin-role" }
                        );
                }

        }
}
