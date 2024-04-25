using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Payment
    {
        //Fk of order is the primary key, add relevant attributes
        [Key, ForeignKey("Order")]
        public int PaymentId { get; set; }

        [Display(Name = "Payment Total")]
        public decimal PaymentTotal { get; set; }

        //Nullable to prevent crashes
        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        public bool IsSuccessful { get; set; }

        public bool IsRefunded { get; set; }

        //Navigational Properties

        public Order Order { get; set; }

        //Card fk
        [ForeignKey("Card")]
        public int CardId { get; set; }
        public Card Card { get; set; }
    }
}