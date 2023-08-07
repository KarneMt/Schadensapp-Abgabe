using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using Schadensapp.Interfaces;
using Schadensapp.Models;
using System.Runtime.Versioning;
using Schadensapp.Database.Context;
using System.Security.Principal;

namespace Schadensapp.Services
{

	public class UserService : IUserService
	{
        // Datenbankkontext für die Anwendung.
        private readonly SchadensappDbContext _context;
        // Schnittstelle zum Zugriff auf das HTTP-Kontextobjekt.
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Konstruktor des Service, der die Abhängigkeiten über Dependency Injection erhält.
        public UserService(SchadensappDbContext context, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
		}

		[SupportedOSPlatform("windows")]
		public UserModel? GetUser()
		{

			string userSID = "";
			if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Identity != null && _httpContextAccessor.HttpContext.User.Identity.Name != null)
			{
                // Ermitteln der Windows-Identität des Benutzers
                WindowsIdentity? windowsIdentity = _httpContextAccessor.HttpContext.User.Identity as WindowsIdentity;
				if (windowsIdentity != null && windowsIdentity.User != null)
				{
					userSID = windowsIdentity.User.Value.ToString();
				}
			}

			UserModel? userModel = null;
			if (userSID != "")
			{
                // Erstellen eines PrincipalContext-Objekts, um Informationen über den Benutzer aus dem Active Directory zu erhalten
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Domain.GetComputerDomain().Name);
				UserPrincipal user = UserPrincipal.FindByIdentity(context, userSID);

                // Standardwert für den Bereich, falls die Abteilungsinformation nicht verfügbar ist
                string bereich = "nicht angegeben";

                // Überprüfen, ob das Benutzerobjekt Informationen zur Abteilung enthält
                if (user.GetUnderlyingObject() is DirectoryEntry directoryEntry)
                {
                    if (directoryEntry.Properties.Contains("department"))
                    {
                        var departmentProperty =  directoryEntry.Properties["department"];
                        if (departmentProperty.Count > 0 && departmentProperty[0] != null)
                        {
                            // Nehme den ersten Wert, um die erste Abteilung zu erhalten
                            bereich = departmentProperty[0].ToString();
                        }
                    }
				}

                // Erstellen eines UserModel-Objekts mit den abgerufenen Benutzerinformationen
                userModel = new UserModel()
				{
					EMail = user.EmailAddress,
					Vorname = user.Name,
					Nachname = user.Surname,
					Bereich = bereich,
					Telefon = user.VoiceTelephoneNumber,
					BenutzerID = user.Sid.ToString(),
				};
				return userModel;
			}
            // Wenn die Authentifizierung fehlgeschlagen ist oder der Benutzer keine Windows-Identität hat, wird null zurückgegeben.			
			return userModel;
        }

        [SupportedOSPlatform("windows")]

		public UserModel? GetUserWithSID(string? SID)
		{
			UserModel? userModel = null;
			if (SID != null)
			{
                // Erstellen eines PrincipalContext-Objekts, um Informationen über den Benutzer aus dem Active Directory zu erhalten
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Domain.GetComputerDomain().Name);
                UserPrincipal user = UserPrincipal.FindByIdentity(context, SID);

                // Überprüfen, ob der Benutzer im Active Directory gefunden wurde
                if (user == null)
                {
                    // Wenn der Benutzer nicht gefunden wurde, wird null zurückgegeben
                    return userModel;
                }

                // Standardwert für den Bereich, falls die Abteilungsinformation nicht verfügbar ist
                string bereich = "nicht angegeben";

                // Überprüfen, ob das Benutzerobjekt Informationen zur Abteilung enthält
                if (user.GetUnderlyingObject() is DirectoryEntry directoryEntry)
                {
                    if (directoryEntry.Properties.Contains("department"))
                    {
                        var departmentProperty = directoryEntry.Properties["department"];
                        if (departmentProperty.Count > 0 && departmentProperty[0] != null)
                        {
                            // Nehme den ersten Wert, um die erste Abteilung zu erhalten
                            bereich = departmentProperty[0].ToString();
                        }
                    }
                }

                // Erstellen eines UserModel-Objekts mit den abgerufenen Benutzerinformationen
                userModel = new UserModel()
				{
					EMail = user.EmailAddress,
					Vorname = user.Name,
					Nachname = user.Surname,
					Bereich = bereich,
					Telefon = user.VoiceTelephoneNumber,
					BenutzerID = user.Sid.ToString(),
				};
			}

            // Rückgabe des UserModel-Objekts oder null, wenn keine SID übergeben wurde.
            return userModel;
		}
	}
}