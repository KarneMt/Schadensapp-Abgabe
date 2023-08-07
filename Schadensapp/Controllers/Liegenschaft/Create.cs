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
        // GET: Liegenschaft/Create
        public async Task<IActionResult> Create()
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Datenbankabfrage, um alle aktiven Dienstleister zu laden.
            var dbdienstleister = await _context.Dienstleisters.Where(x => x.IsActive == true).ToListAsync();

            // Eine Liste, um die Dienstleister als DienstleisterView-Objekte zu speichern.
            var dienstleisterlist = new List<DienstleisterView>();

            // Durchlaufen der geladenen Dienstleister und Erstellen von DienstleisterView-Objekten.
            foreach (var x in dbdienstleister)
            {
                // Die Adresse des aktuellen Dienstleisters aus der Datenbank abrufen.
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);

                // Ein neues DienstleisterView-Objekt erstellen und mit den Daten aus der Datenbank befüllen.
                DienstleisterView dienstleisterView = new()
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
                };

                // Das DienstleisterView-Objekt der Liste hinzufügen.
                dienstleisterlist.Add(dienstleisterView);
            }

            // Datenbankabfrage, um alle aktiven Bearbeitungsstellen zu laden.
            List<Bearbeitungsstelle> bearbeitungsstellen = await _context.Bearbeitungsstelles.Where(x => x.IsActive == true).ToListAsync();

            // Ein neues LiegenschaftsView-Objekt erstellen.
            LiegenschaftsView liegenschaftsView = new();

            // Die geladenen Dienstleister und Bearbeitungsstellen dem LiegenschaftsView-Objekt zuweisen.
            liegenschaftsView.DienstleistersListe = dienstleisterlist;
            liegenschaftsView.BearbeitungsstellenListe = bearbeitungsstellen;

            // Die Ansicht mit dem LiegenschaftsView-Objekt zurückgeben.
            return View(liegenschaftsView);
        }

        // POST: Liegenschaft/Create
        // Aktion zum Erstellen einer neuen Liegenschaft basierend auf den Daten aus der "LiegenschaftsView".
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LiegenschaftID,Name,Strasse,Hausnummer,Postleitzahl,Stadt,DienstleisterID,BearbeitungsstelleID")] LiegenschaftsView liegenschaftsView)
        {
            // Eine neue Adresse erstellen und mit den relevanten Daten aus der "LiegenschaftsView" befüllen.
            Adresse adresse = new()
            {
                Hausnummer = liegenschaftsView.Hausnummer,
                Stadt = liegenschaftsView.Stadt,
                Strasse = liegenschaftsView.Strasse,
                Postleitzahl = liegenschaftsView.Postleitzahl,
            };

            // Die Adresse mit Hilfe des AddressService erstellen und in der Datenbank speichern.
            Adresse? adresseErstellt = new AddressService().Create(adresse, _context);

            // Falls die Adresse nicht erstellt werden konnte, NotFound zurückgeben.
            if (adresseErstellt == null)
            {
                return NotFound();
            }

            // Eine neue Liegenschaft-Instanz erstellen und mit den relevanten Daten aus der "LiegenschaftsView" befüllen.
            Liegenschaft liegenschaft = new()
            {
                Name = liegenschaftsView.Name,
                AdresseID = adresseErstellt.AdresseID,
                IsActive = true,
                BearbeitungsstelleID = liegenschaftsView.BearbeitungsstelleID,
                DienstleisterID = liegenschaftsView.DienstleisterID,
            };

            // Überprüfen, ob das Model gültig ist.
            if (ModelState.IsValid)
            {
                // Die Liegenschaft-Instanz zur Datenbank hinzufügen.
                _context.Add(liegenschaft);
                await _context.SaveChangesAsync();

                // Nach dem Speichern zur Index-Ansicht weiterleiten.
                return RedirectToAction(nameof(Index));
            }

            // Falls das Model ungültig ist, die Ansicht mit der ursprünglichen Liegenschaft-Instanz zurückgeben.
            return View(liegenschaft);
        }
    }
}
