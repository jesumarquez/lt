#region Usings

using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Combustible
{
    public class CombustiblePresenter: GenericPresenter
    {
        public CombustiblePresenter(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof) : base(tipoDoc, view, daof)
        {
        }

        private int dia = 26;
        protected override void AddParameter(TipoDocumentoParametro par, string id, string style)
        {
            if (dia > 31) dia = 1;
            switch (par.Nombre)
            {
                case CamposCombustible.Dia: 
                    AddLiteral((dia++).ToString()); 
                    break;
                case CamposCombustible.Cliente:
                    AddLabel(id, style);
                    break;
                case CamposCombustible.Actividad:
                    var cb = AddDropDownList(id, style);
                    cb.Items.Clear();
                    cb.Items.Add(new ListItem(" ", ""));
                    cb.Items.Add(new ListItem("PERFORA - REPASA - REPERFORA - CIRCULA - REG VERTICALIDAD", "PC"));
	                cb.Items.Add(new ListItem("REG VERTICALIDAD TOCTO - ENSAYA - SACA SONDEO - BAJA SONDEO - CALIBRA POZO - ARMA PM - COLOCA VASTAGO","MA"));
	                cb.Items.Add(new ListItem("MONTAJE BOP - TEST BOP - VINCULA CSG - TEST MNIFOLD","BP"));
	                cb.Items.Add(new ListItem("PERFILA","LG"));
	                cb.Items.Add(new ListItem("ENTUBA","EN"));
	                cb.Items.Add(new ListItem("ESPERA","ES"));
	                cb.Items.Add(new ListItem("REPARACION MECANICA","RM"));
	                cb.Items.Add(new ListItem("REPARACION OPERATIVA","RO"));
	                cb.Items.Add(new ListItem("MANIOBRA HERRAMIENTA APRISIONADA","AP"));
	                cb.Items.Add(new ListItem("PREPARACION INYECCION - ACUMULA AGUA - PERFILA VAINAS","AC"));
	                cb.Items.Add(new ListItem("DTM","DM"));
	                cb.Items.Add(new ListItem("REPARACION ELECTRICA","RE"));
	                cb.Items.Add(new ListItem("OPERATIVA","OP"));
                    cb.Items.Add(new ListItem("ESPERA POR FUERZA MAYOR-HUELGA-PARO-CORTE DE RUTA", "FM"));
                    break;
                case CamposCombustible.StockDiario:
                case CamposCombustible.TotalCantidad:
                case CamposCombustible.TotalMotores:
                case CamposCombustible.TotalGenerador:
                case CamposCombustible.TotalEgresosPropios:
                case CamposCombustible.TotalEgresosTerceros:
                case CamposCombustible.TotalStockDiario:
                    AddLabel(id, style); 
                    break;
                default: 
                    base.AddParameter(par, id, style);

                    if (par.Nombre == CamposCombustible.Mes)
                        (GetControlFromView(TipoDocumentoHelper.GetControlName(par)) as TextBox).Text = DateTime.Now.Month.ToString();
                    if (par.Nombre == CamposCombustible.Anio)
                        (GetControlFromView(TipoDocumentoHelper.GetControlName(par)) as TextBox).Text = DateTime.Now.Year.ToString();
                    break;
            }
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            var txtFecha = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_FECHA) as TextBox;
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;
            txtCodigo.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");

            SetCliente();

            CreateStockScript();
        }

        private void CreateStockScript()
        {
            var script = view.ClientScript.GetWebResourceUrl(GetType(), "Logictracker.Web.Documentos.Combustible.StockDiario.js");
            view.ClientScript.RegisterStartupScript(typeof(string), "StockDiario", string.Format("<script src='{0}' type='text/javascript'></script>", script), false);

            var stockScript = new StringBuilder();
            stockScript.Append("var stockCombustible;");
            stockScript.Append("Sys.Application.add_init(function () {{ ");
            stockScript.Append("stockCombustible = new StockDiario();");
            

            foreach (TipoDocumentoParametro parametro in TipoDocumento.Parametros)
            {
                switch(parametro.Nombre)
                {
                    case CamposCombustible.StockInicial:
                        stockScript.Append(GetScriptAddInicial("0", GetClientID(parametro)));
                        break;
                    case CamposCombustible.StockDiario:
                        for (short i = 0; i < parametro.Repeticion; i++)
                        {
                            stockScript.Append(GetScriptAddStock(i.ToString(), GetClientID(parametro, i)));
                            //if(i < parametro.Repeticion - 1)
                                stockScript.Append(GetScriptAddInicial((i + 1).ToString(), GetClientID(parametro, i)));
                        }
                        break;
                    case CamposCombustible.TotalStockDiario:
                        stockScript.Append(GetScriptAddStock("31", GetClientID(parametro)));
                        break;
                    case CamposCombustible.Cantidad:
                        for (short i = 0; i < parametro.Repeticion; i++)
                        {
                            stockScript.Append(GetScriptAddIngreso(i.ToString(), GetClientID(parametro, i)));
                            stockScript.Append(GetScriptAddIngreso(CamposCombustible.Cantidad, GetClientID(parametro, i)));
                        }
                        break;
                    case CamposCombustible.TotalCantidad:
                        stockScript.Append(GetScriptAddStock(CamposCombustible.Cantidad, GetClientID(parametro)));
                        break;
                    case CamposCombustible.Motores:
                    case CamposCombustible.Generador:
                    case CamposCombustible.EgresosPropios:
                    case CamposCombustible.EgresosTerceros:
                        for (short i = 0; i < parametro.Repeticion; i++)
                        {
                            stockScript.Append(GetScriptAddEgreso(i.ToString(), GetClientID(parametro, i)));
                            stockScript.Append(GetScriptAddIngreso(parametro.Nombre, GetClientID(parametro, i)));
                        }
                        break;
                    case CamposCombustible.TotalMotores:
                        stockScript.Append(GetScriptAddStock(CamposCombustible.Motores, GetClientID(parametro)));
                        break;
                    case CamposCombustible.TotalGenerador:
                        stockScript.Append(GetScriptAddStock(CamposCombustible.Generador, GetClientID(parametro)));
                        break;
                    case CamposCombustible.TotalEgresosPropios:
                        stockScript.Append(GetScriptAddStock(CamposCombustible.EgresosPropios, GetClientID(parametro)));
                        break;
                    case CamposCombustible.TotalEgresosTerceros:
                        stockScript.Append(GetScriptAddStock(CamposCombustible.EgresosTerceros, GetClientID(parametro)));
                        break;
                }
            }
            stockScript.Append("}});");
            view.RegisterScript("StockValues", stockScript.ToString());
        }
        private string GetClientID(TipoDocumentoParametro parametro)
        {
            return GetControlFromView(TipoDocumentoHelper.GetControlName(parametro)).ClientID;
        }
        private string GetClientID(TipoDocumentoParametro parametro, short repeticion)
        {
            return GetControlFromView(TipoDocumentoHelper.GetControlName(parametro, repeticion)).ClientID;
        }

        private string GetScriptAddInicial(string day, string txt)
        {
            return string.Format("stockCombustible.addInicial('{0}', '{1}');", day, txt);
        }
        private string GetScriptAddIngreso(string day, string txt)
        {
            return string.Format("stockCombustible.addIngreso('{0}', '{1}');", day, txt);
        }
        private string GetScriptAddEgreso(string day, string txt)
        {
            return string.Format("stockCombustible.addEgreso('{0}', '{1}');", day, txt);
        }
        private string GetScriptAddStock(string day, string txt)
        {
            return string.Format("stockCombustible.addStock('{0}', '{1}');", day, txt);
        }

        protected override void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.cbEmpresa_SelectedIndexChanged(sender, e);
            SetCliente();
        }
        protected override void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.cbLinea_SelectedIndexChanged(sender, e);
            SetCliente();
        }
        protected override void cbEquipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            base.cbEquipo_SelectedIndexChanged(sender, e);
            SetCliente();
        }

        private void SetCliente()
        {
            var cbCliente = GetControlFromView(TipoDocumentoHelper.GetControlName(TipoDocumentoHelper.GetByName(CamposCombustible.Cliente))) as Label;
            cbCliente.Text = string.Empty;

            var cbEquipo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_PARENTI19) as DropDownList;
            if (cbEquipo.SelectedIndex < 0) return;
            var equipo = DAOFactory.EquipoDAO.FindById(Convert.ToInt32(cbEquipo.SelectedValue));
            
            cbCliente.Text = equipo.Cliente.Descripcion;
        }

        protected override void SetParameterValue(DocumentoValor val)
        {
            if(val.Parametro.Nombre == CamposCombustible.Actividad)
            {
                var id = (val.Parametro.Repeticion != 1)
                         ? TipoDocumentoHelper.GetControlName(val.Parametro, val.Repeticion)
                         : TipoDocumentoHelper.GetControlName(val.Parametro);
                var control = GetControlFromView(id);
                if (control == null) return;
                SetDropDownValue(control, val.Valor);
            }
            else base.SetParameterValue(val);
        }
    }
}
