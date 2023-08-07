using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Database.Context;
using Schadensapp.Interfaces;
using Schadensapp.Models.Database;

namespace Schadensapp.Services
{
    public class AddressService : IAddressService
    {

        public Adresse? Create(Adresse adresse, SchadensappDbContext context)
        {
            if (adresse == null || context.Adresses == null)
            {
                return null;
            }
            else
            {
                context.Adresses.Add(adresse);
                context.SaveChanges();
                return adresse;
            }
        }

        public async Task<Adresse?> EditAsync(Adresse adresse, SchadensappDbContext context)
        {
            if (adresse == null || context.Adresses == null)
            {
                return null;
            }
            else
            {
                Adresse? dbadresse = GetAddress(adresse.AdresseID, context);
                if (dbadresse != null)
                {
                    try
                    {
                        context.Update(adresse);
                        await context.SaveChangesAsync();
                        return adresse;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AdresseExists(adresse.AdresseID, context))
                        {
                            return null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
                return null;
        }

        public Adresse? GetAddress(int? id, SchadensappDbContext context)
        {
            if (id == null || context.Adresses == null)
            {
                return null;
            }

            Adresse? adresse = context.Adresses.FirstOrDefault(m => m.AdresseID == id);
            return adresse;
        }

            private bool AdresseExists(int id, SchadensappDbContext context)
            {
                return (context.Adresses?.Any(e => e.AdresseID == id)).GetValueOrDefault();
            }
        }
}
