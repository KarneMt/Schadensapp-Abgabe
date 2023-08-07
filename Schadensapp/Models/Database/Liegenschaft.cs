namespace Schadensapp.Models.Database
{
    public class Liegenschaft
    {
        public int LiegenschaftID { get; set; }
        public string? Name { get; set; }
        public int DienstleisterID { get; set; }
        public int AdresseID { get; set; }
        public bool? IsActive { get; set; }
        public int BearbeitungsstelleID { get; set; }
    }
}
