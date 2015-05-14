#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.ControlDeCombustible
{
    [Serializable]
    public class Caudalimetro : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Codigo { get; set; }
        public virtual Tanque Tanque { get; set; }
        public virtual Equipo Equipo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual bool EsDeEntrada { get; set; }
        public virtual Double CaudalMaximo { get; set; }
        public virtual Int32 TiempoSinReportar { get; set; }
    }
}
