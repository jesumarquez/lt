using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using System.Data;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class OdometroStatusDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public OdometroStatusDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IEnumerable FindByVehiculosAndOdometros(List<int> vehiculos, List<int> odometros, bool porVencer)
        {
            var odometerVehicles = DAOFactory.MovOdometroVehiculoDAO.GetForVehicles(vehiculos, odometros, porVencer);
            
            var results = odometerVehicles
                .Select(
                v => new OdometroStatus
               {
                   Interno = v.Vehiculo.Interno,
                   Patente = v.Vehiculo.Patente,
                   CentroDeCosto = v.Vehiculo.CentroDeCostos != null ? v.Vehiculo.CentroDeCostos.Descripcion : string.Empty,
                   Referencia = v.Vehiculo.Referencia,
                   Blue = (v.Odometro.PorTiempo && v.Dias >= v.Odometro.ReferenciaTiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.ReferenciaKm + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.ReferenciaHoras + v.AjusteHoras)
                                ? (byte)47
                                : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma2Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma2Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma2Horas + v.AjusteHoras) 
                                    ? v.Odometro.Alarma2Blue
                                    : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma1Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma1Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma1Horas + v.AjusteHoras)
                                        ? v.Odometro.Alarma1Blue 
                                        : (byte)47,
                   Green = (v.Odometro.PorTiempo && v.Dias >= v.Odometro.ReferenciaTiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.ReferenciaKm + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.ReferenciaHoras + v.AjusteHoras)
                                ? (byte)47
                                : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma2Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma2Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma2Horas + v.AjusteHoras) 
                                    ? v.Odometro.Alarma2Green
                                    : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma1Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma1Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma1Horas + v.AjusteHoras)
                                        ? v.Odometro.Alarma1Green 
                                        : (byte)255,
                   Red = (v.Odometro.PorTiempo && v.Dias >= v.Odometro.ReferenciaTiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.ReferenciaKm + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.ReferenciaHoras + v.AjusteHoras)
                                ? (byte)255
                                : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma2Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma2Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma2Horas + v.AjusteHoras) 
                                        ? v.Odometro.Alarma2Red
                                        : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma1Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma1Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma1Horas + v.AjusteHoras)
                                            ? v.Odometro.Alarma1Red 
                                            : (byte)173,
                   UltimoUpdate = v.UltimoUpdate != null ? ((DateTime)v.UltimoUpdate).ToDisplayDateTime() : DateTime.MinValue,
                   Responsable = v.Vehiculo.Chofer != null ? v.Vehiculo.Chofer.Entidad.Descripcion : " - ",
                   Odometro = v.Odometro.Descripcion,
                   Tipo = v.Vehiculo.TipoCoche.Descripcion,
                   KilometrosReferencia = v.Odometro.PorKm ? v.Odometro.ReferenciaKm : (double?)null,
                   KilometrosFaltantes = v.Odometro.PorKm ? v.Odometro.ReferenciaKm - v.Kilometros + v.AjusteKilometros : (double?)null,
                   TiempoReferencia = v.Odometro.PorTiempo ? v.Odometro.ReferenciaTiempo : (int?)null,
                   TiempoFaltante = v.Odometro.PorTiempo ? v.Odometro.ReferenciaTiempo - v.Dias + v.AjusteDias : (int?)null,
                   HorasReferencia = v.Odometro.PorHoras ? v.Odometro.ReferenciaHoras : (double?)null,
                   HorasFaltantes = v.Odometro.PorHoras ? v.Odometro.ReferenciaHoras - v.Horas + v.AjusteHoras : (double?)null,
                   Priority = (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma2Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma2Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma2Horas + v.AjusteHoras) ? 2
                            : (v.Odometro.PorTiempo && v.Dias >= v.Odometro.Alarma1Tiempo + v.AjusteDias) || (v.Odometro.PorKm && v.Kilometros >= v.Odometro.Alarma1Km + v.AjusteKilometros) || (v.Odometro.PorHoras && v.Horas >= v.Odometro.Alarma1Horas + v.AjusteHoras) ? 1 : 0
               })
               .ToList();

            return results.OrderBy(o => o.Odometro).ThenByDescending(o => o.Priority).ThenBy(o => o.Tipo).ThenBy(o => o.Interno);
        }

        public DataRow GetOdometersSummary(List<int> vehiculos, List<int> odometros)
        {
            var odometers = DAOFactory.MovOdometroVehiculoDAO.GetForVehicles(vehiculos, odometros, false);

            var dt = new DataTable("Odometers");

            dt.Columns.Add("1er Aviso", typeof(int));
            dt.Columns.Add("2do Aviso", typeof(int));
            dt.Columns.Add("Vencidos", typeof(int));

            var row = dt.NewRow();            
            row["1er Aviso"] = odometers.Count(o => !o.Vencido() && !o.SuperoSegundoAviso() && o.SuperoPrimerAviso());
            row["2do Aviso"] = odometers.Count(o => !o.Vencido() && o.SuperoSegundoAviso());
            row["Vencidos"] = odometers.Count(o => o.Vencido());;
            
            return row;
        }
    }
}
