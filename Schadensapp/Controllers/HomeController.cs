using Microsoft.AspNetCore.Mvc;
using Schadensapp.Database.Context;
using Schadensapp.Models;
using System.Diagnostics;
using Schadensapp.Services.Mail;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
	[Authorize]
    /*
     Die [Authorize]-Anmerkung ist ein Attribut in ASP.NET Core, das auf Controller oder Aktionen angewendet wird, 
     um sicherzustellen, dass nur authentifizierte Benutzer auf diese Ressourcen zugreifen können. 
     Es ist ein wesentlicher Bestandteil der Authentifizierung und Autorisierung in ASP.NET Core-Anwendungen.
     */

    // Dies ist der Controller für die Entität "Meldung".
    public partial class HomeController : Controller
    {
        // Das ILogger-Objekt wird für das Logging verwendet, um Informationen, Warnungen und Fehler zu protokollieren.
        private readonly ILogger<HomeController> _logger;

        // Der Datenbankkontext für die Anwendung.
        private readonly SchadensappDbContext _context;

        // Der IEmailSender wird für den E-Mail-Versanddienst verwendet, um E-Mails zu senden.
        private readonly IEmailSender mailService;

        // Das IHttpContextAccessor-Objekt wird verwendet, um auf den HTTP-Kontext der aktuellen Anfrage zuzugreifen.
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Konstruktor des Controllers, der die Abhängigkeiten über Dependency Injection erhält.
        public HomeController(ILogger<HomeController> logger, SchadensappDbContext context, IEmailSender mailService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _context = context;
            this.mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult PrivacyAsync()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ErrorAsync()
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDeniedAsync()
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

        // Hilfsmethode, die prüft, ob eine Meldung mit der angegebenen ID existiert.
        private bool MeldungExists(string id)
        {
            return (_context.Meldungs?.Any(e => e.MeldungID == id)).GetValueOrDefault();
        }
    }
}