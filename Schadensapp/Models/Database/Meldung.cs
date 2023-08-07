using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace Schadensapp.Models.Database
{
    public class Meldung
    {
        public string? MeldungID { get; set; }
        public DateTime DatumUhr { get; set; }
        [Required(ErrorMessage = "Eine Angabe zum Raum wird benötigt")]

        public string? Raum { get; set; }
        [Required(ErrorMessage = "Eine Angabe zum Gebäudeteil wird benötigt")]

        public string? Gebäudeteil { get; set; }
        [Required(ErrorMessage = "Eine Angabe zur Ebene wird benötigt")]

        public string? Ebene { get; set; }
        public string? Status { get; set; }

        [Required(ErrorMessage = "Eine Beschreibung wird benötigt")]
        [StringLength(maximumLength: 200, MinimumLength = 10, ErrorMessage = "Die Länge der Beschreibung muss zwischen 10-200 Zeichen sein.")]
        public string? Beschreibung { get; set; }

        [StringLength(maximumLength: 200, MinimumLength = 0, ErrorMessage = "Die Länge der Anmerkung muss zwischen 0-200 Zeichen sein.")]
        public string? Anmerkung { get; set; }
        [Required(ErrorMessage = "Eine Angabe der Dringlichkeit wird benötigt")]

        public string? Dringlichkeit { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Eine Angabe der Liegenschaft wird benötigt")]
        public int? LiegenschaftID { get; set; }
        public string? BenutzerID { get; set; }
    }
}
