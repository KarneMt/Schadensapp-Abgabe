using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schadensapp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adresses",
                columns: table => new
                {
                    AdresseID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Strasse = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Hausnummer = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Postleitzahl = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Stadt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresses", x => x.AdresseID);
                });

            migrationBuilder.CreateTable(
                name: "Bearbeitungsstelles",
                columns: table => new
                {
                    BearbeitungsstelleID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MapGruppe = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    EMail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bearbeitungsstelles", x => x.BearbeitungsstelleID);
                });

            migrationBuilder.CreateTable(
                name: "Dienstleisters",
                columns: table => new
                {
                    DienstleisterID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Firmenname = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Abteilung = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    AdresseID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    EMail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dienstleisters", x => x.DienstleisterID);
                });

            migrationBuilder.CreateTable(
                name: "Liegenschafts",
                columns: table => new
                {
                    LiegenschaftID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    DienstleisterID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AdresseID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    BearbeitungsstelleID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liegenschafts", x => x.LiegenschaftID);
                });

            migrationBuilder.CreateTable(
                name: "Meldungs",
                columns: table => new
                {
                    MeldungID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DatumUhr = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Raum = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Gebäudeteil = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ebene = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Beschreibung = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Anmerkung = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    Dringlichkeit = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    LiegenschaftID = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    BenutzerID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meldungs", x => x.MeldungID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adresses");

            migrationBuilder.DropTable(
                name: "Bearbeitungsstelles");

            migrationBuilder.DropTable(
                name: "Dienstleisters");

            migrationBuilder.DropTable(
                name: "Liegenschafts");

            migrationBuilder.DropTable(
                name: "Meldungs");
        }
    }
}
