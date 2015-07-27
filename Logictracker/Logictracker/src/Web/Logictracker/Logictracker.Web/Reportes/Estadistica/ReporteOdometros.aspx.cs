using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesOdometrosReporteOdometros : SecuredGridReportPage<OdometroStatusVo>
    {
        protected override string GetRefference() { return "REPORTE_ODOMETROS"; }
        protected override string VariableName { get { return "REPORTE_ODOMETROS"; } }
        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocation.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected) : null;
        }
        
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        protected override List<OdometroStatusVo> GetResults()
        {
            ToogleItems(lbMovil);
            ToogleItems(lbOdometro);
            var inicio = DateTime.UtcNow;
 
            var vehiculos = lbMovil.SelectedValues;
            if (chkConDispositivo.Checked)
            {
                vehiculos = (from v in vehiculos
                             let ve = DAOFactory.CocheDAO.FindById(v)
                             where ve.Dispositivo != null
                             select v).ToList();
            }

            var results = ReportFactory.OdometroStatusDAO.FindByVehiculosAndOdometros(vehiculos, lbOdometro.SelectedValues, chkVencimiento.Checked);
			var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

            var ret = (from OdometroStatus m in results select new OdometroStatusVo(m)).ToList();

            if (lbOdometro.SelectedValues.Contains(-10))
            {
                var coches = DAOFactory.CocheDAO.GetList(new[] {ddlLocation.Selected}, new[] {ddlPlanta.Selected})
                                                .Where(c => vehiculos.Contains(c.Id));

                ret.AddRange(coches.Select(coche => new OdometroStatusVo(coche)));
            }

            STrace.Trace("Reporte de Odometros", String.Format("Reporte: Duración de la consulta: {0} segundos", duracion));

            return ret;
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            var dic = new Dictionary<string, string>();
            var sMoviles = new StringBuilder();
            var sOdometros = new StringBuilder();

            foreach (var movil in lbMovil.SelectedValues)
            {
                if (!sMoviles.ToString().Equals(""))
                    sMoviles.Append(",");

                sMoviles.Append(movil.ToString("#0"));
            }
            foreach (var odometro in lbOdometro.SelectedValues)
            {
                if (!sOdometros.ToString().Equals(""))
                    sOdometros.Append(",");

                sOdometros.Append(odometro.ToString("#0"));
            }

            dic.Add("MOVILES", sMoviles.ToString());
            dic.Add("ODOMETROS", sOdometros.ToString());
            dic.Add("VENCIMIENTO", chkVencimiento.Checked.ToString());
            
            return dic;
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, OdometroStatusVo dataItem)
        {
            if (dataItem == null) return;

            var col = Color.FromArgb(100, dataItem.Red, dataItem.Green, dataItem.Blue);

            e.Row.BackColor = col;
            e.Row.ForeColor = col.GetBrightness() > 0.45 ? Color.Black : Color.White;

            grid.Columns[OdometroStatusVo.IndexKmTotales].Visible = lbOdometro.SelectedValues.Contains(-10);
            if (lbOdometro.Items.Count == 1 || (lbOdometro.SelectedValues.Count == 1 && lbOdometro.SelectedValues.Contains(-10)))
            {
                grid.Columns[OdometroStatusVo.IndexKilometrosReferencia].Visible = false;
                grid.Columns[OdometroStatusVo.IndexKilometrosFaltantes].Visible = false;
                grid.Columns[OdometroStatusVo.IndexTiempoReferencia].Visible = false;
                grid.Columns[OdometroStatusVo.IndexTiempoFaltante].Visible = false;
                grid.Columns[OdometroStatusVo.IndexHorasReferencia].Visible = false;
                grid.Columns[OdometroStatusVo.IndexHorasFaltantes].Visible = false;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                 {
                     { CultureManager.GetEntity("PARENTI01"), ddlLocation.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected).RazonSocial : "Todos"},
                     { CultureManager.GetEntity("PARENTI02"), ddlPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected).Descripcion : "Todos"},
                     { CultureManager.GetEntity("PARENTI03"), GetSelectedMovilesDescription(true) }
                 };
        }

        private string GetSelectedOdometrosDescription(bool showDescription)
        {
            var ids = string.Empty;

            if (lbOdometro.GetSelectedIndices().Length == 0) lbOdometro.ToogleItems();

            ids = lbOdometro.GetSelectedIndices().Aggregate(ids, (current, index) => string.Concat(current, string.Format("{0},", showDescription ? lbOdometro.Items[index].Text : lbOdometro.Items[index].Value)));

            return ids.TrimEnd(',');
        }

        private string GetSelectedMovilesDescription(bool showDescription)
        {
            var ids = string.Empty;

            if (lbMovil.GetSelectedIndices().Length == 0) lbMovil.ToogleItems();

            ids = lbMovil.GetSelectedIndices().Aggregate(ids, (current, index) => string.Concat(current, string.Format("{0}, ", showDescription ? lbMovil.Items[index].Text : lbMovil.Items[index].Value)));

            return ids.TrimEnd(',');
        }

        protected override string GetDescription(string reporte)
        {
            var linea = GetLinea();
            if (lbMovil.SelectedValues.Contains(0)) lbMovil.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base: {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Reporte: {0},", reporte);
            sDescription.AppendFormat("Moviles: {0}, ", lbMovil.SelectedStringValues);
            sDescription.AppendFormat("Odometros: {0}, ", lbOdometro.SelectedStringValues);

            return sDescription.ToString();
        }

        protected override int GetCompanyId()
        {
            return GetEmpresa().Id;
        }

        protected override List<int> GetSelectedListByField(string field)
        {
            if ("vehicles".Equals(field))
            {
                if (lbMovil.SelectedValues.Contains(0)) lbMovil.ToogleItems();
                return lbMovil.SelectedValues;                                
            }
            else
            {
                if (lbOdometro.SelectedValues.Contains(0)) lbOdometro.ToogleItems();
                return lbOdometro.SelectedValues;
            }
        }

        protected override string GetSelectedVehicles()
        {
            var sVehiculos = new StringBuilder();

            if (lbMovil.SelectedValues.Contains(0)) lbMovil.ToogleItems();

            foreach (var vehiculo in lbMovil.SelectedValues)
            {
                if (!sVehiculos.ToString().Equals(""))
                    sVehiculos.Append(",");

                sVehiculos.Append(vehiculo.ToString());
            }

            return sVehiculos.ToString();
        }

        protected override string GetOdometerType()
        {
            var sOdometro = new StringBuilder();

            if (lbOdometro.SelectedValues.Contains(0)) lbMovil.ToogleItems();

            foreach (var odom in lbOdometro.SelectedValues)
            {
                if (!sOdometro.ToString().Equals(""))
                    sOdometro.Append(",");

                sOdometro.Append(odom.ToString());
            }

            return sOdometro.ToString();
        }
    }
}
