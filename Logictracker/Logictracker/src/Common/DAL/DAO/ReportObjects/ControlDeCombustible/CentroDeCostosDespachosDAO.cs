#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class CentroDeCostosDespachosDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public CentroDeCostosDespachosDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IEnumerable<CentroDeCostosDespachos> FindByPlanta(int planta, List<int> mobiles, DateTime startDate, DateTime endDate)
        {
            var list = DAOFactory.MovimientoDAO.FindDespachosBetweenDatesAndMobilesForABase(mobiles, planta, startDate, endDate);

            var lista = (from descri1 in (from e in list select e.Tanque.Linea.Descripcion).Distinct().ToList()
                    let res = (from e in list where e.Tanque.Linea.Descripcion == descri1 select e)
                    select new CentroDeCostosDespachos
                               {
                                   CantDespachos = (res).Count(),
                                   CantVehiculos = (from v in res select v.Coche.Id).ToList().Distinct().Count(),
                                   TotalDespachado = res.Aggregate<Movimiento, double>(0, (current, e) => current + e.Volumen),
                                   CentroDeCostos = descri1
                               }).ToList();

         return lista.OrderBy(obj => obj.CentroDeCostos);
        } 
    }
}