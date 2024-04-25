using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{
    public class StaffCreateOrderViewModel
    {

     public Order Order { get; set; }


        [Display(Name = "Date")]
        public DateTime? OrderDate { get; set; }

        [Display(Name = "Date Cancelled")]
        public DateTime? DateCancelled { get; set; }

        [Display(Name = "Total")]
        public decimal OrderTotal { get; set; }

        [Display(Name = "Paid")]
        public bool IsPaid { get; set; }

        //Status of order enum
        public OrderStatus OrderStatus { get; set; }

        public decimal Subtotal { get; set; }

        public decimal? MembershipDiscount { get; set; }

        public decimal? VatAmount { get; set; }

        public string DeliveryAddress { get; set; }

        public string DeliveryTown { get; set; }

        [Display(Name = "Post Code")]
        public string DeliveryPostCode { get; set; }

        public string DeliveryCountry { get; set; }

        [Display(Name = "VAT")]
        public bool AddVat { get; set; }

    }
}