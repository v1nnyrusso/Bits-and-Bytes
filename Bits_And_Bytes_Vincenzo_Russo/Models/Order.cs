using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    public class Order
    {
        //OrderId primary key, add relevant order attributes
        [Display(Name = "Order Number")]
        [Key]
        public int OrderId { get; set; }

        //Nullable datetime attributes to prevent crashes from datetime to datetime2 conversions
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

        //0 param constructor to create new orderlines obj instance
        public Order()
        {
            OrderLines = new List<OrderLine>();
        }



        //Navigational Properties
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        //Customer foreign key
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
  
        public Customer Customer { get; set; }

        //Payment nav property
        public Payment Payment { get; set; }

       

    }

    //orderstatus enum to display what status the order is at
    public enum OrderStatus { Cancelled, CancellationRequested, Started, Placed, Delivery, Dispatched, Completed }
}