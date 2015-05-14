#region Usings

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

#endregion

namespace Urbetrack.Common.Utils
{
    /// <summary>
    /// Class for handlig windos security helper methods.
    /// </summary>
    public class WindowsSecurityHelper : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// Properties for impersonating current user.
        /// </summary>
        private const Int32 Logon32LogonInteractive = 2;
        private const Int32 Logon32ProviderDefault = 0;
        private WindowsImpersonationContext _impersonationContext;

        #endregion

        #region Public Methods

        /// <summary>
        /// Impersonates current user with the provided credentials.
        /// </summary>
        /// <returns></returns>
        public Boolean ImpersonateValidUser()
        {
            WindowsIdentity tempWindowsIdentity;

            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(ConfigurationHelper.ImpersonateUser, ConfigurationHelper.ImpersonateDomain, ConfigurationHelper.ImpersonatePassword, Logon32LogonInteractive,
                    Logon32ProviderDefault, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);

                        _impersonationContext = tempWindowsIdentity.Impersonate();

                        CloseHandle(token);
                        CloseHandle(tokenDuplicate);

                        return true;
                    }
                }
            }

            if (token != IntPtr.Zero) CloseHandle(token);

            if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

            return false;
        }

        /// <summary>
        /// Undo aspnet impersonation.
        /// </summary>
        public void UndoImpersonation() { _impersonationContext.Undo(); }

        /// <summary>
        /// Gets user name for the specified user sid.
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static String GetNameFromSid(String sid)
        {
            IntPtr psid;
            int use;
            var cbName = 0;
            var cbDom = 0;

            ConvertStringSidToSid(sid, out psid);

            LookupAccountSid(null, psid, null, ref cbName, null, ref cbDom, out use);

            var name = new StringBuilder(cbName);
            var dom = new StringBuilder(cbDom);

            LookupAccountSid(null, psid, name, ref cbName, dom, ref cbDom, out use);

            Marshal.FreeHGlobal(psid);

            return name.ToString();
        }

        /// <summary>
        /// Dispose all allocated resources.
        /// </summary>
        public void Dispose()
        {
            if (_impersonationContext == null) return;

            _impersonationContext.Dispose();

            _impersonationContext = null;
        }

        #endregion

        #region External Methods

        [DllImport("advapi32.dll")]
        private static extern Int32 LogonUserA(String lpszUserName, String lpszDomain, String lpszPassword, Int32 dwLogonType, Int32 dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Int32 DuplicateToken(IntPtr hToken, Int32 impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern Boolean CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean ConvertStringSidToSid(String stringSid, out IntPtr psid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean LookupAccountSid(String lpSystemName, IntPtr sid, [Out] StringBuilder name, ref Int32 cbName, [Out] StringBuilder referencedDomainName,
            ref Int32 cbReferencedDomainName, out Int32 peUse);

        #endregion
    }
}
