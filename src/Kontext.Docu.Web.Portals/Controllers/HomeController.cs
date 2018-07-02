using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kontext.Docu.Web.Portals.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace Kontext.Docu.Web.Portals.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DismissCookieBanner(string returnUrl)
        {
            Response.Cookies.Append(
                Constants.CookieNameForAcceptedKey, "Y",
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(90) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult ShowCookieBanner(string returnUrl)
        {
            Response.Cookies.Delete(Constants.CookieNameForAcceptedKey);
            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                Constants.CookieNameForRequestCulture,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
