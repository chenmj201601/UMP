//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    092b761a-7f72-438c-a46e-d6fc705dc56d
//        CLR Version:              4.0.30319.18063
//        Name:                     Impersonate
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSftp
//        File Name:                Impersonate
//
//        created by Charley at 2015/9/10 17:50:56
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace UMPSftp
{
    /// <summary>
    /// Impersonate helper.
    /// </summary>
    public class Impersonate
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private extern static bool LogonUser(string userName, string domainName, string password, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        public static bool LogonAndImpersonateUser(string userName, string password)
        {
            try
            {
                IntPtr tokenHandle = new IntPtr(0);
                IntPtr dupeTokenHandle = new IntPtr(0);

                const int LOGON32_PROVIDER_DEFAULT = 0;
                //This parameter causes LogonUser to create a primary token.
                const int LOGON32_LOGON_INTERACTIVE = 2;
                const int SecurityImpersonation = 2;

                tokenHandle = IntPtr.Zero;
                dupeTokenHandle = IntPtr.Zero;

                // Call LogonUser to obtain an handle to an access token.
                if (!LogonUser(userName, "", password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref tokenHandle))
                {
                    return false;
                }

                bool retVal = DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle);
                if (!DuplicateToken(tokenHandle, SecurityImpersonation, ref dupeTokenHandle))
                {
                    CloseHandle(tokenHandle);
                    return false;
                }


                // The token that is passed to the following constructor must 
                // be a primary token to impersonate.
                WindowsIdentity newId = new WindowsIdentity(tokenHandle);
                WindowsImpersonationContext impersonatedUser = newId.Impersonate();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
