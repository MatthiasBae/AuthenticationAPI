﻿using AuthenticateUserApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticateUserApi.Services {
    public class TokenService : ITokenService {

        private readonly IConfiguration Configuration;

        public TokenService(IConfiguration configuration) {
            this.Configuration = configuration;
        }

        public JwtSecurityToken GenerateAccessToken(List<Claim> claims) {
            //https://www.c-sharpcorner.com/article/jwt-authentication-with-refresh-tokens-in-net-6-0/
            var configKey = this.Configuration["Jwt:Key"];
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));
            var tokenValidInMinutes = int.Parse(this.Configuration["Jwt:AccessTokenLifetime"]);
            

            var token = new JwtSecurityToken(
                issuer: this.Configuration["JWT:ValidIssuer"],
                audience: this.Configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(authKey,SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        public string GenerateRefreshToken() {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token) {
            var tokenValidationParameters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token,tokenValidationParameters,out SecurityToken securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token");
            }
            
            return principal;

        }
    }
}
