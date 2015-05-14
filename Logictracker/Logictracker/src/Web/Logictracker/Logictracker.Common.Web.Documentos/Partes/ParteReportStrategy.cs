using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos.Partes;
using Logictracker.Web.Documentos.Interfaces;

namespace Logictracker.Web.Documentos.Partes
{
    public class ParteReportStrategy: IReportStrategy
    {
        private readonly DAOFactory daoFactory;

        public ParteReportStrategy(DAOFactory daof)
        {
            daoFactory = daof;
        }

        #region IReportStrategy Members

        public IList GetData(params object[] parameters)
        {
            var aseguradora = Convert.ToInt32(parameters[0]);
            var locacion = Convert.ToInt32(parameters[1]);
            var linea = Convert.ToInt32(parameters[2]);
            var movil = Convert.ToInt32(parameters[3]);
            var equipo = Convert.ToInt32(parameters[4]);
            var inicio = Convert.ToDateTime(parameters[5]);
            var fin = Convert.ToDateTime(parameters[6]);
            var estado = Convert.ToInt32(parameters[7]);

            var usuario = WebSecurity.AuthenticatedUser.Id;

            IList result = new ArrayList();
            var list = daoFactory.DocumentoDAO.FindParteReport(aseguradora, locacion, linea, movil, equipo, inicio, fin, estado, usuario);
            foreach (Documento doc in list)
            {
                var parte = new PartePersonal(doc, daoFactory);

                result.Add(parte);
            }

            var byEquipo = new Dictionary<string, List<PartePersonal>>();

            foreach (PartePersonal parte in result)
            {
                if (!byEquipo.ContainsKey(parte.Equipo))
                    byEquipo.Add(parte.Equipo, new List<PartePersonal>());

                byEquipo[parte.Equipo].Add(parte);
            }

            var transportista = daoFactory.TransportistaDAO.FindById(aseguradora);
            foreach (var eq in byEquipo.Keys)
            {
                var oEquipo = daoFactory.EquipoDAO.FindById(byEquipo[eq][0].IdEquipo);
                var tarifa = daoFactory.TarifaTransportistaDAO.GetTarifaParaCliente(transportista.Id,
                                                                                                    oEquipo.Cliente.Id);

                var tarifaCorto = tarifa != null ? tarifa.TarifaTramoCorto : transportista.TarifaTramoCorto;
                var tarifaLargo = tarifa != null ? tarifa.TarifaTramoLargo : transportista.TarifaTramoLargo;

                double kms = (from p in byEquipo[eq] select p.KmTotal).Sum();
                var kmsc = (from p in byEquipo[eq] select p.KmTotalCalculado).Sum();
                foreach (var parte in byEquipo[eq])
                {
                    parte.Importe = parte.KmTotal *
                                    (kms < 14000 ? tarifaCorto : tarifaLargo);
                    parte.ImporteControlado = parte.KmTotalCalculado *
                                              (kmsc < 14000 ? tarifaCorto : tarifaLargo);
                }
            }
            return result;
        }

        #endregion

        public string getValue(string par, Documento doc)
        {
            foreach (DocumentoValor valor in doc.Parametros)
            {
                if (valor.Parametro.Nombre == par)
                    return valor.Valor;
            }
            return null;
        }
        public string[] getValueList(string par, Documento doc)
        {
            var list = new List<KeyValuePair<int, string>>();
            foreach (DocumentoValor valor in doc.Parametros)
                if (valor.Parametro.Nombre == par)
                    list.Add(new KeyValuePair<int, string>(valor.Repeticion, valor.Valor));

            list.Sort(new KVComparer());

            var l = new List<string>();
            foreach (var pair in list)
                l.Add(pair.Value);

            return l.ToArray();
        }
    }

    internal class KVComparer: IComparer<KeyValuePair<int, string>>
    {
        #region IComparer<KeyValuePair<int,string>> Members

        public int Compare(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return x.Key.CompareTo(y.Key);
        }

        #endregion
    }
}