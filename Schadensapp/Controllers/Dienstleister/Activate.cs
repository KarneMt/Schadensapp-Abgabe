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
        // Aktion zum Aktivieren eines Dienstleisters basierend auf seiner ID.
        public async Task<IActionResult> Aktivieren(int? id)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Überprüfen, ob eine gültige Dienstleister-ID übergeben wurde und ob die Dienstleister-Datenbank nicht leer ist.
            if (id == null || _context.Dienstleisters == null)
            {
                return NotFound();
            }

            // Den entsprechenden Dienstleister in der Datenbank abrufen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == id);

            // Wenn ein Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            //Die entsprechnde Adresse des Dienstleister über seine ID Abrufen
            Adresse? adresse = new AddressService().GetAddress(dienstleister.AdresseID, _context);

            // Den Dienstleister + Adresse in ein BearbeitungsstellenView-Modell übertragen.
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

            // Die Ansicht mit dem DienstleisterView-Modell zurückgeben.
            return View(dienstleisterView);
        }

        // POST: Dienstleister/Delete/5
        [HttpPost, ActionName("Aktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktivierenConfirmed(int id)
        {
            // Überprüfen, ob die Entity-Set 'SchadensappDbContext.Dienstleisters' null ist.
            if (_context.Dienstleisters == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Bearbeitungsstelles'  is null.");
            }

            // Den entsprechenden Dienstleister in der Datenbank anhand der übergebenen ID suchen.
            var dienstleister = await _context.Dienstleisters.FindAsync(id);

            // Wenn der Dienstleister nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister != null)
            {
                try
                {
                    // Den Dienstleister als aktiv markieren.
                    dienstleister.IsActive = true;

                    // Den aktualisierte Dienstleister in der Datenbank speichern.
                    _context.Dienstleisters.Update(dienstleister);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wenn der Dienstleister nicht mehr existiert, NotFound zurückgeben.
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

            // Nach der Aktivierung zur Index-Ansicht weiterleiten.
            return RedirectToAction(nameof(Index));
        }
    }
}
