namespace Schadensapp.Models.Database
{
    public class Adresse
    {
        public int AdresseID { get; set; }
        public string? Strasse { get; set; }
        public string? Hausnummer { get; set; }
        public int? Postleitzahl { get; set; }
        public string? Stadt { get; set; }
    }
}
