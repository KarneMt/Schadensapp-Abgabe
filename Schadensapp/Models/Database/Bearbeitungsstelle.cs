using System.ComponentModel.DataAnnotations;


namespace Schadensapp.Models.Database
{
    public class Bearbeitungsstelle
    {
        public int BearbeitungsstelleID { get; set; }
        [Required(ErrorMessage = "Eine Angabe zur Map-Gruppe wird benötigt")]
        public string? MapGruppe { get; set; }
        [Required(ErrorMessage = "Eine Angabe zum Namen der Bearbeitungsstelle wird benötigt")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Eine Angabe zur Telefonnummer wird benötigt")]
        public string? Telefone { get; set; }
        [Required(ErrorMessage = "Eine Angabe zur E-Mail wird benötigt")]
        public string? EMail { get; set; }
        public bool? IsActive { get; set; }
    }
}
