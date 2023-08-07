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
        // GET: Liegenschaft/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            // Suchen der Liegenschaft in der Datenbank anhand der übergebenen ID.
            var liegenschaft = await _context.Liegenschafts.FindAsync(id);
            if (liegenschaft == null)
            {
                return NotFound();
            }

            // Holen der Adresse für die Liegenschaft aus dem AddressService.
            Adresse? adresseLiegenschaft = new AddressService().GetAddress(liegenschaft.AdresseID, _context);

            // Suchen des Dienstleisters für die Liegenschaft in der Datenbank.
            var liegenschaftDienstleister = await _context.Dienstleisters.FindAsync(liegenschaft.DienstleisterID);
            if (liegenschaftDienstleister == null)
            {
                return NotFound();
            }

            // Holen der Adresse für den Dienstleister aus dem AddressService.
            Adresse? adresseLiegenschaftDienstleister = new AddressService().GetAddress(liegenschaftDienstleister.AdresseID, _context);

            // Suchen der Bearbeitungsstelle für die Liegenschaft in der Datenbank.
            var liegenschaftBearbeitungsstelle = await _context.Bearbeitungsstelles.FindAsync(liegenschaft.BearbeitungsstelleID);
            if (liegenschaftBearbeitungsstelle == null)
            {
                return NotFound();
            }

            // Holen aller aktiven Dienstleister aus der Datenbank.
            var dbdienstleister = await _context.Dienstleisters.Where(x => x.IsActive == true).ToListAsync();
            var dienstleisterlist = new List<DienstleisterView>();
            foreach (var x in dbdienstleister)
            {
                // Holen der Adresse für den aktuellen Dienstleister aus dem AddressService.
                Adresse? adresseDienstleister = new AddressService().GetAddress(x.AdresseID, _context);
                DienstleisterView dienstleisterView = new()
                {
                    DienstleisterID = x.DienstleisterID,
                    Abteilung = x.Abteilung,
                    IsActive = x.IsActive,
                    Firmenname = x.Firmenname,
                    AdresseID = x.AdresseID,
                    Hausnummer = adresseDienstleister?.Hausnummer,
                    Postleitzahl = adresseDienstleister?.Postleitzahl,
                    Stadt = adresseDienstleister?.Stadt,
                    Strasse = adresseDienstleister?.Strasse,
                };
                dienstleisterlist.Add(dienstleisterView);
            }

            // Holen aller aktiven Bearbeitungsstellen aus der Datenbank.
            List<Bearbeitungsstelle> bearbeitungsstellen = await _context.Bearbeitungsstelles.Where(x => x.IsActive == true).ToListAsync();

            // Erstellen einer LiegenschaftsView, die die Liegenschaft, den Dienstleister und die Bearbeitungsstellen enthält.
            LiegenschaftsView liegenschaftsView = new()
            {
                LiegenschaftID = liegenschaft.LiegenschaftID,
                Name = liegenschaft.Name,
                IsActive = liegenschaft.IsActive,
                AdresseID = liegenschaft.AdresseID,
                Hausnummer = adresseLiegenschaft?.Hausnummer,
                Postleitzahl = adresseLiegenschaft?.Postleitzahl,
                Stadt = adresseLiegenschaft?.Stadt,
                Strasse = adresseLiegenschaft?.Strasse,
                DienstleisterID = liegenschaftDienstleister.DienstleisterID,
                DienstleisterFirmenname = liegenschaftDienstleister.Firmenname,
                DienstleisterAbteilung = liegenschaftDienstleister.Abteilung,
                DienstleisterStrasse = adresseLiegenschaftDienstleister?.Strasse,
                DienstleisterHausnummer = adresseLiegenschaftDienstleister?.Hausnummer,
                DienstleisterPostleitzahl = adresseLiegenschaftDienstleister?.Postleitzahl,
                DienstleisterStadt = adresseLiegenschaftDienstleister?.Stadt,
                BearbeitungsstelleID = liegenschaftBearbeitungsstelle.BearbeitungsstelleID,
                BearbeitungsstelleName = liegenschaftBearbeitungsstelle.Name,
                BearbeitungsstelleMapGruppe = liegenschaftBearbeitungsstelle.MapGruppe,
                BearbeitungsstelleTelefone = liegenschaftBearbeitungsstelle.Telefone,
                BearbeitungsstelleEMail = liegenschaftBearbeitungsstelle.EMail,
                DienstleistersListe = dienstleisterlist,
                BearbeitungsstellenListe = bearbeitungsstellen
            };

            // Zurückgeben der LiegenschaftsView an die View.
            return View(liegenschaftsView);
        }

        // POST: Liegenschaft/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LiegenschaftID,Name,AdresseID,Strasse,Hausnummer,Postleitzahl,Stadt,IsActive,DienstleisterID,BearbeitungsstelleID")] LiegenschaftsView liegenschaftsView)
        {
            if (id != liegenschaftsView.LiegenschaftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Erstellen einer neuen Adresse-Instanz mit den aktualisierten Informationen.
                Adresse adresse = new()
                {
                    AdresseID = liegenschaftsView.AdresseID,
                    Hausnummer = liegenschaftsView.Hausnummer,
                    Stadt = liegenschaftsView.Stadt,
                    Strasse = liegenschaftsView.Strasse,
                    Postleitzahl = liegenschaftsView.Postleitzahl,
                };

                // Erstellen einer neuen Liegenschaft-Instanz mit den aktualisierten Informationen.
                Liegenschaft liegenschaft = new()
                {
                    LiegenschaftID = liegenschaftsView.LiegenschaftID,
                    Name = liegenschaftsView.Name,
                    AdresseID = liegenschaftsView.AdresseID,
                    IsActive = liegenschaftsView.IsActive,
                    BearbeitungsstelleID = liegenschaftsView.BearbeitungsstelleID,
                    DienstleisterID = liegenschaftsView.DienstleisterID,
                };

                try
                {
                    // Aktualisieren der Liegenschaft in der Datenbank.
                    _context.Update(liegenschaft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LiegenschaftExists(liegenschaftsView.LiegenschaftID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Aktualisieren der Adresse in der Datenbank mithilfe des AddressService.
                Adresse? answer = await new AddressService().EditAsync(adresse, _context);

                if (answer == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(liegenschaftsView);
        }
    }
}
