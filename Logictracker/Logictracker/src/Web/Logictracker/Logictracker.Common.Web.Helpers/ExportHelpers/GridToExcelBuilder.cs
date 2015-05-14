using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Logictracker.Culture;
using Logictracker.Types.ValueObjects;
using System.IO;
using Logictracker.Utils;
using Logictracker.Web.Helpers.ExtensionMethods;
using Microsoft.Win32;

namespace Logictracker.Web.Helpers.ExportHelpers
{
    public class GridToExcelBuilder : BaseCsvBuilder
    {
        private class ExcelColumn
        {
            public string PropertyName;
            public string PropertyType;
            public string DataFormatString;
            public string ExcelCol;
            public uint ExcelRow;
            public int LastIndex;
            public uint Style;
            public string ConditionalFormatStart;

            public CellValues ExcelCellValue
            {
                get
                {
                    var numericType = new[] { typeof(double).FullName, typeof(int).FullName };
                    var dateType = new[] { typeof(DateTime).FullName };
                    var boolType = new[] { typeof(bool).FullName };

                    if (numericType.Any(e => e == PropertyType)) return CellValues.Number;
                    if (dateType.Any(e => e == PropertyType)) return CellValues.Date;
                    return boolType.Any(e => e == PropertyType) ? CellValues.Boolean : CellValues.SharedString;
                }
            }
        }

        private readonly string _filename;
        private readonly MemoryStream _stream;
        private readonly SpreadsheetDocument _document;
        private readonly WorkbookPart _wbPart;
        private const string SheetName = "Informe";
        private readonly List<ExcelColumn> _excelColumns = new List<ExcelColumn>();

        public GridToExcelBuilder(string path)
        {
            _stream = new MemoryStream();
            File.Open(path, FileMode.Open).CopyTo(_stream);

            _stream.Position = 0;
            _document = SpreadsheetDocument.Open(_stream, true);
            _wbPart = _document.WorkbookPart;
        }
        public GridToExcelBuilder(string templateName, string recurso)
        {
            var carpeta = string.IsNullOrEmpty(recurso) ? "Logictracker" : recurso;

            _filename = HttpContext.Current.Server.MapPath("~/ExcelTemplate/" + carpeta + "/" + templateName);
            _stream = new MemoryStream();
            try
            {
                File.Open(_filename, FileMode.Open).CopyTo(_stream);
            }
            catch (Exception)
            {
                _filename = HttpContext.Current.Server.MapPath("~/ExcelTemplate/Logictracker/" + templateName);
                File.Open(_filename, FileMode.Open).CopyTo(_stream);
            }
            
            _stream.Position = 0;
            _document = SpreadsheetDocument.Open(_stream, true);
            _wbPart = _document.WorkbookPart;
        }

        public override void GenerateHeader(string reportName, Dictionary<string, string> filters)
        {
            foreach (var filter in filters)
            {
                if (filter.Key.Trim().Equals(string.Empty)) continue;

                var range = _wbPart.GetDefinedName(filter.Key.Replace(" ", "_").Replace("/", "_").Replace("%", "_").Replace("-", "_"));
                _wbPart.UpdateValue(SheetName, range, filter.Value, 0, CellValues.SharedString);
            }
            _wbPart.UpdateValue(SheetName, _wbPart.GetDefinedName("Titulo"), reportName, 0, CellValues.SharedString);
        }

        public void GenerateColumns<T>(List<T> list)
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.GetGridMappingAttributes().Any())
                .OrderBy(p => (p.GetGridMappingAttributes().First()).Index);

