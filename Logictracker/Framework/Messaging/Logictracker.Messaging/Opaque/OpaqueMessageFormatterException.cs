#region Usings

using System;

#endregion

namespace Logictracker.MsmqMessaging.Opaque
{
    public class OpaqueMessageFormatterException : Exception
    {
        public OpaqueMessageFormatterException(string s) : base(s)
        {
        }
    }
}