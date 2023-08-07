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
        // GET: Liegenschaft/Delete/5
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

            // Überprüfen, ob die ID der Liegenschaft null ist oder die Datenbank-Tabelle "Liegenschafts" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Liegenschafts == null)
            {
                return NotFound();
            }

            // Die entsprechende Liegenschafts-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var liegenschaft = await _context.Liegenschafts.FirstOrDefaultAsync(m => m.LiegenschaftID == id);

            // Falls die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (liegenschaft == null)
            {
                return NotFound();
            }

            // Eine Liste für die Ansicht erstellen.
            var list = new List<LiegenschaftsView>();

            // Die Adresse der Liegenschaft aus der Datenbank abrufen.
            Adresse? adresse = new AddressService().GetAddress(liegenschaft.AdresseID, _context);

            // Den Dienstleister der Liegenschaft aus der Datenbank abrufen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == liegenschaft.DienstleisterID);

            // Falls der Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            // Die Bearbeitungsstelle der Liegenschaft aus der Datenbank abrufen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == liegenschaft.BearbeitungsstelleID);

            // Falls die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle == null)
            {
                return NotFound();
            }

            // Ein neues LiegenschaftsView-Objekt erstellen und mit den Daten der gefundenen Liegenschaft, Adresse, Dienstleister und Bearbeitungsstelle befüllen.
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
                BearbeitungsstelleID = bearbeitungsstelle.BearbeitungsstelleID,
                BearbeitungsstelleName = bearbeitungsstelle.Name,
            };

            // Die Ansicht mit dem LiegenschaftsView-Objekt zurückgeben.
            return View(dienstleisterView);
        }

        // POST: Liegenschaft/Delete/5
        // Aktion zum Bestätigen der Deaktivierung einer Liegenschaft anhand der übergebenen ID.
        [HttpPost, ActionName("Deaktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeaktivierenConfirmed(int id)
        {
            // Überprüfen, ob die Datenbank-Tabelle "Liegenschafts" null ist.
            if (_context.Liegenschafts == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Liegenschafts'  is null.");
            }

            // Die entsprechende Liegenschafts-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var liegenschaft = await _context.Liegenschafts.FindAsync(id);

            // Falls die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (liegenschaft != null)
            {
                try
                {
                    // Die IsActive-Eigenschaft der Liegenschaft auf false setzen, um sie zu deaktivieren.
                    liegenschaft.IsActive = false;

                    // Die Änderungen an der Liegenschaft in der Datenbank speichern.
                    _context.Liegenschafts.Update(liegenschaft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Falls eine Ausnahme auftritt, überprüfen, ob die Liegenschaft immer noch existiert.
                    if (!LiegenschaftExists(liegenschaft.LiegenschaftID))
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
