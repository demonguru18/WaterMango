using Microsoft.AspNetCore.Identity;

namespace WaterMangoApp.Model.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public string UserRole { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Country { get; set; }
    }
}