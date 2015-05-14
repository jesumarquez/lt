#region Usings

using System.Diagnostics;

#endregion

namespace Logictracker.Diagnostics
{
    public class Debugger
    {
        #region Lanzador Automatico del Debugger

		public static readonly MultiChoiceSwitch LaunchDebuggerOnMainSwitch = new MultiChoiceSwitch("launch_debugger", "Disparar el Debugger al pasar por ese punto?.");

        public static void LaunchDebugger(int code)
        {
            if (LaunchDebuggerOnMainSwitch.IsSelection(code))
            {
                System.Diagnostics.Debugger.Launch();
            }
        }

        #endregion
    }

	public class MultiChoiceSwitch : Switch
	{
		public MultiChoiceSwitch(string displayName, string description)
			: base(displayName, description)
		{
		}

		public MultiChoiceSwitch(string displayName, string description, string defaultSwitchValue)
			: base(displayName, description, defaultSwitchValue)
		{
		}

		public bool IsSelection(int code)
		{
			return SwitchSetting == code;
		}
	}
}
