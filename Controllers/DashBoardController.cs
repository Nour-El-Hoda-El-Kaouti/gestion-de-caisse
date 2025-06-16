using Microsoft.AspNetCore.Mvc;

namespace gestion_de_caisse.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
