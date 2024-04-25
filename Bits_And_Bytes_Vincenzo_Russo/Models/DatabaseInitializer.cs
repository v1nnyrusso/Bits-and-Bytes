using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels;
using System.Runtime.ExceptionServices;
using System.Web.UI.WebControls;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    //DatabaseInitializer class, this will seed data to the database.
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<BitsBytesDbContext>
    {
        protected override void Seed(BitsBytesDbContext context)
        {
            base.Seed(context);

            //********************Create Users*************************

            //If there are no records stored in the Users table
            if(!context.Users.Any())
            {
                //First, to create some roles, and then store them in the Roles table
                //To create and store roles, we need a RoleManager instance

                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //If the Staff role doesn't exist >
                if (!roleManager.RoleExists("Manager"))
                {
                    //Create it
                    roleManager.Create(new IdentityRole("Manager"));
                }

                //If the Sales Assistant role doesn't exist ...
                if(!roleManager.RoleExists("Sales Assistant"))
                {
                    roleManager.Create(new IdentityRole("Sales Assistant"));
                }

                //If the Customer role doesn't exist...
                if(!roleManager.RoleExists("Customer"))
                {
                    roleManager.Create(new IdentityRole("Customer"));
                }

                //If the suspended role doesn't exist...
                if(!roleManager.RoleExists("Suspended"))
                {
                    roleManager.Create(new IdentityRole("Suspended"));
                }

                //********************Create Users*************************

                //To create members or staff, we need a UserManager instance
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));

                //Creating the Manager
                //First, i'll check if a Manager exists in the database
                if(userManager.FindByName("manager@bitsandbytes.com") == null)
                {
                    //For testing, create non-protective password validator
                    userManager.PasswordValidator = new PasswordValidator
                    {
                        RequireDigit = false,
                        RequiredLength = 1,
                        RequireLowercase = false,
                        RequireUppercase = false,
                        RequireNonLetterOrDigit = false

                    };


                    //Create a new Staff object; Manager, which inherits from User, so the attributes are carried a long
                    //From inheritance
                    var Manager = new Staff
                    {
                        UserName = "manager@bitsandbytes.com",
                        FirstName = "Vincenzo",
                        LastName = "Russo",
                        Email = "manager@bitsandbytes.com",
                        DOB = DateTime.Parse("05/05/1999"),
                        StaffPosition = StaffPosition.Manager,
                        Address = "41 Evan Drive",
                        Town = "Glasgow",
                        PostCode = "G46 6NQ",
                        PhoneNumber = "07528883633",
                        RegisteredAt = DateTime.Today.AddYears(-4),
                        IsSuspended = false,
                        EmailConfirmed = true,
                        Country = "United Kingdom"



                    };

                    //Add this Manager to users Table, Password1 will be the seeded password
                    userManager.Create(Manager, "Password1");
                    userManager.AddToRole(Manager.Id, "Manager");

                    //Create a Sales_Assistant Staff member
                    //First, we will check if Sales Assistant already exists in the database
                    if(userManager.FindByName("ben@bitsandbytes.com") == null)
                    {
                        var SalesAssistant = new Staff
                        {
                            UserName = "ben@bitsandbytes.com",
                            Email = "ben@bitsandbytes.com",
                            FirstName = "Ben",
                            LastName = "Jimberly",
                            DOB = DateTime.Parse("12/05/2001"),
                            StaffPosition = StaffPosition.Sales_Assistant,
                            Address = "1 GreenHill Zone",
                            Town = "South Island",
                            PostCode = "123 R1NG",
                            PhoneNumber = "07123456789",
                            RegisteredAt = DateTime.Today.AddYears(-1),
                            IsSuspended = false,
                            EmailConfirmed = true,
                            Country = "United Kingdom"
                        };
                        //Add the SalesAssistant to the Users table
                        userManager.Create(SalesAssistant, "ring");
                        //Assign it to the SalesAssitant role
                        userManager.AddToRole(SalesAssistant.Id, "Sales Assistant");

                        //Seeding a few customers

                        //If this customer doesn't exist, create them
                        var Customer1 = new Customer
                        {
                            UserName = "jeff90@bitsandbytes.com",
                            Email = "jeff90@bitsandbytes.com",
                            FirstName = "Jeffrey",
                            LastName = "Smith",
                            DOB = DateTime.Parse("05/05/2001"),
                            Address = "2 GreenHill Zone",
                            Town = "South Island",
                            PostCode = "456 R1NG",
                            PhoneNumber = "07122454729",
                            RegisteredAt = DateTime.Today.AddYears(-1),
                            IsMember = true,
                            OrderCount = 90,
                            IsSuspended = false,
                            EmailConfirmed = true,
                            Country = "United Kingdom"


                        };

                        //Add this customer to the Users table
                        userManager.Create(Customer1, "Customer");
                        userManager.AddToRoles(Customer1.Id, "Customer");


                        //If this customer doesn't exist, create them
                        var Customer2 = new Customer
                        {
                            UserName = "jim60@bitsandbytes.com",
                            Email = "jim60@bitsandbytes.com",
                            FirstName = "Jim",
                            LastName = "Smith",
                            DOB = DateTime.Parse("05/05/2001"),
                            Address = "3 GreenHill Zone",
                            Town = "South Island",
                            PostCode = "456 R1NG",
                            PhoneNumber = "07122454729",
                            RegisteredAt = DateTime.Today.AddYears(-1),
                            IsMember = true,
                            OrderCount = 60,
                            IsSuspended = false,
                            EmailConfirmed = true,
                            Country = "United Kingdom"


                        };

                        //Add this customer to the Users table
                        userManager.Create(Customer2, "Customer");
                        userManager.AddToRoles(Customer2.Id, "Customer");


                        //If this customer doesn't exist, create them
                        var Customer3 = new Customer
                        {
                            UserName = "john30@bitsandbytes.com",
                            Email = "john30@bitsandbytes.com",
                            FirstName = "John",
                            LastName = "Smith",
                            DOB = DateTime.Parse("05/05/2001"),
                            Address = "4 GreenHill Zone",
                            Town = "South Island",
                            PostCode = "456 R1NG",
                            PhoneNumber = "07122454729",
                            RegisteredAt = DateTime.Today.AddYears(-1),
                            IsMember = true,
                            OrderCount = 30,
                            IsSuspended = false,
                            EmailConfirmed = true,
                            Country = "United Kingdom"


                        };

                        //Add this customer to the Users table
                        userManager.Create(Customer3, "Customer");
                        userManager.AddToRoles(Customer3.Id, "Customer");

                        //A suspended customer
                        var girl = new Customer
                        {
                            UserName = "girl@bitsandbytes.com",
                            Email = "girl@bitsandbytes.com",
                            FirstName = "Girl",
                            LastName = "Gal",
                            DOB = DateTime.Parse("05/09/2004"),
                            Address = "1 Twinkle Park",
                            Town = "Station Square",
                            PostCode = "123 R1NG",
                            PhoneNumber = "07122255729",
                            RegisteredAt = DateTime.Today.AddDays(-5),
                            IsMember = false,
                            OrderCount = 0,
                            IsSuspended = true,
                            EmailConfirmed = true,
                            Country = "United Kingdom"

                        };
                        userManager.Create(girl, "suspended1");
                        userManager.AddToRoles(girl.Id, "Suspended");




                        context.SaveChanges();


                        //***************************Create Products and Categories****************

                        //Create categories
                        var computerPartsCategory = new Category { Name = "Computer Parts" };

                        var computerSystemsCategory = new Category { Name = "Computer Systems" };

                        //Add new categories to categories table
                        context.Categories.Add(computerPartsCategory);
                        context.Categories.Add(computerSystemsCategory);

                        context.SaveChanges();

                        //Create some products
                        var product1 = new Product
                        {
                            ProductName = "ASUS ROG Zephyrus G14 Gaming Laptop",
                            ProductDescription = "Powerful gaming laptop with AMD Ryzen 9 processor and NVIDIA GeForce RTX 3060 graphics",
                            ProductPrice = 1440.00m,
                            ProductDiscount = 0,
                            ProductStock = 30,
                            IsDiscounted = false,
                            IsBOGOF = false,
                            ProductType = ProductType.Laptops,
                            Category = computerSystemsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Laptops/asus-zephyr.jpg"
                        };

                        var product2 = new Product
                        {
                            ProductName = "Acer Predator X27 Gaming Monitor",
                            ProductDescription = "27-inch 4K UHD gaming monitor with NVIDIA G-SYNC technology and HDR",
                            ProductPrice = 1650.50m,
                            ProductDiscount = 0.10m,
                            ProductStock = 20,
                            IsDiscounted = true,
                            IsBOGOF = false,
                            ProductType = ProductType.Monitors,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Monitors/acer-predator.jpg"
                        };

                        var product3 = new Product
                        {
                            ProductName = "Logitech MX Master 3 Wireless Mouse",
                            ProductDescription = "Advanced wireless mouse with ultrafast and quiet MagSpeed scrolling and customizable buttons",
                            ProductPrice = 100.50m,
                            ProductDiscount = 0,
                            ProductStock = 100,
                            IsDiscounted = false,
                            IsBOGOF = false,
                            ProductType = ProductType.Peripherals,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Peripherals/logitec-mouse.jpg"

                        };

                        var product4 = new Product
                        {
                            ProductName = "Dell XPS Desktop Special Edition",
                            ProductDescription = "High-performance desktop PC with 11th Gen Intel Core processor and NVIDIA GeForce RTX 3070 graphics",
                            ProductPrice = 2150.20m,
                            ProductDiscount = 0,
                            ProductStock = 50,
                            IsDiscounted = false,
                            IsBOGOF = false,
                            ProductType = ProductType.PCs,
                            Category = computerSystemsCategory,
                            InStock = true,
                            ImageUrl = "/Images/PCs/dell-desktop.jpg"

                        };

                        var product5 = new Product
                        {
                            ProductName = "Anker PowerExpand+ 7-in-1 USB-C Hub",
                            ProductDescription = "Compact and versatile USB-C hub with 4K HDMI, SD/microSD card readers, and 2 USB-A ports",
                            ProductPrice = 40.25m,
                            ProductDiscount = 0,
                            ProductStock = 200,
                            IsDiscounted = false,
                            IsBOGOF = true,
                            ProductType = ProductType.Peripherals,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Peripherals/anker-usbc.jpg"

                        };

                        var product6 = new Product
                        {
                            ProductName = "Klipsch Reference R-51M Bookshelf Speakers",
                            ProductDescription = "High-quality bookshelf speakers with copper-spun IMG woofers and horn-loaded tweeters",
                            ProductPrice = 345.00m,
                            ProductDiscount = 0,
                            ProductStock = 10,
                            IsDiscounted = false,
                            IsBOGOF = false,
                            ProductType = ProductType.Speakers,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Speakers/klipsch-speakers.jpg"
                        };

                        var product7 = new Product
                        {
                            ProductName = "ASUS ZenScreen MB16AC Portable Monitor",
                            ProductDescription = "15.6-inch Full HD portable monitor with USB Type-C and hybrid-signal solution",
                            ProductPrice = 245.15m,
                            ProductDiscount = 0.05m,
                            ProductStock = 30,
                            IsDiscounted = true,
                            IsBOGOF = false,
                            ProductType = ProductType.Monitors,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Monitors/asus-portable.png"
                        };

                        var product8 = new Product
                        {
                            ProductName = "Dell UltraSharp U2721DE 27-inch Monitor",
                            ProductDescription = "27-inch QHD monitor with USB-C and height-adjustable stand",
                            ProductPrice = 545.00m,
                            ProductDiscount = 0,
                            ProductStock = 30,
                            IsDiscounted = false,
                            IsBOGOF = false,
                            ProductType = ProductType.Monitors,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Monitors/dell-monitor.jpg"
                        };

                        var product9 = new Product
                        {
                            ProductName = "Razer DeathAdder V2 Pro Wireless Gaming Mouse",
                            ProductDescription = "The Razer DeathAdder V2 Pro is a wireless gaming mouse with Razer Focus+ 20K DPI Optical Sensor and up to 70 hours of battery life.",
                            ProductPrice = 123.45m,
                            ProductDiscount = 0.10m,
                            ProductStock = 50,
                            IsDiscounted = true,
                            IsBOGOF = false,
                            ProductType = ProductType.Peripherals,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Peripherals/razer-mouse.jpg"
                        };


                        var product10 = new Product
                        {
                            ProductName = "Asus ROG Zephyrus G15 Laptop",
                            ProductDescription = "Gaming laptop with AMD Ryzen 9 processor and NVIDIA GeForce RTX 3060 graphics",
                            ProductPrice = 1800.00m,
                            ProductDiscount = 0.10m,
                            ProductStock = 10,
                            IsDiscounted = true,
                            IsBOGOF = false,
                            ProductType = ProductType.Laptops,
                            Category = computerSystemsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Laptops/asus-zephyrus.png"

                        };

                        var product11 = new Product
                        {
                            ProductName = "Sonos One (Gen 2) Smart Speaker",
                            ProductDescription = "Smart speaker with voice control and Alexa built-in",
                            ProductPrice = 195.95m,
                            ProductDiscount = 0,
                            ProductStock = 60,
                            IsDiscounted = false,
                            IsBOGOF = true,
                            ProductType = ProductType.Speakers,
                            Category = computerPartsCategory,
                            InStock = true,
                            ImageUrl = "/Images/Speakers/Sonos-One.jpg"
                        };


                        //Add them to the products db tabel, save changes
                        context.Products.Add(product1);
                        context.Products.Add(product2);
                        context.Products.Add(product3);
                        context.Products.Add(product4);
                        context.Products.Add(product5);
                        context.Products.Add(product6);
                        context.Products.Add(product7);
                        context.Products.Add(product8);
                        context.Products.Add(product9);
                        context.Products.Add(product10);
                        context.Products.Add(product11);

                        context.SaveChanges();

                       

                    }

                }

            }

        }
    }
   

}