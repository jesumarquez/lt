#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Types.ValueObjects.Documentos.Partes
{
    [Serializable]
    public class PartePersonal
    {
        #region Constants

        public const short Controlado = 1;
        public const short SinControlar = 0;
        public const short Verificado = 2;

        #endregion

        #region Private Properties

        private double _KmTotalCalculado { get; set; }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public short Estado { get; set; }

        public string Descripcion { get; set; }

        public string Codigo { get; set; }

        public DateTime Fecha { get; set; }

        public int IdEmpresa { get; set; }

        public string Empresa { get; set; }

        public string Equipo { get; set; }

        public int IdEquipo { get; set; }

        public int IdCentroCostos { get; set; }

        public string CentroCostos { get; set; }

        public int Vehiculo { get; set; }

        public string Salida { get; set; }

        public string Llegada { get; set; }

        public string Interno { get; set; }

        public int KmTotal { get; set; }

        public string Observaciones { get; set; }

        public string TipoServicio { get; set; }

        public string Grupo
        {
            get
            {
                return TipoServicio == ParteCampos.ListaTipoServicios[0] ? Equipo : TipoServicio;
            }
        }

        public double Importe { get; set; }
        public double ImporteControlado { get; set; }
        public double DiffImporte { get { return ImporteControlado - Importe; } }

        public List<TurnoPartePersonal> Turnos { get; set; }

        public double KmTotalCalculado { get { return Math.Round(_KmTotalCalculado, 2); } set { _KmTotalCalculado = value; } }

        public int KmTotalGps { get; set; }

        public TimeSpan Horas { get; set; }
        public TimeSpan HorasControladas { get; set; }
        public TimeSpan DiffHoras { get { return HorasControladas == TimeSpan.Zero ? TimeSpan.Zero : HorasControladas.Subtract(Horas); } }
        public int Minutos { get { return (int)Math.Round(Horas.TotalMinutes); } }
        public int MinutosControlados { get { return (int)Math.Round(HorasControladas.TotalMinutes); } }
        public int DiffMinutos { get { return (int)Math.Round(DiffHoras.TotalMinutes); } }
        public double DiffKmTotal { get { return KmTotalCalculado - Convert.ToDouble(KmTotal); } }

        #endregion

        #region Constructors

        public PartePersonal() { }

        public PartePersonal(Documento doc, DAOFactory daof)
        {
            Codigo = doc.Codigo;
            Descripcion = doc.Descripcion;
            Fecha = doc.Fecha;
            Id = doc.Id;
            Estado = Convert.ToInt16(doc.Valores.ContainsKey(ParteCampos.EstadoControl)
                                         ? doc.Valores[ParteCampos.EstadoControl]
                                         : SinControlar);
            Salida = doc.Valores[ParteCampos.LugarPartida].ToString();
            Observaciones = doc.Valores.ContainsKey(ParteCampos.Observaciones) ? doc.Valores[ParteCampos.Observaciones].ToString():"";
            Llegada = doc.Valores[ParteCampos.LugarLlegada].ToString();
            KmTotal = Convert.ToInt32(doc.Valores[ParteCampos.KilometrajeTotal]);
            _KmTotalCalculado = 0;
            KmTotalGps = 0;
            IdEquipo = Convert.ToInt32(doc.Valores[ParteCampos.Equipo]);
            Equipo = daof.EquipoDAO.FindById(IdEquipo).Descripcion;
            TipoServicio = doc.Valores.ContainsKey(ParteCampos.TipoServicio) 
                ? ParteCampos.ListaTipoServicios[Convert.ToInt32(doc.Valores[ParteCampos.TipoServicio])]
                : ParteCampos.ListaTipoServicios[0];
            IdCentroCostos = Convert.ToInt32(doc.Valores.ContainsKey(ParteCampos.CentroCostos)
                                                 ? doc.Valores[ParteCampos.CentroCostos]
                                                 : -1);
            CentroCostos = IdCentroCostos > 0 ? daof.CentroDeCostosDAO.FindById(IdCentroCostos).Descripcion : "";
            IdEmpresa = Convert.ToInt32(doc.Valores[ParteCampos.Empresa]);
            Empresa = daof.TransportistaDAO.FindById(IdEmpresa).Descripcion;
            Vehiculo = Convert.ToInt32(doc.Valores[ParteCampos.Vehiculo]);
            Interno = daof.CocheDAO.FindById(Vehiculo).Interno;
            Importe = 0;
            Turnos = new List<TurnoPartePersonal>();


            var alPozoSalida = doc.Valores[ParteCampos.SalidaAlPozo] as List<object>;
            var alPozoLlegada = doc.Valores[ParteCampos.LlegadaAlPozo] as List<object>;
            var delPozoSalida = doc.Valores[ParteCampos.SalidaDelPozo] as List<object>;
            var delPozoLlegada = doc.Valores[ParteCampos.LlegadaDelPozo] as List<object>;
            var km = doc.Valores[ParteCampos.Kilometraje] as List<object>;
            var resp = doc.Valores[ParteCampos.Responsable] as List<object>;

            var alPozoSalidaC = doc.Valores.ContainsKey(ParteCampos.SalidaAlPozoControl)
                                    ? doc.Valores[ParteCampos.SalidaAlPozoControl] as List<object>
                                    : new List<object>();

            var alPozoLlegadaC = doc.Valores.ContainsKey(ParteCampos.LlegadaAlPozoControl)
                                     ? doc.Valores[ParteCampos.LlegadaAlPozoControl] as List<object>
                                     : new List<object>();

            var delPozoSalidaC = doc.Valores.ContainsKey(ParteCampos.SalidaDelPozoControl)
                                     ? doc.Valores[ParteCampos.SalidaDelPozoControl] as List<object>
                                     : new List<object>();

            var delPozoLlegadaC = doc.Valores.ContainsKey(ParteCampos.LlegadaDelPozoControl)
                                      ? doc.Valores[ParteCampos.LlegadaDelPozoControl] as List<object>
                                      : new List<object>();

            var kmC = doc.Valores.ContainsKey(ParteCampos.KilometrajeControl)
                          ? doc.Valores[ParteCampos.KilometrajeControl] as List<object>
                          : new List<object>();

            var kmGps = doc.Valores.ContainsKey(ParteCampos.KilometrajeGps)
                            ? doc.Valores[ParteCampos.KilometrajeGps] as List<object>
                            : new List<object>();
            var odoIni = doc.Valores.ContainsKey(ParteCampos.OdometroInicial)
                            ? doc.Valores[ParteCampos.OdometroInicial] as List<object>
                            : new List<object>();
            var odoFin = doc.Valores.ContainsKey(ParteCampos.OdometroFinal)
                            ? doc.Valores[ParteCampos.OdometroFinal] as List<object>
                            : new List<object>();

            Horas = TimeSpan.Zero;
            HorasControladas = TimeSpan.Zero;

            for (var i = 0; i < alPozoSalida.Count; i++)
            {
                var turno = new TurnoPartePersonal
                            	{
                            		AlPozoSalida = Convert.ToDateTime(alPozoSalida[i], CultureInfo.InvariantCulture),
                            		AlPozoLlegada = (alPozoLlegada.Count > i)
                            		                	? Convert.ToDateTime(alPozoLlegada[i], CultureInfo.InvariantCulture)
                            		                	: DateTime.MinValue,
                            		DelPozoSalida = (delPozoSalida.Count > i)
                            		                	? Convert.ToDateTime(delPozoSalida[i], CultureInfo.InvariantCulture)
                            		                	: DateTime.MinValue,
                            		DelPozoLlegada = (delPozoLlegada.Count > i)
                            		                 	? Convert.ToDateTime(delPozoLlegada[i], CultureInfo.InvariantCulture)
                            		                 	: DateTime.MinValue,
                            		Km = (km.Count > i)
                            		     	? Convert.ToInt32(km[i])
                            		     	: 0,
                            		Responsable = (resp.Count > i)
                            		              	? resp[i].ToString()
                            		              	: string.Empty,
                            		AlPozoSalidaControl = (alPozoSalidaC.Count > i)
                            		                      	? Convert.ToDateTime(alPozoSalidaC[i], CultureInfo.InvariantCulture)
                            		                      	: DateTime.MinValue,
                            		AlPozoLlegadaControl = (alPozoLlegadaC.Count > i)
                            		                       	? Convert.ToDateTime(alPozoLlegadaC[i],
                            		                       	                     CultureInfo.InvariantCulture)
                            		                       	: DateTime.MinValue,
                            		DelPozoSalidaControl = (delPozoSalidaC.Count > i)
                            		                       	? Convert.ToDateTime(delPozoSalidaC[i],
                            		                       	                     CultureInfo.InvariantCulture)
                            		                       	: DateTime.MinValue,
                            		DelPozoLlegadaControl = (delPozoLlegadaC.Count > i)
                            		                        	? Convert.ToDateTime(delPozoLlegadaC[i],
                            		                        	                     CultureInfo.InvariantCulture)
                            		                        	: DateTime.MinValue,
                            		KmControl = (kmC.Count > i)
                            		            	? Convert.ToInt32(kmC[i])
                            		            	: 0,
                            		KmGps = (kmGps.Count > i)
                            		        	? Convert.ToInt32(kmGps[i])
                            		        	: 0,
                            		OdometroInicial = (odoIni.Count > i)
                            		                  	? Convert.ToInt32(odoIni[i])
                            		                  	: 0,
                            		OdometroFinal = (odoFin.Count > i)
                            		                	? Convert.ToInt32(odoFin[i])
                            		                	: 0
                            	};

            	Turnos.Add(turno);
                Horas = Horas.Add(turno.AlPozoLlegada.Subtract(turno.AlPozoSalida));
                Horas = Horas.Add(turno.DelPozoLlegada.Subtract(turno.DelPozoSalida));

                if (turno.AlPozoLlegadaControl != DateTime.MinValue)
                {
                    HorasControladas = HorasControladas.Add(turno.AlPozoLlegadaControl.Subtract(turno.AlPozoSalidaControl));
                    HorasControladas = HorasControladas.Add(turno.DelPozoLlegadaControl.Subtract(turno.DelPozoSalidaControl));
                }
            }

            foreach (var o in kmC)
                _KmTotalCalculado += Convert.ToInt32(o);

            foreach (var o in kmGps)
            {
                KmTotalGps += Convert.ToInt32(o);
            }
        }

        #endregion
    }
}