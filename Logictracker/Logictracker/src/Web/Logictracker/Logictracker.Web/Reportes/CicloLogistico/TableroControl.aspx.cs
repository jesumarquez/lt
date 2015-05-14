using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class TableroControl : SecuredBaseReportPage<string>
    {
        protected override string VariableName { get { return "TABLERO_CONTROL_DISTRIBUCION"; } }
        protected override string GetRefference() { return "TABLERO_CONTROL_DISTRIBUCION"; }
        protected override InfoLabel LblInfo { get { return null; } }
        protected override bool CsvButton { get { return false; } }
        protected override bool ExcelButton { get { return false; } }
        protected override void ExportToCsv() { }
        protected override void ExportToExcel() { }
        protected override bool HideSearch { get { return true; } }

        private const double PorcCumplimiento = 100.0;
        private static readonly Color ColorOk = Color.YellowGreen;
        private static readonly Color ColorNotOk = Color.Tomato;

        public int Empresa
        {
            get { return (ViewState["Empresa"] != null) ? Convert.ToInt32(ViewState["Empresa"]) : 0; } 
            set { ViewState["Empresa"] = value; }
        }
        public int Linea
        {
            get { return (ViewState["Linea"] != null) ? Convert.ToInt32(ViewState["Linea"]) : 0; }
            set { ViewState["Linea"] = value; }
        }
        public int Departamento
        {
            get { return (ViewState["Departamento"] != null) ? Convert.ToInt32(ViewState["Departamento"]) : 0; }
            set { ViewState["Departamento"] = value; }
        }
        public DateTime Desde
        {
            get { return (ViewState["Desde"] != null) ? Convert.ToDateTime(ViewState["Desde"]) : DateTime.MinValue; }
            set { ViewState["Desde"] = value; }
        }
        public DateTime Hasta
        {
            get { return (ViewState["Hasta"] != null) ? Convert.ToDateTime(ViewState["Hasta"]) : DateTime.MinValue; }
            set { ViewState["Hasta"] = value; }
        }
        public int Dias
        {
            get { return (ViewState["Dias"] != null) ? Convert.ToInt32(ViewState["Dias"]) : 0; }
            set { ViewState["Dias"] = value; }
        }
        public int DiasHistorico
        {
            get { return (ViewState["DiasHistorico"] != null) ? Convert.ToInt32(ViewState["DiasHistorico"]) : 0; }
            set { ViewState["DiasHistorico"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtDesde.SelectedDate = DateTime.UtcNow.Date;
                dtHasta.SelectedDate = DateTime.UtcNow.Date.AddDays(1).AddMinutes(-1);
                pnlCc.Visible = false;
                pnlSubcc.Visible = false;
                pnlVehiculos.Visible = false;
            }
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            Empresa = ddlEmpresa.Selected;
            Linea = ddlPlanta.Selected;
            Departamento = ddlDepartamento.Selected;
            Desde = dtDesde.SelectedDate.Value;
            Hasta = dtHasta.SelectedDate.Value;
            Dias = GetDiasDeSemana(dtDesde.SelectedDate.Value, dtHasta.SelectedDate.Value);
            DiasHistorico = GetDiasDeSemana(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

            var ccDataSource = new List<List<string>>(); 
            var centros = DAOFactory.CentroDeCostosDAO.GetList(new[] {Empresa},
                                                               new[] {Linea},
                                                               new[] {Departamento});
            
            foreach (var cc in centros)
            {
                var subCentros = DAOFactory.SubCentroDeCostosDAO.GetList(new[] {Empresa},
                                                                         new[] {Linea},
                                                                         new[] {Departamento},
                                                                         new[] {cc.Id});
                var cantSubCentros = subCentros.Count();
                var objetivoTotal = (from subcc in subCentros
                                     let vehiculos = DAOFactory.CocheDAO.GetList(new[] {Empresa},
                                                                                 new[] {Linea}, 
                                                                                 new[] {-1}, // TIPO VEHICULO
                                                                                 new[] {-1}, // TRANSPORTISTA
                                                                                 new[] {Departamento}, 
                                                                                 new[] {cc.Id},
                                                                                 new[] {subcc.Id})
                                     select (subcc.Objetivo * vehiculos.Count * Dias)).Sum();

                var entregados = DAOFactory.EntregaDistribucionDAO.GetList(new[] {Empresa},
                                                                           new[] {Linea},
                                                                           new[] {-1}, // TRANSPORTISTAS
                                                                           new[] {Departamento},
                                                                           new[] {cc.Id},
                                                                           new[] {-1}, // SUBCC
                                                                           new[] {-1}, // VEHICULOS
                                                                           new[] {-1}, // VIAJES
                                                                           new[] {(int)EntregaDistribucion.Estados.Completado},
                                                                           Desde.ToDataBaseDateTime(),
                                                                           Hasta.ToDataBaseDateTime()).Count;
                var ccRegister = new List<string>
                                     {
                                         cc.Id.ToString("#0"),
                                         cc.Descripcion,
                                         cantSubCentros.ToString("#0"),
                                         objetivoTotal.ToString("#0"),
                                         entregados.ToString("#0"),
                                         objetivoTotal > 0 ? ((double)entregados/(double)objetivoTotal*100.0).ToString("#0.00") : "0,00"
                                     };
                ccDataSource.Add(ccRegister);
            }

            gridCentrosDeCostos.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI37");
            gridCentrosDeCostos.Columns[1].HeaderText = CultureManager.GetMenu("SUBCENTROS_COSTOS");
            gridCentrosDeCostos.Columns[2].HeaderText = CultureManager.GetLabel("OBJETIVO");
            gridCentrosDeCostos.Columns[3].HeaderText = CultureManager.GetLabel("REALIZADO");
            gridCentrosDeCostos.Columns[4].HeaderText = "%";
            gridCentrosDeCostos.DataSource = ccDataSource;
            gridCentrosDeCostos.DataBind();
            gridCentrosDeCostos.SelectedIndex = -1;
            pnlCc.Visible = true;
            pnlSubcc.Visible = false;
            pnlVehiculos.Visible = false;
        }

        protected void GridCentroDeCostosOnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var list = e.Row.DataItem as List<string>;
                if (list != null && list.Count == 6)
                {
                    int idCc;
                    if (!int.TryParse(list[0], out idCc) || idCc <= 0) return;

                    var lblCentroDeCosto = e.Row.FindControl("lblCentroDeCosto") as LinkButton;
                    if (lblCentroDeCosto != null)
                    {
                        lblCentroDeCosto.Text = list[1];
                        lblCentroDeCosto.Attributes.Add("IdCC", idCc.ToString("#0"));
                    }

                    var lblCantSubCentros = e.Row.FindControl("lblCantSubCentros") as Label;
                    if (lblCantSubCentros != null) lblCantSubCentros.Text = list[2];

                    var lblObjetivo = e.Row.FindControl("lblObjetivo") as Label;
                    if (lblObjetivo != null) lblObjetivo.Text = list[3];

                    var lblRealizado = e.Row.FindControl("lblRealizado") as Label;
                    if (lblRealizado != null) lblRealizado.Text = list[4];

                    var lblPorcRealizado = e.Row.FindControl("lblPorcRealizado") as Label;
                    if (lblPorcRealizado != null) lblPorcRealizado.Text = list[5] + "%";

                    double porc;
                    if (double.TryParse(list[5], out porc) && porc >= PorcCumplimiento)
                        e.Row.BackColor = ColorOk;
                    else
                        e.Row.BackColor = ColorNotOk;
                }
            }
        }

        protected void LblCentroDeCostoOnClick(object sender, EventArgs e)
        {
            var lbl = sender as LinkButton;
            if (lbl == null) return;
            int idCc;
            if (!int.TryParse(lbl.Attributes["IdCC"], out idCc) || idCc <= 0) return;
            var cc = DAOFactory.CentroDeCostosDAO.FindById(idCc);
            if (cc == null) return;

            var sccDataSource = new List<List<string>>();
            var subcentros = DAOFactory.SubCentroDeCostosDAO.GetList(new[] { Empresa },
                                                                     new[] { Linea },
                                                                     new[] { Departamento },
                                                                     new[] { idCc });
            
            foreach (var scc in subcentros)
            {
                var vehiculos = DAOFactory.CocheDAO.GetList(new[] { Empresa },
                                                            new[] { Linea },
                                                            new[] { -1 }, // TIPO VEHICULO
                                                            new[] { -1 }, // TRANSPORTISTA
                                                            new[] { Departamento },
                                                            new[] { idCc },
                                                            new[] { scc.Id });
                var cantVehiculos = vehiculos.Count();
                var objetivoTotal = scc.Objetivo * vehiculos.Count * Dias;
                var entregados = DAOFactory.EntregaDistribucionDAO.GetList(new[] {Empresa},
                                                                           new[] {Linea},
                                                                           new[] {-1}, // TRANSPORTISTAS
                                                                           new[] {Departamento},
                                                                           new[] {idCc},
                                                                           new[] {scc.Id},
                                                                           new[] {-1}, // VEHICULOS
                                                                           new[] {-1}, // VIAJES,
                                                                           new[] {(int)EntregaDistribucion.Estados.Completado},
                                                                           Desde.ToDataBaseDateTime(),
                                                                           Hasta.ToDataBaseDateTime()).Count;

                var objetivoHistorico = scc.Objetivo * vehiculos.Count * DiasHistorico;
                var entregadosHistorico = DAOFactory.EntregaDistribucionDAO.GetList(new[] {Empresa},
                                                                                    new[] {Linea},
                                                                                    new[] {-1}, // TRANSPORTISTAS
                                                                                    new[] {Departamento},
                                                                                    new[] {idCc},
                                                                                    new[] {scc.Id},
                                                                                    new[] {-1}, // VEHICULOS
                                                                                    new[] {-1}, // VIAJES,
                                                                                    new[] {(int)EntregaDistribucion.Estados.Completado},
                                                                                    DateTime.UtcNow.AddDays(-30).ToDataBaseDateTime(),
                                                                                    DateTime.UtcNow.ToDataBaseDateTime()).Count;

                var sccRegister = new List<string>
                                      {
                                          scc.Id.ToString("#0"),
                                          scc.Descripcion,
                                          cantVehiculos.ToString("#0"),
                                          objetivoTotal.ToString("#0"),
                                          entregados.ToString("#0"),
                                          objetivoTotal > 0 ? ((double) entregados/(double) objetivoTotal*100.0).ToString("#0.00") : "0,00",
                                          objetivoHistorico > 0 ? ((double) entregadosHistorico/(double) objetivoHistorico*100.0).ToString("#0.00") : "0,00"
                                      };
                sccDataSource.Add(sccRegister);
            }

            gridSubCentrosDeCostos.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI99");
            gridSubCentrosDeCostos.Columns[1].HeaderText = CultureManager.GetMenu("PAR_VEHICULOS");
            gridSubCentrosDeCostos.Columns[2].HeaderText = CultureManager.GetLabel("OBJETIVO");
            gridSubCentrosDeCostos.Columns[3].HeaderText = CultureManager.GetLabel("REALIZADO");
            gridSubCentrosDeCostos.Columns[4].HeaderText = "%";
            gridSubCentrosDeCostos.Columns[5].HeaderText = "% " + CultureManager.GetLabel("HISTORICO");
            gridSubCentrosDeCostos.DataSource = sccDataSource;
            gridSubCentrosDeCostos.DataBind();
            gridSubCentrosDeCostos.SelectedIndex = -1;
            pnlSubcc.Visible = true;
            pnlVehiculos.Visible = false;
        }

        protected void GridSubCentroDeCostosOnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var list = e.Row.DataItem as List<string>;
                if (list != null && list.Count == 7)
                {
                    int idScc;
                    if (!int.TryParse(list[0], out idScc) || idScc <= 0) return;

                    var lblSubCentroDeCosto = e.Row.FindControl("lblSubCentroDeCosto") as LinkButton;
                    if (lblSubCentroDeCosto != null)
                    {
                        lblSubCentroDeCosto.Text = list[1];
                        lblSubCentroDeCosto.Attributes.Add("IdSCC", idScc.ToString("#0"));
                    }

                    var lblCantVehiculos = e.Row.FindControl("lblCantVehiculos") as Label;
                    if (lblCantVehiculos != null) lblCantVehiculos.Text = list[2];

                    var lblObjetivo = e.Row.FindControl("lblObjetivo") as Label;
                    if (lblObjetivo != null) lblObjetivo.Text = list[3];

                    var lblRealizado = e.Row.FindControl("lblRealizado") as Label;
                    if (lblRealizado != null) lblRealizado.Text = list[4];

                    var lblPorcRealizado = e.Row.FindControl("lblPorcRealizado") as Label;
                    if (lblPorcRealizado != null) lblPorcRealizado.Text = list[5] + "%";

                    var lblPorcHistorico = e.Row.FindControl("lblPorcHistorico") as Label;
                    if (lblPorcHistorico != null) lblPorcHistorico.Text = list[6] + "%";

                    double porc;
                    if (double.TryParse(list[5], out porc) && porc >= PorcCumplimiento)
                        e.Row.BackColor = ColorOk;
                    else
                        e.Row.BackColor = ColorNotOk;
                }
            }
        }

        protected void LblSubCentroDeCostoOnClick(object sender, EventArgs e)
        {
            var lbl = sender as LinkButton;
            if (lbl == null) return;
            int idScc;
            if (!int.TryParse(lbl.Attributes["IdSCC"], out idScc) || idScc <= 0) return;
            var scc = DAOFactory.SubCentroDeCostosDAO.FindById(idScc);
            if (scc == null) return;

            var vDataSource = new List<List<string>>();
            var vehiculos = DAOFactory.CocheDAO.GetList(new[] {Empresa},
                                                        new[] {Linea},
                                                        new[] {-1}, // TIPO VEHICULO
                                                        new[] {-1}, // TRANSPORTISTA
                                                        new[] {Departamento},
                                                        new[] {scc.CentroDeCostos.Id},
                                                        new[] {idScc});

            foreach (var veh in vehiculos)
            {
                var objetivoTotal = scc.Objetivo * Dias;
                var entregados = DAOFactory.EntregaDistribucionDAO.GetList(new[] {Empresa},
                                                                           new[] {Linea},
                                                                           new[] {-1}, // TRANSPORTISTAS
                                                                           new[] {Departamento},
                                                                           new[] {scc.CentroDeCostos.Id},
                                                                           new[] {scc.Id},
                                                                           new[] {veh.Id},
                                                                           new[] {-1}, // VIAJES
                                                                           new[] {(int)EntregaDistribucion.Estados.Completado},
                                                                           Desde.ToDataBaseDateTime(),
                                                                           Hasta.ToDataBaseDateTime()).Count;
                var diasHistorico = GetDiasDeSemana(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
                var objetivoHistorico = scc.Objetivo * diasHistorico;
                var entregadosHistorico = DAOFactory.EntregaDistribucionDAO.GetList(new[] {Empresa},
                                                                                    new[] {Linea},
                                                                                    new[] {-1}, // TRANSPORTISTAS
                                                                                    new[] {Departamento},
                                                                                    new[] {scc.CentroDeCostos.Id},
                                                                                    new[] {scc.Id},
                                                                                    new[] {veh.Id},
                                                                                    new[] {-1}, // VIAJES
                                                                                    new[] {(int)EntregaDistribucion.Estados.Completado},
                                                                                    DateTime.UtcNow.AddDays(-30).ToDataBaseDateTime(),
                                                                                    DateTime.UtcNow.ToDataBaseDateTime()).Count;

                var vRegister = new List<string>
                                      {
                                          veh.Interno,
                                          objetivoTotal.ToString("#0"),
                                          entregados.ToString("#0"),
                                          objetivoTotal > 0 ? ((double) entregados/(double) objetivoTotal*100.0).ToString("#0.00") : "0,00",
                                          objetivoHistorico > 0 ? ((double) entregadosHistorico/(double) objetivoHistorico*100.0).ToString("#0.00") : "0,00"
                                      };
                vDataSource.Add(vRegister);
            }

            gridVehiculos.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI03");
            gridVehiculos.Columns[1].HeaderText = CultureManager.GetLabel("OBJETIVO");
            gridVehiculos.Columns[2].HeaderText = CultureManager.GetLabel("REALIZADO");
            gridVehiculos.Columns[3].HeaderText = "%";
            gridVehiculos.Columns[4].HeaderText = "% " + CultureManager.GetLabel("HISTORICO");
            gridVehiculos.DataSource = vDataSource;
            gridVehiculos.DataBind();
            gridVehiculos.SelectedIndex = -1;
            pnlVehiculos.Visible = true;
        }

        protected void GridVehiculosOnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var list = e.Row.DataItem as List<string>;
                if (list != null && list.Count == 5)
                {
                    var lblVehiculo = e.Row.FindControl("lblVehiculo") as Label;
                    if (lblVehiculo != null) lblVehiculo.Text = list[0];

                    var lblObjetivo = e.Row.FindControl("lblObjetivo") as Label;
                    if (lblObjetivo != null) lblObjetivo.Text = list[1];

                    var lblRealizado = e.Row.FindControl("lblRealizado") as Label;
                    if (lblRealizado != null) lblRealizado.Text = list[2];

                    var lblPorcRealizado = e.Row.FindControl("lblPorcRealizado") as Label;
                    if (lblPorcRealizado != null) lblPorcRealizado.Text = list[3] + "%";

                    var lblPorcHistorico = e.Row.FindControl("lblPorcHistorico") as Label;
                    if (lblPorcHistorico != null) lblPorcHistorico.Text = list[4] + "%";

                    double porc;
                    if (double.TryParse(list[3], out porc) && porc >= PorcCumplimiento)
                        e.Row.BackColor = ColorOk;
                    else
                        e.Row.BackColor = ColorNotOk;
                }
            }
        }

        protected override List<string> GetResults() { return new List<string>(); }

        private static int GetDiasDeSemana(DateTime start, DateTime end)
        {
            var thisDate = start.Date;
            var finalDate = end.Date;
            var weekDays = 0;
            while (thisDate <= finalDate)
            {
                if (thisDate.DayOfWeek != DayOfWeek.Saturday && thisDate.DayOfWeek != DayOfWeek.Sunday) 
                    weekDays++;
                
                thisDate = thisDate.AddDays(1);
            }

            return weekDays;
        }
    }
}
