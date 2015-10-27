using System;
using System.Collections.Generic;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class LineaTelefonicaImport : SecuredImportPage
    {
        private const string Reporte = "Procesado: {0} de {1}<br/> Importados: {2} ({3}%)<br/>Erroneos: {4} ({5}%)<br/>Sin Importar: {6} ({7}%)<br/>Tiempo: {8}<br/>";
        private const string Result = "{0};{1};{2};{3}<br/>";

        protected override bool Redirect { get { return false; } }

        protected override string RedirectUrl { get { return "LineaTelefonicaLista.aspx"; } }

        protected override string VariableName { get { return "PAR_LINEA_TELEFONICA"; } }

        protected override string GetRefference() { return "PAR_LINEA_TELEFONICA"; }

        protected override List<FieldValue> GetMappingFields() { return Fields.List; }

        protected override void ValidateMapping()
        {
            var fields = GetMappingFields();
            for(var i = 1; i < fields.Count; i++)
            {
                var field = fields[i];
                if(!IsMapped(field.Value)) throw new ApplicationException("Falta mapear " + field.Name);
            }
        }

        protected override bool ValidateRow(ImportRow row)
        {
            ValidateDouble(row[GetColumnByValue(Fields.NumeroLinea.Value)], "NUMERO_LINEA");            
            ValidateDouble(row[GetColumnByValue(Fields.Imei.Value)], "SIM");

            return true;
        }

        protected override void Import(List<ImportRow> rows)
        {
            var procesados = 0;
            var erroneos = 0;
            var sinProcesar = 0;
            var index = 0;
            var start = DateTime.Now.Ticks;
            var list = new List<LineaTelefonica>(rows.Count);

            foreach (var row in rows)
            {
                try
                {
                    index++;
                    var numeroLinea = row[GetColumnByValue(Fields.NumeroLinea.Value)];
                    var imei = row[GetColumnByValue(Fields.Imei.Value)];
                    
                    var linea = new LineaTelefonica { NumeroLinea = numeroLinea,
                                                      Imei = imei,
                                                      Baja = false };

                    if (!DAOFactory.LineaTelefonicaDAO.IsImeiUnique(0, imei))
                    {
                        sinProcesar++;
                        Log("SIM DUPLICADO", index, linea);
                    }
                    else
                    {
                        if (FaltanDatos(linea))
                        {
                            sinProcesar++;
                            Log("FALTAN DATOS", index, linea);
                        }
                        else
                            list.Add(linea);
                    }
                }
                catch (Exception)
                {
                    erroneos++;
                    Log("FILA ERRONEA", index);
                }
            }

            foreach (var linea in list)
            {
                try
                {
                    DAOFactory.LineaTelefonicaDAO.SaveOrUpdate(linea);
                    procesados++;                    
                }
                catch (Exception)
                {
                    erroneos++;
                    Log("FILA ERRONEA", index, linea);
                }
            }
            
            const int totalWidth = 200;
            var percent = index*100/rows.Count;
            litProgress.Text = string.Format(@"<div style='margin: auto; border: solid 1px #999999; background-color: #FFFFFF; width: {0}px; height: 10px;'>
                <div style='background-color: #0000AA; width: {1}px; height: 10px; font-size: 8px; color: #CCCCCC;'>{2}%</div>
                </div>", totalWidth, percent * totalWidth / 100, percent);
            lblDirs.Text = string.Format(Reporte,
                                         procesados + erroneos,
                                         rows.Count,
                                         procesados,
                                         rows.Count > 0 ? (procesados * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         erroneos,
                                         rows.Count > 0 ? (erroneos * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         sinProcesar,
                                         rows.Count > 0 ? (sinProcesar * 100.0 / rows.Count).ToString("0.00") : "0.00",
                                         TimeSpan.FromTicks(DateTime.Now.Ticks - start));
            panelProgress.Visible = true;
        }

        private static bool FaltanDatos(LineaTelefonica linea)
        {
            return (linea.NumeroLinea == null || linea.Imei == string.Empty);
        }

        private void Log(string status, int index)
        {
            Log(status, index, null);
        }

        private void Log(string status, int index, LineaTelefonica linea)
        {
            lblResult.Text += string.Format(Result, 
                                            new[] { status, 
                                                    " INDICE: " + index, 
                                                    " Linea = " + ((linea != null && linea.NumeroLinea != null) ? linea.NumeroLinea : "VACIO"),
                                                    " SIM = " + ((linea != null && linea.Imei != null) ? linea.Imei : "VACIO")
                                                  });
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue NumeroLinea = new FieldValue("NumeroLinea");
            public static readonly FieldValue Imei = new FieldValue("Imei");
            
            public static List<FieldValue> List
            {
                get { return new List<FieldValue> { NumeroLinea,
                                                    Imei }; }
            }
        }

        #endregion
    }
}