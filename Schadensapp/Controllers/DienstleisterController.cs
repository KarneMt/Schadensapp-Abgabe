using Microsoft.AspNetCore.Mvc;
using Schadensapp.Database.Context;

namespace Schadensapp.Controllers
{
    // Dies ist der Controller für die Entität "Dienstleister".
    public partial class DienstleisterController : Controller
    {
        // Datenbankkontext für die Anwendung.
        private readonly SchadensappDbContext _context;
        // Schnittstelle zum Zugriff auf das HTTP-Kontextobjekt.
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Konstruktor des Controllers, der die Abhängigkeiten über Dependency Injection erhält.
        public DienstleisterController(SchadensappDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hilfsmethode, die prüft, ob ein Dienstleister mit der angegebenen ID existiert.
        private bool DienstleisterExists(int id)
        {
            return (_context.Dienstleisters?.Any(e => e.DienstleisterID == id)).GetValueOrDefault();
        }
    }
}
