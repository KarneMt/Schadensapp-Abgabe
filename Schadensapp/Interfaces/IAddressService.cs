using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schadensapp.Database.Context;
using Schadensapp.Models.Database;

namespace Schadensapp.Interfaces
{
    public interface IAddressService
    {

        // GET: Address
        public Adresse? GetAddress(int? id, SchadensappDbContext context);

        // Address/Create
        public Adresse? Create(Adresse adresse, SchadensappDbContext context);

        // Address/Edit
        public Task<Adresse?> EditAsync(Adresse adresse, SchadensappDbContext context);

    }
}
