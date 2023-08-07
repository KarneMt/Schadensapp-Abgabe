using Microsoft.AspNetCore.Mvc;
using Schadensapp.Database.Context;

namespace Schadensapp.Controllers
{
    // Dies ist der Controller für die Entität "Liegenschaft".
    public partial class LiegenschaftController : Controller
    {
        // Datenbankkontext für die Anwendung.
        private readonly SchadensappDbContext _context;
        // Schnittstelle zum Zugriff auf das HTTP-Kontextobjekt.
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Konstruktor des Controllers, der die Abhängigkeiten über Dependency Injection erhält.
        public LiegenschaftController(SchadensappDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hilfsmethode, die prüft, ob eine Liegenschaft mit der angegebenen ID existiert.
        private bool LiegenschaftExists(int id)
        {
            return (_context.Liegenschafts?.Any(e => e.LiegenschaftID == id)).GetValueOrDefault();
        }
    }
}
