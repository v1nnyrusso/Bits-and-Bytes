using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{
    public class CheckoutViewModel
    {
        //We need the customer, and address details as attributes. DOB, order, card and payment objects too
        //IsMember to ask the member to become a member, use a display name
       
        public Customer Customer { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Town { get; set; }

        [Required]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public DateTime? DOB { get; set; }

        public Order Order { get; set; }
        public Card Card { get; set; }
        public Payment Payment { get; set; }

        [Display(Name = "Become a Member?")]
        public bool IsMember { get; set; }
    }

}