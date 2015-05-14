#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobileExtraHoursDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileExtraHoursDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public List<MobileExtraHours> GetMobilesExtraHours(IEnumerable<int> mobiles,DateTime desde, DateTime hasta)
        {
            var cocheDAO = DAOFactory.CocheDAO;

            var results = new List<MobileExtraHours>();

            foreach (var mobileId in mobiles)
            {
                var mobile = cocheDAO.FindById(mobileId);

                //var inicioTurno = mobile.InicioTurno >= 0.001 ? mobile.InicioTurno : mobile.TipoCoche.InicioTurno;
                //var finTurno = mobile.FinTurno >= 0.001 ? mobile.FinTurno : mobile.TipoCoche.FinTurno;
                var actualDate = desde;
                var exceededHours = 0.0;

                var data = DAOFactory.DatamartDAO.GetBetweenDatesWithMovement(mobile.Id, desde, hasta);

                while (actualDate <= hasta && actualDate <= DateTime.Now)
                {
                    var list = data.Where(d => d.Vehicle.Id == mobile.Id && d.Begin >= actualDate && d.Begin <= actualDate.AddDays(1));

                    if (!list.Any())
                    {
                        actualDate = actualDate.AddDays(1);

                        continue;
                    }

                    actualDate = actualDate.AddDays(1);
                }

                if (exceededHours > 0.001) results.Add(new MobileExtraHours { ExtraHours = exceededHours, Interno = mobile.Interno });
            }

            return results;
        }
    }
}