            foreach (var property in properties)
            {
                var attribute = property.GetGridMappingAttributes().First();

                if (attribute.IsDataKey || (attribute.IsTemplate && (attribute.DataFormatString.Trim().Equals(string.Empty) || attribute.DataFormatString.Trim().Equals("{0}")))) continue;
                if (string.IsNullOrEmpty(attribute.HeaderText) && attribute.ResourceName == null && attribute.VariableName == null) continue;

                //var templateName = !string.IsNullOrEmpty(attribute.ExcelTemplateName)
                //    ? attribute.ExcelTemplateName
                //    : Format(string.IsNullOrEmpty(attribute.HeaderText)
                //                            ? CultureManager.GetString(attribute.ResourceName, attribute.VariableName)
                //                            : attribute.HeaderText).Replace(" ", "_").Replace("/", "_").Replace("%", "_").Replace("-", "_");

                var templateName = attribute.VariableName.Replace(" ", "_").Replace("/", "_").Replace("%", "_").Replace("-", "_");

                var headerText = CultureManager.GetString(attribute.ResourceName, attribute.VariableName);

                var excelData = _wbPart.GetDefinedName(templateName);
                var excelCol = new ExcelColumn
                                   {
                                       ExcelCol = ExcelExtensions.ParseExcelColumn(excelData),
                                       ExcelRow = ExcelExtensions.ParseExcelRow(excelData),
                                       PropertyName = property.Name,
                                       PropertyType = property.PropertyType.FullName,
                                       DataFormatString = attribute.ExcelTextFormat,
                                   };
                var templateCell = excelCol.ExcelCol + (excelCol.ExcelRow + 1);
                excelCol.Style = _wbPart.GetStyle(SheetName, templateCell);
                excelCol.ConditionalFormatStart = _wbPart.HasConditionals(SheetName, templateCell) ? templateCell : string.Empty;
                _excelColumns.Add(excelCol);
                _wbPart.UpdateValue(SheetName, _wbPart.GetDefinedName(templateName), headerText, 0, CellValues.SharedString);
            }
        }
        public void GenerateCols(IEnumerable list)
        {
            var first = true;
            
            foreach (var result in list)
            {
                if (first)
                {
                    var properties = result.GetType().GetProperties()
                        .Where(p => p.GetGridMappingAttributes().Any())
                        .Where(p => !p.GetGridMappingAttributes().First().IsDataKey)
                        .OrderBy(p => (p.GetGridMappingAttributes().First()).Index)
                        .ToArray();

                    foreach (var property in properties)
                    {
                        var attribute = property.GetGridMappingAttributes().First();

                        if (attribute.IsDataKey || attribute.IsTemplate) continue;
                        if (string.IsNullOrEmpty(attribute.HeaderText) && attribute.ResourceName == null && attribute.VariableName == null) continue;

                        var headerText = Format(string.IsNullOrEmpty(attribute.HeaderText)
                                                    ? CultureManager.GetString(attribute.ResourceName, attribute.VariableName)
                                                    : attribute.HeaderText).Replace(" ", "_").Replace("/", "_").Replace("%", "_").Replace("-", "_");

                        var excelData = _wbPart.GetDefinedName(headerText);
                        var excelCol = new ExcelColumn
                        {
                            ExcelCol = ExcelExtensions.ParseExcelColumn(excelData),
                            ExcelRow = ExcelExtensions.ParseExcelRow(excelData),
                            PropertyName = property.Name,
                            PropertyType = property.PropertyType.FullName,
                        };
                        var templateCell = excelCol.ExcelCol + (excelCol.ExcelRow + 1);
                        excelCol.Style = _wbPart.GetStyle(SheetName, templateCell);
                        excelCol.ConditionalFormatStart = _wbPart.HasConditionals(SheetName, templateCell) ? templateCell : string.Empty;
                        _excelColumns.Add(excelCol);
                    }
                    first = false;
                }
            }
        }

        public delegate string CustomFormat(Type type, object value);
        public void GenerateFields<T>(List<T> list)
        {
            GenerateFields(list, null);
        }
        public void GenerateFields<T>(List<T> list, CustomFormat customFormat)
        {
            foreach (var data in list)
            {
                foreach (var excelColumn in _excelColumns)
                {
                    excelColumn.LastIndex++;

                    var value = Convert.ToString(data.GetReflectedValue(excelColumn.PropertyName), CultureInfo.InvariantCulture);
                    var style = excelColumn.Style;
                    var excelCellValue = excelColumn.ExcelCellValue;

                    if (excelColumn.DataFormatString.ToLower() == "custom" && customFormat != null)
                    {
                        value = customFormat(Type.GetType(excelColumn.PropertyType), value);
                    }
                    else
                    {
                        if (excelColumn.PropertyType == typeof (TimeSpan).FullName)
                        {
                            var tmv = (TimeSpan) data.GetReflectedValue(excelColumn.PropertyName);

                            //if (tmv.Days > 0 || tmv.Hours > 0)
                              //  value = string.Format("{0:00} - {1:00}:{2:00}", tmv.Days, tmv.Hours, tmv.Minutes);
                            //else
                                value = string.Format("{0:00}:{1:00}", tmv.Hours, tmv.Minutes);
//                                value = string.Format("{0:00}:{1:00}:{2:00}", tmv.Hours, tmv.Minutes, tmv.Seconds);

                        }
                        else if (excelColumn.PropertyType == typeof (DateTime).FullName ||
                                 excelColumn.PropertyType == typeof (DateTime?).FullName)
                        {
                            try
                            {
                                var tmv = (DateTime) data.GetReflectedValue(excelColumn.PropertyName);
                                value = string.Format(excelColumn.DataFormatString, tmv);
                                    //tmv.ToString(excelColumn.DataFormatString.Replace("{0:", "").Replace("}", ""));
                            }
                            catch (Exception)
                            {
                                value = string.Empty;
                            }
                            excelCellValue = CellValues.SharedString;
                        }
                    }

                    var address = excelColumn.ExcelCol + (excelColumn.ExcelRow + excelColumn.LastIndex);
                    _wbPart.UpdateValue(SheetName, address, value, style, excelCellValue);
                }
            }
            // Actualizo los condicionales
            foreach (var ex in _excelColumns.Where(e => !string.IsNullOrEmpty(e.ConditionalFormatStart)))
            {
                _wbPart.UpdateConditionalOver(SheetName, ex.ConditionalFormatStart, ex.ConditionalFormatStart + ":" + ex.ExcelCol + (ex.ExcelRow + ex.LastIndex));
            }
        }
        public void GenerateField(object result)
        {
            foreach (var excelColum in _excelColumns)
            {
                excelColum.LastIndex++;

                var value = Convert.ToString(result.GetReflectedValue(excelColum.PropertyName), CultureInfo.InvariantCulture);

                if (excelColum.PropertyType == typeof(TimeSpan).FullName)
                {
                    var tmv = (TimeSpan)result.GetReflectedValue(excelColum.PropertyName);
                    value = string.Format("{0:00} - {1:00}:{2:00}", tmv.Days, tmv.Hours, tmv.Minutes);
                }
                else if (excelColum.PropertyType == typeof(DateTime).FullName)
                {
                    var tmv = (DateTime)result.GetReflectedValue(excelColum.PropertyName);
                    value = tmv.ToOADate().ToString(CultureInfo.InvariantCulture);
                }

                var address = excelColum.ExcelCol + (excelColum.ExcelRow + excelColum.LastIndex);
                _wbPart.UpdateValue(SheetName, address, value, excelColum.Style, excelColum.ExcelCellValue);
            }
            // Actualizo los condicionales
            foreach (var ex in _excelColumns.Where(e => !string.IsNullOrEmpty(e.ConditionalFormatStart)))
            {
                _wbPart.UpdateConditionalOver(SheetName, ex.ConditionalFormatStart, ex.ConditionalFormatStart + ":" + ex.ExcelCol + (ex.ExcelRow + ex.LastIndex));
            }
        }

        public void AddExcelItemList(Dictionary<string, string> list)
        {
            var i = 15;
            foreach (var item in list)
            {
                if (item.Key.Trim().Equals(string.Empty)) continue;

                _wbPart.UpdateValue(SheetName, "A" + i, item.Key, 0, CellValues.SharedString);
                _wbPart.UpdateValue(SheetName, "B" + i, item.Value, 0, CellValues.Number);
                i++;
            }

            var wsparts = _wbPart.WorksheetParts.ToArray();

            foreach (var wsp in wsparts)
            {
                if (wsp.DrawingsPart != null && wsp.DrawingsPart.ChartParts.Count() > 0)
                {
                    var chartPart = wsp.DrawingsPart.ChartParts.First();

                    foreach (var formula in
                             chartPart.ChartSpace.Descendants<DocumentFormat.OpenXml.Drawing.Charts.Formula>())
                    {
                        if (formula.Text.Contains("Informe!"))
                        {
                            var col = formula.Text.Split('$')[1];
                            formula.Text += ":$" + col + "$" + (i-1);
                        }
                    }
                    chartPart.ChartSpace.Save();
                }
            }
        }

        public void AddExcelExtraItemList(List<string> extraItems)
        {
            var i = 15;

            foreach (var item in extraItems)
            {
                var ch = 'C';
                var values = item.Split('|');

                foreach (var value in values)
                {
                    _wbPart.UpdateValue(SheetName, ch + i.ToString("#0"), value, 0, CellValues.SharedString);
                    ch = GetNextChar(ch);
                }
                i++;
            }
        }

        private static char GetNextChar(char ch)
        {
            char ret;
            switch (ch)
            {
                case 'A': ret = 'B'; break;
                case 'B': ret = 'C'; break;
                case 'C': ret = 'D'; break;
                case 'D': ret = 'E'; break;
                case 'E': ret = 'F'; break;
                case 'F': ret = 'G'; break;
                case 'G': ret = 'H'; break;
                case 'H': ret = 'I'; break;
                case 'I': ret = 'J'; break;
                case 'J': ret = 'K'; break;
                case 'K': ret = 'L'; break;
                case 'L': ret = 'M'; break;
                case 'M': ret = 'N'; break;
                case 'N': ret = 'O'; break;
                case 'O': ret = 'P'; break;
                case 'P': ret = 'Q'; break;
                case 'Q': ret = 'R'; break;
                case 'R': ret = 'S'; break;
                case 'S': ret = 'T'; break;
                case 'T': ret = 'U'; break;
                case 'U': ret = 'V'; break;
                case 'V': ret = 'W'; break;
                case 'W': ret = 'X'; break;
                case 'X': ret = 'Y'; break;
                case 'Y': ret = 'Z'; break;
                default: ret = 'Z';break;
            }
            
            return ret;
        }

        /// <summary>
        /// Cierra la edicion en memoria y guarda en disco
        /// </summary>
        /// <returns></returns>
        public string CloseAndSave()
        {
            var tempfn = Path.GetTempFileName();
            _document.Close();
            var temp = File.Open(tempfn, FileMode.Create);
            _stream.CopyTo(temp);
            temp.Close();
            return tempfn;
        }
        public MemoryStream Close()
        {
            _document.Close();
            _stream.Position = 0;
            return _stream;
        }
    }

	public static class Stream
	{
		private const int BufferSize = 32768;
		public static void CopyTo(this System.IO.Stream input, System.IO.Stream output)
		{
			input.Position = 0;
			var buffer = new byte[BufferSize];
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, read);
			}
		}


	}
}
