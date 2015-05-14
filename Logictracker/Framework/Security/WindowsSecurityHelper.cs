#region Usings

using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Security
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
			try
			{
				try
				{
					if (RevertToSelf())
					{
						if (LogonUserA(Config.Services.ImpersonateUser, Config.Services.ImpersonateDomain, Config.Services.ImpersonatePassword, Logon32LogonInteractive, Logon32ProviderDefault, ref token) != 0)
						{
							if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
							{
								tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
								_impersonationContext = tempWindowsIdentity.Impersonate();
								return true;
							}
							else
							{
								STrace.Trace(typeof(WindowsSecurityHelper).FullName, String.Format("Win32 Error: DuplicateToken: {0}", GetLastError()));
							}
						}
						else
						{
							STrace.Trace(typeof(WindowsSecurityHelper).FullName, String.Format("Win32 Error: LogonUserA ImpersonateUser={0}, ImpersonateDomain={1}, ImpersonatePassword={2}, Logon32LogonInteractive={3}, Logon32ProviderDefault={4}, Error={5}", Config.Services.ImpersonateUser, Config.Services.ImpersonateDomain, Config.Services.ImpersonatePassword, Logon32LogonInteractive, Logon32ProviderDefault, GetLastError()));
						}
					}
					else
					{
						STrace.Trace(typeof(WindowsSecurityHelper).FullName, String.Format("Win32 Error: RevertToSelf: {0}", GetLastError()));
					}
				}
				finally
				{
					if (token != IntPtr.Zero) CloseHandle(token);
				}
			}
			finally
			{
				if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);
			}

            return false;
        }

        /// <summary>
        /// Undo aspnet impersonation.
        /// </summary>
        public void UndoImpersonation() { _impersonationContext.Undo(); }


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

		[DllImport("kernel32.dll")]
		private static extern int GetLastError();

        #endregion
    }
}
