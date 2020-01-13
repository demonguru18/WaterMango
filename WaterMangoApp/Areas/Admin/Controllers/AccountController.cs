using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WaterMangoApp.Factory;
using WaterMangoApp.Model.AccountModels;
using WaterMangoApp.Model.HttpModels;
using WaterMangoApp.Model.IdentityModels;

namespace WaterMangoApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private const string AccessToken = "access_token";
        private const string UserId = "user_id";
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CookieOptions _cookieOptions;
        private readonly IServiceProvider _provider;
        


        public AccountController(IAuthService authService, ILogger logger, CookieOptions cookieOptions, IHttpContextAccessor httpContextAccessor, IServiceProvider provider)
        {
            _logger = logger;
            _authService = authService;
            _cookieOptions = cookieOptions;
            _httpContextAccessor = httpContextAccessor;
            _provider = provider;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            
            await Task.Delay(0);
            
            try
            {
                // Check if user is already logged in 
                if (!Request.Cookies.ContainsKey(AccessToken) || !Request.Cookies.ContainsKey(UserId))
                {
                    return View();
                }
                
                // Add code to validate cookie
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
          
            try
            {
                var userId = _httpContextAccessor.HttpContext.Request.Cookies["user_id"];
                if (userId != null)
                {
                    string[] cookiesToDelete = { "access_token", "user_id" };
                    DeleteAllCookies(cookiesToDelete);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error($"The following error Occured {ex.Message}");
            }
            _logger.Information("User logged out.");
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                try
                {
                    var jwtToken = await _authService.Auth(model);
                    var protectorProvider = _provider.GetService<IDataProtectionProvider>();
                    var protector = protectorProvider.CreateProtector("WaterMangoTokenProtector");
                    var protectedToken = protector.Protect(jwtToken.Token);
                    var protectedUserId = protector.Protect(jwtToken.UserId);
                    const int expireTime = 60; 
                    SetCookie("access_token", protectedToken, expireTime);
                    SetCookie("user_id", protectedUserId, expireTime);
                }
                catch (Exception e)
                {
                    _logger.Error("Invalid Username/Password was entered");
                    return Unauthorized("Authentication Failed");
                }
                _logger.Information($"User logged in. {model.Email}");
                return RedirectToLocal(returnUrl);
            }
            
            // return error
            ModelState.AddModelError("", "Username/Password was not Found");
            _logger.Error("Invalid Username/Password was entered");
            return Unauthorized(new { LoginError = "Please Check the Login Credentials - Invalid Username/Password was entered" });
        }
        
        // Method to Create Error Response
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
        private void SetCookie(string key, string value, int? expireTime)
        {
            if (expireTime.HasValue)
            {
                _cookieOptions.Secure = true;
                _cookieOptions.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            }
            else
            {
                _cookieOptions.Secure = true;
                _cookieOptions.Expires = DateTime.Now.AddMilliseconds(10);
            }

            _cookieOptions.HttpOnly = true;
            _cookieOptions.SameSite = SameSiteMode.Strict;
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, _cookieOptions);

        }
        private void DeleteCookie(string key)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
        }
        private string Get(string key)
        {
            return _httpContextAccessor.HttpContext.Request.Cookies[key];
        }
        private void DeleteAllCookies(IEnumerable<string> cookiesToDelete)
        {
            foreach (var key in cookiesToDelete)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
            }
        }
        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? (IActionResult) Redirect(returnUrl)
                : RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}