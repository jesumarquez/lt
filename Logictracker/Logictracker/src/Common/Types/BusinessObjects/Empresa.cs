using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Empresa : IComparable, IDataIdentify, IAuditable, IEquatable<Empresa>
    {
        Type IAuditable.TypeOf() { return GetType(); }

        public static class Params
        {
            public const string PeriodosInicio = "periodos.inicio";
            public const string CicloDistribucionOrdenar = "ciclo.distribucion.ordenar";
            public const string CicloDistribucionCerrar = "ciclo.distribucion.cerrar";
            public const string VelocidadPromedio = "velocidad.promedio";
            public const string PumpHabilitado = "pumpcontrol.habilitado";
            public const string PumpUsuario = "pumpcontrol.usuario";
            public const string PumpPassword = "pumpcontrol.password";
            public const string PumpCompany = "pumpcontrol.company";
            public const string LoJackApiHabilitado = "lojackapi.habilitado";
            public const string LoJackApiUsuario = "lojackapi.usuario";
            public const string LoJackApiPassword = "lojackapi.password";
            public const string LoJackApiGuid = "lojackapi.guid";
            public const string TurnoSoporte = "turno.soporte";
            public const string MinutosEnAmarillo = "minutos.amarillo";
            public const string HorasEnRojo = "horas.rojo";
            public const string LogicoutMolinete = "logicout.molinete";
            public const string ControlaInicioDistribucion = "distribucion.controla.inicio";
            public const string CierreDistribucionAutomatico = "distribucion.cierre.automatico";
            public const string MarginMinutes = "distribucion.margin.minutes";
            public const string EndMarginMinutes = "distribucion.end.margin.minutes";
            public const string MinutosMinimosDeViaje = "distribucion.minutos.minimos.viaje";
            public const string SegundosMinimosEnDetencion = "distribucion.segundos.minimos.detencion";
            public const string TipoCalculoCostoKm = "tipo.calculo.costo.km";
            public const string InicioDistribucionPorSalidaDeBase = "distribucion.inicio.salida.base";
            public const string InicioDistribucionPorMensaje = "distribucion.inicio.mensaje";
            public const string InicioDistribucionCodigoMensaje = "distribucion.inicio.mensaje.codigo";
            public const string InicioDistribucionSiguienteAlCerrar = "distribucion.inicio.siguiente";
            public const string DistribucionReportaEmpleado = "distribucion.reporta.empleado";

            public const string MesesConsultaPosiciones = "meses.consulta.posiciones";
            public const string EliminaRutas = "elimina.rutas";
            public const string EliminaPuntosDeEntrega = "elimina.puntos";
            public const string EliminaAntiguedadMeses = "elimina.antiguedad.meses";
            public const string EstadoCierreDistribucion = "distribucion.estado.cierre";
            public const string CambiaEstado = "mantenimiento.cambia.estado";
            public const string CambiaEstadoDias = "mantenimiento.cambia.estado.dias";
            public const string CambiaEstadoGarmin = "mantenimiento.cambia.estado.garmin";
            public const string CambiaEstadoGarminDias = "mantenimiento.cambia.estado.garmin.dias";
            public const string ProductKey = "product.key";
            public const string OrdenRutaGarmin = "orden.ruta.garmin";
            public const string MonitoreoGarmin = "monitoreo.garmin";
            public const string Totalizadores = "totalizadores.lista";
            public const string IconoPorCentroDeCosto = "icono.por.cc";
            public const string Icono = "icono.";
            public const string GoogleMapsEnabled = "google.maps.enabled";
            public const string MessageCountEnabled = "msg.count.enabled";
            public const string MessageCountValue = "msg.count.value";

            public const string AsignoDistribucionPorMensaje = "distribucion.asigno.mensaje";
            public const string AsignoDistribucionPrefijoMensaje = "distribucion.asigno.mensaje.prefijo";
            public const string CierreDistribucionPorMensaje = "distribucion.cierre.mensaje";
            public const string CierreDistribucionCodigoMensaje = "distribucion.cierre.mensaje.codigo";
            public const string CierreDistribucionCompleta = "distribucion.cierre.completa";

            public const string DistribucionEvaluaSoloGeocercasViaje = "distribucion.geocercas.solo.viaje";
            public const string TiposGeocercaViaje = "distribucion.tipos.geocerca.viaje";

            public const string KpiCantidadPagina = "kpi.cantidad.pagina";
        }
        public static class OrdenRuta
        {
            public const short Ruteador = 0;
            public const short DescripcionAsc = 1;
            public const short DescripcionDesc = 2;
        }

        #region Public Properties

        public virtual int Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string RazonSocial { get; set; }
        public virtual string Fantasia { get; set; }
        public virtual bool Baja { get; set; }
        public virtual string TimeZoneId { get; set; }
        public virtual bool IdentificaChoferes { get; set; }
        public virtual bool CochesPorDistrito { get; set; }
        public virtual int FrecuenciaReporte { get; set;}

        private IList<ParametroEmpresa> _parametros;
        public virtual IList<ParametroEmpresa> Parametros
        {
            get { return _parametros ?? (_parametros = new List<ParametroEmpresa>()); }
            set { _parametros = value; }
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj)
        {
            var castObj = obj as Empresa;
            return Equals(castObj);
        }

        public override int GetHashCode() { return 27*57*Id.GetHashCode(); }

        public virtual bool Equals(Empresa other)
        {
            return other != null && Id == other.Id && Id != 0;
        }

        public override string ToString() { return RazonSocial; }

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;
            var emp = obj as Empresa;
            return emp == null ? 1 : Codigo.CompareTo(emp.Codigo);
        }

        #endregion

        #region Parametros

        public virtual string GetParameter(string nombre)
        {
            var param = Parametros.FirstOrDefault(p => p.Nombre.ToLower() == nombre);
            return param != null ? param.Valor : null;
        }
        
        public virtual int PeriodosInicio
        {
            get
            {
                var valor = GetParameter(Params.PeriodosInicio);
                int dia;
                if (valor == null || !int.TryParse(valor, out dia)) return 1;
                if(dia >= 1 && dia <= 31) return dia;
                return 1;
            }
        } 
        public virtual int VelocidadPromedio
        {
            get
            {
                var valor = GetParameter(Params.VelocidadPromedio);
                int velo;
                if (valor == null || !int.TryParse(valor, out velo)) return 20;
                return velo;
            }
        }
        public virtual bool PumpHabilitado
        {
            get
            {
                var valor = GetParameter(Params.PumpHabilitado);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual string PumpUsuario { get { return GetParameter(Params.PumpUsuario); } }
        public virtual string PumpPassword { get { return GetParameter(Params.PumpPassword); } }
        public virtual string PumpCompany { get { return GetParameter(Params.PumpCompany); } }
        public virtual bool LoJackApiHabilitado
        {
            get
            {
                var valor = GetParameter(Params.LoJackApiHabilitado);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual string LoJackApiUsuario { get { return GetParameter(Params.LoJackApiUsuario); } }
        public virtual string LoJackApiPassword { get { return GetParameter(Params.LoJackApiPassword); } }
        public virtual string LoJackApiGuid { get { return GetParameter(Params.LoJackApiGuid); } }
        public virtual double MinutosEnAmarillo
        {
            get
            {
                var valor = GetParameter(Params.MinutosEnAmarillo);
                double min;
                if (valor == null || !double.TryParse(valor, out min)) return 5.0;
                return min;
            }
        }
        public virtual double HorasEnRojo
        {
            get
            {
                var valor = GetParameter(Params.HorasEnRojo);
                double horas;
                if (valor == null || !double.TryParse(valor, out horas)) return 48.0;
                return horas;
            }
        }
        public virtual bool LogicoutMolinete
        {
            get
            {
                var valor = GetParameter(Params.LogicoutMolinete);
                bool v;
                if (valor == null || !bool.TryParse(valor, out v)) return false;
                return v;
            }
        }
        public virtual bool ControlaInicioDistribucion
        {
            get
            {
                var valor = GetParameter(Params.ControlaInicioDistribucion);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual bool CierreDistribucionAutomatico
        {
            get
            {
                var valor = GetParameter(Params.CierreDistribucionAutomatico);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        /// <summary>
        /// Define si los ciclos de distribución se cierran automáticamente cuando pasan los minutos especificados en EndMarginMinutes del cierre programado.
        /// Default: true.
        /// </summary>
        public virtual bool CicloDistribucionCerrar
        {
            get
            {
                var valor = GetParameter(Params.CicloDistribucionCerrar);
                bool v;
                return valor == null || !bool.TryParse(valor, out v) || v;
            }
        }

        public virtual int MarginMinutes
        {
            get
            {
                var valor = GetParameter(Params.MarginMinutes);
                int v;
                if (valor == null || !int.TryParse(valor, out v)) return 60;
                return v;
            }
        }
        public virtual int EndMarginMinutes
        {
            get
            {
                var valor = GetParameter(Params.EndMarginMinutes);
                int v;
                if (valor == null || !int.TryParse(valor, out v)) return 720;
                return v;
            }
        }
        public virtual int MinutosMinimosDeViaje
        {
            get
            {
                var valor = GetParameter(Params.MinutosMinimosDeViaje);
                int v;
                if (valor == null || !int.TryParse(valor, out v)) return 60;
                return v;
            }
        }
        public virtual int SegundosMinimosEnDetencion
        {
            get
            {
                var valor = GetParameter(Params.SegundosMinimosEnDetencion);
                int v;
                if (valor == null || !int.TryParse(valor, out v)) return 120;
                return v;
            }
        }

        public virtual string TipoCalculoCostoKm
        {
            get
            {
                var valor = GetParameter(Params.TipoCalculoCostoKm);
                if (valor == null || valor.Trim() == string.Empty) return "teorico";
                return valor.ToLowerInvariant();
            }
        }

        public virtual bool InicioDistribucionSiguienteAlCerrar
        {
            get
            {
                var valor = GetParameter(Params.InicioDistribucionSiguienteAlCerrar);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual bool InicioDistribucionPorSalidaDeBase
        {
            get
            {
                var valor = GetParameter(Params.InicioDistribucionPorSalidaDeBase);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual bool InicioDistribucionPorMensaje
        {
            get
            {
                var valor = GetParameter(Params.InicioDistribucionPorMensaje);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual string InicioDistribucionCodigoMensaje
        {
            get
            {
                var valor = GetParameter(Params.InicioDistribucionCodigoMensaje);
                if (valor == null || valor.Trim() == string.Empty) return "21";
                return valor.ToLowerInvariant();
            }
        }
        public virtual bool CierreDistribucionPorMensaje
        {
            get
            {
                var valor = GetParameter(Params.CierreDistribucionPorMensaje);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual string CierreDistribucionCodigoMensaje
        {
            get
            {
                var valor = GetParameter(Params.CierreDistribucionCodigoMensaje);
                if (valor == null || valor.Trim() == string.Empty) return "22";
                return valor.ToLowerInvariant();
            }
        }
        public virtual bool CierreDistribucionCompleta
        {
            get
            {
                var valor = GetParameter(Params.CierreDistribucionCompleta);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return true;
                return aplica;
            }
        }
        public virtual bool AsignoDistribucionPorMensaje
        {
            get
            {
                var valor = GetParameter(Params.AsignoDistribucionPorMensaje);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual string AsignoDistribucionPrefijoMensaje
        {
            get
            {
                var valor = GetParameter(Params.AsignoDistribucionPrefijoMensaje);
                return valor == null ? string.Empty : valor.Trim().ToLowerInvariant();
            }
        }

        public virtual int MesesConsultaPosiciones
        {
            get
            {
                var valor = GetParameter(Params.MesesConsultaPosiciones);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 3;
                return cant;
            }
        }
        public virtual short EstadoCierreDistribucion
        {
            get
            {
                var valor = GetParameter(Params.EstadoCierreDistribucion);
                short estado;
                if (valor == null || !short.TryParse(valor, out estado)) return EntregaDistribucion.Estados.SinVisitar;
                return estado;
            }
        }

        public virtual bool EliminaRutas
        {
            get
            {
                var valor = GetParameter(Params.EliminaRutas);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual bool EliminaPuntosDeEntrega
        {
            get
            {
                var valor = GetParameter(Params.EliminaPuntosDeEntrega);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }
        public virtual int EliminaAntiguedadMeses
        {
            get
            {
                var valor = GetParameter(Params.EliminaAntiguedadMeses);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 3;
                return cant;
            }
        }

        public virtual bool CambiaEstado
        {
            get
            {
                var valor = GetParameter(Params.CambiaEstado);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return true;
                return aplica;
            }
        }
        public virtual int CambiaEstadoDias
        {
            get
            {
                var valor = GetParameter(Params.CambiaEstadoDias);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 7;
                return cant;
            }
        }

        public virtual bool CambiaEstadoGarmin
        {
            get
            {
                var valor = GetParameter(Params.CambiaEstadoGarmin);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return true;
                return aplica;
            }
        }
        public virtual int CambiaEstadoGarminDias
        {
            get
            {
                var valor = GetParameter(Params.CambiaEstadoGarminDias);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 7;
                return cant;
            }
        }

        public virtual string ProductKey
        {
            get
            {
                var valor = GetParameter(Params.ProductKey);
                if (valor == null || valor.Trim() == string.Empty) return string.Empty;
                return valor.ToLowerInvariant();
            }
        }

        public virtual short OrdenRutaGarmin
        {
            get
            {
                var valor = GetParameter(Params.OrdenRutaGarmin);
                switch (valor)
                {
                    case "0": return OrdenRuta.Ruteador;
                    case "1": return OrdenRuta.DescripcionAsc;
                    case "2": return OrdenRuta.DescripcionDesc;
                    default: return OrdenRuta.Ruteador;
                }
            }
        }

        public virtual bool MonitoreoGarmin
        {
            get
            {
                var valor = GetParameter(Params.MonitoreoGarmin);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }

        public virtual List<short> Totalizadores
        {
            get
            {
                var lista = new List<short>();
                var valor = GetParameter(Params.Totalizadores);
                if (!string.IsNullOrEmpty(valor))
                {
                    try
                    {
                        lista = valor.Split('.').Select(t => Convert.ToInt16(t)).ToList();
                    }
                    catch (Exception) { }
                }
                
                return lista;
            }
        }

        public virtual bool IconoPorCentroDeCosto
        {
            get
            {
                var valor = GetParameter(Params.IconoPorCentroDeCosto);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }

        public virtual string GetUrlIcono(string codigo)
        {
            var valor = GetParameter(Params.Icono + codigo.ToLower());
            if (valor == null || valor.Trim() == string.Empty) return string.Empty;
            return valor;
        }

        public virtual bool GoogleMapsEnabled
        {
            get
            {
                var valor = GetParameter(Params.GoogleMapsEnabled);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return true;
                return aplica;
            }
        }

        public virtual bool MessageCountEnabled
        {
            get
            {
                var valor = GetParameter(Params.MessageCountEnabled);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }

        public virtual int MessageCountValue
        {
            get
            {
                var valor = GetParameter(Params.MessageCountValue);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 1000;
                return cant;
            }
        }

        public virtual bool EvaluaSoloGeocercasViaje
        {
            get
            {
                var valor = GetParameter(Params.DistribucionEvaluaSoloGeocercasViaje);
                bool aplica;
                if (valor == null || !bool.TryParse(valor, out aplica)) return false;
                return aplica;
            }
        }

        public virtual List<int> TiposGeocercaViaje
        {
            get
            {
                var lista = new List<int>();
                var valor = GetParameter(Params.TiposGeocercaViaje);
                if (!string.IsNullOrEmpty(valor))
                {
                    try
                    {
                        lista = valor.Split(',').Select(t => Convert.ToInt32(t)).ToList();
                    }
                    catch (Exception) { }
                }

                return lista;
            }
        }

        public virtual int KpiCantidadPagina
        {
            get
            {
                var valor = GetParameter(Params.KpiCantidadPagina);
                int cant;
                if (valor == null || !int.TryParse(valor, out cant)) return 10;
                return cant;
            }
        }

        #endregion
    }
}