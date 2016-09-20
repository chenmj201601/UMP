using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    public interface IADUtility
    {
        ADUser GetADUser(string userName);
        ADUser GetADUserByMobile(string mobile);
        DirectoryEntry GetOnlyOneDEObject(string ldapQueryString);
        ADGroupCollection GetAllGroups();
        System.Collections.Generic.List<DirectoryEntry> GetAllDEObject(string ldapQueryString);
        ADUserCollection GetAllUsers();
        ADUserCollection GetAllUsers(string ldapQueryString);
        DirectoryEntry GetDEByLDAPPath(string ldapPath);
        string ADServerLDAP { get; }
    }
}
