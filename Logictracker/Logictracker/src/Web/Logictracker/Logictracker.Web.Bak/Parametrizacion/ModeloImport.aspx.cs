using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ModeloImport : SecuredImportPage
    {
        private const string Reporte = "Procesado: {0} de {1}<br/> Importados: {2} ({3}%)<br/>Erroneos: {4} ({5}%)<br/>Sin Importar: {6} ({7}%)<br/>Tiempo: {8}<br/>";
        private const string Result = "{0};{1};{2};{3};{4}<br/>";

        protected override bool Redirect { get { return false; } }
        protected override string RedirectUrl { get { return "ModeloLista.aspx"; } }
        protected override string VariableName { get { return "PAR_MODELOS"; } }
        protected override string GetRefference() { return "PAR_MODELOS"; }

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
            ValidateInt32(row[GetColumnByValue(Fields.Costo.Value)], "COSTO");
            ValidateInt32(row[GetColumnByValue(Fields.VidaUtil.Value)], "VIDA_UTIL");
            ValidateDouble(row[GetColumnByValue(Fields.Capacidad.Value)], "CAPACIDAD");            
            ValidateDouble(row[GetColumnByValue(Fields.Rendimiento.Value)], "RENDIMIENTO");
            ValidateInt32(row[GetColumnByValue(Fields.Codigo.Value)], "CODE");
            ValidateEmpty(row[GetColumnByValue(Fields.Marca.Value)], "PARENTI06");
            ValidateEmpty(row[GetColumnByValue(Fields.Descripcion.Value)], "DESCRIPCION");
            
            return true;
        }

        protected override void Import(List<ImportRow> rows)
        {
            var procesados = 0;
            var erroneos = 0;
            var sinProcesar = 0;
            var index = 0;
            var start = DateTime.Now.Ticks;
            var list = new List<Modelo>(rows.Count);

            foreach (var row in rows)
            {
                try
                {
                    index++;
                    var costo = Convert.ToInt32(row[GetColumnByValue(Fields.Costo.Value)]);
                    var vidaUtil = Convert.ToInt32(row[GetColumnByValue(Fields.VidaUtil.Value)]);
                    var rendimiento = Convert.ToDouble(row[GetColumnByValue(Fields.Rendimiento.Value)]);
                    var capacidad = Convert.ToDouble(row[GetColumnByValue(Fields.Capacidad.Value)]);
                    var descripcion = row[GetColumnByValue(Fields.Descripcion.Value)];
                    var codigo = row[GetColumnByValue(Fields.Codigo.Value)];
                    var marcaDescripcion = row[GetColumnByValue(Fields.Marca.Value)];
                    var empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                    var linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    var marca = DAOFactory.MarcaDAO.GetByDescripcion((empresa != null ? empresa.Id : 0), (linea != null ? linea.Id : 0), marcaDescripcion);
                    
                    var modelo = new Modelo { Costo = costo,
                                              VidaUtil = vidaUtil,
                                              Rendimiento = rendimiento,
                                              Capacidad = capacidad,
                                              Descripcion = descripcion,
                                              Codigo = codigo,
                                              Marca = marca,
                                              Empresa = empresa,
                                              Linea = linea,
                                              Baja = false };
                    
                    if (FaltanDatos(modelo))
                    {
                        sinProcesar++;
                        Log("FALTAN DATOS", index, modelo);
                    }
                    else
                        list.Add(modelo);
                }
                catch (Exception)
                {
                    erroneos++;
                    Log("FILA ERRONEA", index);
                }
            }

            foreach (var mod in list)
            {
                try
                {
                    DAOFactory.ModeloDAO.SaveOrUpdate(mod);                   
                    procesados++;
                }
                catch (Exception)
                {
                    erroneos++;
                    Log("FILA ERRONEA", index, mod);
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

        private static bool FaltanDatos(Modelo modelo)
        {
            return (modelo.Marca == null || modelo.Descripcion == string.Empty);
        }

        private void Log(string status, int index)
        {
            Log(status, index, null);
        }

        private void Log(string status, int index, Modelo mod)
        {
            lblResult.Text += string.Format(Result,
                                            new[]
                                                {
                                                    status,
                                                    " INDICE: " + index,
                                                    " Marca = " +
                                                    ((mod != null && mod.Marca != null) ? mod.Marca.ToString() : "VACIO")
                                                });
        }

        #region SubClasses

        private static class Fields
        {
            public static readonly FieldValue Marca = new FieldValue("Marca");
            public static readonly FieldValue Descripcion = new FieldValue("Descripcion");
            public static readonly FieldValue Codigo = new FieldValue("Código");
            public static readonly FieldValue Rendimiento = new FieldValue("Rendimiento");
            public static readonly FieldValue Capacidad = new FieldValue("Capacidad");
            public static readonly FieldValue Costo = new FieldValue("Costo");
            public static readonly FieldValue VidaUtil = new FieldValue("Vida Util");
            
            public static List<FieldValue> List
            {
                get { return new List<FieldValue> { Marca,
                                                    Descripcion,
                                                    Codigo,
                                                    Rendimiento,
                                                    Capacidad,
                                                    Costo,
                                                    VidaUtil}; }
            }
        }

        #endregion
    }
}