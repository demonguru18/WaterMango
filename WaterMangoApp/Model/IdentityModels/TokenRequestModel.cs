namespace WaterMangoApp.Model.IdentityModels
{
    public class TokenRequestModel
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}