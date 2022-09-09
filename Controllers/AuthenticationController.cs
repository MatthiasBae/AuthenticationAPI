using AuthenticateUserApi.Models;
using AuthenticateUserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticateUserApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller {

        private readonly UserContext UserContext;
        private readonly UserManager<AppUser> UserManager;
        private readonly SignInManager<AppUser> SignInManager;

        public AuthenticationController(UserContext userContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            this.SignInManager = signInManager;
            this.UserManager = userManager;
        }


        [HttpPost]
        public async Task<IActionResult> Register(string username, string mail, string password) {
            var userMailUsed = this.UserManager.FindByEmailAsync(mail) == null
                ? false
                : true;

            var userNameUsed = this.UserManager.FindByNameAsync(username) == null
                ? false
                : true;

            if(userMailUsed || userNameUsed) {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response {
                        Status = "Error",
                        Message = "Username or Mail already in use"
                    }
                );
            }


            var user = new AppUser { 
                UserName = username, 
                Email = mail, 
                Password = password 
            };

            user.PasswordHash = this.UserManager.PasswordHasher.HashPassword(user,password);

            var result = await this.UserManager.CreateAsync(user);
            if(!result.Succeeded) {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response {
                        Status = "Error",
                        Message = "Could not create the user. Please check your credentials"
                    }
               );
            }

            return StatusCode(
                StatusCodes.Status201Created,
                    new Response {
                        Status = "Success",
                        Message = "User has been created successfully"
                    }
            );
        }

        public async Task<IActionResult> Login(string mail,string password) {

            var user = await this.UserContext.Users
                .Where(item => item.Email == mail && item.PasswordHash == new PasswordHasher(null).VerifyHashedPassword();
            if(user == null) {
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    new Response {
                        Status = "Error",
                        Message = "Credentials not correct"
                    }
                );
            }

            var isPasswordValid = user.PasswordHash==this.UserManager.PasswordHasher.HashPassword(user,password)
                ? true
                : false;

            if(!isPasswordValid) {
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    new Response {
                        Status = "Error",
                        Message = "Credentials not correct"
                    }
                );
            }


            }
    }
}
