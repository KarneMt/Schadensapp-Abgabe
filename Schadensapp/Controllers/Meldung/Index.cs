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
        public async Task<IActionResult> UserIndex(string searchString, int liegenschaftenSearch, bool eigeneIsTrue)
        {
            // Die Benutzerrolle aus dem HttpContext abrufen
            string? userRole = HttpContext.Items["UserRole"] as string;
            ViewBag.UserRole = userRole;

            // Wenn die Benutzerrolle nicht vorhanden ist, zur Zugriffsverweigerungsseite weiterleiten.
            if (userRole == null)
            {
                return RedirectToAction("AccessDenied", "AccessDenied");
            }

            // Abrufen aller Liegenschaften aus der Datenbank.
            List<Liegenschaft> liegenschaftdb = await _context.Liegenschafts.ToListAsync();

            // Erstellen einer Liste von Liegenschaften, um die Daten für die Indexseite aufzubereiten.
            List<LiegenschaftenListe> liegenschaftslist = new();
            foreach (var x in liegenschaftdb)
            {
                // Abrufen der Adresse für die aktuelle Liegenschaft.
                Adresse? adresse = new AddressService().GetAddress(x.AdresseID, _context);

                // Erstellen eines LiegenschaftenListe-Objekts, um die Daten für die Indexseite aufzubereiten.
                LiegenschaftenListe liegenschaften = new()
                {
                    LiegenschaftID = x.LiegenschaftID,
                    Name = x.Name,
                    Hausnummer = adresse?.Hausnummer,
                    Postleitzahl = adresse?.Postleitzahl,
                    Stadt = adresse?.Stadt,
                    Strasse = adresse?.Strasse,
                };

                // Das LiegenschaftenListe-Objekt wird der Liste hinzugefügt.
                liegenschaftslist.Add(liegenschaften);
            }

            // Abrufen aller Meldungen aus der Datenbank.
            var meldungen = await _context.Meldungs.ToListAsync();

            // Erstellen einer Liste von MeldungsViewIndex-Objekten, um die Daten für die Indexseite aufzubereiten.
            List<MeldungsViewIndex> list = new();

            // Wenn eigeneIsTrue nicht wahr ist, Meldungen durchgehen und MeldungsViewIndex-Objekte erstellen
            // basierend auf den Liegenschaften für jede Meldung.
            if (eigeneIsTrue != true)
            {
                foreach (var item in meldungen)
                {
                    var liegenschaft = liegenschaftslist.FirstOrDefault(x => x.LiegenschaftID == item.LiegenschaftID);
                    if (liegenschaft != null)
                    {
                        // Eigenschaften setzen...
                        MeldungsViewIndex eintrag = new()
                        {
                            MeldungID = item.MeldungID,
                            Beschreibung = item.Beschreibung,
                            Status = item.Status,
                            Ebene = item.Ebene,
                            Gebäudeteil = item.Gebäudeteil,
                            Raum = item.Raum,
                            DatumUhr = item.DatumUhr,
                            LiegenschaftsName = liegenschaft.Name,
                            Strasse = liegenschaft.Strasse,
                            Hausnummer = liegenschaft.Hausnummer,
                            Postleitzahl = liegenschaft.Postleitzahl,
                            Stadt = liegenschaft.Stadt
                        };
                        list.Add(eintrag);
                    }
                }

            }
            else
            {
                // Andernfalls, wenn eigeneIsTrue wahr ist, das Benutzermodell abrufen und Meldungen durchgehen,
                // um MeldungsViewIndex-Objekte zu erstellen, wenn die BenutzerID übereinstimmt.

                UserModel? model = new UserService(_context, _httpContextAccessor).GetUser();
                if (model == null)
                {
                    return RedirectToAction("AccessDenied", "AccessDenied");
                }

                foreach (var item in meldungen)
                {
                    if (item.BenutzerID == model.BenutzerID)
                    {
                        var liegenschaft = liegenschaftslist.FirstOrDefault(x => x.LiegenschaftID == item.LiegenschaftID);
                        if (liegenschaft != null)
                        {
                            // Eigenschaften setzen...
                            MeldungsViewIndex eintrag = new()
                            {
                                MeldungID = item.MeldungID,
                                Beschreibung = item.Beschreibung,
                                Status = item.Status,
                                Ebene = item.Ebene,
                                Gebäudeteil = item.Gebäudeteil,
                                Raum = item.Raum,
                                DatumUhr = item.DatumUhr,
                                LiegenschaftsName = liegenschaft.Name,
                                Strasse = liegenschaft.Strasse,
                                Hausnummer = liegenschaft.Hausnummer,
                                Postleitzahl = liegenschaft.Postleitzahl,
                                Stadt = liegenschaft.Stadt
                            };
                            list.Add(eintrag);
                        }
                    }
                }
            }

            // Sortieren der Liste der Meldungen nach dem Datum/Uhrzeit in absteigender Reihenfolge.
            list.Sort((x, y) => DateTime.Compare(y.DatumUhr, x.DatumUhr));

            // Überprüfen, ob eine Suche durchgeführt wurde (durch searchString oder liegenschaftenSearch).
            if (!string.IsNullOrEmpty(searchString) || liegenschaftenSearch >= 1)
            {
                // Wenn eine Suche durchgeführt wurde, wird die Methode SearchMeldung aufgerufen, um die Liste zu filtern.
                // meldungsViewIndexListFiltered enthält die gefilterte Liste.
                MeldungsViewIndexList meldungsViewIndexListFiltered = new SearchService().SearchMeldung(list, searchString, liegenschaftenSearch, liegenschaftslist);
                return View(meldungsViewIndexListFiltered);
            }

            // Wenn keine Suche durchgeführt wurde, wird die ungefilterte Liste zusammen mit der Liste der Liegenschaften an die Ansicht übergeben.
            MeldungsViewIndexList meldungsViewIndexList = new()
            {
                MeldungsViewIndex = list,
                LiegenschaftenListe = liegenschaftslist

            };
            return View(meldungsViewIndexList);
        }

        // GET: Meldung
        public async Task<IActionResult> Index(string searchString, int liegenschaftenSearch)
        {
            string? userRole = HttpContext.Items["UserRole"] as string;
            List<string>? groupList = HttpContext.Items["GroupList"] as List<string>;
            ViewBag.UserRole = userRole;
            if (groupList == null || userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "AccessDenied");
            }
            int bearbeitungsstellenID = 0;

            List<Liegenschaft> liegenschaftdb = new();
            foreach (string item in groupList)
            {
                int.TryParse(item, out bearbeitungsstellenID);
                liegenschaftdb.AddRange(await _context.Liegenschafts.Where(x => x.BearbeitungsstelleID == bearbeitungsstellenID).ToListAsync());
            }

            List<LiegenschaftenListe> liegenschaftslist = new();
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

            List<Meldung> meldungen = new();
            foreach (var item in liegenschaftdb)
            {
                meldungen.AddRange(await _context.Meldungs.Where(x => x.LiegenschaftID == item.LiegenschaftID).ToListAsync());
            }

            List<MeldungsViewIndex> list = new();

            foreach (var item in meldungen)
            {
                var liegenschaft = liegenschaftslist.FirstOrDefault(x => x.LiegenschaftID == item.LiegenschaftID);
                if (liegenschaft != null)
                {
                    MeldungsViewIndex eintrag = new()
                    {
                        MeldungID = item.MeldungID,
                        Beschreibung = item.Beschreibung,
                        Status = item.Status,
                        Ebene = item.Ebene,
                        Gebäudeteil = item.Gebäudeteil,
                        Raum = item.Raum,
                        DatumUhr = item.DatumUhr,
                        LiegenschaftsName = liegenschaft.Name,
                        Strasse = liegenschaft.Strasse,
                        Hausnummer = liegenschaft.Hausnummer,
                        Postleitzahl = liegenschaft.Postleitzahl,
                        Stadt = liegenschaft.Stadt
                    };
                    list.Add(eintrag);
                }
            }

            list.Sort((x, y) => DateTime.Compare(y.DatumUhr, x.DatumUhr));


            if (!string.IsNullOrEmpty(searchString) || liegenschaftenSearch >= 1)
            {
                MeldungsViewIndexList meldungsViewIndexListFiltered = new SearchService().SearchMeldung(list, searchString, liegenschaftenSearch, liegenschaftslist);
                return View(meldungsViewIndexListFiltered);
            }

            MeldungsViewIndexList meldungsViewIndexList = new()
            {
                MeldungsViewIndex = list,
                LiegenschaftenListe = liegenschaftslist
            };
            return View(meldungsViewIndexList);
        }
    }
}