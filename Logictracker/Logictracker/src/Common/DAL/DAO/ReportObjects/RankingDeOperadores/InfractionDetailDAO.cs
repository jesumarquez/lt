using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects.RankingDeOperadores;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.ReportObjects.RankingDeOperadores
{
    /// <summary>
    /// Infraction details data access class.
    /// </summary>
    public class InfractionDetailDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public InfractionDetailDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a list of infraction details filtered by the givenn search criteria.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InfractionDetail> GetInfractionsDetails(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, List<int> operators, DateTime initialDate, DateTime endDate)
        {
            var infracciones = DAOFactory.InfraccionDAO.GetByEmpleados(operators, initialDate, endDate);

            return infracciones.Select(infraction => 
                                       new InfractionDetail
                                           {
                                               Id = infraction.Id,
                                               Latitude = infraction.Latitud,
                                               Longitude = infraction.Longitud,
                                               Vehiculo = infraction.Vehiculo.Interno,
                                               Ponderacion = GetPonderacionInfraccion(infraction),
                                               Calificacion = GetGravedadInfraccion(infraction),
                                               Operador = infraction.Empleado != null ? infraction.Empleado.Entidad.Descripcion : "Sin Chofer Identificado",
                                               Exceso = (int)(infraction.Alcanzado - infraction.Permitido),
                                               Inicio = infraction.Fecha.ToDisplayDateTime(),
                                               Pico = (int)infraction.Alcanzado,
                                               DuracionSegundos = (int) (infraction.FechaFin.HasValue ? ((DateTime) infraction.FechaFin).Subtract(infraction.Fecha).TotalSeconds : 0)
                                           });
        }

        public IEnumerable<InfractionDetail> GetInfractionsDetailsByVehicles(List<int> vehiculos, DateTime initialDate, DateTime endDate)
        {
            var infracciones = DAOFactory.InfraccionDAO.GetByVehiculos(vehiculos, initialDate, endDate);

            return infracciones.Select(infraction =>
                                       new InfractionDetail
                                       {
                                           Id = infraction.Id,
                                           Latitude = infraction.Latitud,
                                           Longitude = infraction.Longitud,
                                           Vehiculo = infraction.Vehiculo.Interno,
                                           Ponderacion = GetPonderacionInfraccion(infraction),
                                           Calificacion = GetGravedadInfraccion(infraction),
                                           Operador = infraction.Empleado != null ? infraction.Empleado.Entidad.Descripcion : "Sin Chofer Identificado",
                                           Exceso = (int)(infraction.Alcanzado - infraction.Permitido),
                                           Inicio = infraction.Fecha.ToDisplayDateTime(),
                                           Pico = (int)infraction.Alcanzado,
                                           DuracionSegundos = (int)(infraction.FechaFin.HasValue ? ((DateTime)infraction.FechaFin).Subtract(infraction.Fecha).TotalSeconds : 0)
                                       });
        }

        public DataTable GetInfractionsSummary(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, List<int> operators, DateTime initialDate, DateTime endDate)
        {
            var infracciones = DAOFactory.InfraccionDAO.GetByEmpleados(operators, initialDate, endDate);

            var infractionsTable = new DataTable("Infractions");
            DataColumn workCol = infractionsTable.Columns.Add("DriverId", typeof(Int32));
            workCol.AllowDBNull = false;
            //workCol.Unique = true;

            infractionsTable.Columns.Add("Conductor", typeof(String));
            infractionsTable.Columns.Add("Vehiculo", typeof(String));
            infractionsTable.Columns.Add("Grave", typeof(int));
            infractionsTable.Columns.Add("Media", typeof(int));
            infractionsTable.Columns.Add("Leve", typeof(int));

            foreach (var operador in operators)
            {
                var infraccionesOperador = infracciones.Where(i => i.Empleado.Id == operador);

                if (infraccionesOperador.Count() > 0)
                {

                    var row = infractionsTable.NewRow();
                    row["DriverId"] = operador;
                    row["Conductor"] = infraccionesOperador.FirstOrDefault().Empleado.Entidad.Descripcion;
                    row["Vehiculo"] = infraccionesOperador.FirstOrDefault().Vehiculo.Interno;
                    row["Grave"] = infraccionesOperador.Count(i => GetGravedadInfraccion(i) == 3);
                    row["Media"] = infraccionesOperador.Count(i => GetGravedadInfraccion(i) == 2);
                    row["Leve"] = infraccionesOperador.Count(i => GetGravedadInfraccion(i) == 1);

                    infractionsTable.Rows.Add(row);
                }
            }
            return infractionsTable;
        }

        #endregion

        #region Private Methods

        private List<PuntajeExcesoVelocidad> _puntajesVelocidad;
        private List<PuntajeExcesoTiempo> _puntajesTiempo;

        private IEnumerable<PuntajeExcesoVelocidad> PuntajesVelocidad { get { return _puntajesVelocidad ?? (_puntajesVelocidad = DAOFactory.Session.Query<PuntajeExcesoVelocidad>().Cacheable().OrderByDescending(p => p.Porcentaje).ToList()); } }       
        private IEnumerable<PuntajeExcesoTiempo> PuntajesTiempo { get { return _puntajesTiempo ?? (_puntajesTiempo = DAOFactory.Session.Query<PuntajeExcesoTiempo>().Cacheable().OrderByDescending(p => p.Segundos).ToList()); } }

        private Double GetPonderacionInfraccion(Infraccion infraction)
        {
            return (GetSpeedingPoints(infraction) * GetDurationPoints(infraction)) / 1000.0;
        }
        public int GetSpeedingPoints(Infraccion infraction)
        {
            if (infraction.Permitido == 0 || infraction.Alcanzado == 0) return 0;
            var porcentaje = ((infraction.Alcanzado - infraction.Permitido) * 100) / infraction.Permitido;
            var p = PuntajesVelocidad.FirstOrDefault(speed => speed.Porcentaje <= porcentaje);
            return p != null ? p.Puntaje : 0;
        }
        public int GetDurationPoints(Infraccion infraction)
        {
            var duration = infraction.Alcanzado- infraction.Permitido;
            var p = PuntajesTiempo.FirstOrDefault(time => time.Segundos <= duration);
            return p != null ? p.Puntaje : 0;
        }

        /// <summary>
        /// Gets the ponderation associated to the current infraction.
        /// </summary>
        /// <param name="infraction"></param>
        /// <returns></returns>
        //private Double GetPonderacionInfraccion(LogMensaje infraction)
        //{
        //    var reportFactory = new ReportFactory(DAOFactory);

        //    return (reportFactory.PuntajeExcesoVelocidadDAO.GetSpeedingPoints(infraction) * reportFactory.PuntajeExcesoTiempoDAO.GetDurationPoints(infraction)) / 1000.0;
        //}

        /// <summary>
        /// Gets the severity of the current infraction.
        /// </summary>
        /// <param name="infraction"></param>
        /// <returns></returns>
        private static int GetGravedadInfraccion(Infraccion infraction)
        {
            if (infraction.Permitido == 0 || infraction.Alcanzado == 0) return 0;

            var difference = infraction.Alcanzado - infraction.Permitido;

            return difference <= 9 ? 1 : difference > 9 && difference <= 17 ? 2 : 3;
        }
        #endregion
    }
}
