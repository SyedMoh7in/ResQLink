using Microsoft.AspNetCore.Mvc;

namespace ResQLink.Areas.Responder.Controllers
{
    [Area("Responder")]
    public class MedResponderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
