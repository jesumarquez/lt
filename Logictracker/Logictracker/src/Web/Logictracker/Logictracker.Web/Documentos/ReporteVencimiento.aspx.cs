using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using System.Linq;
using System.Web.UI;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.ValueObjects.Documentos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Documentos
{
    public partial class ReporteVencimiento : SecuredGridReportPage<ReporteVencimientoVo>
    {
        protected override string GetRefference() { return "VENCIMIENTO_DOC"; }
        protected override string VariableName { get { return "DOC_REP_VENCIMIENTO"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (IsPostBack) return;

            dtFecha.SetDate();
        }

        protected override Empresa GetEmpresa()
        {
            return (cbLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(cbLocacion.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (cbPlanta != null && cbPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(cbPlanta.Selected) : null;
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            int dias;
            if (!int.TryParse(txtDiasAviso.Text, out dias))
            {
                dias = 30;
                txtDiasAviso.Text = dias.ToString("#0");
            }
            var data = new SearchData
                           {
                               Empresa = cbLocacion.Selected,
                               Linea = cbPlanta.Selected,
                               Transportista = cbTransportista.Selected,
                               TiposDocumento = cbTipoDocumento.SelectedValues.ToArray(),
                               Fecha = dtFecha.SelectedDate.GetValueOrDefault(),
                               DiasAviso = dias,
                               SoloConAviso = chkConAviso.Checked
                           };
            data.Save(ViewState);
            base.BtnSearchClick(sender, e);
        }

        protected override List<ReporteVencimientoVo> GetResults()
        {
            //var empresas = new List<int>();
            //var lineas = new List<int>();
            var data = SearchData.Load(ViewState);
            //if(data.Empresa > 0)empresas.Add(data.Empresa);
            //if(data.Linea > 0)lineas.Add(data.Linea);

            //if(data.Empresa <= 0 && data.Linea <= 0)
            //{
            //    empresas.AddRange((from Empresa e in DAOFactory.EmpresaDAO.GetList() select e.Id).ToList());
            //    lineas.AddRange((from Linea l in DAOFactory.LineaDAO.GetList(new[]{-1}) select l.Id).ToList());
            //}

            var list = DAOFactory.DocumentoDAO.FindByTipo(data.TiposDocumento, new List<int> { data.Empresa }, new List<int> { data.Linea }, new List<int> { data.Transportista });
        
            if (data.SoloConAviso)
                return (from Documento d in list 
                        where d.Vencimiento.HasValue && d.Vencimiento.Value.Subtract(data.Fecha).TotalDays < data.DiasAviso
                        orderby d.Vencimiento
                        select new ReporteVencimientoVo(d, data.Fecha)).ToList();

            return (from Documento d in list orderby !d.Vencimiento.HasValue, d.Vencimiento select new ReporteVencimientoVo(d, data.Fecha)).ToList();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteVencimientoVo dataItem)
        {
            var data = SearchData.Load(ViewState);
            if (dataItem.DiasAlVencimiento == 9999) GridUtils.GetCell(e.Row, ReporteVencimientoVo.IndexDiasAlVencimiento).Text = "";
            else if (dataItem.DiasAlVencimiento < 0) e.Row.BackColor = Color.Red;
            else if (dataItem.DiasAlVencimiento < data.DiasAviso) e.Row.BackColor = Color.Yellow;
            else e.Row.BackColor = Color.Green;
        }

        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            if (cbTipoDocumento.SelectedValues.Contains(0)) cbTipoDocumento.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base: {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0},", reporte);
            sDescription.AppendFormat("Tipo de Documento: {0}, ", cbTipoDocumento.SelectedStringValues);

            return sDescription.ToString();
        }

        protected override DateTime GetSinceDateTime()
        {
            return dtFecha.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override List<int> GetDocumentsList()
        {
            if (cbTipoDocumento.SelectedValues.Contains(0)) cbTipoDocumento.ToogleItems();
            return cbTipoDocumento.SelectedValues;
        }

        [Serializable]
        private class SearchData
        {
            public int Empresa;
            public int Linea;
            public int Transportista;
            public DateTime Fecha;
            public int[] TiposDocumento;
            public int DiasAviso;
            public bool SoloConAviso;

            public void Save(StateBag viewstate)
            {
                viewstate["SearchData"] = this;
            }
            public static SearchData Load(StateBag viewstate)
            {
                return viewstate["SearchData"] as SearchData;
            }
        }
    }
}
