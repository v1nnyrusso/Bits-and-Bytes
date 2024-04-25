using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    //New customer class that inherits from User
    public class Customer : User
    {
        //****************************************************
        //Extending User with the following properties

        //CustomerId string property that = generated IdentityUser Id
        public string CustomerId
        {
            get
            {
                return Id;
            }
            set { }
        }
        
        
        //MembershipId string property that collects the first initial of a customer's firstname, plus the id of that customer 
        public string MembershipId
        {
            get
            {
                //Only if they are a member
                if(IsMember == true)
                {                
                    //Use substring method that returns the first character in the string of customer's first name 
                    return FirstName.Substring(0, 1) + CustomerId;
                }
                else
                {
                    return null;
                }
         
            }
            set { }
            
        }

        //Attributes for only customer
        [Display(Name = "Member")]
        public bool IsMember { get; set; }

        [Display(Name = "Order Count")]
        public int OrderCount { get; set; }


        //Nav properties
        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Card> Cards { get; set; }

    }
    
}