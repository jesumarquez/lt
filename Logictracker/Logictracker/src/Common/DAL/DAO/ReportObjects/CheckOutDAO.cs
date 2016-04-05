using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;
using Logictracker.Messaging;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class CheckOutDAO : ReportDAO
    {
        public CheckOutDAO(DAOFactory daoFactory) : base(daoFactory) {}

        public List<CheckOut> GetCheckOuts(int empresa, int linea, int transportista, DateTime desde, DateTime hasta, int minutosPeriodo)
        {
            var vehicles = DAOFactory.CocheDAO.GetList(new[]{empresa}, new[]{linea}, new[]{-1}, new[]{transportista}).Select(v => v.Id).ToList();
            var oLinea = DAOFactory.LineaDAO.FindById(linea);

            var salidas = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(vehicles, MessageCode.OutsideGeoRefference.GetMessageCode(), desde, hasta, 3)
                                                  .Where(e => e.IdPuntoDeInteres.HasValue 
                                                           && e.IdPuntoDeInteres.Value == oLinea.ReferenciaGeografica.Id);

            var inicio = desde;
            var fin = inicio.AddMinutes(minutosPeriodo);

            var list = new List<CheckOut>();
            while (inicio < hasta)
            {
                var cantidad = salidas.Where(s => s.Fecha >= inicio && s.Fecha < fin).Select(d => d.Coche.Id).Distinct().Count();
                var checkout = new CheckOut(inicio, cantidad);
                list.Add(checkout);
                
                inicio = inicio.AddMinutes(minutosPeriodo);
                fin = inicio.AddMinutes(minutosPeriodo);
            }

            return list;
        }
    }
}
