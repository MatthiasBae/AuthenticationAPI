using AuthenticateUserApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthenticateUserApi.Services {
    public interface ITokenService {
        
        public JwtSecurityToken GenerateAccessToken(List<Claim> claims);
        public string GenerateRefreshToken();
    }
}
