using Microsoft.AspNetCore.Mvc;

namespace ResQLink.Areas.Responder.Controllers
{
    public class FiretruckResponderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
