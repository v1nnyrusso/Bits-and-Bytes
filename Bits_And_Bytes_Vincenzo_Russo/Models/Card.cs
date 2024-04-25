using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Card
    {
        //Primary Key CardId
        //Add relevant attributes for card 
        [Key]
        public int CardId { get; set; }

        
        [Required]
        [Display(Name = "Name on Card")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Display(Name = "CVV2")]
        [Range(0, int.MaxValue, ErrorMessage = "CVV2 cannot be a negative number.")]
        public int? Cvv2 { get; set; }


        [Required]
        [Display(Name = "Expiry Month")]
        public int ExpiryMonth { get; set; }

        [Required]
        [Display(Name = "Expiry Year")]
        public int ExpiryYear { get; set; }

        //Navigational Properties
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        //One to Many nav property
        public virtual ICollection<Payment> Payments { get; set; }
    }
}