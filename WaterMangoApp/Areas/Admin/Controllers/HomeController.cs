using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WaterMangoApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private const string AccessToken = "access_token";
        private const string User_Id = "user_id";
        public IActionResult Index()
        {
            if (!Request.Cookies.ContainsKey(AccessToken) || !Request.Cookies.ContainsKey(User_Id))
            {
                // Authorization header not found.
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }
            return View();
        }

    }
}