using Microsoft.AspNetCore.Mvc;
using Schadensapp.Models.Database;
using Schadensapp.Models;
using Schadensapp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Schadensapp.Controllers
{
    [Authorize]
    public partial class DienstleisterController
    {
        // GET: Dienstleister/Edit/5
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

            // Überprüfen, ob die übergebene ID null ist oder die Datenbank-Tabelle "Dienstleisters" null ist.
            // Falls einer dieser Fälle zutrifft, NotFound zurückgeben.
            if (id == null || _context.Dienstleisters == null)
            {
                return NotFound();
            }

            // Den Dienstleister mit der übergebenen ID aus der Datenbank abrufen.
            var dienstleister = await _context.Dienstleisters.FindAsync(id);

            // Falls der Dienstleiter nicht gefunden wurde, NotFound zurückgeben.
            if (dienstleister == null)
            {
                return NotFound();
            }

            // Die Adresse des Dienstleisters aus der Datenbank abrufen.
            Adresse? adresse = new AddressService().GetAddress(dienstleister.AdresseID, _context);

            // Eine Instanz von DienstleisterView erstellen und mit den Daten des Dienstleisters und der Adresse füllen.
            DienstleisterView dienstleisterView = new DienstleisterView
            {
                DienstleisterID = dienstleister.DienstleisterID,
                Abteilung = dienstleister.Abteilung,
                IsActive = dienstleister.IsActive,
                Firmenname = dienstleister.Firmenname,
                AdresseID = dienstleister.AdresseID,
                Hausnummer = adresse?.Hausnummer,
                Postleitzahl = adresse?.Postleitzahl,
                Stadt = adresse?.Stadt,
                Strasse = adresse?.Strasse,
                EMail = dienstleister.EMail
            };

            // Die Details des Dienstleisters anzeigen.
            return View(dienstleisterView);
        }

        // POST: Dienstleister/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DienstleisterID,Firmenname,Abteilung,AdresseID,IsActive,Strasse,Hausnummer,Postleitzahl,Stadt,EMail")] DienstleisterView dienstleisterView)
        {
            if (id != dienstleisterView.DienstleisterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Erstellt ein Dienstleister-Objekt mit den Werten aus dem DienstleisterView-Objekt.
                Dienstleister dienstleister = new Dienstleister
                {
                    DienstleisterID = dienstleisterView.DienstleisterID,
                    Firmenname = dienstleisterView.Firmenname,
                    Abteilung = dienstleisterView.Abteilung,
                    AdresseID = dienstleisterView.AdresseID,
                    IsActive = dienstleisterView.IsActive,
                    EMail = dienstleisterView.EMail
                };

                // Erstellt ein Adresse-Objekt mit den Werten aus dem DienstleisterView-Objekt.
                Adresse adresse = new Adresse
                {
                    AdresseID = dienstleisterView.AdresseID,
                    Strasse = dienstleisterView.Strasse,
                    Hausnummer = dienstleisterView.Hausnummer,
                    Postleitzahl = dienstleisterView.Postleitzahl,
                    Stadt = dienstleisterView.Stadt
                };
                try
                {
                    // Aktualisiert den Dienstleister in der Datenbank.
                    _context.Update(dienstleister);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Wenn der Dienstleister nicht gefunden wird, wird eine NotFound-Antwort zurückgegeben.
                    if (!DienstleisterExists(dienstleister.DienstleisterID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Bearbeitet die Adresse in der Datenbank und aktualisiert sie.
                Adresse? answer = await new AddressService().EditAsync(adresse, _context);

                if (answer == null)
                {
                    return NotFound();
                }

                // Nach erfolgreicher Bearbeitung wird zur Index-Ansicht zurückgeleitet.
                return RedirectToAction(nameof(Index));
            }

            // Wenn das Model ungültig ist, wird die View mit den vorhandenen Eingaben erneut angezeigt.
            return View(dienstleisterView);
        }
    }
}
