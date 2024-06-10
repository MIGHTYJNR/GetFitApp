using GetFitApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace GetFitApp.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        /*[Display(Name = "User Type")]
        [Required(ErrorMessage = "Please select a type")]*/
        public UserType UserType { get; set; } = default!;
    }
}
