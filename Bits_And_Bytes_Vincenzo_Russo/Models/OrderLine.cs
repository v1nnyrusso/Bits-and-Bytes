using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class OrderLine
    {
        //OrderLineId primary key, add relevant attributes
        [Key]
        public int OrderLineId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [Display(Name = "Total Amount")]
        public decimal LineTotal { get; set; }


        //Navigational Properties
        //Order fk
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        //Product fk
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}