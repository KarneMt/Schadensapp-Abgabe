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
        // GET: Dienstleister/Delete/5
        public async Task<IActionResult> Deaktivieren(int? id)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Überprüfen, ob die ID des Dienstleisters null ist oder die Datenbank-Tabelle "Dienstleisters" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Dienstleisters == null)
            {
                return NotFound();
            }

            // Die entsprechende Dienstleister-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == id);

            // Falls der Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            // Die Adresse des Dienstleisters aus der Datenbank abrufen.
            Adresse? adresse = new AddressService().GetAddress(dienstleister.AdresseID, _context);

            // Ein neues DienstleisterView-Objekt erstellen und mit den Daten des gefundenen Dienstleisters befüllen.
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

            // Die Liegenschaften abrufen, die zu diesem Dienstleister gehören.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.DienstleisterID == dienstleisterView.DienstleisterID).ToListAsync();

            // Eine Liste von LiegenschaftenListe-Objekten erstellen und mit den Daten der gefundenen Liegenschaften befüllen.
            dienstleisterView.LiegenschaftenListe = new List<LiegenschaftenListe>();
            foreach (var x in liegenschaftdb)
            {
                Adresse? adresseLiegenschaft = new AddressService().GetAddress(x.AdresseID, _context);
                LiegenschaftenListe liegenschaften = new LiegenschaftenListe
                {
                    LiegenschaftID = x.LiegenschaftID,
                    Name = x.Name,
                    Hausnummer = adresseLiegenschaft?.Hausnummer,
                    Postleitzahl = adresseLiegenschaft?.Postleitzahl,
                    Stadt = adresseLiegenschaft?.Stadt,
                    Strasse = adresseLiegenschaft?.Strasse,
                };
                dienstleisterView.LiegenschaftenListe.Add(liegenschaften);
            }

            // Die Ansicht mit dem DienstleisterView-Objekt zurückgeben.
            return View(dienstleisterView);
        }

        // POST: Dienstleister/Delete/5
        // Aktion zum Bestätigen der Deaktivierung eines Dienstleisters anhand der übergebenen ID.
        [HttpPost, ActionName("Deaktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeaktivierenConfirmed(int id)
        {
            // Überprüfen, ob die Datenbank-Tabelle "Dienstleisters" null ist.
            if (_context.Dienstleisters == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Dienstleisters'  is null.");
            }

            // Die entsprechende Dienstleister-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var dienstleister = await _context.Dienstleisters.FindAsync(id);

            // Falls der Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister != null)
            {
                try
                {
                    // Die IsActive-Eigenschaft des Dienstleisters auf false setzen, um ihn zu deaktivieren.
                    dienstleister.IsActive = false;

                    // Die Änderungen am Dienstleister in der Datenbank speichern.
                    _context.Update(dienstleister);
                    await _context.SaveChangesAsync();

                    // Alle Liegenschaften abrufen, die zu diesem Dienstleister gehören.
                    List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.DienstleisterID == dienstleister.DienstleisterID).ToListAsync();

                    // Jede Liegenschaft in der Liste durchlaufen und deren IsActive-Eigenschaft auf false setzen, um sie zu deaktivieren.
                    foreach (var item in liegenschaftdb)
                    {
                        item.IsActive = false;

                        // Die Änderungen an der Liegenschaft in der Datenbank speichern.
                        _context.Liegenschafts.Update(item);
                        await _context.SaveChangesAsync();
                    };
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Falls eine Ausnahme auftritt, überprüfen, ob der Dienstleister immer noch existiert.
                    if (!DienstleisterExists(dienstleister.DienstleisterID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nach der Deaktivierung zur Index-Ansicht weiterleiten.
            return RedirectToAction(nameof(Index));
        }
    }
}
