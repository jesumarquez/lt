#region Usings

using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("PumpControl")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Logictracker")]
[assembly: AssemblyProduct("PumpControl")]
[assembly: AssemblyCopyright("Copyright © Logictracker 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5045d782-2093-4093-91f9-ae9661f804de")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace PumpControl.Properties
{
    public class ConfigPumpControl
    {
        public const string PumpControlNamespaceUri = "http://walks.ath.cx/e/pumpcontrol";
        public static readonly XNamespace PumpControlNamespace = XNamespace.Get(PumpControlNamespaceUri);
    }
}