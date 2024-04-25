using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Category
    {
        //Primary key category id, add relevant attributes
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string Name { get; set; }


        //Nav property
        //One to Many relationship
        public virtual List<Product> Products { get; set; }
    }
}