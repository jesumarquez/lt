#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Documentos
{
    [Serializable]
    public class TipoDocumento : IAuditable, ISecurable
    {
        private ISet _parametros;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; } 

        #endregion

        public virtual string Nombre { get; set; }

        public virtual string Descripcion { get; set; }

        public virtual Funcion Funcion { get; set; }

        public virtual ISet Parametros
        {
            get { return _parametros ?? (_parametros = new ListSet()); }
        }

        public virtual string Strategy { get; set; }

        public virtual ISet Estrategias { get; set; }

        public virtual string Template { get; set; }

        public virtual bool AplicarAVehiculo { get; set; }
        public virtual bool AplicarAEmpleado { get; set; }
        public virtual bool AplicarATransportista { get; set; }
        public virtual bool AplicarAEquipo { get; set; }
        public virtual bool ControlaConsumo { get; set; }
        public virtual bool RequerirVencimiento { get; set; }
        public virtual bool RequerirPresentacion { get; set; }
        public virtual short PrimerAviso { get; set; }
        public virtual short SegundoAviso { get; set; }
        public virtual bool Baja { get; set; }


        public override string ToString() { return Descripcion; }
    }
 }
