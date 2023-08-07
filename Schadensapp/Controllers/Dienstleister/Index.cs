using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class DienstleisterController
    {
        // GET: Dienstleister
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

            // Überprüfen, ob die Tabelle für Dienstleisterstellen vorhanden ist. Wenn nicht, wird NotFound zurückgegeben.
            if (_context.Dienstleisters == null)
            {
                return NotFound();
            }

            // Abrufen aller Dienstleister aus der Datenbank.
            var dbdienstleister = await _context.Dienstleisters.ToListAsync();

            // Für jeden Dienstleister in der Datenbank wird ein DienstleisterView-Objekt erstellt und zur Liste hinzugefügt.
            var list = new List<DienstleisterView>();
            foreach (var x in dbdienstleister)
            {
                //Abrufen der Adresse des jeweiligen Dienstleisters
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);
                DienstleisterView dienstleisterView = new DienstleisterView
                {
                    DienstleisterID = x.DienstleisterID,
                    Abteilung = x.Abteilung,
                    IsActive = x.IsActive,
                    Firmenname = x.Firmenname,
                    AdresseID = x.AdresseID,
                    Hausnummer = adresse?.Hausnummer,
                    Postleitzahl = adresse?.Postleitzahl,
                    Stadt = adresse?.Stadt,
                    Strasse = adresse?.Strasse,
                    EMail = x.EMail
                };
                list.Add(dienstleisterView);
            }

            // Die Liste der Dienstleister wird nach dem Namen der Bearbeitungsstelle sortiert.
            list.Sort((x, y) => string.Compare(x.Firmenname, y.Firmenname));

            // Wenn ein Suchbegriff angegeben ist, wird die Liste der Dienstleister entsprechend gefiltert.
            if (!string.IsNullOrEmpty(searchString))
            {
                DienstleisterViewIndex dienstleisterViewFiltered = new SearchService().SearchDienstleister(list, searchString);
                return View(dienstleisterViewFiltered);
            }

            // Wenn kein Suchbegriff angegeben ist, wird die gesamte Liste der Dienstleister in die View übertragen.
            DienstleisterViewIndex dienstleisterViewIndex = new DienstleisterViewIndex
            {
                DienstleisterView = list
            };

            return View(dienstleisterViewIndex);
        }
    }
}
