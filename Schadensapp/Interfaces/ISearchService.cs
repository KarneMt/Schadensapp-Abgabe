using Schadensapp.Models;

namespace Schadensapp.Interfaces
{
    public interface ISearchInterface
    {
        public MeldungsViewIndexList SearchMeldung(List<MeldungsViewIndex> list, string searchString, int liegenschaftSearch, List<LiegenschaftenListe> liegenschaftslist);
        public BearbeitungsstellenViewIndex SearchBearbeitungsstellen(List<BearbeitungsstellenView> list, string searchString);
        public LiegenschaftsViewIndex SearchLiegenschaften(List<LiegenschaftsView> list, string searchString);
        public DienstleisterViewIndex SearchDienstleister(List<DienstleisterView> list, string searchString);
    }
}
