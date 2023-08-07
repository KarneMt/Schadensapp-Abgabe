using Microsoft.AspNetCore.Mvc;
using Schadensapp.Interfaces;
using Schadensapp.Models;

namespace Schadensapp.Services
{
    public class SearchService : ISearchInterface
    {
        // Methode zum Suchen von Bearbeitungsstellen in der Liste basierend auf dem Suchbegriff
        public BearbeitungsstellenViewIndex SearchBearbeitungsstellen(List<BearbeitungsstellenView> list, string searchString)
        {
            IEnumerable<BearbeitungsstellenView> filteredList = list.Where(s => (s.Bearbeitungsstelle.MapGruppe != null) && s.Bearbeitungsstelle.MapGruppe.Contains(searchString) || (s.Bearbeitungsstelle.Name != null) && s.Bearbeitungsstelle.Name.Contains(searchString) || (s.Bearbeitungsstelle.EMail != null) && s.Bearbeitungsstelle.EMail.Contains(searchString) || (s.Bearbeitungsstelle.Telefone != null) && s.Bearbeitungsstelle.Telefone.Contains(searchString));
            BearbeitungsstellenViewIndex bearbeitungsstellenViewIndex = new()
            {
                BearbeitungsstellenView = filteredList.ToList(),
                Filter = searchString
            };
            return bearbeitungsstellenViewIndex;
        }

        // Methode zum Suchen von Dienstleister in der Liste basierend auf dem Suchbegriff
        public DienstleisterViewIndex SearchDienstleister(List<DienstleisterView> list, string searchString)
        {

            IEnumerable<DienstleisterView> filteredList = list.Where(s => (s.EMail != null) && s.EMail.Contains(searchString) || (s.Abteilung != null) && s.Abteilung.Contains(searchString) || (s.Firmenname != null) && s.Firmenname.Contains(searchString) || (s.Hausnummer != null) && s.Hausnummer.Contains(searchString) || (s.Strasse != null) && s.Strasse.Contains(searchString) || (s.Stadt != null) && s.Stadt.Contains(searchString) || (s.Postleitzahl.HasValue) && int.TryParse(searchString, out int r) && (s.Postleitzahl == r ));
            DienstleisterViewIndex dienstleisterViewIndexFiltered = new()
            {
                DienstleisterView = filteredList.ToList(),
                Filter = searchString
            };
            return dienstleisterViewIndexFiltered;
        }

        // Methode zum Suchen von Liegenschaften in der Liste basierend auf dem Suchbegriff
        public LiegenschaftsViewIndex SearchLiegenschaften(List<LiegenschaftsView> list, string searchString)
        {
            IEnumerable<LiegenschaftsView> filteredList = list.Where(s => (s.BearbeitungsstelleName != null) && s.BearbeitungsstelleName.Contains(searchString) || (s.DienstleisterFirmenname != null) && s.DienstleisterFirmenname.Contains(searchString) || (s.Name != null) && s.Name.Contains(searchString) || (s.Hausnummer != null) && s.Hausnummer.Contains(searchString) || (s.Strasse != null) && s.Strasse.Contains(searchString) || (s.Stadt != null) && s.Stadt.Contains(searchString) || (s.Postleitzahl.HasValue) && int.TryParse(searchString, out int r) && (s.Postleitzahl == r));
            LiegenschaftsViewIndex liegenschaftsViewIndex = new()
            {
                LiegenschaftsView = filteredList.ToList(),
                Filter = searchString
            };
            return liegenschaftsViewIndex;
        }

