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
        // GET: Meldung/Create
        public async Task<IActionResult> Create()
        {
            // Überprüfen der Benutzerrolle aus dem HttpContext.
            string? userRole = HttpContext.Items["UserRole"] as string;
            ViewBag.UserRole = userRole;
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUser();

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (model == null || userRole == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Datenbankabfrage, um alle aktiven Liegenschaften zu laden.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.IsActive == true).ToListAsync();

            // Eine Liste, um die Liegenschaften als LiegenschaftenListe-Objekte zu speichern.
            var liegenschaftslist = new List<LiegenschaftenListe>();

            // Durchlaufen der geladenen Liegenschaften und Erstellen von LiegenschaftenListe-Objekten.
            foreach (var x in liegenschaftdb)
            {
                // Die Adresse der aktuellen Liegenschaft aus der Datenbank abrufen.
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);

                // Ein neues LiegenschaftenListe-Objekt erstellen und mit den Daten aus der Datenbank befüllen.
                LiegenschaftenListe liegenschaften = new()
                {
                    LiegenschaftID = x.LiegenschaftID,
                    Name = x.Name,
                    Hausnummer = adresse?.Hausnummer,
                    Postleitzahl = adresse?.Postleitzahl,
                    Stadt = adresse?.Stadt,
                    Strasse = adresse?.Strasse,
                };

                // Das LiegenschaftenListe-Objekt der Liste hinzufügen.
                liegenschaftslist.Add(liegenschaften);
            }

            // Ein neues MeldungsView-Objekt erstellen.
            MeldungsView meldungsview = new()
            {
                // Die Liste der Liegenschaften und die Benutzerinformation dem MeldungsView-Objekt zuweisen.
                Liegenschaftslist = liegenschaftslist,
                User = model
            };

            // Die Ansicht mit dem MeldungsView-Objekt zurückgeben.
            return View(meldungsview);
        }

        // POST: Meldung/Create
        // Aktion zum Erstellen einer neuen Meldung basierend auf den Daten aus der Meldung-Instanz.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Raum,Gebäudeteil,Ebene,Beschreibung,Dringlichkeit,LiegenschaftID")] Meldung meldung)
        {
            // Den aktuellen Benutzer aus der Datenbank abrufen.
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUser();

            // Falls der Benutzer nicht gefunden wurde, zur Zugriffsverweigerungsseite weiterleiten.
            if (model == null)
			{
				return RedirectToAction("AccessDenied", "AccessDenied");
			}

            // Die BenutzerID und das Status-Feld der Meldung-Instanz befüllen.
            meldung.BenutzerID = model.BenutzerID;
            meldung.Status = "In Bearbeitung";

            // Das Datum und die Uhrzeit der Meldung festlegen.
            meldung.DatumUhr = DateTime.Now;

            // Eine eindeutige ID für die Meldung generieren.
            meldung.MeldungID = new UUIDService().CreateUUIDwithDate();

            // Überprüfen, ob das Model gültig ist.
            if (ModelState.IsValid)
            {
                // Die Meldung-Instanz zur Datenbank hinzufügen.
                _context.Add(meldung);
                await _context.SaveChangesAsync();

                // Die entsprechende Liegenschaft in der Datenbank abrufen.
                var liegenschaft = _context.Liegenschafts.FirstOrDefault(x => x.LiegenschaftID == meldung.LiegenschaftID);

                // Wenn die E-Mail-Adresse des Benutzers vorhanden ist und die Liegenschaft gefunden wurde, eine Benachrichtigung per E-Mail an den Benutzer senden.
                if (model.EMail != null && liegenschaft != null)
                {
                    string messageUser = "Ihre Schadenmeldung mit der ID '" + meldung.MeldungID + "' in der Liegenschaft '" + liegenschaft.Name + "' und der Beschreibung '" + meldung.Beschreibung + "' wurde erfolgreich eingereicht.";
                    await mailService.SendEmailAsync(model.EMail, "Schadensmeldung eingereicht", messageUser);

                    // Die entsprechende Bearbeitungsstelle in der Datenbank abrufen.
                    var bearbeitungsstelle = _context.Bearbeitungsstelles.FirstOrDefault(x => x.BearbeitungsstelleID == liegenschaft.BearbeitungsstelleID);

                    // Wenn die Bearbeitungsstelle gefunden wurde und deren E-Mail-Adresse vorhanden ist, eine Benachrichtigung per E-Mail an die Bearbeitungsstelle senden.
                    if (bearbeitungsstelle != null && bearbeitungsstelle.EMail != null)
                    {
                        string messageBearbeitungsstelle = "Eine neue Schadensmeldung mit der ID '" + meldung.MeldungID + "' in der Liegenschaft '" + liegenschaft.Name + "' und der Beschreibung '" + meldung.Beschreibung + "' wurde eingereicht.";
                        await mailService.SendEmailAsync(bearbeitungsstelle.EMail, "Neue Schadensmeldung [Priorität: " + meldung.Dringlichkeit + "]", messageBearbeitungsstelle);
                    }
                }

                // Nach dem Speichern zur UserIndex-Ansicht weiterleiten.
                return RedirectToAction(nameof(UserIndex));
            }

            // Falls das Model ungültig ist, die Ansicht mit der ursprünglichen Meldung-Instanz zurückgeben.
            return View(meldung);
        }
    }
}