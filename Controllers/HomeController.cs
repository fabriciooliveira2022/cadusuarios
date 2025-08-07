using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace SeuProjeto.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Nome = HttpContext.Session.GetString("Nome");
            ViewBag.Foto = HttpContext.Session.GetString("Foto");
            return View();
        }
    }
}