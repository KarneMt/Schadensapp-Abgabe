#nullable disable
using Microsoft.EntityFrameworkCore;
using Schadensapp.Models.Database;

namespace Schadensapp.Database.Context
{

    public class SchadensappDbContext : DbContext
    {
        public SchadensappDbContext(DbContextOptions<SchadensappDbContext> options) : base(options){}

        public DbSet<Adresse> Adresses { get; set; }
        public DbSet<Bearbeitungsstelle> Bearbeitungsstelles { get; set; }
        public DbSet<Dienstleister> Dienstleisters { get; set; }
        public DbSet<Liegenschaft> Liegenschafts { get; set; }
        public DbSet<Meldung> Meldungs { get; set; }
    }
}

/*
 Das obige C#-Codebeispiel definiert eine DbContext-Klasse mit dem Namen SchadensappDbContext. Diese Klasse ist für die Kommunikation mit der Datenbank verantwortlich und stellt Eigenschaften für verschiedene Entitätssätze bereit, die mit der Datenbank verknüpft sind.

Hier ist eine kurze Erläuterung zu den Eigenschaften der SchadensappDbContext:

    Adresse: Dies ist ein DbSet für den Zugriff auf die Entitätssammlung Adresses. Es repräsentiert eine Tabelle in der Datenbank, die Adressinformationen speichert.

    Bearbeitungsstelle: Dies ist ein DbSet für den Zugriff auf die Entitätssammlung Bearbeitungsstelles. Es repräsentiert eine Tabelle in der Datenbank, die Informationen über Bearbeitungsstellen speichert.

    Dienstleister: Dies ist ein DbSet für den Zugriff auf die Entitätssammlung Dienstleisters. Es repräsentiert eine Tabelle in der Datenbank, die Informationen über Dienstleister speichert.

    Liegenschaft: Dies ist ein DbSet für den Zugriff auf die Entitätssammlung Liegenschafts. Es repräsentiert eine Tabelle in der Datenbank, die Informationen über Liegenschaften speichert.

    Meldung: Dies ist ein DbSet für den Zugriff auf die Entitätssammlung Meldungs. Es repräsentiert eine Tabelle in der Datenbank, die Informationen über Meldungen speichert.

Die SchadensappDbContext-Klasse wird von DbContext abgeleitet und verwendet den Konstruktor mit dem Parameter options, der einen DbContextOptions<SchadensappDbContext>-Objekttyp erwartet. Dieses Objekt enthält die Konfigurationsoptionen für die Datenbankverbindung und wird normalerweise von der Startup-Klasse während der Konfiguration der Anwendung erstellt.

In dieser SchadensappDbContext-Klasse können Sie auch OnModelCreating-Methode überschreiben, um das Modell der Entitäten weiter zu konfigurieren oder Beziehungen zwischen den Entitäten zu definieren.
 */