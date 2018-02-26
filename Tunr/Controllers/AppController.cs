using Microsoft.AspNetCore.Mvc;

namespace Tunr.Controllers
{
    public class AppController: Controller
    {
        public ActionResult Index()
        {
            return File("~/index.html", "text/html");
        }
    }
}