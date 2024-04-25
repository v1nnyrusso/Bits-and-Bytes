using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{
    public class CreateCustomerViewModel
    {

        //We need the relevant create customer attribtues, add error messages for non inputs
        [Required(ErrorMessage = "Please enter a first name.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //Nullable datetime to avoid conversion crashses
        [Required(ErrorMessage = "Please enter a DOB.")]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Please enter an address.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter a town.")]
        public string Town { get; set; }

        [Required(ErrorMessage = "Please enter a valid postcode.")]
        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required(ErrorMessage = "Please enter a country.")]
        [Display(Name = "Country")]
        public string Country{ get; set; }

        [Required(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        //Displaying in the format of country code, area code then local phone number (DisplayFormat tag)
        [DataType(DataType.PhoneNumber)]
        [DisplayFormat(DataFormatString = "{0:(0)#### ### ####}")]
        public string TelephoneNumber { get; set; }

        [Display(Name = "Phone Confirm")]
        public bool PhoneConfirm { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [Display(Name = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Display(Name = "Email Confirm")]
        public string EmailConfirm { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "OrderCount cannot be a negative number.")]
        public int OrderCount { get; set; }


        //DisplayFormat tag specifying DataFormatString to follow british date
        [Display(Name = "Joined")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? RegisteredAt { get; set; }

        //Required to prevent crashes
        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}