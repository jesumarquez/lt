#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class TicketReportDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public TicketReportDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public List<TicketReport> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosdeCosto, IEnumerable<int> vehiculos, IEnumerable<int> estados, DateTime desde, DateTime hasta)
        {
            return DAOFactory.TicketDAO.GetList(empresas, 
                                                lineas, 
                                                transportistas, 
                                                departamentos,
                                                centrosdeCosto, 
                                                new[] {-1},
                                                vehiculos, 
                                                estados, 
                                                new[] {-1},
                                                new[] {-1},
                                                new[] {-1},
                                                desde, 
                                                hasta)
                                       .Select(ticket => new TicketReport(ticket)).ToList();
        }

        /*Beware: Returns a DataSet. Used only for the mobilesTickets Report*/
        public DataSet GetTicketsByDateAndMobiles(IEnumerable<int> mobiles, DateTime begginDate, DateTime endDate, int linea)
        {
            var mobilesString = ConvertListIntoString(mobiles);

            var ds = new DataSet();

            using (var command = DAOFactory.CreateCommand() as SqlCommand)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_getTicketsStatus";

                CreateParameter(command, DbType.DateTime, "@desde", begginDate);
                CreateParameter(command, DbType.DateTime, "@hasta", endDate);
                CreateParameter(command, DbType.String, "@mobiles", mobilesString);
                CreateParameter(command, DbType.Int32, "@linea", linea);

                var da = new SqlDataAdapter(command);

                da.Fill(ds);
            }

            return ds;
        }

        public DataSet GetTicketsTimeDifferencesByDateAndMobiles(IEnumerable<int> mobiles, DateTime desde,DateTime hasta,int linea )
        {
            var mobilesString = ConvertListIntoString(mobiles);

            var ds = new DataSet();

            using (var command = DAOFactory.CreateCommand() as SqlCommand)
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_get_TicketTimeDifferences";

                CreateParameter(command, DbType.DateTime, "@desde", desde);
                CreateParameter(command, DbType.DateTime, "@hasta", hasta);
                CreateParameter(command, DbType.String, "@mobiles", mobilesString);
                CreateParameter(command, DbType.Int32, "@linea", linea);

                var da = new SqlDataAdapter(command);

                da.Fill(ds);
            }

            return ds;
        }

        private static void CreateParameter(SqlCommand command, DbType type, string name, object value)
        {
            var param = command.CreateParameter();

            param.DbType = type;
            param.ParameterName = name;
            param.Value = value;

            command.Parameters.Add(param);
        }

        #region Private Methods

        /// <summary>
        /// Transforms a List into a Comma Sepparated Values (CSV) String
        /// </summary>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        private static string ConvertListIntoString(IEnumerable<int> mobiles)
        {
            var mobilesString = mobiles.Aggregate(String.Empty, (current, id) => String.Concat(current, id.ToString(), ','));

            mobilesString = mobilesString.TrimEnd(',');

            return mobilesString;
        }

        #endregion
    }
}