        // Methode zum Durchsuchen von Meldungen
        public MeldungsViewIndexList SearchMeldung(List<MeldungsViewIndex> list, string searchString, int liegenschaftenSearch, List<LiegenschaftenListe> liegenschaftslist)
        {
            string? liegenschaftsName = null;
            if(liegenschaftslist != null && liegenschaftenSearch >= 1)
            {
                var temp = liegenschaftslist.Find(x  => x.LiegenschaftID == liegenschaftenSearch);
                #pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises. Wird danach nur noch mit Null-Prüfung verwendet
                liegenschaftsName = temp.Name;
                #pragma warning restore CS8602
            }

            //Es wird nur nach Liegenschaften gefiltert
            if(liegenschaftsName != null && searchString == null) {
                IEnumerable<MeldungsViewIndex> filteredList = list.Where(s => s.LiegenschaftsName == liegenschaftsName);
                MeldungsViewIndexList meldungsViewIndexListFiltered = new()
                {
                    MeldungsViewIndex = filteredList.ToList(),
                    LiegenschaftenFilter = liegenschaftenSearch,
                    LiegenschaftenListe = liegenschaftslist
                };
                return meldungsViewIndexListFiltered;

            }

            //Es wird nur nach Sucheingabe gefiltert
            if (liegenschaftsName == null && searchString != null) {
                searchString = searchString.ToLower();
                IEnumerable<MeldungsViewIndex> filteredList = list.Where(s => (s.Raum != null) && s.Raum.ToLower().Contains(searchString) || (s.Beschreibung != null) && s.Beschreibung.ToLower().Contains(searchString) || (s.Ebene != null) && s.Ebene.ToLower().Contains(searchString) || (s.Gebäudeteil != null) && s.Gebäudeteil.ToLower().Contains(searchString) || (s.Status != null) && s.Status.ToLower().Contains(searchString) || (s.LiegenschaftsName != null) && s.LiegenschaftsName.ToLower().Contains(searchString) || (s.Hausnummer != null) && s.Hausnummer.ToLower().Contains(searchString) || (s.Strasse != null) && s.Strasse.ToLower().Contains(searchString) || (s.Stadt != null) && s.Stadt.ToLower().Contains(searchString) || (s.Postleitzahl.HasValue) && int.TryParse(searchString, out int r) && (s.Postleitzahl == r));
                MeldungsViewIndexList meldungsViewIndexListFiltered = new()
                {
                    MeldungsViewIndex = filteredList.ToList(),
                    Filter = searchString,
                    LiegenschaftenListe = liegenschaftslist
                };
                return meldungsViewIndexListFiltered;


            }

            //Es wird nach Liegenschaften und Sucheingabe gefiltert
            if (liegenschaftsName != null && searchString != null) {
                searchString = searchString.ToLower();
                IEnumerable<MeldungsViewIndex> filteredList = list.Where(s => s.LiegenschaftsName == liegenschaftsName && (s.Raum != null) && s.Raum.ToLower().Contains(searchString) || (s.Beschreibung != null) && s.Beschreibung.ToLower().Contains(searchString) || (s.Ebene != null) && s.Ebene.ToLower().Contains(searchString) || (s.Gebäudeteil != null) && s.Gebäudeteil.ToLower().Contains(searchString) || (s.Status != null) && s.Status.ToLower().Contains(searchString) || (s.LiegenschaftsName != null) && s.LiegenschaftsName.ToLower().Contains(searchString) || (s.Hausnummer != null) && s.Hausnummer.ToLower().Contains(searchString) || (s.Strasse != null) && s.Strasse.ToLower().Contains(searchString) || (s.Stadt != null) && s.Stadt.ToLower().Contains(searchString) || (s.Postleitzahl.HasValue) && int.TryParse(searchString, out int r) && (s.Postleitzahl == r));
                MeldungsViewIndexList meldungsViewIndexListFiltered = new()
                {
                    MeldungsViewIndex = filteredList.ToList(),
                    Filter = searchString,
                    LiegenschaftenFilter = liegenschaftenSearch,
                    LiegenschaftenListe = liegenschaftslist
                };
                return meldungsViewIndexListFiltered;
            }

            MeldungsViewIndexList meldungsViewIndexList = new()
            {
                MeldungsViewIndex = list
            };
            return meldungsViewIndexList;
        }
    }
}
