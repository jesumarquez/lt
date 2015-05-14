#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Logictracker.Security;

#endregion

namespace Logictracker.Types.ValueObjects
{
    [Serializable]
    public class BaseCsvBuilder
    {
        #region Private Properties

        private readonly List<string> _propertyNames = new List<string>();

        #endregion

        #region Constants

        protected const string GridEmptyString = "&nbsp;"; /*how an empty column is represented while converting a DataTable into the source of a grid*/

        #endregion

        #region Protected Properties

        protected readonly StringBuilder Sb = new StringBuilder();
        protected char Separator = ';';
        protected char AlternateSeparator { get { return Separator == ';' ? ',' : ';'; } }

        #endregion

        public BaseCsvBuilder()
        {
        }

        public BaseCsvBuilder(char separator)
        {
            Separator = separator;
        }

        #region Protected Methods

        /// <summary>
        /// Removes the & and te acute;
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected static string Format(string text) { return HttpUtility.HtmlDecode(text).Normalize(); }

        #endregion

        #region HeaderMethods

        /// <summary>
        /// Generates the Report Header.
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="filters"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public virtual void GenerateHeader(String reportName, Dictionary<string,string> filters, DateTime startDate, DateTime endDate)
        {
            Sb.AppendLine(reportName + Separator);
            Sb.AppendLine("Fecha de Emisión:" + Separator + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            GenerateCultureInfo();

            var headers = (from header in filters where header.Value != null select header).ToList();

            foreach (var filter in headers) Sb.AppendLine(Format(filter.Key) + ":" + Separator + Format(filter.Value));

            if (!startDate.Equals(null)) Sb.AppendLine("Desde: " + Separator + startDate.ToString("dd/MM/yyyy"));
            if (!endDate.Equals(null)) Sb.AppendLine("Hasta: " + Separator + endDate.ToString("dd/MM/yyyy"));

            Sb.AppendLine("");
        }

        /// <summary>
        /// Generates the Report Header
        /// Use null when parameter is not used.
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="filters"></param>
        public virtual void GenerateHeader(String reportName, Dictionary<string,string> filters)
        {
            Sb.AppendLine(reportName + Separator);
            Sb.AppendLine("Fecha de Emisión:" + Separator + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

            GenerateCultureInfo();

            foreach (var filter in filters) Sb.AppendLine(filter.Key + ":" + Separator + filter.Value);

            Sb.AppendLine("");
        }

        #endregion

        #region Column Methods

        /// <summary>
        /// Sets the propertyNames used to fill the grid with reflection
        /// </summary>
        /// <param name="list"></param>
        public virtual void SetPropertyNames(IEnumerable list)
        {
            _propertyNames.Clear();

            foreach (var o in list) { _propertyNames.Add(o == null ? String.Empty : o.ToString()); }
        }

        /// <summary>
        /// Generates the columns Names of the Report
        /// </summary>
        /// <param name="columnNames"></param>
        protected virtual void GenerateColumns(IEnumerable columnNames)
        {
        	_propertyNames.Clear();

            var headers = columnNames.Cast<object>().Aggregate("", (current, obj) => String.Concat(current, String.Format("{0}" + Separator, Format(obj.ToString()))));

            Sb.AppendLine(headers.TrimEnd(Separator));
        }

        #endregion

        #region Field Methods

        /// <summary>
        /// Fills the report with the Items of the list. Used in report with paging enabled.
        /// </summary>
        /// <param name="list"></param>
        public virtual void GenerateFields(IEnumerable list)
        {
            var fields = (from object o in list
                          where o != null
                          select _propertyNames.Select(df => string.IsNullOrEmpty(df)
                                                                 ? String.Empty
                                                                 : o.GetType().GetProperty(df).GetValue(o, null)).
                              Aggregate(String.Empty,
                                        (current, val) => String.Concat(current, String.Format("{0}" + Separator, val))));
        }

        #endregion

        #region Culture Methods

        /// <summary>
        /// Adds culture information to the csv.
        /// </summary>
        private void GenerateCultureInfo()
        {
            var userData = WebSecurity.AuthenticatedUser;

            if (userData == null) return;

            var gmtModifier = TimeSpan.FromHours(userData.GmtModifier).ToString();

            gmtModifier = gmtModifier.Substring(0, gmtModifier.LastIndexOf(':'));

            if (!gmtModifier.StartsWith("-")) gmtModifier = string.Concat("+", gmtModifier);

            Sb.AppendLine(String.Format("Informacion regional" + Separator + "Idioma:{0}" + Separator + "GMT{1}", userData.Culture.DisplayName, gmtModifier));
            Sb.AppendLine("");
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Generates the CSV File.
        /// </summary>
        public virtual string Build()
        {
            Sb.AppendLine();

            return Sb.ToString();
        }

        #endregion

        #region Others

        /// <summary>
        /// Generates a csv row.
        /// </summary>
        /// <param name="row"></param>
        public virtual void GenerateRow(string row) { Sb.AppendLine(Format(row)); }

        #endregion
    }
}
