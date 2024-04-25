using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Product
    {
        //Product primary key ProductId
        //Add necessary attributes for a product
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Price")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Stock Level")]
        public int ProductStock { get; set; }

        [Display(Name ="Discounted")]
        public bool IsDiscounted { get; set; }

        [Display(Name = "Product Discount")]
        public decimal ProductDiscount { get; set; }

        [Display(Name = "Buy One Get One Free")]
        public bool IsBOGOF { get; set; }

        [Display(Name = "In Stock")]
        public bool InStock { get; set; }

        //Storing the path to the image 
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }


        //Product subcategory producttype enum
        public ProductType ProductType { get; set; }

        //Navigational Properties

        //Fk with category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        //Many to many
        public virtual ICollection<Order> Orders { get; set; }
        public virtual List<OrderLine> OrderLines { get; set; }

    }

    //Product type sub category enum
    public enum ProductType
    {
        Monitors,
        Speakers,
        Peripherals,
        PCs,
        Laptops
    }
}