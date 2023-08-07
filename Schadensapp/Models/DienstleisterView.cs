using System.ComponentModel.DataAnnotations;


namespace Schadensapp.Models
{
    public class DienstleisterView
    {
        public int DienstleisterID { get; set; }
        [Required(ErrorMessage = "Es muss der Name des Dienstleisters angegeben werden")]
        public string? Firmenname { get; set; }
        [Required(ErrorMessage = "Es muss die Abteilung des Dienstleisters angegeben werden")]
        public string? Abteilung { get; set; }

        public bool? IsActive { get; set; }
        public int AdresseID { get; set; }
        [Required(ErrorMessage = "Es muss eine Straße angegeben werden")]
        public string? Strasse { get; set; }
        [Required(ErrorMessage = "Es muss eine Hausnummer angegeben werden")]
        public string? Hausnummer { get; set; }
        [Required(ErrorMessage = "Es muss eine Postleitzahl angegeben werden")]
        public int? Postleitzahl { get; set; }
        [Required(ErrorMessage = "Es muss eine Stadt angegeben werden")]
        public string? Stadt { get; set; }
        [Required(ErrorMessage = "Es muss eine E-Mail angegeben werden")]
        public string? EMail { get; set; }


        public List<LiegenschaftenListe>? LiegenschaftenListe { get; set; }
    }

    public class DienstleisterViewIndex
    {
        public List<DienstleisterView>? DienstleisterView { get; set; }
        public string? Filter { get; set; }
    }
}
