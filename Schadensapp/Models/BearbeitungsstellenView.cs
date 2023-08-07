using Schadensapp.Models.Database;

namespace Schadensapp.Models
{
    public class BearbeitungsstellenView
    {

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten.
        public Bearbeitungsstelle Bearbeitungsstelle { get; set; }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten.
        public List<LiegenschaftenListe>? LiegenschaftenListe { get; set; }
    }

    public class BearbeitungsstellenViewIndex
    {
        public List<BearbeitungsstellenView>? BearbeitungsstellenView { get; set; }
        public string? Filter { get; set; }
    }
}
