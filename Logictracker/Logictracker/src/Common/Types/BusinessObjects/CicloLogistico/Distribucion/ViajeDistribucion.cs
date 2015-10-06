using System;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class ViajeDistribucion : IAuditable, ISecurable, IHasVehiculo, IHasEmpleado, IHasCentroDeCosto, IHasSubCentroDeCosto
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Pendiente = 0;
            public const short EnCurso = 1;
            public const short Anulado = 8;
            public const short Cerrado = 9;

            public static string GetLabelVariableName(short estado)
            {
                switch (estado)
                {
                    case Pendiente: return "VIAJE_STATE_PENDIENTE";
                    case EnCurso: return "VIAJE_STATE_ENCURSO";
                    case Anulado: return "VIAJE_STATE_ANULADO";
                    case Cerrado: return "VIAJE_STATE_CERRADO";
                    default: return "VIAJE_STATE_PENDIENTE";
                }
            }
        }

        public static class Tipos
        {
            public const short Ordenado = 0;
            public const short Desordenado = 1;
            public const short RecorridoFijo = 2;
        }

        Type IAuditable.TypeOf() { return GetType(); }

        /// <summary>
        /// Hardcode for Cache
        /// </summary>
        public const string CurrentCacheKey = "ViajeDistribucion.Current";

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }
        public virtual SubCentroDeCostos SubCentroDeCostos { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual TipoCicloLogistico TipoCicloLogistico { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual string Codigo { get; set; }
        public virtual int NumeroViaje { get; set; }
        public virtual DateTime Inicio { get; set; }
        public virtual DateTime Fin { get; set; }
        public virtual short Estado { get; set; }
        public virtual short Tipo { get; set; }
        public virtual int Desvio { get; set; }
        public virtual bool RegresoABase { get; set; }
        public virtual DateTime? InicioReal { get; set; }

        public virtual bool Controlado { get; set; }
        public virtual Usuario UsuarioControl { get; set; }
        public virtual DateTime? FechaControl { get; set; }
        public virtual string Motivo { get; set; }
        public virtual string Comentario { get; set; }
        public virtual int Umbral { get; set; }
        public virtual DateTime? Alta { get; set; }
        public virtual bool ProgramacionDinamica { get; set; }
        public virtual DateTime? Recepcion { get; set; }

        public virtual DateTime? MinTiempoProgramado { get; set; }
        public virtual DateTime? MaxTiempoProgramado { get; set; }

        public virtual int EntregasTotalCount { get; set; }
        public virtual int EntregasTotalCountConBases { get; set; }

        public virtual int EntregasCompletadosCount { get; set; }
        public virtual int EntregasVisitadosCount { get; set; }
        public virtual int EntregasEnSitioCount { get; set; }
        public virtual int EntregasEnZonaCount { get; set; }
        public virtual int EntregasNoCompletadosCount { get; set; }
        public virtual int EntregasNoVisitadosCount { get; set; }
        public virtual int EntregasPendientesCount { get; set; }
        
        public virtual int EntregasNomencladasCount { get; set; }

        private IList<EntregaDistribucion> _detalles;
        public virtual IList<EntregaDistribucion> Detalles
        {
            get { return _detalles ?? (_detalles = new List<EntregaDistribucion>()); }
            set { _detalles = value; }
        }

        private IList<RecorridoDistribucion> _recorrido;
        public virtual IList<RecorridoDistribucion> Recorrido
        {
            get { return _recorrido ?? (_recorrido = new List<RecorridoDistribucion>()); }
            set { _recorrido = value; }
        }

        private IList<EvenDistri> _eventos;
        public virtual IList<EvenDistri> EventosDistri
        {
            get { return _eventos ?? (_eventos = new List<EvenDistri>()); }
            set { _eventos = value; }
        }

        public virtual IEnumerable<SegmentoRecorrido> SegmentosRecorrido
        {
            get
            {
                for(var i = 0; i < Recorrido.Count - 1; i++)
                {
                    yield return new SegmentoRecorrido {Index = i, Inicio = Recorrido[i], Fin = Recorrido[i + 1]};
                }
            }
        }

        public virtual IList<EntregaDistribucion> GetEntregasOrdenadas()
        {
            if (Tipo == Tipos.Desordenado)
            {
                var salida = Detalles.First();
                var llegada = Detalles.Last();
                if (llegada.Linea == null) llegada = null;
                var det = Detalles.Where(e => e.Linea == null).OrderBy(e => e.ManualOEntrada).ToList();
                det.Insert(0, salida);
                if (llegada != null && Detalles.Count > 1) det.Add(llegada);
                return det.ToList();
            }
            return Detalles.ToList();
        }

        public virtual IList<EntregaDistribucion> GetEntregasPorOrdenReal()
        {
            if (Tipo == Tipos.Desordenado)
            {
                var salidas = Detalles.Where(e => e.Linea != null);
                var llegada = Detalles.Last();
                if (llegada.Linea == null) llegada = null;
                var det = Detalles.Where(e => e.Linea == null && e.Estado != EntregaDistribucion.Estados.Cancelado)
                                  .OrderBy(e => e.FechaMin).ToList();
                if (salidas.Any()) det.Insert(0, salidas.First());
                det.AddRange(Detalles.Where(e => e.Linea == null && e.Estado == EntregaDistribucion.Estados.Cancelado)
                   .OrderBy(e => e.FechaMin));
                if (llegada != null && Detalles.Count > 1) det.Add(llegada);
                return det.ToList();
            }
            return Detalles.ToList();
        }

        public virtual IList<EntregaDistribucion> GetEntregasPorOrdenManual()
        {
            var salidas = Detalles.Where(e => e.Linea != null);
            var llegada = Detalles.Last();
            if (llegada.Linea == null) llegada = null;
            
            var det = Detalles.Where(e => e.Linea == null && e.Manual.HasValue)
                              .OrderBy(e => e.Manual.Value).ToList();
                
            if (salidas.Any()) det.Insert(0, salidas.First());
            det.AddRange(Detalles.Where(e => e.Linea == null && !e.Manual.HasValue)
                                 .OrderBy(e => e.Programado));
            
            if (llegada != null && Detalles.Count > 1) det.Add(llegada);
            return det.ToList();
        }

        public virtual IEnumerable<LogMensaje> GetEventos()
        {
            return EventosDistri.Select(e => e.LogMensaje);
        }

        public class SegmentoRecorrido
        {
            public int Index { get; set; }
            public RecorridoDistribucion Inicio { get; set; }
            public RecorridoDistribucion Fin { get; set; }
        }
    }
}
