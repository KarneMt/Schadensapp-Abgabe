using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
	[Authorize]
	public partial class HomeController
    {
        // GET: Meldung/Delete/5
        // Aktion zum Anzeigen der Details einer Meldung anhand der übergebenen ID.
        public async Task<IActionResult> Delete(string? id)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Überprüfen, ob die ID der Meldung null ist oder die Datenbank-Tabelle "Meldungs" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Meldungs == null)
            {
                return NotFound();
            }

            // Die entsprechende Meldungs-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var meldung = await _context.Meldungs.FirstOrDefaultAsync(m => m.MeldungID == id);

            // Falls die Meldung nicht gefunden wurde, NotFound zurückgeben.
            if (meldung == null)
            {
                return NotFound();
            }

            // Den Benutzer der Meldung aus der Datenbank abrufen.
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUserWithSID(meldung.BenutzerID);

            // Alle Liegenschaften abrufen, die mit dieser Meldung verknüpft sind, und sie in einer Liste von Liegenschaftsobjekten speichern.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.LiegenschaftID == meldung.LiegenschaftID).ToListAsync();
            var liegenschaftslist = new List<LiegenschaftenListe>();
            foreach (var x in liegenschaftdb)
            {
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);
                LiegenschaftenListe liegenschaften = new()
                {
                    LiegenschaftID = x.LiegenschaftID,
                    Name = x.Name,
                    Hausnummer = adresse?.Hausnummer,
                    Postleitzahl = adresse?.Postleitzahl,
                    Stadt = adresse?.Stadt,
                    Strasse = adresse?.Strasse,
                };
                liegenschaftslist.Add(liegenschaften);
            }

            // Ein neues MeldungsView-Objekt erstellen und mit den Daten der gefundenen Meldung, Benutzer und Liegenschaften befüllen.
            #pragma warning disable CS8601 // Mögliche Nullverweiszuweisung. Ist nicht schlimm wenn das Model "User" leer ist, kein Fehler.
            MeldungsView meldungsview = new()
            {
                Liegenschaftslist = liegenschaftslist,
                User = model,
                Meldung = meldung

            };
            #pragma warning restore CS8601

            // Die Ansicht mit dem MeldungsView-Objekt zurückgeben.
            return View(meldungsview);
        }

        // POST: Meldung/Delete/5
        // Aktion zum Bestätigen des Löschens einer Meldung anhand der übergebenen ID.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Überprüfen, ob die Datenbank-Tabelle "Meldungs" null ist.
            if (_context.Meldungs == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Meldungs'  is null.");
            }

            // Die entsprechende Meldungs-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var meldung = await _context.Meldungs.FindAsync(id);

            // Falls die Meldung gefunden wurde, aus der Datenbank entfernen.
            if (meldung != null)
            {
                _context.Meldungs.Remove(meldung);
            }

            // Die Änderungen in der Datenbank speichern.
            await _context.SaveChangesAsync();

            // Zur Index-Ansicht weiterleiten, um die aktualisierte Liste der Meldungen anzuzeigen.
            return RedirectToAction(nameof(Index));
        }
    }
}
