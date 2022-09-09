using AuthenticateUserApi.Models;

namespace AuthenticateUserApi.Services {
    public interface ITokenService {
        public string GenerateAccessToken(AppUser user);
        public string GenerateRefreshToken();
    }
}
