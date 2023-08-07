using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services.Mail.Model;
using Schadensapp.Services;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
	[Authorize]
	public partial class HomeController
    {
        // GET: Meldung/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
			ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
            {
                return RedirectToAction("AccessDenied", "AccessDenied");
            }

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Meldungs" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Meldungs == null)
            {
                return NotFound();
            }

            // Abrufen der Meldung mit der angegebenen ID aus der Datenbank.
            var meldung = await _context.Meldungs.FindAsync(id);
            if (meldung == null)
            {
                return NotFound();
            }

            // Abrufen des zugehörigen Benutzers mit der BenutzerID aus der Meldung.
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUserWithSID(meldung.BenutzerID);

            // Wenn der Benutzer nicht gefunden wird, wird er auf die ReadError-Seite der AccessDenied-Controller weitergeleitet.
            if (model == null)
			{
				return RedirectToAction("ReadError", "AccessDenied");
			}

            // Abrufen der aktiven Liegenschaften aus der Datenbank.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.LiegenschaftID == meldung.LiegenschaftID).ToListAsync();
            liegenschaftdb = await _context.Liegenschafts.Where(x => x.IsActive == true).ToListAsync();
            var liegenschaftslist = new List<LiegenschaftenListe>();
            foreach (var x in liegenschaftdb)
            {
                // Abrufen der Adresse für jede Liegenschaft.
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

            // Erstellen einer MeldungsView-Instanz, die die Meldung, den Benutzer und die Liste der aktiven Liegenschaften enthält.
            MeldungsView meldungsview = new()
            {
                Liegenschaftslist = liegenschaftslist,
                User = model,
                Meldung = meldung
            };

            // Die Meldungsdetails werden an die View zurückgegeben.
            return View(meldungsview);
        }

        // POST: Meldung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string action, [Bind("MeldungID,Status,LiegenschaftID,BenutzerID,Raum,Gebäudeteil,Ebene,Beschreibung,Anmerkung,Dringlichkeit")] Meldung meldung)
        {
			if (id != meldung.MeldungID)
            {
                return NotFound();
            }

            // Abrufen des zugehörigen Benutzers mit der BenutzerID aus der Meldung.
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUserWithSID(meldung.BenutzerID);

            // Wenn der Benutzer nicht gefunden wird, wird er auf die ReadError-Seite der AccessDenied-Controller weitergeleitet.
            if (model == null)
			{
				return RedirectToAction("ReadError", "AccessDenied");
			}

            // Übersetzen des ausgewählten Statuscodes in den entsprechenden Statusnamen.
            switch (meldung.Status)
            {
                case "1":
                    meldung.Status = "In Bearbeitung";
                    break;
                case "2":
                    meldung.Status = "Abgelehnt";

                    break;
                case "3":
                    meldung.Status = "Auftrag erteilt";

                    break;
                case "4":
                    meldung.Status = "Abgeschlossen";

                    break;
                default:
                    meldung.Status = "Fehler";

                    break;
            }

            try
            {
                // Abrufen der alten Meldung aus der Datenbank, um ihre Werte zu aktualisieren.
                var oldmeldung = await _context.Meldungs.FirstOrDefaultAsync(m => m.MeldungID == id);
                if (oldmeldung != null)
                {
                    oldmeldung.Anmerkung = meldung.Anmerkung;
                    oldmeldung.Dringlichkeit = meldung.Dringlichkeit;
                    oldmeldung.Raum = meldung.Raum;
                    oldmeldung.Gebäudeteil = meldung.Gebäudeteil;
                    oldmeldung.Status = meldung.Status;
                    oldmeldung.Ebene = meldung.Ebene;
                    oldmeldung.Beschreibung = meldung.Beschreibung;
                    oldmeldung.LiegenschaftID = meldung.LiegenschaftID;

                    switch (action)
                    {
                        case "Freigeben":
                            oldmeldung.Status = "Auftrag erteilt";

                            var liegenschaft1 = _context.Liegenschafts.FirstOrDefault(x => x.LiegenschaftID == oldmeldung.LiegenschaftID);

                            // Senden einer E-Mail an den Benutzer, wenn der Status auf "Auftrag erteilt" geändert wird.
                            if (model.EMail != null && liegenschaft1 != null)
                            {
                                string messageUser = "Ihre Schadenmeldung mit der ID '" + oldmeldung.MeldungID + "' in der Liegenschaft '" + liegenschaft1.Name + "' und der Beschreibung '" + oldmeldung.Beschreibung + "' hat eine Statusänderung. Der neue Status ist: " + oldmeldung.Status + ".";
                                await mailService.SendEmailAsync(model.EMail, "Statusaktualisierung ihrer Schadensmeldung", messageUser);
                            }

                            // Senden einer E-Mail an den Dienstleister und die Bearbeitungsstelle, wenn der Status auf "Auftrag erteilt" geändert wird.
                            if (liegenschaft1 != null)
                            {
                                // Abrufen des Dienstleisters, der der Liegenschaft zugeordnet ist, aus der Datenbank.
                                var dienstleister = await _context.Dienstleisters.FirstOrDefaultAsync(x => x.DienstleisterID == liegenschaft1.DienstleisterID);

                                // Wenn ein zugehöriger Dienstleister gefunden wurde:
                                if (dienstleister != null)
                                {
                                    // Abrufen der Adresse des Dienstleisters aus der Datenbank.
                                    Adresse? adresseDienstleister = new AddressService().GetAddress(dienstleister.AdresseID, _context);

                                    // Abrufen der Bearbeitungsstelle für die Liegenschaft aus der Datenbank.
                                    var bearbeiter = await _context.Bearbeitungsstelles.FirstOrDefaultAsync(x => x.BearbeitungsstelleID == liegenschaft1.BearbeitungsstelleID);

                                    // Abrufen der Adresse der Liegenschaft aus der Datenbank.
                                    Adresse? adresseLiegenschaft = new AddressService().GetAddress(liegenschaft1.AdresseID, _context);

                                    // Erstellen eines PDF-Modells, das Informationen über die Meldung, den Benutzer, den Dienstleister, die Liegenschaft und die Bearbeitungsstelle enthält.
                                    PDFModel mhm = new()
                                    {
                                        Meldung = oldmeldung,
                                        User = model,
                                        Dienstleister = dienstleister,
                                        Liegenschaft = liegenschaft1,
                                        Bearbeitungsstelle = bearbeiter,
                                        AdresseDienstleister = adresseDienstleister,
                                        AdresseLiegenschaft = adresseLiegenschaft
                                    };

                                    // Erstellen einer E-Mail-Nachricht für die Bearbeitungsstelle.
                                    string messageBearbeitungsstelle = "Eine neue Schadensmeldung '" + oldmeldung.MeldungID + "' in der Liegenschaft '" + liegenschaft1.Name + "' wurde eingereicht.";

                                    // Senden einer E-Mail an den Dienstleister mit dem PDF-Modell als Anhang.
                                    if (dienstleister != null && dienstleister.EMail != null)
                                    {
                                        await mailService.SendEmailAsyncWithAttachment(dienstleister.EMail, "Schadensmeldung eingereicht [Priorität: " + oldmeldung.Dringlichkeit + "]", messageBearbeitungsstelle, mhm);
                                    }
                                }
                            }
                            break;

                        case "Ablehnen":
                            // Wenn der Status auf "Abgelehnt" geändert wird:
                            oldmeldung.Status = "Abgelehnt";

                            // Abrufen der zugehörigen Liegenschaft aus der Datenbank.
                            var liegenschaft2 = _context.Liegenschafts.FirstOrDefault(x => x.LiegenschaftID == oldmeldung.LiegenschaftID);

                            // Senden einer E-Mail an den Benutzer, wenn der Status auf "Abgelehnt" geändert wird.
                            if (model.EMail != null && liegenschaft2 != null)
                            {
                                string messageUser = "Ihre Schadenmeldung mit der ID '" + oldmeldung.MeldungID + "' in der Liegenschaft '" + liegenschaft2.Name + "' und der Beschreibung '" + oldmeldung.Beschreibung + "' hat eine Statusänderung. Der neue Status ist: " + oldmeldung.Status + ".";
                                await mailService.SendEmailAsync(model.EMail, "Statusaktualisierung ihrer Schadensmeldung", messageUser);
                            }
                            break;
                        default:
                            break;
                    }

                    // Die alte Meldung in der Datenbank aktualisieren.
                    _context.Update(oldmeldung);
                    await _context.SaveChangesAsync();
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeldungExists(meldung.MeldungID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Nachdem die Bearbeitung abgeschlossen ist, wird der Benutzer zur Index-Seite weitergeleitet.
            return RedirectToAction(nameof(Index));
        }
    }
}