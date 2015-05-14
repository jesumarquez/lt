#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects;
using Logictracker.Web.Helpers.C1Helpers;

#endregion

namespace Logictracker.Web.Helpers.ExportHelpers
{
    [Serializable]
	public class GridToCSVBuilder : BaseCsvBuilder
    {
        public GridToCSVBuilder()
        {
            
        }
        public GridToCSVBuilder(char separator): base(separator)
        {
        }

        #region Public Methods

        /// <summary>
        /// Generates the columns of the report*/
        /// </summary>
        /// <param name="groupColumns"></param>
        /// <param name="grid"></param>
        public void GenerateColumns(/*IEnumerable<string> groupColumns, */C1GridView grid)
        {
            var headers = new List<string>();
            var properties = new List<string>();

            //if (groupColumns != null) headers.AddRange(groupColumns.Where(groupColumn => !String.IsNullOrEmpty(groupColumn)));

           foreach (var c in from C1Field c in grid.Columns where (/*c.Visible &&*/ c.HeaderText != string.Empty) || (c as C1ResourceGroupColumn != null) select c)
            {
                headers.Add(Format(c.HeaderText));

                var col = c as C1BoundField;

                properties.Add(col != null ? Format(col.DataField) : String.Empty);
            }

            GenerateColumns(headers);

            SetPropertyNames(properties);
        }

        /// <summary>
        /// Fills the report with the Items of the grid. Used when The report doesnt allow paging
        /// </summary>
        /// <param name="grid"></param>
        public void GenerateFields(C1GridView grid) { ExportGridFields(String.Empty, grid); }


        /// <summary>
        /// Exports All the Grid, including Columns and Fields.
        /// </summary>
        /// <param name="grid"></param>
        public void ExportGrid(C1GridView grid)
        {
            GenerateColumns(/*null, */grid);
            GenerateFields(grid);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Fills the report with the items of the grid beggining the row with the initialString.
        /// </summary>
        /// <param name="rowInitialStr"></param>
        /// <param name="grid"></param>
        private void ExportGridFields(String rowInitialStr, C1GridView grid)
        {
            foreach (C1GridViewRow row in grid.Rows)
            {
                var data = rowInitialStr;

                for (var i = 0; i < grid.Columns.Count; i++)
                    if (grid.Columns[i].HeaderText != String.Empty)
                        data = String.Concat(data, String.Format("{0}" + Separator, row.Cells[i].Text.Equals(GridEmptyString) ? String.Empty : Format(row.Cells[i].Text).Replace(Separator, AlternateSeparator).Replace("\r", " ").Replace("\n", " ")));

                Sb.AppendLine(data.TrimEnd(Separator));
            }

            Sb.AppendLine();
        }

        #endregion
    }
}
