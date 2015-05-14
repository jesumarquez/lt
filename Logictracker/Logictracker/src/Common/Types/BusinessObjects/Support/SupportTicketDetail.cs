#region Usings

using System;

#endregion

namespace Logictracker.Types.BusinessObjects.Support
{
    public class SupportTicketDetail
    {
        public virtual int Id { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual short Estado { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual string Descripcion{ get; set; }
        public virtual SupportTicket SupportTicket { get; set; }
    }
}
