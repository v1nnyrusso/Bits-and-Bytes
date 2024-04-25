using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Bits_And_Bytes_Vincenzo_Russo.Models.ViewModels
{
    public class IndexViewModel
    {
        //Indicates whether the user has set a password
        public bool HasPassword { get; set; }

        //List of user login information
        public IList<UserLoginInfo> Logins { get; set; }

        //User's phone number
        public string PhoneNumber { get; set; }

        //Indicates whether two-factor authentication is enabled for the user
        public bool TwoFactor { get; set; }

        //Indicates whether the browser should remember the user
        public bool BrowserRemembered { get; set; }
    }

    public class ManageLoginsViewModel
    {
        //List of current user logins
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        //List of other login options available
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        //The purpose of the factor (e.g., adding a phone number, changing password, etc.)
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        //The new password entered by the user
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        //The confirmation of the new password entered by the user
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        //The user's current password
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        //The new password entered by the user
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        //The confirmation of the new password entered by the user
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        //The phone number to be added by the user
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        //The verification code entered by the user
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        //The phone number to be verified
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        //The selected two-factor authentication provider
        public string SelectedProvider { get; set; }

        //Collection of available two-factor authentication providers
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}
