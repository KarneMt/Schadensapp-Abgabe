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
        // Aktion zum Anzeigen der Details einer Meldung anhand der übergebenen ID.
        public async Task<IActionResult> Complete(string? id)
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

            // Falls der Benutzer nicht gefunden wurde, zur ReadError-Ansicht weiterleiten.
            if (model == null)
			{
                return RedirectToAction("ReadError", "AccessDenied");
            }

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

            // Ein neues MeldungsView-Objekt erstellen und mit den Daten der gefundenen Meldung, des Benutzers und der Liegenschaften befüllen.
            MeldungsView meldungsview = new()
            {
                Liegenschaftslist = liegenschaftslist,
                User = model,
                Meldung = meldung
            };

            // Die Details-Ansicht mit dem MeldungsView-Objekt zurückgeben.
            return View(meldungsview);
        }

        // POST: Meldung/Delete/5
        // Aktion zum Bestätigen der Markierung einer Meldung als "Abgeschlossen" anhand der übergebenen ID.
        [HttpPost, ActionName("Complete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteConfirmed(string? id)
        {
            // Überprüfen, ob die ID der Meldung null ist oder die Datenbank-Tabelle "Meldungs" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Meldungs == null)
            {
                return NotFound();
            }

            // Die entsprechende Meldungs-Instanz in der Datenbank anhand der übergebenen ID suchen.
            var meldung = await _context.Meldungs.FindAsync(id);

            // Falls die Meldung nicht gefunden wurde, NotFound zurückgeben.
            if (meldung == null)
            {
                return NotFound();
            }
            else
            {
                // Den Benutzer der Meldung aus der Datenbank abrufen.
                UserModel? model = new UserService(_context, _httpContextAccessor).GetUserWithSID(meldung.BenutzerID);

                // Falls der Benutzer nicht gefunden wurde, zur AccessDenied-Ansicht weiterleiten.
                if (model == null)
				{
					return RedirectToAction("AccessDenied", "AccessDenied");
				}

                // Den Status der Meldung auf "Abgeschlossen" setzen.
                meldung.Status = "Abgeschlossen";

                // Die Änderungen in der Datenbank speichern.
                _context.Meldungs.Update(meldung);
                await _context.SaveChangesAsync();

                // Den zugehörigen Liegenschaftsdatensatz abrufen.
                var liegenschaft = _context.Liegenschafts.FirstOrDefault(x => x.LiegenschaftID == meldung.LiegenschaftID);

                // Wenn die E-Mail-Adresse des Benutzers vorhanden ist und die Liegenschaft gefunden wurde,
                // eine E-Mail-Benachrichtigung über die Statusänderung der Meldung senden.
                if (model.EMail != null && liegenschaft != null)
                {
                    string messageUser = "Ihre Schadenmeldung mit der ID '" + meldung.MeldungID + "' in der Liegenschaft '" + liegenschaft.Name + "' und der Beschreibung '" + meldung.Beschreibung + "' hat eine Statusänderung. Der neue Status ist: " + meldung.Status + ".";
                    await mailService.SendEmailAsync(model.EMail, "Statusaktualisierung ihrer Schadensmeldung", messageUser);
                }
            }

            // Zur Index-Ansicht weiterleiten, um die aktualisierte Liste der Meldungen anzuzeigen.
            return RedirectToAction(nameof(Index));
        }
    }
}
