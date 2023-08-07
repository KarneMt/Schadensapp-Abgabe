using Schadensapp.Interfaces;

namespace Schadensapp.Services
{
    public class UUIDService : IUUIDService
    {
        public string CreateUUIDwithDate()
        {
            Guid myuuid = Guid.NewGuid();
            DateTime dateTime = DateTime.Now;
            string myuuidString = dateTime.ToString("dd/MM/yyyy-HH:mm:ss.fff-") + myuuid.ToString();
            return myuuidString;
        }
    }
}

/*
 Die UUIDService-Klasse implementiert das Interface IUUIDService und bietet eine Methode CreateUUIDwithDate(), um eine UUID (Universally Unique Identifier) zusammen mit dem aktuellen Datum und der Uhrzeit zu erstellen.

Hier ist eine kurze Erläuterung zur Funktionsweise der Methode:

    Guid.NewGuid(): Diese Methode generiert eine neue zufällige GUID (UUID).

    DateTime.Now: Diese Methode gibt das aktuelle Datum und die Uhrzeit zurück.

    dateTime.ToString("dd/MM/yyyy-HH:mm:ss.fff-"): Dieser Teil formatiert das DateTime-Objekt in einem bestimmten Format, hier in "dd/MM/yyyy-HH:mm:ss.fff-", um Tag, Monat, Jahr, Stunden, Minuten, Sekunden und Millisekunden zu erhalten.

    myuuid.ToString(): Hier wird die zufällige GUID in einen String konvertiert.

Die Methode kombiniert das formatierte Datum und die Uhrzeit mit der GUID und gibt sie als eine Zeichenfolge (String) zurück. Das Ergebnis ist eine eindeutige Zeichenfolge, die das aktuelle Datum und die Uhrzeit sowie die UUID enthält.
 */