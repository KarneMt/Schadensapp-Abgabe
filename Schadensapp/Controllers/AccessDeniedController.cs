using Microsoft.AspNetCore.Mvc;

namespace Schadensapp.Controllers
{
    public class AccessDeniedController : Controller
	{
		public IActionResult AccessDenied()
		{
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
            ViewBag.UserRole = userRole;

            return View();
		}

        public IActionResult ReadError()
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
            ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
            {
                return RedirectToAction("AccessDenied", "AccessDenied");
            }
            return View();
        }
    }
}
