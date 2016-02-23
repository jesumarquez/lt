using C1.Web.UI.Controls.C1GridView;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Logictracker.Web.Reportes.CicloLogistico
{
    public partial class ProyeccionStock : SecuredBaseReportPage<ProyeccionStock.StockProyectado>
    {
        protected override string GetRefference() { return "REP_PROYECCION_STOCK"; }
        protected override string VariableName { get { return "REP_PROYECCION_STOCK"; } }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool ExcelButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }
        protected override bool HideSearch { get { return true; } }
        protected override void ExportToCsv() { }
        protected override void ExportToExcel() { }
        protected override List<StockProyectado> GetResults() { return new List<StockProyectado>(); }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            gridStock.Visible = false;
        }

        protected void FiltersSelectedIndexChanged(object sender, EventArgs e)
        {
            gridStock.Visible = false;
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {   
            gridStock.Visible = true;
            var resultados = new List<StockProyectado>();
            var desde = DateTime.Today.ToDataBaseDateTime();
            var hasta = desde.AddDays(11);

            var stocks = DAOFactory.StockVehicularDAO.GetList(cbEmpresa.SelectedValues, cbZona.SelectedValues, cbTipoVehiculo.SelectedValues);
            var viajes = DAOFactory.ViajeDistribucionDAO.GetList(cbEmpresa.Selected, desde, hasta);

            foreach (var stock in stocks)
            {
                // DIA 0
                var lleganDia0 = viajes.Count(v => v.Fin >= desde && v.Fin <= desde.AddDays(1)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia0 = viajes.Count(v => v.Inicio >= desde && v.Inicio <= desde.AddDays(1)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia0 = stock.Detalles.Count + lleganDia0 - salenDia0;
                // DIA 1
                var lleganDia1 = viajes.Count(v => v.Fin >= desde.AddDays(1) && v.Fin <= desde.AddDays(2)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia1 = viajes.Count(v => v.Inicio >= desde.AddDays(1) && v.Inicio <= desde.AddDays(2)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia1 = dia0 + lleganDia1 - salenDia1;
                // DIA 2
                var lleganDia2 = viajes.Count(v => v.Fin >= desde.AddDays(2) && v.Fin <= desde.AddDays(3)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia2 = viajes.Count(v => v.Inicio >= desde.AddDays(2) && v.Inicio <= desde.AddDays(3)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia2 = dia1 + lleganDia2 - salenDia2;
                // DIA 3
                var lleganDia3 = viajes.Count(v => v.Fin >= desde.AddDays(3) && v.Fin <= desde.AddDays(4)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia3 = viajes.Count(v => v.Inicio >= desde.AddDays(3) && v.Inicio <= desde.AddDays(4)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia3 = dia2 + lleganDia3 - salenDia3;
                // DIA 4
                var lleganDia4 = viajes.Count(v => v.Fin >= desde.AddDays(4) && v.Fin <= desde.AddDays(5)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia4 = viajes.Count(v => v.Inicio >= desde.AddDays(4) && v.Inicio <= desde.AddDays(5)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia4 = dia3 + lleganDia4 - salenDia4;
                // DIA 5
                var lleganDia5 = viajes.Count(v => v.Fin >= desde.AddDays(5) && v.Fin <= desde.AddDays(6)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia5 = viajes.Count(v => v.Inicio >= desde.AddDays(5) && v.Inicio <= desde.AddDays(6)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia5 = dia4 + lleganDia5 - salenDia5;
                // DIA 6
                var lleganDia6 = viajes.Count(v => v.Fin >= desde.AddDays(6) && v.Fin <= desde.AddDays(7)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia6 = viajes.Count(v => v.Inicio >= desde.AddDays(6) && v.Inicio <= desde.AddDays(7)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia6 = dia5 + lleganDia6 - salenDia6;
                // DIA 7
                var lleganDia7 = viajes.Count(v => v.Fin >= desde.AddDays(7) && v.Fin <= desde.AddDays(8)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia7 = viajes.Count(v => v.Inicio >= desde.AddDays(7) && v.Inicio <= desde.AddDays(8)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia7 = dia6 + lleganDia7 - salenDia7;
                // DIA 8
                var lleganDia8 = viajes.Count(v => v.Fin >= desde.AddDays(8) && v.Fin <= desde.AddDays(9)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia8 = viajes.Count(v => v.Inicio >= desde.AddDays(8) && v.Inicio <= desde.AddDays(9)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia8 = dia7 + lleganDia8 - salenDia8;
                // DIA 9
                var lleganDia9 = viajes.Count(v => v.Fin >= desde.AddDays(9) && v.Fin <= desde.AddDays(10)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia9 = viajes.Count(v => v.Inicio >= desde.AddDays(9) && v.Inicio <= desde.AddDays(10)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia9 = dia8 + lleganDia9 - salenDia9;
                // DIA 10
                var lleganDia10 = viajes.Count(v => v.Fin >= desde.AddDays(10) && v.Fin <= desde.AddDays(11)
                                                && v.TipoCoche == stock.TipoCoche
                                                && stock.Zona.Referencias.Contains(v.Detalles.Last().ReferenciaGeografica));
                var salenDia10 = viajes.Count(v => v.Inicio >= desde.AddDays(10) && v.Inicio <= desde.AddDays(11)
                                               && v.TipoCoche == stock.TipoCoche
                                               && stock.Zona.Referencias.Contains(v.Detalles.First().ReferenciaGeografica));
                var dia10 = dia9 + lleganDia10 - salenDia10;
                
                var result = new StockProyectado
                {
                    Zona = stock.Zona.Descripcion,
                    TipoVehiculo = stock.TipoCoche.Descripcion,
                    Dia1 = dia1,
                    Dia2 = dia2,
                    Dia3 = dia3,
                    Dia4 = dia4,
                    Dia5 = dia5,
                    Dia6 = dia6,
                    Dia7 = dia7,
                    Dia8 = dia8,
                    Dia9 = dia9,
                    Dia10 = dia10
                };

                resultados.Add(result);
            }

            gridStock.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI89");
            gridStock.Columns[1].HeaderText = CultureManager.GetEntity("PARENTI17");
            gridStock.Columns[2].HeaderText = desde.AddDays(1).ToString("dd/MM");
            gridStock.Columns[3].HeaderText = desde.AddDays(2).ToString("dd/MM");
            gridStock.Columns[4].HeaderText = desde.AddDays(3).ToString("dd/MM");
            gridStock.Columns[5].HeaderText = desde.AddDays(4).ToString("dd/MM");
            gridStock.Columns[6].HeaderText = desde.AddDays(5).ToString("dd/MM");
            gridStock.Columns[7].HeaderText = desde.AddDays(6).ToString("dd/MM");
            gridStock.Columns[8].HeaderText = desde.AddDays(7).ToString("dd/MM");
            gridStock.Columns[9].HeaderText = desde.AddDays(8).ToString("dd/MM");
            gridStock.Columns[10].HeaderText = desde.AddDays(9).ToString("dd/MM");
            gridStock.Columns[11].HeaderText = desde.AddDays(10).ToString("dd/MM");

            for (var i = 0; i < gridStock.Columns.Count; i++)
            {
                gridStock.Columns[i].HeaderStyle.Font.Bold = true;
                gridStock.Columns[i].HeaderStyle.Font.Size = FontUnit.Larger;
                if (i < 2) gridStock.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                else gridStock.Columns[i].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            }

            gridStock.DataSource = resultados.OrderBy(r => r.Zona).ThenBy(r => r.TipoVehiculo);
            gridStock.DataBind();
        }

        protected void GridStockOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var result = e.Row.DataItem as StockProyectado;
                if (result != null)
                {
                    var lbl = e.Row.FindControl("lblZona") as Label;
                    if (lbl != null) lbl.Text = result.Zona;
                    lbl = e.Row.FindControl("lblTipoVehiculo") as Label;
                    if (lbl != null) lbl.Text = result.TipoVehiculo;
                    lbl = e.Row.FindControl("lblDia1") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia1.ToString("#0");
                        lbl.ForeColor = result.Dia1 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia2") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia2.ToString("#0");
                        lbl.ForeColor = result.Dia2 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia3") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia3.ToString("#0");
                        lbl.ForeColor = result.Dia3 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia4") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia4.ToString("#0");
                        lbl.ForeColor = result.Dia4 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia5") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia5.ToString("#0");
                        lbl.ForeColor = result.Dia5 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia6") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia6.ToString("#0");
                        lbl.ForeColor = result.Dia6 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia7") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia7.ToString("#0");
                        lbl.ForeColor = result.Dia7 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia8") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia8.ToString("#0");
                        lbl.ForeColor = result.Dia8 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia9") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia9.ToString("#0");
                        lbl.ForeColor = result.Dia9 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                    lbl = e.Row.FindControl("lblDia10") as Label;
                    if (lbl != null)
                    {
                        lbl.Text = result.Dia10.ToString("#0");
                        lbl.ForeColor = result.Dia10 < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    }
                }
            }
        }
        
        public class StockProyectado
        {
            public string Zona { get; set; }
            public string TipoVehiculo { get; set; }
            public int Dia1 { get; set; }
            public int Dia2 { get; set; }
            public int Dia3 { get; set; }
            public int Dia4 { get; set; }
            public int Dia5 { get; set; }
            public int Dia6 { get; set; }
            public int Dia7 { get; set; }
            public int Dia8 { get; set; }
            public int Dia9 { get; set; }
            public int Dia10 { get; set; }
        }
    }
}
