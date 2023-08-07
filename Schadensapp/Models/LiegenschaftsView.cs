using System.ComponentModel.DataAnnotations;

using Schadensapp.Models.Database;

namespace Schadensapp.Models
{
    public class LiegenschaftsView
    {
        public int LiegenschaftID { get; set; }

        [Required(ErrorMessage = "Es muss der Name der Liegenschaft angegeben werden")]
        public string? Name { get; set; }
        public int AdresseID { get; set; }
        public bool? IsActive { get; set; }

        [Required(ErrorMessage = "Es muss eine Straße angegeben werden")]
        public string? Strasse { get; set; }

        [Required(ErrorMessage = "Es muss eine Hausnummer angegeben werden")]
        public string? Hausnummer { get; set; }

        [Required(ErrorMessage = "Es muss eine Postleitzahl angegeben werden")]
        public int? Postleitzahl { get; set; }

        [Required(ErrorMessage = "Es muss eine Stadt angegeben werden")]
        public string? Stadt { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Es muss ein Dienstleister ausgewählt werden")]
        public int DienstleisterID { get; set; }
        public string? DienstleisterFirmenname { get; set; }
        public string? DienstleisterAbteilung { get; set; }
        public string? DienstleisterStrasse { get; set; }
        public string? DienstleisterHausnummer { get; set; }
        public int? DienstleisterPostleitzahl { get; set; }
        public string? DienstleisterStadt { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Es muss eine Bearbeitungsstelle ausgewählt werden")]
        public int BearbeitungsstelleID { get; set; }
        public string? BearbeitungsstelleName { get; set; }
        public string? BearbeitungsstelleMapGruppe { get; set; }
        public string? BearbeitungsstelleTelefone { get; set; }
        public string? BearbeitungsstelleEMail { get; set; }
        public List<DienstleisterView>? DienstleistersListe { get; set; }
        public List<Bearbeitungsstelle>? BearbeitungsstellenListe { get; set; }
    }

    public class LiegenschaftenListe
    {
        public int LiegenschaftID { get; set; }
        public string? Name { get; set; }
        public string? Strasse { get; set; }
        public string? Hausnummer { get; set; }
        public int? Postleitzahl { get; set; }
        public string? Stadt { get; set; }
    }

    public class LiegenschaftsViewIndex
    {
        public List<LiegenschaftsView>? LiegenschaftsView { get; set; }
        public string? Filter { get; set;  }
    }
}
