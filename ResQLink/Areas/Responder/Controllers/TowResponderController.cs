using Microsoft.AspNetCore.Mvc;

namespace ResQLink.Areas.Responder.Controllers
{
    public class TowResponderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
