using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle/Details/5
        // Aktion zum Anzeigen der Details einer Bearbeitungsstelle anhand der übergebenen ID.
        public async Task<IActionResult> Details(int? id)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Bearbeitungsstelles" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Bearbeitungsstelles == null)
            {
                return NotFound();
            }

            // Die entsprechende Bearbeitungsstelle in der Datenbank anhand der übergebenen ID suchen.
            var bearbeitungsstelleDb = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == id);

            // Falls die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelleDb == null)
            {
                return NotFound();
            }

            // Eine BearbeitungsstellenView-Instanz erstellen und die gefundene Bearbeitungsstelle zuweisen.
            BearbeitungsstellenView bearbeitungsstellenView = new BearbeitungsstellenView
            {
                Bearbeitungsstelle = bearbeitungsstelleDb
            };

            // Die Details-Ansicht mit der BearbeitungsstellenView anzeigen.
            return View(bearbeitungsstellenView);
        }
    }
}
