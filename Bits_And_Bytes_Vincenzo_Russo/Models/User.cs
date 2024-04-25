using System.Data.Entity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Web.Services.Description;

namespace Bits_And_Bytes_Vincenzo_Russo.Models
{
  
    public abstract class User : IdentityUser
    {
        //****************************************************
        //Extending IdentityUser with the following properties

        //Using a DisplayAttribute tag, we can change how the attribute will be displayed

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DOB { get; set; }

        public string Address { get; set; }

        public string Town { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        public string Country { get; set; }

        //DisplayFormat tag specifying DataFormatString to follow british date
        [Display(Name = "Joined")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RegisteredAt { get; set; }


        [Display(Name = "Suspended")]
        public bool IsSuspended { get; set; }

        //We need the ApplicatioNUserManager to get the user's current role
        private ApplicationUserManager userManager;

        //*****************************************************

        //the currentRole property is not mapped as a field in the Users table
        //I need it to get the current role from the user that is logged in
        [NotMapped]
        [Display(Name = "Current Role")]
        public string CurrentRole
        {
            get
            {
                
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }

                
                var role = userManager.GetRoles(Id).SingleOrDefault();

                //If no role is found make the 'role' no role found, to prevent crashes
                return role ?? "No Role Found";
            }
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            //Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            //Add custom user claims here
            return userIdentity;
        }
    }


}