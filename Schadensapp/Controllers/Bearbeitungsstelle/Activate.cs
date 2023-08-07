using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle/Delete/5
        // Aktion zum Aktivieren einer Bearbeitungsstelle basierend auf ihrer ID.
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

            // Überprüfen, ob eine gültige Bearbeitungsstellen-ID übergeben wurde und ob die Bearbeitungsstellen-Datenbank nicht leer ist.
            if (id == null || _context.Bearbeitungsstelles == null)
            {
                return NotFound();
            }

            // Die entsprechende Bearbeitungsstelle in der Datenbank abrufen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == id);

            // Wenn die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle == null)
            {
                return NotFound();
            }

            // Die Bearbeitungsstelle in ein BearbeitungsstellenView-Modell übertragen.
            BearbeitungsstellenView bearbeitungsstellenView = new()
            {
                Bearbeitungsstelle = bearbeitungsstelle
            };

            // Die Ansicht mit dem BearbeitungsstellenView-Modell zurückgeben.
            return View(bearbeitungsstellenView);
        }

        // POST: Bearbeitungsstelle/Delete/5
        [HttpPost, ActionName("Aktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktivierenConfirmed(int id)
        {
            // Überprüfen, ob die Entity-Set 'SchadensappDbContext.Bearbeitungsstelles' null ist.
            if (_context.Bearbeitungsstelles == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Bearbeitungsstelles'  is null.");
            }

            // Die entsprechende Bearbeitungsstelle in der Datenbank anhand der übergebenen ID suchen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FindAsync(id);

            // Wenn die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle != null)
            {
                try
                {
                    // Die Bearbeitungsstelle als aktiv markieren.
                    bearbeitungsstelle.IsActive = true;

                    // Die aktualisierte Bearbeitungsstelle in der Datenbank speichern.
                    _context.Bearbeitungsstelles.Update(bearbeitungsstelle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wenn die Bearbeitungsstelle nicht mehr existiert, NotFound zurückgeben.
                    if (!BearbeitungsstelleExists(bearbeitungsstelle.BearbeitungsstelleID))
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
