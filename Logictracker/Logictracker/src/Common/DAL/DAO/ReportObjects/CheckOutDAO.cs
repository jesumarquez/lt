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

        public List<CheckOut> GetReporte(int empresa, int linea, int transportista, DateTime desde, DateTime hasta, int minutosPeriodo, int modo)
        {
            var init = desde.Date.AddHours(3);

            var vehicles = DAOFactory.CocheDAO.GetList(new[]{empresa}, new[]{linea}, new[]{-1}, new[]{transportista}).Select(v => v.Id).ToList();
            var oLinea = DAOFactory.LineaDAO.FindById(linea);
            var list = new List<CheckOut>();
            var inicio = init;
            var fin = inicio.AddMinutes(minutosPeriodo);

            if (vehicles.Any())
            {
                switch (modo)
                {
                    case CheckOut.Modo.Acumulado:
                    case CheckOut.Modo.AcumuladoPorc:
                        var geocerca = DAOFactory.ReferenciaGeograficaDAO.FindGeocerca(oLinea.ReferenciaGeografica.Id);
                        var inicial = DAOFactory.LogPosicionDAO.GetVehiclesInside(vehicles, geocerca, init);
                        var total = (double)vehicles.Count();

                        var entradas = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(vehicles, MessageCode.InsideGeoRefference.GetMessageCode(), init, hasta, 3)
                                                                .Where(e => e.IdPuntoDeInteres.HasValue
                                                                        && e.IdPuntoDeInteres.Value == oLinea.ReferenciaGeografica.Id);
                        var salidas = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(vehicles, MessageCode.OutsideGeoRefference.GetMessageCode(), init, hasta, 3)
                                                                .Where(e => e.IdPuntoDeInteres.HasValue
                                                                        && e.IdPuntoDeInteres.Value == oLinea.ReferenciaGeografica.Id);

                        while (inicio < hasta)
                        {
                            var ins = entradas.Where(s => s.Fecha >= inicio && s.Fecha < fin).Select(d => d.Coche.Id).Distinct().Count();
                            var outs = salidas.Where(s => s.Fecha >= inicio && s.Fecha < fin).Select(d => d.Coche.Id).Distinct().Count();
                            inicial = inicial + ins - outs;
                            var checkout = new CheckOut(inicio, 0);

                            switch (modo)
                            {
                                case CheckOut.Modo.Acumulado:
                                    checkout.Cantidad = (int)inicial;
                                    break;
                                case CheckOut.Modo.AcumuladoPorc:
                                    var porc = inicial / total * 100;
                                    checkout.Cantidad = (int)porc;
                                    break;
                            }

                            if (inicio >= desde) list.Add(checkout);

                            inicio = inicio.AddMinutes(minutosPeriodo);
                            fin = inicio.AddMinutes(minutosPeriodo);
                        }
                        break;
                    case CheckOut.Modo.CheckIn:
                    case CheckOut.Modo.CheckOut:
                        var msj = string.Empty;
                        switch (modo)
                        {
                            case CheckOut.Modo.CheckIn: msj = MessageCode.InsideGeoRefference.GetMessageCode(); break;
                            case CheckOut.Modo.CheckOut: msj = MessageCode.OutsideGeoRefference.GetMessageCode(); break;
                        }

                        var eventos = DAOFactory.LogMensajeDAO.GetByVehiclesAndCode(vehicles, msj, inicio, hasta, 3)
                                                              .Where(e => e.IdPuntoDeInteres.HasValue
                                                                       && e.IdPuntoDeInteres.Value == oLinea.ReferenciaGeografica.Id);
                        if (eventos.Any())
                        {
                            while (inicio < hasta)
                            {
                                var cantidad = eventos.Where(s => s.Fecha >= inicio && s.Fecha < fin).Select(d => d.Coche.Id).Distinct().Count();
                                var checkout = new CheckOut(inicio, cantidad);

                                list.Add(checkout);

                                inicio = inicio.AddMinutes(minutosPeriodo);
                                fin = inicio.AddMinutes(minutosPeriodo);
                            }
                        }
                        break;
                    }
                    break;
            }

            return list;
        }
    }
}
