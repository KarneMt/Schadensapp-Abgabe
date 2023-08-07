using Microsoft.AspNetCore.Mvc;
using Schadensapp.Database.Context;

namespace Schadensapp.Controllers
{
    // Dies ist der Controller für die Entität "Bearbeitungsstelle".
    public partial class BearbeitungsstelleController : Controller
    {
        // Datenbankkontext für die Anwendung.
        private readonly SchadensappDbContext _context;
        // Schnittstelle zum Zugriff auf das HTTP-Kontextobjekt.
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Konstruktor des Controllers, der die Abhängigkeiten über Dependency Injection erhält.
        public BearbeitungsstelleController(SchadensappDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Hilfsmethode, die prüft, ob eine Bearbeitungsstelle mit der angegebenen ID existiert.
        private bool BearbeitungsstelleExists(int id)
        {
            return (_context.Bearbeitungsstelles?.Any(e => e.BearbeitungsstelleID == id)).GetValueOrDefault();
        }
    }
}
