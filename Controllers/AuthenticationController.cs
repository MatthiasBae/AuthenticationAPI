using AuthenticateUserApi.Models;
using AuthenticateUserApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticateUserApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller {

        private readonly UserContext UserContext;
        private readonly UserManager<AppUser> UserManager;
        private readonly SignInManager<AppUser> SignInManager;
        private readonly IConfiguration Configuration;
        private readonly ITokenService TokenService;

        public AuthenticationController(UserContext userContext, IConfiguration configuration, ITokenService tokenService, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            this.SignInManager = signInManager;
            this.UserManager = userManager;
            this.Configuration = configuration;
            this.UserContext = userContext;
            this.TokenService = tokenService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(string username, string mail, string password) {
            var userMailUsed = await this.UserManager.FindByEmailAsync(mail) == null
                ? false
                : true;

            var userNameUsed = await this.UserManager.FindByNameAsync(username) == null
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
            };

            var result = await this.UserManager.CreateAsync(user,password);
            if(!result.Succeeded) {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response {
                        Status = "Error",
                        Message = "Could not create the user. Please check your credentials"
                    }
                );
            }

            var roleResult = await this.UserManager.AddToRoleAsync(user,"User");
            if(!roleResult.Succeeded) {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new Response {
                        Status = "Error",
                        Message = "Created the user but could not assign the user role"
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

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string mail,string password) {
            //@TODO: implement lockout after to many attempts
            var user = await this.UserManager.FindByEmailAsync(mail);
            var isPwValid = await this.UserManager.CheckPasswordAsync(user,password);

            if(user == null || !isPwValid) {
                return Unauthorized();
            }

            var authClaims = new List<Claim> {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var userRoles = await this.UserManager.GetRolesAsync(user);

            foreach(var role in userRoles) {
                authClaims.Add(new Claim(ClaimTypes.Role,role));
            }
            var accessToken = this.TokenService.GenerateAccessToken(authClaims);
            var refreshToken = this.TokenService.GenerateRefreshToken();
            var refreshTokenLifetime = int.Parse(this.Configuration["Jwt:RefreshTokenLifetime"]);

            user.RefreshToken = refreshToken;
            user.RefreshTokenValidTo = DateTime.Now.AddMinutes(refreshTokenLifetime);

            var result = await this.UserManager.UpdateAsync(user);

            if(!result.Succeeded) {
                return Unauthorized();
            }

            return Ok(new {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            });
        }

        [HttpGet]
        [Route("refresh-tokens")]
        public async Task<IActionResult> RefreshTokens([FromBody] TokenModel tokens) {
            var principals = this.TokenService.GetPrincipalFromExpiredToken(tokens.AccessToken);

            if(principals == null) {
                return BadRequest("Invalid accesstoken or refreshtoken");
            }

            string username = principals.Identity.Name;

            var user = await this.UserManager.FindByNameAsync(username);
            if(user == null || tokens.RefreshToken != user.RefreshToken || user.RefreshTokenValidTo <= DateTime.Now) {
                return BadRequest("Invalid accesstoken or refreshtoken");
            }

            var newAccessToken = this.TokenService.GenerateAccessToken(principals.Claims.ToList());
            var newRefreshToken = this.TokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await this.UserManager.UpdateAsync(user);

            return StatusCode(
                StatusCodes.Status201Created,
                    new {
                        AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                        RefreshToken = newRefreshToken
                    }
            );
        }
    }
}
