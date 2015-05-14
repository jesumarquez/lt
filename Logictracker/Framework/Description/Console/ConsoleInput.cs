#region Usings

using System;
using System.Diagnostics;
using System.Threading;

#endregion

namespace Logictracker.Description.Console
{
	public class ConsoleInput : IDisposable
    {
		public ConsoleInput(IConsoleInputListener listener)
		{
			_listener = listener;
            _reader = new Thread(ReadConsoleProc);
            _reader.Start();
        }

		public void Dispose()
		{
            _reader.Abort();
            _syncPoint.WaitOne();
        }

		private readonly IConsoleInputListener _listener;
		private readonly Thread _reader;
		private readonly AutoResetEvent _syncPoint = new AutoResetEvent(false);

        private void ReadConsoleProc()
        {
			try
			{
				while (true)
				{
					try
					{
						switch (_listener.InputMode)
						{
							case ConsoleInputMode.GetLine: _listener.OnLine(System.Console.ReadLine()); break;
							case ConsoleInputMode.GetKey: _listener.OnKey(System.Console.ReadKey(true)); break;
							case ConsoleInputMode.GetKeyWithEcho: _listener.OnKey(System.Console.ReadKey(false)); break;
						}
					}
					catch (ThreadAbortException e)
					{
						Debug.WriteLine(e);
						return;
					}
					catch (Exception e)
					{
						Debug.WriteLine(e);
					}
				}
			}
			finally
			{
				_syncPoint.Set();
			}
        }
    }

	public interface IConsoleInputListener
	{
		ConsoleInputMode InputMode { get; }
		void OnKey(ConsoleKeyInfo inKey);
		void OnLine(String line);
	}

	public enum ConsoleInputMode
	{
		GetLine,
		GetKey,
		GetKeyWithEcho,
	}
}