
using CarSalesPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CarSalesPortal.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
       
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails {  get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Brand New", ItemNumber = 1},
                new Category { Id = 2, Name = "Second Hand", ItemNumber = 2 }              
                );


            



            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    Id = 1,
                    Make = "Volkswagen",
                    Model = "Polo Vivo 1.4 Comfortline",
                    Year = "2018",
                    Colour = "White",
                    Price = 15000,
                    Status = " Published",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Car
                {
                    Id = 2,
                    Make = "Volkswagen",
                    Model = "Golf MK7 1.2 TSi",
                    Year = "2015",
                    Colour = "White",
                    Price = 200000,
                    Status = "published",
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Car
                {
                    Id = 3,
                    Make = "Volkswagen",
                    Model = "Golf MK7 1.2 TSi",
                    Year = "2019",
                    Colour = "White",
                    Price = 180000,
                    Status = " published",
                    CategoryId = 1,
                    ImageUrl = ""
                   
                },
                new Car
                {
                    Id = 4,
                    Make = "Volkswagen",
                    Model = "Polo Hatch 1.0TSI Comfortline R-Line",
                    Year = "2021",
                    Colour = "Orange",
                    Price = 250000,
                    Status = " published",
                    CategoryId = 2,
                    ImageUrl = ""
                },
                new Car
                {
                    Id = 5,
                    Make = "Volkswagen",
                    Model = "Polo Tsi 1.0",
                    Year = "2019",
                    Colour = "Black",
                    Price = 99,
                    Status = " published",
                    CategoryId = 2,
                    ImageUrl = ""
                }
                //new Car
                //{
                //    Id = 6,
                //    Make = "Toyota",
                //    Model = "Toyota Hilux Legend 50 2.8 Gd-6",
                //    Year = "2019",
                //    Colour = "Orange",
                //    Price = 99,
                //    Status = " published",
                //    CategoryId = 2,
                //    ImageUrl = ""
                //}


                );

        }

    }
}
