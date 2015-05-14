#region Usings

using System;

#endregion

namespace Urbetrack.Comm.Core.Transport.XBeeRLP
{
    public class XBeeAddress
    {
        public ushort Addr { get; set; }

        public override int GetHashCode()
        {
            return Addr;
        }

        public override string ToString()
        {
            return String.Format("{0}", Addr);
        }
    }
}