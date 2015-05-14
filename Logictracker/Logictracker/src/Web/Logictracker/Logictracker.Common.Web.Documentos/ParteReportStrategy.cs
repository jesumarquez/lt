using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Urbetrack.DAL.Factories;
using Urbetrack.Types.BusinessObjects.Documentos;
using Urbetrack.Types.ReportObjects;

namespace Urbetrack.Documentos
{
    public class ParteReportStrategy: IReportStrategy
    {
        private DAOFactory daoFactory = null;

        public ParteReportStrategy()
        {
            daoFactory = new DAOFactory();
        }

        public IList GetData(params object[] parameters)
        {
            int linea = Convert.ToInt32(parameters[0]);
            int aseguradora = Convert.ToInt32(parameters[1]);
            DateTime inicio = Convert.ToDateTime(parameters[2]);
            DateTime fin = Convert.ToDateTime(parameters[3]);

            IList result = new ArrayList();
            IList list = daoFactory.DocumentoDAO.FindParteReport(linea, aseguradora, inicio, fin);
            foreach (Documento doc in list)
            {
                PartePersonal parte = new PartePersonal();
                parte.Codigo = doc.Codigo;
                parte.Descripcion = doc.Descripcion;
                parte.Fecha = doc.Fecha;
                parte.Id = doc.Id;
                parte.Salida = getValue("Lugar Partida", doc);
                parte.Observaciones = getValue("Observaciones", doc);
                parte.Llegada = getValue("Lugar Llegada", doc);
                parte.KmTotal = Convert.ToInt32(getValue("Kilometraje Total", doc));
                parte.Equipo = getValue("Equipo", doc);
                parte.Empresa = daoFactory.TransportistaDAO.FindById(Convert.ToInt32(getValue("Empresa", doc))).Descripcion;
                parte.Vehiculo = Convert.ToInt32(getValue("Vehiculo", doc));
                parte.Turnos = new List<TurnoPartePersonal>();

                string[] alPozoSalida = getValueList("Hs Salida Al Pozo", doc);
                string[] alPozoLlegada = getValueList("Hs Llegada Al Pozo", doc);
                string[] delPozoSalida = getValueList("Hs Salida Del Pozo", doc);
                string[] delPozoLlegada = getValueList("Hs Llegada Del Pozo", doc);
                string[] km = getValueList("Kilometraje", doc);
                string[] resp = getValueList("Responsable", doc);

                for(int i = 0; i < alPozoSalida.Length; i++)
                {
                    TurnoPartePersonal turno = new TurnoPartePersonal();
                    turno.AlPozoSalida = Convert.ToDateTime(alPozoSalida[i], CultureInfo.InvariantCulture);
                    if (alPozoLlegada.Length > i)
                        turno.AlPozoLlegada = Convert.ToDateTime(alPozoLlegada[i], CultureInfo.InvariantCulture);
                    if (delPozoSalida.Length > i)
                        turno.DelPozoSalida = Convert.ToDateTime(delPozoSalida[i], CultureInfo.InvariantCulture);
                    if (delPozoLlegada.Length > i)
                        turno.DelPozoLlegada = Convert.ToDateTime(delPozoLlegada[i], CultureInfo.InvariantCulture);
                    if (km.Length > i)
                        turno.Km = Convert.ToInt32(km[i]);
                    if (resp.Length > i)
                        turno.Responsable = resp[i];

                    parte.Turnos.Add(turno);
                }

                parte.KmTotalCalculado = 0; 
                foreach (TurnoPartePersonal turno in parte.Turnos)
                {
                    try
                    {
                        parte.KmTotalCalculado +=
                            daoFactory.CocheDAO.GetDistance(parte.Vehiculo, turno.AlPozoSalida, turno.AlPozoLlegada) +
                            daoFactory.CocheDAO.GetDistance(parte.Vehiculo, turno.DelPozoSalida, turno.DelPozoLlegada);
                    }catch
                    {
                    }
                }
                result.Add(parte);
            }
            return result;
        }
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
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            foreach (DocumentoValor valor in doc.Parametros)
                if (valor.Parametro.Nombre == par)
                    list.Add(new KeyValuePair<int, string>(valor.Repeticion, valor.Valor));

            list.Sort(new KVComparer());

            List<string> l = new List<string>();
            foreach (KeyValuePair<int, string> pair in list)
                l.Add(pair.Value);

            return l.ToArray();
        }
    }
    internal class KVComparer: IComparer<KeyValuePair<int, string>>
    {
        public int Compare(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }
}
