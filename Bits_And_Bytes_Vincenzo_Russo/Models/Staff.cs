using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
    //Staff class extending from user, in order to break up staff positions via enumeration attribute
    public class Staff : User
    {
        //Staff position enum
        [Display(Name = "Staff Position")]
        public StaffPosition StaffPosition { get; set; }
    }

    //Create an enumeration to differentiate staff positions
    public enum StaffPosition
    {
        Manager,
        Sales_Assistant
    }

}