using Schadensapp.Models.Database;
using System.ComponentModel.DataAnnotations;

namespace Schadensapp.Models
{
    public class MeldungsViewIndex
    {
        public string? MeldungID { get; set; }
        public DateTime DatumUhr { get; set; }
        public string? Raum { get; set; }
        public string? Gebäudeteil { get; set; }
        public string? Ebene { get; set; }
        public string? Status { get; set; }
        public string? Beschreibung { get; set; }
        public string? LiegenschaftsName { get; set; }
        public string? Strasse { get; set; }
        public string? Hausnummer { get; set; }
        public int? Postleitzahl { get; set; }
        public string? Stadt { get; set; }

    }

    #pragma warning disable CS8618
    public class MeldungsViewIndexList {
        public List<MeldungsViewIndex> MeldungsViewIndex { get; set; }
        public string? Filter { get; set; }
        public int? LiegenschaftenFilter { get; set; }
        public bool? EigeneMeldungen { get; set; }
        public List<LiegenschaftenListe>? LiegenschaftenListe { get; set; }

    }

    public class Meldungen
    {
        public List<Meldung>? MeldungsEintrag { get; set; }
    }

    public class MeldungsView
    {
        public List<LiegenschaftenListe>? Liegenschaftslist { get; set; }
        public UserModel User { get; set; }
        public Meldung Meldung { get; set; }
    }
    #pragma warning restore CS8618
}
