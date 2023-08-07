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
        // GET: Liegenschaft/Details/5
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

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Liegenschafts" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Liegenschafts == null)
            {
                return NotFound();
            }

            // Die entsprechende Liegenschaft in der Datenbank anhand der übergebenen ID suchen.
            var liegenschaft = await _context.Liegenschafts.FirstOrDefaultAsync(m => m.LiegenschaftID == id);

            // Falls die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (liegenschaft == null)
            {
                return NotFound();
            }

            // Eine leere Liste von LiegenschaftsView erstellen.
            var list = new List<LiegenschaftsView>();

            // Die Adresse der Liegenschaft aus der Datenbank anhand der AdresseID abrufen.
            Adresse? adresse = new AddressService().GetAddress(liegenschaft.AdresseID, _context);

            // Den Dienstleister der Liegenschaft aus der Datenbank abrufen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == liegenschaft.DienstleisterID);

            // Falls der Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            // Die Adresse des Dienstleisters aus der Datenbank anhand der AdresseID abrufen.
            Adresse? adresseDienstleister = new AddressService().GetAddress(dienstleister.AdresseID, _context);

            // Die Bearbeitungsstelle der Liegenschaft aus der Datenbank abrufen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == liegenschaft.BearbeitungsstelleID);

            // Falls die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle == null)
            {
                return NotFound();
            }

            // Eine Instanz von LiegenschaftsView erstellen und mit den Daten aus der Datenbank befüllen.
            LiegenschaftsView dienstleisterView = new()
            {
                LiegenschaftID = liegenschaft.LiegenschaftID,
                Name = liegenschaft.Name,
                IsActive = liegenschaft.IsActive,
                AdresseID = liegenschaft.AdresseID,
                Hausnummer = adresse?.Hausnummer,
                Postleitzahl = adresse?.Postleitzahl,
                Stadt = adresse?.Stadt,
                Strasse = adresse?.Strasse,
                DienstleisterID = dienstleister.DienstleisterID,
                DienstleisterFirmenname = dienstleister.Firmenname,
                DienstleisterAbteilung = dienstleister.Abteilung,
                DienstleisterStrasse = adresseDienstleister?.Strasse,
                DienstleisterHausnummer = adresseDienstleister?.Hausnummer,
                DienstleisterPostleitzahl = adresseDienstleister?.Postleitzahl,
                DienstleisterStadt = adresseDienstleister?.Stadt,
                BearbeitungsstelleID = bearbeitungsstelle.BearbeitungsstelleID,
                BearbeitungsstelleName = bearbeitungsstelle.Name,
                BearbeitungsstelleMapGruppe = bearbeitungsstelle.MapGruppe,
                BearbeitungsstelleTelefone = bearbeitungsstelle.Telefone,
                BearbeitungsstelleEMail = bearbeitungsstelle.EMail,
            };

            // Die Details-Ansicht mit der erstellten LiegenschaftsView anzeigen.
            return View(dienstleisterView);
        }
    }
}
