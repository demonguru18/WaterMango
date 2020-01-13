using System;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WaterMangoApp.Data;
using WaterMangoApp.Factory;
using WaterMangoApp.Helpers;
using WaterMangoApp.Model.AccountModels;
using WaterMangoApp.Model.HttpModels;
using WaterMangoApp.Model.IdentityModels;

namespace WaterMangoApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly AppSettings _appSettings;

        private readonly ApplicationDbContext _db;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, ApplicationDbContext db)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _db = db;
        }

        public async Task<TokenResponseModel> Auth(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var rememberMe = model.RememberMe;
            
            if (user == null) return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
            
            var roles = await _userManager.GetRolesAsync(user);
            
            if (roles.FirstOrDefault() != "Admin") return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
            
            if (!await _userManager.CheckPasswordAsync(user, model.Password)) return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
            
            if (!await _userManager.IsEmailConfirmedAsync(user)) return CreateErrorResponseToken("Email Not Confirmed", HttpStatusCode.Unauthorized);

            try
            {
                var authToken = await GenerateNewToken(user, model);
                return authToken;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not create a token => {ex.Message}");
            }
            return CreateErrorResponseToken("Request Not Supported", HttpStatusCode.Unauthorized);
        }
        
        // Creating JWT Authentication Token
        private async Task<TokenResponseModel> GenerateNewToken(ApplicationUser user, LoginViewModel model)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            var roles = await _userManager.GetRolesAsync(user); 
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim("LoggedOn", DateTime.Now.ToString(CultureInfo.InvariantCulture)),           
                }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Site,
                Audience = _appSettings.Audience,
                Expires = (string.Equals(roles.FirstOrDefault(), "Admin", StringComparison.CurrentCultureIgnoreCase)) ? DateTime.UtcNow.AddMinutes(60) : DateTime.UtcNow.AddMinutes(Convert.ToDouble(_appSettings.ExpireTime))
            };
            
            var userClaims = await _userManager.GetClaimsAsync(user);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            var authToken = new TokenResponseModel
            {
                Token = token.ToString(),
                Expiration = token.ValidTo,
                Role = roles.FirstOrDefault(),
                Username = user.UserName,
                UserId = user.Id,
                ResponseInfo = CreateResponse("Auth Token Created", HttpStatusCode.OK)
            };

            return authToken;
        }

        private static TokenResponseModel CreateErrorResponseToken(string errorMessage, HttpStatusCode statusCode)
        {
            var errorToken = new TokenResponseModel
            {
                Token = null,
                Username = null,
                Role = null,
                Expiration = DateTime.Now,
                ResponseInfo = CreateResponse(errorMessage, statusCode)
            };

            return errorToken;
        }
        
        private static ResponseStatusInfoModel CreateResponse(string errorMessage, HttpStatusCode statusCode)
        {
            var responseStatusInfo = new ResponseStatusInfoModel
            {
                Message = errorMessage, StatusCode = statusCode
            };

            return responseStatusInfo;
        }
    }
}