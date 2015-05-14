/*#region Usings

using System;

#endregion

namespace Logictracker.Description
{
    [FrameworkElement(XName = "ShowConsoleInput", IsContainer = false)]
    public class ShowConsoleInput : FrameworkElement
    {
        [RoutedMessageHandler("ConsoleGetLine")]
        public void ConsoleGetLine(string line)
        {
            System.Console.WriteLine("GetLine: {0}", line);
        }

        [RoutedMessageHandler("ConsoleGetKey")]
        public void ConsoleGetKey(ConsoleKeyInfo key)
        {
            System.Console.WriteLine("KeyInfo:");
            System.Console.WriteLine("  ConsoleKey = {0}", key.Key);
            System.Console.WriteLine("  Modifiers  = {0}", key.Modifiers);
        }

        [RoutedMessageHandler("ConsoleGetKeyWithEcho")]
        public void ConsoleGetKeyWithEcho(ConsoleKeyInfo key)
        {
            System.Console.WriteLine("ConsoleGetKeyWithEcho: {0}", key);
        }
    }
}//*/