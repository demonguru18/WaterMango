using System;
using WaterMangoApp.Model.HttpModels;

namespace WaterMangoApp.Model.IdentityModels
{
    public class TokenResponseModel
    {
        public string Token { get; set; } // jwt token
        public DateTime Expiration { get; set; } // expiry time
        public string Role { get; set; } // user role
        public string Username { get; set; } // user name
        public string UserId { get; set; } // user id
        public ResponseStatusInfoModel ResponseInfo { get; set; } // user name
    }
}