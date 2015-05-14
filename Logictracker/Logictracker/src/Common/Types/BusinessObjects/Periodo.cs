#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Periodo : IAuditable, IHasEmpresa
    {
        public const short Abierto = 0;
        public const short Cerrado = 1;
        public const short Liquidado = 2;

        private ISet _detalles;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Descripcion { get; set; }
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
        public virtual short Estado { get; set; }
        public virtual Empresa Empresa { get; set; }

        public virtual ISet Detalles
        {
            get { return _detalles ?? (_detalles = new ListSet()); }
        }

        public virtual DateTime FechaHoraHasta
        {
            get
            {
                return FechaHasta.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
        }
    }
}
