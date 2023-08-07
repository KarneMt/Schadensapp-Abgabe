using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class BearbeitungsstelleController
    {
        // GET: Bearbeitungsstelle/Edit/5
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

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Bearbeitungsstelles" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Bearbeitungsstelles == null)
            {
                return NotFound();
            }

            // Die Bearbeitungsstelle aus der Datenbank anhand der übergebenen ID abrufen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FindAsync(id);

            // Wenn die Bearbeitungsstelle nicht gefunden wurde, wird eine NotFound-Antwort zurückgegeben, um anzuzeigen, dass die Ressource nicht vorhanden ist.
            if (bearbeitungsstelle == null)
            {
                return NotFound();
            }

            // Eine Instanz von BearbeitungsstellenView erstellen und mit den Daten der gefundenen Bearbeitungsstelle befüllen.
            BearbeitungsstellenView bearbeitungsstellenView = new BearbeitungsstellenView
            {
                Bearbeitungsstelle = bearbeitungsstelle
            };

            // Die Ansicht mit der erstellten BearbeitungsstellenView anzeigen.
            return View(bearbeitungsstellenView);
        }

        // POST: Bearbeitungsstelle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BearbeitungsstelleID,MapGruppe,Name,Telefone,EMail,IsActive")] Bearbeitungsstelle bearbeitungsstelle)
        {
            // Überprüfen, ob die übergebene ID mit der ID der zu bearbeitenden Bearbeitungsstelle übereinstimmt.
            if (id != bearbeitungsstelle.BearbeitungsstelleID)
            {
                return NotFound();
            }

            // Überprüfen, ob das Model die Validierungskriterien erfüllt.
            if (ModelState.IsValid)
            {
                try
                {
                    // Die bearbeitete Bearbeitungsstelle in die Datenbank aktualisieren.
                    _context.Update(bearbeitungsstelle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wenn die Bearbeitungsstelle nicht gefunden wurde, wird eine NotFound-Antwort zurückgegeben,
                    // um anzuzeigen, dass die Ressource nicht vorhanden ist.
                    if (!BearbeitungsstelleExists(bearbeitungsstelle.BearbeitungsstelleID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Wenn es ein Concurrency-Problem gab, wird die Ausnahme weitergegeben.
                        throw;
                    }
                }
                // Nachdem die Bearbeitung erfolgreich war, wird der Benutzer zur Index-Seite weitergeleitet.
                return RedirectToAction(nameof(Index));
            }

            // Wenn das Model nicht gültig ist, wird die BearbeitungsstellenView erstellt und zurückgegeben,
            // um die Bearbeitungsstelle in der Bearbeitungsansicht erneut anzuzeigen.
            BearbeitungsstellenView bearbeitungsstellenView = new()
            {
                Bearbeitungsstelle = bearbeitungsstelle
            };

            // Die Ansicht mit dem BearbeitungsstellenView anzeigen.
            return View(bearbeitungsstellenView);
        }
    }
}
