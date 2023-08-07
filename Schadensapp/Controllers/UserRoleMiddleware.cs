using Microsoft.EntityFrameworkCore;
using Schadensapp.Database.Context;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.Versioning;
using System.Security.Principal;

public class UserRoleMiddleware
{
	private readonly RequestDelegate _next;
	private readonly string _connectionString;

	public UserRoleMiddleware(RequestDelegate next, string connectionString)
	{
		_next = next;
		_connectionString = connectionString;
	}

    // Eine interne Klasse zur Speicherung der Benutzerrolle und der zugehörigen Gruppenliste.
    public class GroupRole
	{
		public string? UserRole { get; set; }
        public List<string>? GroupList { get; set; }
    }

    // Diese Methode wird von der Middleware aufgerufen, wenn eine Anfrage verarbeitet wird.
    [SupportedOSPlatform("windows")]
	public async Task Invoke(HttpContext context)
	{
		// Ihre Logik zur Bestimmung der Benutzerrolle hier...
		GroupRole groupRole = await DetermineUserRole(context);

		// Die Benutzerrolle im HttpContext speichern
		context.Items["UserRole"] = groupRole.UserRole;
        context.Items["GroupList"] = groupRole.GroupList;

        await _next(context);
	}

    // Methode zur Bestimmung der Benutzerrolle und Gruppenliste des angemeldeten Benutzers.
    private async Task<GroupRole> DetermineUserRole(HttpContext username)
	{
        GroupRole groupRole = new()
        {
            GroupList = new()
        };

        string userSID = "";
		if (username != null && username.User.Identity != null && username.User.Identity.Name != null)
		{
			WindowsIdentity? windowsIdentity = username.User.Identity as WindowsIdentity;
			if (windowsIdentity != null && windowsIdentity.User != null)
			{
				userSID = windowsIdentity.User.Value.ToString();
			}
		}
        groupRole.UserRole = "User";
        
		if (userSID != "")
		{
            // Das Windows-Identitätsobjekt erstellen und die Benutzerinformationen abrufen.
            PrincipalContext context = new(ContextType.Domain, Domain.GetComputerDomain().Name);
			UserPrincipal user = UserPrincipal.FindByIdentity(context, userSID);

            // Die Gruppen ermitteln, zu denen der Benutzer gehört.
            PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

            // Ein Datenbankkontextobjekt erstellen, um auf die Datenbank zuzugreifen.
            var optionsBuilder = new DbContextOptionsBuilder<SchadensappDbContext>();
			optionsBuilder.UseOracle(_connectionString);
			using SchadensappDbContext _ctx = new(optionsBuilder.Options);

            // Überprüfen, ob der Benutzer Mitglied einer aktiven Bearbeitungsstelle ist.
            foreach (var item in groups)
			{
				if (item.Name != null && _ctx != null)
				{
					var group = await _ctx.Bearbeitungsstelles.Where(x => x.IsActive == true).FirstOrDefaultAsync(x => x.MapGruppe == item.Name);
					if (group != null)
					{
						groupRole.GroupList.Add(group.BearbeitungsstelleID.ToString());
						groupRole.UserRole = "Admin";
					}
				}
			}
		}
		return groupRole;
	}
}
