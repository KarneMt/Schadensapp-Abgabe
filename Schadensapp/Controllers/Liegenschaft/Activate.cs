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
        // Aktion zum Aktivieren einer Liegenschaft basierend auf der ID.
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

            // Überprüfen, ob eine gültige Liegenschaft-ID übergeben wurde und ob die Liegenschaft-Datenbank nicht leer ist.
            if (id == null || _context.Liegenschafts == null)
            {
                return NotFound();
            }

            // Die entsprechende Liegenschaft in der Datenbank abrufen.
            var liegenschaft = await _context.Liegenschafts.FirstOrDefaultAsync(m => m.LiegenschaftID == id);

            // Wenn die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (liegenschaft == null)
            {
                return NotFound();
            }

            //Die entsprechnde Adresse der Liegenschaft über die ID Abrufen
            Adresse? adresse = new AddressService().GetAddress(liegenschaft.AdresseID, _context);

            // Den entsprechenden Dienstleiter in der Datenbank abrufen.
            var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(m => m.DienstleisterID == liegenschaft.DienstleisterID);

            // Wenn kein Dienstleiter nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }
            //Adresse initialiseren
            Adresse? adresseDienstleister = new();

            //Wenn Aktiver Dienstleister gefunden wurde die dazugehörige Adresse ermitteln
            if (dienstleister.IsActive == false)
            {
                dienstleister = null;
            }
            else
            {
                adresseDienstleister = new AddressService().GetAddress(dienstleister.AdresseID, _context);
            }

            // Die entsprechenden Bearbeitungsstellen in der Datenbank abrufen.
            var bearbeitungsstelle = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(m => m.BearbeitungsstelleID == liegenschaft.BearbeitungsstelleID);

            // Wenn keine Bearbeitungsstelle nicht gefunden wurde, NotFound zurückgeben.
            if (bearbeitungsstelle == null)
            {
                return NotFound();
            }

            //Leere liste wird initialisiert
            List<Bearbeitungsstelle> bearbeitungsstellen = new();

            //Schauen ob die zugeordnete Bearbeitungsstelle auch noch Aktiv ist
            if (bearbeitungsstelle.IsActive == false)
            {
                bearbeitungsstelle = null;
            }
            else
            {
                //Falls keine Aktive Bearbeitzungsstelle vorhanden war werden alle aktiv vorhandenen ausgelesen 
                bearbeitungsstellen = await _context.Bearbeitungsstelles.Where(x => x.IsActive == true).ToListAsync();
            }

            //Leere liste wird initialisiert
            var dienstleisterlist = new List<DienstleisterView>();

            //Falls keine aktiven Dienstleister gefunden wurden werden aktive ausgelesen, sowie die dazugehörige Adresse
            if (dienstleister == null)
            {
                var dbdienstleister = await _context.Dienstleisters.Where(x => x.IsActive == true).ToListAsync();
                foreach (var x in dbdienstleister)
                {
                    adresseDienstleister = new AddressService().GetAddress(x.AdresseID, _context);
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
            }

            //Daten in ein LiegenschaftsView-Modell übergeben
            LiegenschaftsView liegenschaftsView = new()
            {
                LiegenschaftID = liegenschaft.LiegenschaftID,
                Name = liegenschaft.Name,
                IsActive = liegenschaft.IsActive,
                AdresseID = liegenschaft.AdresseID,
                Hausnummer = adresse?.Hausnummer,
                Postleitzahl = adresse?.Postleitzahl,
                Stadt = adresse?.Stadt,
                Strasse = adresse?.Strasse,
            };

            //Dienstleister an das LiegenschaftsView-Modell übergeben
            if (dienstleisterlist.Count >= 1)
            {
                //Falls keine Aktiven vorhanden waren wird die Liste übergeben
                liegenschaftsView.DienstleistersListe = dienstleisterlist;
            }
            else
            {
                //Waren ein Aktiver Dienstleister vorhanden, so wird dieser übergeben
                if (dienstleister != null)
                {
                    liegenschaftsView.DienstleisterID = dienstleister.DienstleisterID;
                    liegenschaftsView.DienstleisterFirmenname = dienstleister.Firmenname;
                }
            }

            //Bearbeitungsstellen an das LiegenschaftsView-Modell übergeben
            if (bearbeitungsstellen.Count >= 1)
            {
                //Falls keine Aktiven vorhanden waren wird die Liste übergeben
                liegenschaftsView.BearbeitungsstellenListe = bearbeitungsstellen;
            }
            else
            {
                //Waren eine Aktive vorhanden, so wird diese übergeben
                if (bearbeitungsstelle != null)
                {
                    liegenschaftsView.BearbeitungsstelleID = bearbeitungsstelle.BearbeitungsstelleID;
                    liegenschaftsView.BearbeitungsstelleName = bearbeitungsstelle.Name;
                }
            }

            // Die Ansicht mit dem LiegenschaftsView-Modell zurückgeben.
            return View(liegenschaftsView);
        }

        // POST: Liegenschaft/Delete/5
        [HttpPost, ActionName("Aktivieren")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AktivierenConfirmed(int id, [Bind("LiegenschaftID,DienstleisterID,BearbeitungsstelleID")] LiegenschaftsView liegenschaftsView)
        {
            // Überprüfen, ob die Entity-Set 'SchadensappDbContext.Liegenschafts' null ist.
            if (_context.Liegenschafts == null)
            {
                return Problem("Entity set 'SchadensappDbContext.Bearbeitungsstelles'  is null.");
            }

            // Die entsprechende Liegenschaft in der Datenbank anhand der übergebenen ID suchen.
            var liegenschaft = await _context.Liegenschafts.FindAsync(id);

            // Wenn die Liegenschaft nicht gefunden wurde, NotFound zurückgeben.
            if (liegenschaft != null)
            {
                try
                {
                    // Überprüfen, ob eine DienstleisterID übergeben wurde und diese nicht gleich 0 ist.
                    if (liegenschaftsView.DienstleisterID != 0)
                    {
                        liegenschaft.DienstleisterID = liegenschaftsView.DienstleisterID;
                    }

                    // Überprüfen, ob eine BearbeitungsstelleID übergeben wurde und diese nicht gleich 0 ist.
                    if (liegenschaftsView.BearbeitungsstelleID != 0)
                    {
                        liegenschaft.BearbeitungsstelleID = liegenschaftsView.BearbeitungsstelleID;
                    }

                    // Die Liegenschaft als aktiv markieren.
                    liegenschaft.IsActive = true;

                    // Die aktualisierte Liegenschaft in der Datenbank speichern.
                    _context.Liegenschafts.Update(liegenschaft);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wenn die Liegenschaft nicht mehr existiert, NotFound zurückgeben.
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

            // Nach der Aktivierung zur Index-Ansicht weiterleiten.
            return RedirectToAction(nameof(Index));
        }
    }
}
