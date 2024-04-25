using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bits_And_Bytes_Vincenzo_Russo.Models;

namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{

    public class ShoppingCartViewModel
    {

        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}