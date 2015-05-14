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
    public class CentroDeCargaRemitosDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public CentroDeCargaRemitosDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IEnumerable<CentroDeCargaRemitos> FindAllByDistritoPlantaAndFecha(int distrito, int planta, DateTime startDate, DateTime endDate)
        {
            var list = DAOFactory.MovimientoDAO.FindRemitosAjustesByTanque(distrito, planta, startDate, endDate);
            
            var lista = (from descri1 in (from Movimiento e in list select e.Tanque.Linea.Descripcion).Distinct().ToList()
                         let results = (from Movimiento e in list where e.Tanque.Linea.Descripcion == descri1 select e)
                         select new CentroDeCargaRemitos
                                    {
                                        CantAjustes = (from ajustes in results
                                                       where ajustes.TipoMovimiento.Codigo.Equals("A")
                                                       select ajustes).Count(),
                                        CantRemitos = (from ajustes in results
                                                       where ajustes.TipoMovimiento.Codigo.Equals("R")
                                                       select ajustes).Count(),
                                        TotalCargado = results.Aggregate<Movimiento, double>(0, (current, r) => current + r.Volumen),
                                        CentroDeCostos = descri1
                                    }).ToList();

            return lista.OrderBy(desc => desc.CentroDeCostos);

        }
    }
}