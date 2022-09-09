using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthenticateUserApi.Models {
    public class User : IdentityUser {
        public int UserId { get; set; }

        [Required]
        public int UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, PasswordPropertyText(true)]
        public string Password { get; set; }

    }
}
