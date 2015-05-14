#region Usings

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;

#endregion

namespace Logictracker.Runtime
{
	[RunInstaller(true)]
	public partial class Instalador : Installer
	{
		public Instalador()
		{
			InitializeComponent();
		}

		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);

			//obtener datos de instalacion
			var targetDir = Context.Parameters["TargetDir"];

			var xmlDirName = String.Format(@"{0}Applications", targetDir);
			if (!Directory.Exists(xmlDirName))
				Directory.CreateDirectory(xmlDirName);
			AddFullControlPermissionToDir(xmlDirName, "NetworkService");

			var logDirName = String.Format(@"{0}logs", targetDir);
			if (!Directory.Exists(logDirName))
				Directory.CreateDirectory(logDirName);
			AddFullControlPermissionToDir(logDirName, "NetworkService");

			var otaDirName = String.Format(@"{0}FOTA", targetDir);
			if (!Directory.Exists(otaDirName))
				Directory.CreateDirectory(otaDirName);
			AddFullControlPermissionToDir(otaDirName, "NetworkService");
		}

		private static void AddFullControlPermissionToDir(String dir, String user)
		{
			var directorySecurity = Directory.GetAccessControl(dir);
			directorySecurity.AddAccessRule(
				new FileSystemAccessRule(
					user,
					FileSystemRights.FullControl,
					InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
					PropagationFlags.None,
					AccessControlType.Allow));
			Directory.SetAccessControl(dir, directorySecurity);
		}
	}
}
