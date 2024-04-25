using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Cart
    {
        //CartId is the primary key, add relevant attributes
        [Key]
        public string CartId { get; set; }
        
        public int Count { get; set; }

        public decimal CartTotal { get; set; }

        //Column typename change to datetime2 to avoid crashes
        [Column(TypeName = "datetime2")]
        public DateTime DateCreated { get; set; }

        //Foreign key product
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }


}