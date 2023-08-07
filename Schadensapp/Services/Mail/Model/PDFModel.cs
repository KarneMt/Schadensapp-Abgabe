using Schadensapp.Models;
using Schadensapp.Models.Database;

namespace Schadensapp.Services.Mail.Model
{
    public class PDFModel
    {
        public Meldung? Meldung { get; set; }
        public UserModel? User { get; set; }
        public Bearbeitungsstelle? Bearbeitungsstelle { get; set; }
        public Dienstleister? Dienstleister { get; set; }
        public Adresse? AdresseDienstleister { get; set; }
        public Liegenschaft? Liegenschaft { get; set; }
        public Adresse? AdresseLiegenschaft { get; set; }

    }
}
