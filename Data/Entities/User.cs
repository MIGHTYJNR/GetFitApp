using GetFitApp.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace GetFitApp.Data.Entities
{
    public class User : IdentityUser
    {
        public UserType UserType { get; set; }
    }
}
