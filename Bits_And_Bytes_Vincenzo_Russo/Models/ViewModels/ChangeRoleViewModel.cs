using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{
    public class ChangeRoleViewModel
    {
        //We need the useranme, oldrole, SelectListItem of roles and Role 
        public string UserName { get; set; }

        public string OldRole { get; set; }

        public ICollection<SelectListItem> Roles { get; set; }

        [Required, Display(Name = "Role")]
        public string Role { get; set; }

    }
}