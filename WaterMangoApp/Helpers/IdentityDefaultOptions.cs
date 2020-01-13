namespace WaterMangoApp.Helpers
{
    public class IdentityDefaultOptions
    {
        /*---------------------------------------------------------------------------------------------------*/
        /*                              Password Properties                                                  */
        /*---------------------------------------------------------------------------------------------------*/
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public int PasswordRequiredUniqueChars { get; set; }
        /*---------------------------------------------------------------------------------------------------*/
        /*                              Lockout Properties                                                   */
        /*---------------------------------------------------------------------------------------------------*/
        public double LockoutDefaultLockoutTimeSpanInMinutes { get; set; }
        public int LockoutMaxFailedAccessAttempts { get; set; }
        public bool LockoutAllowedForNewUsers { get; set; }
        /*---------------------------------------------------------------------------------------------------*/
        /*                              User Properties                                                      */
        /*---------------------------------------------------------------------------------------------------*/
        public bool UserRequireUniqueEmail { get; set; }
        public bool SignInRequireConfirmedEmail { get; set; }
        /*---------------------------------------------------------------------------------------------------*/
        /*                              Cookie Properties                                                    */
        /*---------------------------------------------------------------------------------------------------*/
        public bool CookieHttpOnly { get; set; }
        public double CookieExpiration { get; set; }
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string AccessDeniedPath { get; set; }
        public string AccessDeniedPathAdmin { get; set; }
        public bool SlidingExpiration { get; set; }
    }
}