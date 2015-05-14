#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class ConsumosPorMotorDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public ConsumosPorMotorDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        public IList<ConsumosMotor> FindConsumosForMotores(List<int> motores, DateTime startDate, DateTime endDate)
        {
            var movimientos = DAOFactory.MovimientoDAO.FindDespachosBetweenDatesAndMobiles(motores, startDate, endDate);

            return (from motor in motores
                    select (from mov in movimientos where mov.Caudalimetro.Id == motor select mov).ToList()
                    into movMotor where movMotor.Count > 0
                    select new ConsumosMotor
                          {
                              DifVolumen = movMotor.Sum(movi => movi.Volumen),
                              CaudalMinimo = movMotor.Count(movi => movi.Caudal != 0) > 0 
                                            ? movMotor.Where(movi => movi.Caudal != 0).Min(movi => movi.Caudal) : 0,
                              CaudalMaximo = movMotor.Max(movi => movi.Caudal),
                              CantidadConsumos = movMotor.Select(movi => movi.Id).Count(),
                              HsEnMarcha = movMotor.Max(movi => movi.HsEnMarcha) - movMotor.Min(movi => movi.HsEnMarcha),
                              DescripcionMotor = movMotor.First().Caudalimetro.Descripcion,
                              CentroDeCostos = movMotor.First().Caudalimetro.Equipo.Descripcion,
                              IDMotor = movMotor.First().Caudalimetro.Id,
                              UltiMedicion = movMotor.OrderByDescending(movi => movi.Fecha).Select(movi => movi.Fecha).FirstOrDefault()
                          }).ToList();
        }

        #endregion
    }
}
