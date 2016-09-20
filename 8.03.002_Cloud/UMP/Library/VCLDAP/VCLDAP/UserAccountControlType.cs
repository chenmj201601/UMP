using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    /**/
    /// <summary>
    /// Specifies flags that control password, lockout, disable/enable, script, and home directory behavior for the user. This property also contains a flag that indicates the account type of the object. The flags are defined in LMACCESS.H
    /// </summary>
    public enum UserAccountControlType : int
    {
        // 4 bytes. 
        UF_SCRIPT = 0x000001,
        UF_ACCOUNTDISABLE = 0x000002,
        UF_HOMEDIR_REQUIRED = 0x000008,
        UF_LOCKOUT = 0x000010,
        UF_PASSWD_NOTREQD = 0x000020,
        UF_PASSWD_CANT_CHANGE = 0x000040,
        UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x000080,
        /**/
        /// <summary>
        /// This is an account for users whose primary account is in another domain. This account provides user access to this domain, but not to any domain that trusts this domain. The User Manager refers to this account type as a local user account. 
        /// </summary>
        UF_TEMP_DUPLICATE_ACCOUNT = 0x000100,
        /**/
        /// <summary>
        /// This is a default account type that represents a typical user
        /// </summary>
        UF_NORMAL_ACCOUNT = 0x000200,
        /**/
        /// <summary>
        /// This is a permit to trust account for a Windows NT domain that trusts other domains. 
        /// </summary>
        UF_INTERDOMAIN_TRUST_ACCOUNT = 0x000800,
        /**/
        /// <summary>
        /// This is a computer account for a Windows NT Workstation/Windows 2000 Professional or Windows NT Server/Windows 2000 Server that is a member of this domain.
        /// </summary>
        UF_WORKSTATION_TRUST_ACCOUNT = 0x001000,
        /**/
        /// <summary>
        /// This is a computer account for a Windows NT Backup Domain Controller that is a member of this domain. 
        /// </summary>
        UF_SERVER_TRUST_ACCOUNT = 0x002000,
        UF_MACHINE_ACCOUNT_MASK = UF_INTERDOMAIN_TRUST_ACCOUNT | UF_WORKSTATION_TRUST_ACCOUNT | UF_SERVER_TRUST_ACCOUNT,
        UF_ACCOUNT_TYPE_MASK = UF_TEMP_DUPLICATE_ACCOUNT | UF_NORMAL_ACCOUNT | UF_INTERDOMAIN_TRUST_ACCOUNT | UF_WORKSTATION_TRUST_ACCOUNT | UF_SERVER_TRUST_ACCOUNT,
        UF_DONT_EXPIRE_PASSWD = 0x010000,
        UF_MNS_LOGON_ACCOUNT = 0x020000,
        UF_SMARTCARD_REQUIRED = 0x040000,
        UF_TRUSTED_FOR_DELEGATION = 0x080000,
        UF_NOT_DELEGATED = 0x100000,
        UF_USE_DES_KEY_ONLY = 0x200000,
        UF_DONT_REQUIRE_PREAUTH = 0x400000,
        UF_PASSWORD_EXPIRED = 0x800000,
        UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000,
        UF_SETTABLE_BITS = UF_SCRIPT | UF_ACCOUNTDISABLE | UF_LOCKOUT | UF_HOMEDIR_REQUIRED | UF_PASSWD_NOTREQD | UF_PASSWD_CANT_CHANGE | UF_ACCOUNT_TYPE_MASK | UF_DONT_EXPIRE_PASSWD | UF_MNS_LOGON_ACCOUNT | UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED | UF_SMARTCARD_REQUIRED | UF_TRUSTED_FOR_DELEGATION | UF_NOT_DELEGATED | UF_USE_DES_KEY_ONLY | UF_DONT_REQUIRE_PREAUTH | UF_PASSWORD_EXPIRED | UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION
    }
}
