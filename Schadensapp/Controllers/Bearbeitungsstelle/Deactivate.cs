using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle/Delete/5
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


            // Überprüfen, ob die ID der Bearbeitungsstelle null ist oder die Datenbank-Tabelle "Bearbeitungsstelles" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Bearbeitungsstelles == null)
            {
                return NotFound();
            }

            // Die Anzahl der aktiven Bearbeitungsstellen in der Datenbank ermitteln.
            int count = await _context.Bearbeitungsstelles.CountAsync(x => x.IsActive == true);

            // Falls es nur eine aktive Bearbeitungsstelle gibt, zur Deaktivierungsfehler-Ansicht weiterleiten.
            if (count <= 1)
            {
                return RedirectToAction(nameof(DeaktivierenError));
            }
            else
            {
                // Die entsprechende Bearbeitungsstelle in der Datenbank anhand der übergebenen ID suchen.
                var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == id);

                // Falls die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
                if (bearbeitungsstelle == null)
                {
                    return NotFound();
                }

                // Ein neues BearbeitungsstellenView-Objekt erstellen und mit den Daten der gefundenen Bearbeitungsstelle befüllen.
                BearbeitungsstellenView bearbeitungsstellenView = new BearbeitungsstellenView
                {
                    Bearbeitungsstelle = bearbeitungsstelle
                };

                // Die Liegenschaften, die zu dieser Bearbeitungsstelle gehören, aus der Datenbank abrufen.
                List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.BearbeitungsstelleID == bearbeitungsstelle.BearbeitungsstelleID).ToListAsync();

                // Eine Liste von LiegenschaftenListe-Objekten erstellen und mit den Daten der gefundenen Liegenschaften befüllen.
                bearbeitungsstellenView.LiegenschaftenListe = new List<LiegenschaftenListe>();
                foreach (var x in liegenschaftdb)
                {
                    Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);
                    LiegenschaftenListe liegenschaften = new LiegenschaftenListe
                    {
                        LiegenschaftID = x.LiegenschaftID,
                        Name = x.Name,
                        Hausnummer = adresse?.Hausnummer,
                        Postleitzahl = adresse?.Postleitzahl,
                        Stadt = adresse?.Stadt,
                        Strasse = adresse?.Strasse,
                    };
                    bearbeitungsstellenView.LiegenschaftenListe.Add(liegenschaften);
                }

                // Die Ansicht mit dem BearbeitungsstellenView-Objekt zurückgeben.
                return View(bearbeitungsstellenView);
            }
        }

        public IActionResult DeaktivierenError()
        {
            return View();
        }

        // POST: Bearbeitungsstelle/Delete/5
        // Aktion zum Bestätigen der Deaktivierung einer Bearbeitungsstelle anhand der übergebenen ID.
        [HttpPost, ActionName("Deaktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeaktivierenConfirmed(int id)
        {
            // Überprüfen, ob die Datenbank-Tabelle "Bearbeitungsstelles" null ist.
            if (_context.Bearbeitungsstelles == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Bearbeitungsstelles'  is null.");
            }

            // Die entsprechende Bearbeitungsstelle in der Datenbank anhand der übergebenen ID suchen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FindAsync(id);

            // Falls die Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle != null)
            {
                try
                {
                    // Die IsActive-Eigenschaft der Bearbeitungsstelle auf false setzen, um sie zu deaktivieren.
                    bearbeitungsstelle.IsActive = false;

                    // Die Bearbeitungsstelle in der Datenbank als geändert markieren.
                    _context.Bearbeitungsstelles.Update(bearbeitungsstelle);

                    // Die Änderungen in der Datenbank speichern.
                    await _context.SaveChangesAsync();

                    // Alle Liegenschaften abrufen, die zu dieser Bearbeitungsstelle gehören.
                    List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.BearbeitungsstelleID == bearbeitungsstelle.BearbeitungsstelleID).ToListAsync();

                    // Jede Liegenschaft in der Liste durchlaufen und deren IsActive-Eigenschaft auf false setzen, um sie zu deaktivieren.
                    foreach (var item in liegenschaftdb)
                    {
                        item.IsActive = false;

                        // Die Liegenschaft in der Datenbank als geändert markieren.
                        _context.Liegenschafts.Update(item);

                        // Die Änderungen in der Datenbank speichern.
                        await _context.SaveChangesAsync();
                    };
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Falls eine Ausnahme auftritt, überprüfen, ob die Bearbeitungsstelle immer noch existiert.
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

            // Nach der Deaktivierung zur Index-Ansicht weiterleiten.
            return RedirectToAction(nameof(Index));
        }
    }
}
