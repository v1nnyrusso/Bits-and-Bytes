using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Collections;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class BitsBytesDbContext : IdentityDbContext<User>
    {
        //DB Sets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Payment> Payments { get; set; }




        //Db connection string from Web.Config as a reference
        public BitsBytesDbContext()
            : base("BitsandBytesConnection15", throwIfV1Schema: false)
        {
            //Setting a new database intializer
            Database.SetInitializer(new DatabaseInitializer());
        }

        //Create new db context
        public static BitsBytesDbContext Create()
        {
            return new BitsBytesDbContext();
        }
    }
}