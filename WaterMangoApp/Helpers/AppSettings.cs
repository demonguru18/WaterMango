namespace WaterMangoApp.Helpers
{
    public class AppSettings
    {
        /*---------------------------------------------------------------------------------------------------*/
        /*                              Properties for JWT Token Signature                                   */
        /*---------------------------------------------------------------------------------------------------*/
        public string Site { get; set; }
        public string Audience { get; set; }
        public string ExpireTime { get; set; }
        public string Secret { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public string ClientId { get; set; }
    }
}