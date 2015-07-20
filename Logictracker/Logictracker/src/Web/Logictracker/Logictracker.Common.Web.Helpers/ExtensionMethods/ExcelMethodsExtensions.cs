using System;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Logictracker.Web.Helpers.ExtensionMethods
{
    public static class ExcelExtensions
    {
        private static int InsertSharedStringItem(this WorkbookPart wbPart, string value)
        {
            var index = 0;
            var stringTablePart = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault() ??
                                  wbPart.AddNewPart<SharedStringTablePart>();

            var stringTable = stringTablePart.SharedStringTable ?? new SharedStringTable();

            foreach (var item in stringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == value) return index;
                index++;
            }


            stringTable.AppendChild(new SharedStringItem(new Text(value)));
            // Se graba al final
            //stringTable.Save();

            return index;
        }


        private static Cell CreateCell(this Row row, String address)
        {
            var refCell = row.Elements<Cell>().FirstOrDefault(cell => String.Compare(cell.CellReference.Value, address, StringComparison.OrdinalIgnoreCase) > 0);

            // Cells must be in sequential order according to CellReference. 
            // Determine where to insert the new cell.

            var cellResult = new Cell { CellReference = address };

            row.InsertBefore(cellResult, refCell);

            return cellResult;
        }

        private static Row GetRow(this SheetData wsData, UInt32 rowIndex)
        {
            var row = wsData.Elements<Row>().FirstOrDefault(r => r.RowIndex.Value == rowIndex);

            if (row == null)
            {
                row = new Row { RowIndex = rowIndex };
                wsData.Append(new OpenXmlElement[] { row });
            }
            return row;
        }

        public static UInt32 GetRowIndex(string address)
        {
            UInt32 result = 0;

            for (var i = 0; i < address.Length; i++)
            {
                UInt32 l;

                if (!UInt32.TryParse(address.Substring(i, 1), out l)) continue;

                var rowPart = address.Substring(i, address.Length - i);

                if (!UInt32.TryParse(rowPart, out l)) continue;

                result = l;

                break;
            }

            return result;
        }

        private static Cell InsertCellInWorksheet(this Worksheet ws, string addressName)
        {
            var sheetData = ws.GetFirstChild<SheetData>();

            var rowNumber = GetRowIndex(addressName);
            var row = sheetData.GetRow(rowNumber);

            addressName = addressName.Replace("$", "");
            // If the cell you need already exists, return it.
            // If there is not a cell with the specified column name, insert one.  
            var refCell = row.Elements<Cell>().FirstOrDefault(c => c.CellReference.Value == addressName);
            return refCell ?? row.CreateCell(addressName);
        }


        public static void UpdateValue(this WorkbookPart wbPart, string sheetName, string addressName, string value, UInt32Value styleIndex, CellValues dataType)
        {
            //   if (string.IsNullOrEmpty(value)) return;
            //throw new NullReferenceException("value is empty.");

            if (addressName.IndexOf('!') != -1)
            {
                sheetName = addressName.Split('!')[0];
                addressName = addressName.Split('!')[1];
            }
            // Assume failure.

            var sheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);

            if (sheet == null) return;

            var ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;
            var cell = ws.InsertCellInWorksheet(addressName);

            switch (dataType)
            {
                case CellValues.SharedString:
                    if (cell.DataType != null && cell.DataType == CellValues.SharedString)
                    {
                        // Tengo que remover el anterior; al office no le gustan las huerfanas.
                        var oldIdex = int.Parse(cell.CellValue.Text);
                        cell.CellValue.Text = "-1";
                        wbPart.RemoveSharedStringItem(oldIdex);
                        ws.Save();
                    }

                    var stringIndex = wbPart.InsertSharedStringItem(value);
                    cell.CellValue = new CellValue(stringIndex.ToString(CultureInfo.InvariantCulture));
                    cell.DataType = CellValues.SharedString;
                    //        System.Diagnostics.Debug.WriteLine(String.Format("{0} {1} {2}" , cell.CellValue.Text , cell ));
                    break;
                case CellValues.Boolean:
                case CellValues.Number:
                case CellValues.Error:
                case CellValues.String:
                case CellValues.InlineString:
                case CellValues.Date:
                    goto default;
                default:
                    cell.CellValue = new CellValue(value);
                    break;
            }

            cell.DataType = new EnumValue<CellValues>(dataType);

            if (styleIndex > 0)
                cell.StyleIndex = styleIndex;

            // Save the worksheet.
            //ws.Save();
        }

        private static void RemoveSharedStringItem(this WorkbookPart wbPart, int shareStringId)
        {
            var shareStringTablePart = wbPart.SharedStringTablePart;

            // No hay un Tabla de string 
            if (shareStringTablePart == null)
            {
                return;
            }

            // Alguna celda en alguna ws hace referencia ?
            var isReferenced = wbPart.GetPartsOfType<WorksheetPart>().Select(part => part.Worksheet)
                .All(worksheet => !worksheet.GetFirstChild<SheetData>().Descendants<Cell>()
                    .Any(cell => cell.DataType != null && cell.DataType.Value == CellValues.SharedString && cell.CellValue.Text == shareStringId.ToString(CultureInfo.InvariantCulture)));

            if (isReferenced) return;

            // Busco el item
            var item = shareStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(shareStringId);

            if (item == null) return;

            item.Remove();

            // Refresh all the shared string references.
            foreach (var worksheet in wbPart.GetPartsOfType<WorksheetPart>().Select(part => part.Worksheet))
            {
                var cells = worksheet.GetFirstChild<SheetData>().Descendants<Cell>()
                    .Where(c => c.DataType != null && c.DataType.Value == CellValues.SharedString)
                    ;

                foreach (var cell in cells)
                {
                    var itemIndex = int.Parse(cell.CellValue.Text);
                    if (itemIndex > shareStringId)
                    {
                        cell.CellValue.Text = (itemIndex - 1).ToString(CultureInfo.InvariantCulture);
                    }
                }
                worksheet.Save();
            }
            wbPart.SharedStringTablePart.SharedStringTable.Save();
        }

        /// <summary>
        /// Devuelve la columna
        /// </summary>
        /// <param name="excelData">hoja!$A$1</param>
        /// <returns>A</returns>
        public static string ParseExcelColumn(string excelData)
        {
            if (excelData.IndexOf('!') != -1)
            {
                excelData = excelData.Split('!')[1];
            }
            excelData = excelData.Replace("$", "");
            var cIdx = excelData.First(char.IsDigit);
            return excelData.Substring(0, excelData.IndexOf(cIdx));
        }

        /// <summary>
        /// Devuelve la fila
        /// </summary>
        /// <param name="excelData">hoja!$A$1</param>
        /// <returns>1</returns>

        public static uint ParseExcelRow(string excelData)
        {
            if (excelData.IndexOf('!') != -1)
            {
                excelData = excelData.Split('!')[1];
            }
            excelData = excelData.Replace("$", "");
            return GetRowIndex(excelData);
        }

        public static string GetDefinedName(this WorkbookPart wbPart, string name)
        {
            var namedRange = wbPart.Workbook.Descendants<DefinedName>().SingleOrDefault(e => string.Equals(e.Name, name));
            if (namedRange == null)
                throw new Exception(string.Format("No se encuentra {0} en el template", name));
            return namedRange.InnerXml;
        }

        public static uint GetStyle(this WorkbookPart wbPart, string sheetName, string address)
        {
            var cell = wbPart.GetCell(sheetName, address);
            return cell == null || cell.StyleIndex == null ? 0 : cell.StyleIndex;
        }

        public static Cell GetCell(this WorkbookPart wbPart, string sheetName, string address)
        {
            var sheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);

            if (sheet == null) return null;

            var ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;

            var sheetData = ws.GetFirstChild<SheetData>();

            var rowNumber = GetRowIndex(address);
            var row = GetRow(sheetData, rowNumber);

            return
                row.Elements<Cell>().FirstOrDefault(c => c.CellReference != null && c.CellReference.Value == address);
        }


        public static bool HasConditionals(this WorkbookPart wbPart, string sheetName, string address)
        {
            var sheet = wbPart.Workbook.Descendants<Sheet>().First(s => s.Name == sheetName);
            var ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;
            return ws.Descendants<ConditionalFormatting>().Any(e => e.SequenceOfReferences.InnerText == address);
        }

        public static void UpdateConditionalOver(this WorkbookPart wbPart, string sheetName, string start, string newDef)
        {
            var sheet = wbPart.Workbook.Descendants<Sheet>().First(s => s.Name == sheetName);
            var ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;
            var k = ws.Descendants<ConditionalFormatting>().First(e => e.SequenceOfReferences.InnerText == start);
            k.SequenceOfReferences = new ListValue<StringValue> { InnerText = newDef };

        }

    }
}
