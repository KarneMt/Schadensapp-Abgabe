namespace Schadensapp.Models.Database
{
    public class Dienstleister
    {
        public int DienstleisterID { get; set; }
        public string? Firmenname { get; set; }
        public string? Abteilung { get; set; }
        public int AdresseID { get; set; }
        public bool? IsActive { get; set; }
        public string? EMail { get; set; }
    }
}
