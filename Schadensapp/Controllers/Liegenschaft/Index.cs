using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class LiegenschaftController
    {
        // GET: Liegenschaft
        public async Task<IActionResult> Index(string searchString)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
            List<string>? groupList = HttpContext.Items["GroupList"] as List<string>;
            ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist oder der Benutzer keine Admin-Rolle hat,
            // wird zur Zugriffsverweigerungsseite weitergeleitet.
            if (groupList == null || userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "AccessDenied");
            }

            int bearbeitungsstellenID = 0;

            List<Liegenschaft> liegenschaft = new();
            foreach (string item in groupList)
            {
                // Die Gruppenliste enthält Bearbeitungsstellen-IDs. Wir versuchen, sie in Integer zu konvertieren.
                int.TryParse(item, out bearbeitungsstellenID);


                // Abrufen aller Liegenschaften, die mit der aktuellen Bearbeitungsstellen-ID verknüpft sind,
                // und sie der Liste der Liegenschaften hinzufügen.
                liegenschaft.AddRange(await _context.Liegenschafts.Where(x => x.BearbeitungsstelleID == bearbeitungsstellenID).ToListAsync());
            }

            var list = new List<LiegenschaftsView>();
            foreach (var x in liegenschaft)
            {
                // Abrufen der Adresse für die aktuelle Liegenschaft.
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);

                // Abrufen des zugehörigen Dienstleisters für die aktuelle Liegenschaft.
                var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == x.DienstleisterID);
                if (dienstleister == null)
                {
                    return NotFound();
                }

                // Abrufen der zugehörigen Bearbeitungsstelle für die aktuelle Liegenschaft.
                var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == x.BearbeitungsstelleID);
                if (bearbeitungsstelle == null)
                {
                    return NotFound();
                }

                // Erstellen eines LiegenschaftsView-Objekts, um die Daten für die View aufzubereiten.
                LiegenschaftsView dienstleisterView = new()
                {
                    LiegenschaftID = x.LiegenschaftID,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    Hausnummer = adresse?.Hausnummer,
                    Postleitzahl = adresse?.Postleitzahl,
                    Stadt = adresse?.Stadt,
                    Strasse = adresse?.Strasse,
                    DienstleisterFirmenname = dienstleister.Firmenname,
                    BearbeitungsstelleName = bearbeitungsstelle?.Name,
                };

                // Das LiegenschaftsView-Objekt wird der Liste hinzugefügt.
                list.Add(dienstleisterView);
            }

            // Die Liste der Liegenschaften wird nach dem Namen der Liegenschaft sortiert.
            list.Sort((x, y) => string.Compare(x.Name, y.Name));

            // Wenn ein Suchbegriff angegeben ist, wird die Liste der Liegenschaften entsprechend gefiltert,
            // und das Ergebnis wird in eine separate View übertragen.
            if (!string.IsNullOrEmpty(searchString))
            {
                LiegenschaftsViewIndex liegenschaftsViewFiltered = new SearchService().SearchLiegenschaften(list, searchString);
                return View(liegenschaftsViewFiltered);
            }

            // Wenn kein Suchbegriff angegeben ist, wird die gesamte Liste der Liegenschaften in die View übertragen.
            LiegenschaftsViewIndex liegenschaftsView = new()
            {
                LiegenschaftsView = list
            };
            return View(liegenschaftsView);
        }
    }
}
