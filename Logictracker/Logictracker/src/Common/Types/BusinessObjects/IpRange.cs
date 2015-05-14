#region Usings

using System;
using System.Linq;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class IpRange : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Usuario Usuario { get; set; }
        public virtual string IpFrom { get; set; }
        public virtual string IpTo { get; set; }

        public virtual bool IsInRange(string ip)
        {
            try
            {
                if (IpFrom == IpTo) return ip == IpFrom;
                var ips = (from p in ip.Split('.') select Convert.ToInt32(p)).ToList();
                if (ips.Count() != 4) return false;

                var from = (from p in IpFrom.Split('.') select Convert.ToInt32(p)).ToList();
                var to = (from p in IpTo.Split('.') select Convert.ToInt32(p)).ToList();

                for (var i = 0; i < 4; i++)
                    if (ips[i] < from[i] || ips[i] > to[i]) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
