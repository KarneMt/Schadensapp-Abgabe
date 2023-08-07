using Microsoft.AspNetCore.Mvc;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle/Create
        public IActionResult Create()
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Zur View zurückgehen.
            return View();
        }

        // POST: Bearbeitungsstelle/Create
        // Aktion zum Erstellen einer neuen Bearbeitungsstelle basierend auf den Daten aus der "Bearbeitungsstelle".
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BearbeitungsstelleID,MapGruppe,Name,Telefone,EMail")] Bearbeitungsstelle bearbeitungsstelle)
        {
            // Die neu erstellte Bearbeitungsstelle als aktiv markieren.
            bearbeitungsstelle.IsActive = true;

            // Überprüfen, ob das Model gültig ist.
            if (ModelState.IsValid)
            {
                // Die Bearbeitungsstelle zur Datenbank hinzufügen.
                _context.Add(bearbeitungsstelle);
                await _context.SaveChangesAsync();

                // Nach dem Speichern zur Index-Ansicht weiterleiten.
                return RedirectToAction(nameof(Index));
            }

            // Falls das Model ungültig ist, die Bearbeitungsstelle in das BearbeitungsstellenView-Modell übertragen.
            BearbeitungsstellenView bearbeitungsstellenView = new()
            {
                Bearbeitungsstelle = bearbeitungsstelle
            };

            // Die Ansicht mit dem BearbeitungsstellenView-Modell zurückgeben.
            return View(bearbeitungsstellenView);
        }
    }
}
