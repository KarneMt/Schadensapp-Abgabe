using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle
        public async Task<IActionResult> Index(string searchString)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole != "Admin")
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Überprüfen, ob die Tabelle für Bearbeitungsstellen vorhanden ist. Wenn nicht, wird NotFound zurückgegeben.
            if (_context.Bearbeitungsstelles == null)
            {
                return NotFound();
            }

            // Abrufen aller Bearbeitungsstellen aus der Datenbank.
            List<Bearbeitungsstelle> bearbeitungsstellendb = await _context.Bearbeitungsstelles.ToListAsync();

            // Überprüfen, ob Bearbeitungsstellen in der Datenbank gefunden wurden.
            if (bearbeitungsstellendb != null)
            {
                // Erstellen einer Liste von BearbeitungsstellenView-Objekten, um Daten für die View aufzubereiten.
                List<BearbeitungsstellenView> bearbeitungsstellenViews = new List<BearbeitungsstellenView>();

                // Für jede Bearbeitungsstelle in der Datenbank wird ein BearbeitungsstellenView-Objekt erstellt und zur Liste hinzugefügt.
                foreach (var x in bearbeitungsstellendb)
                {
                    BearbeitungsstellenView item = new BearbeitungsstellenView()
                    {
                        Bearbeitungsstelle = x
                    };
                    bearbeitungsstellenViews.Add(item);
                };

                // Die Liste der Bearbeitungsstellen wird nach dem Namen der Bearbeitungsstelle sortiert.
                bearbeitungsstellenViews.Sort((x, y) => string.Compare(x.Bearbeitungsstelle.Name, y.Bearbeitungsstelle.Name));

                // Wenn ein Suchbegriff angegeben ist, wird die Liste der Bearbeitungsstellen entsprechend gefiltert.
                if (!string.IsNullOrEmpty(searchString))
                {
                    // Die Suche wird mit dem Suchbegriff durchgeführt und das Ergebnis wird in die View übertragen.
                    BearbeitungsstellenViewIndex bearbeitungsstellenViewIndexFiltered = new SearchService().SearchBearbeitungsstellen(bearbeitungsstellenViews, searchString);
                    return View(bearbeitungsstellenViewIndexFiltered);
                }

                // Wenn kein Suchbegriff angegeben ist, wird die gesamte Liste der Bearbeitungsstellen in die View übertragen.
                BearbeitungsstellenViewIndex bearbeitungsstellenViewIndex = new BearbeitungsstellenViewIndex
                {
                    BearbeitungsstellenView = bearbeitungsstellenViews
                };

                return View(bearbeitungsstellenViewIndex);
            }
            else
            {
                // Wenn keine Bearbeitungsstellen in der Datenbank gefunden wurden, wird NotFound zurückgegeben.
                return NotFound();
            }
        }

    }
}