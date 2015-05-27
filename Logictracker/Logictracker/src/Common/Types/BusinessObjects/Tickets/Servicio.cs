#region Usings

using System;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

#endregion

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class Servicio : IAuditable
    {
        public const short EstadoProgramado = 0;
        public const short EstadoEnCurso = 1;
        public const short EstadoFinalizado = 2;
        public const short EstadoCancelado = 9;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Owner Owner { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Custom1 { get; set; }
        public virtual string Custom2 { get; set; }
        public virtual string Custom3 { get; set; }
        public virtual short Estado { get; set; }
        public virtual DateTime FechaInicio { get; set; }
        public virtual DateTime FechaFin { get; set; }
        public virtual DateTime FechaAlta { get; set; }
        public virtual CicloLogistico CicloLogistico { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Chofer { get; set; }


        private ISet<DetalleServicio> _detalles;

        public virtual ISet<DetalleServicio> Detalles { get { return _detalles ?? (_detalles = new HashSet<DetalleServicio>()); } }

        public Servicio()
        {
            Owner = new Owner();
        }
    }
}
