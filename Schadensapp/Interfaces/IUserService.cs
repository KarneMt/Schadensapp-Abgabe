using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using Schadensapp.Models;
using Schadensapp.Models.Database;

namespace Schadensapp.Interfaces
{
    public interface IUserService
    {
        public UserModel? GetUser();
        public UserModel? GetUserWithSID(string? SID);
    }
}

