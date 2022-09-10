using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthenticateUserApi.Models {
    public class AppUser : IdentityUser {

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenValidTo { get; set; }
    }
}
