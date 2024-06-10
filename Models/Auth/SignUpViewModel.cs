using GetFitApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Auth
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default!;

        [Display(Name = "User Type")]
        [Required(ErrorMessage = "Please select a type")]
        public UserType UserType { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The entered password and confirm password do not match.")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
