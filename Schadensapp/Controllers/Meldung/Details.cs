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
        // GET: Meldung/Details/5
        public async Task<IActionResult> Details(string? id)
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

            // Die entsprechende Meldung in der Datenbank anhand der übergebenen ID suchen.
            var meldung = await _context.Meldungs.FirstOrDefaultAsync(m => m.MeldungID == id);

            // Falls die Meldung nicht gefunden wurde, NotFound zurückgeben.
            if (meldung == null)
            {
                return NotFound();
            }

            // Den Benutzer anhand der BenutzerID aus der Meldung aus der Datenbank abrufen.
            UserModel? model = new UserService(_context, _httpContextAccessor).GetUserWithSID(meldung.BenutzerID);

            // Wenn der Benutzer nicht gefunden wurde, zur "ReadError" Seite des "AccessDenied" Controllers weiterleiten.
            if (model == null)
			{
                return RedirectToAction("ReadError", "AccessDenied");
            }

            // Die Liegenschaft der Meldung aus der Datenbank abrufen.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.LiegenschaftID == meldung.LiegenschaftID).ToListAsync();

            // Eine Liste von Liegenschaften für die Meldung erstellen und mit den Daten aus der Datenbank befüllen.
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

            // Eine Instanz von MeldungsView erstellen und mit den Daten der Liegenschaft, des Benutzers und der Meldung befüllen.
            MeldungsView meldungsview = new()
            {
                Liegenschaftslist = liegenschaftslist,
                User = model,
                Meldung = meldung
            };

            // Die Details-Ansicht mit der erstellten MeldungsView anzeigen.
            return View(meldungsview);
        }



        // GET: Meldung/Details/5
        public async Task<IActionResult> UserDetails(string? id)
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

            // Die entsprechende Meldung in der Datenbank anhand der übergebenen ID suchen.
            var meldung = await _context.Meldungs.FirstOrDefaultAsync(m => m.MeldungID == id);

            // Falls die Meldung nicht gefunden wurde, NotFound zurückgeben.
            if (meldung == null)
            {
                return NotFound();
            }

            // Die Liegenschaft der Meldung aus der Datenbank abrufen.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.Where(x => x.LiegenschaftID == meldung.LiegenschaftID).ToListAsync();

            // Eine Liste von Liegenschaften für die Meldung erstellen und mit den Daten aus der Datenbank befüllen.
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

            // Eine Instanz von MeldungsView erstellen und mit den Daten der Liegenschaft und der Meldung befüllen.
            MeldungsView meldungsview = new()
            {
                Liegenschaftslist = liegenschaftslist,
                Meldung = meldung
            };

            // Die Details-Ansicht mit der erstellten MeldungsView anzeigen.
            return View(meldungsview);
        }
    }
}
