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
        // GET: Dienstleister/Details/5
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

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Dienstleisters" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Dienstleisters == null)
            {
                return NotFound();
            }

            // Die entsprechenden Dienstleister in der Datenbank anhand der übergebenen ID suchen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == id);

            // Falls der Dienstleiter nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            // Die Adresse des Dienstleisters aus der Datenbank anhand der AdresseID abrufen.
            Adresse? adresse = new AddressService().GetAddress(dienstleister.AdresseID, _context);

            // Eine DienstleisterView-Instanz erstellen und mit den Daten aus der Datenbank befüllen.
            DienstleisterView dienstleisterView = new DienstleisterView
            {
                DienstleisterID = dienstleister.DienstleisterID,
                Abteilung = dienstleister.Abteilung,
                IsActive = dienstleister.IsActive,
                Firmenname = dienstleister.Firmenname,
                AdresseID = dienstleister.AdresseID,
                Hausnummer = adresse?.Hausnummer,
                Postleitzahl = adresse?.Postleitzahl,
                Stadt = adresse?.Stadt,
                Strasse = adresse?.Strasse,
                EMail = dienstleister.EMail
            };

            // Die Details-Ansicht mit der erstellten DienstleisterView anzeigen.
            return View(dienstleisterView);
        }
    }
}
