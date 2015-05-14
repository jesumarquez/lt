#region Usings

using System.Reflection;
using System.Runtime.InteropServices;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("XLinq")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("rnros")]
[assembly: AssemblyProduct("XLinq")]
[assembly: AssemblyCopyright("Copyright © rnros 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("8a74e8b3-4a42-4d11-84a8-262600d4b8f1")]

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

namespace Rnr.XSynq.Properties
{
    
    internal class BuildConfig
    {
        #if STAGE_DEVEL
        internal const bool IncludeExceptionDetailInFaults = true;
        internal const int EvalRulesTraceLevel = 5;
        #else
        internal const bool IncludeExceptionDetailInFaults = false;
        internal const int EvalRulesTraceLevel = 10;
        #endif
    }

    internal class StaticConfiguration
    {
        internal const int SessionPeridicUpdate = 1000 * 2;
        internal const int DocumentPeridicUpdate = 1000 * 3;
    }
}