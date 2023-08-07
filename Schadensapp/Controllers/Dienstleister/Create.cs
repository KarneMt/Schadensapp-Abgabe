using Microsoft.AspNetCore.Mvc;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class DienstleisterController
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

        // POST: Dienstleister/Create
        // Aktion zum Erstellen eines neuen Dienstleister basierend auf den Daten aus der "DienstleisterView".
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Firmenname,Abteilung,Strasse,Hausnummer,Postleitzahl,Stadt,EMail")] DienstleisterView viewdata)
        {
            // Eine Adresse erstellen und mit den relevanten Daten aus der DienstleisterView befüllen.
            Adresse adresse = new()
            {
                Hausnummer = viewdata.Hausnummer,
                Stadt = viewdata.Stadt,
                Strasse = viewdata.Strasse,
                Postleitzahl = viewdata.Postleitzahl,
            };

            // Die Adresse mit Hilfe des AddressService erstellen und in der Datenbank speichern.
            Adresse? adresse1 = new AddressService().Create(adresse, _context);

            // Falls die Adresse nicht erstellt werden konnte, NotFound zurückgeben.
            if (adresse1 == null)
            {
                return NotFound();
            }

            // Eine neue Dienstleister-Instanz erstellen und mit den relevanten Daten aus der DienstleisterView befüllen.
            Dienstleister dienstleister = new()
            {
                Abteilung = viewdata.Abteilung,
                Firmenname = viewdata.Firmenname,
                EMail = viewdata.EMail,
                IsActive = true,
                AdresseID = adresse1.AdresseID,
            };

            // Überprüfen, ob die Dienstleister-Instanz nicht null ist.
            if (dienstleister != null)
            {
                // Die Dienstleister-Instanz zur Datenbank hinzufügen.
                _context.Add(dienstleister);
                await _context.SaveChangesAsync();

                // Nach dem Speichern zur Index-Ansicht weiterleiten.
                return RedirectToAction(nameof(Index));
            }

            // Falls die Dienstleister-Instanz null ist, die Ansicht mit der ursprünglichen DienstleisterView zurückgeben.
            return View(viewdata);
        }
    }
}
