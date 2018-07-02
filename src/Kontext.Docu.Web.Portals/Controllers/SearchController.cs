using Microsoft.AspNetCore.Mvc;

namespace Kontext.Docu.Web.Portals.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.SearchKeyWord = Request.Query["q"];
            return View();
        }
    }
}
