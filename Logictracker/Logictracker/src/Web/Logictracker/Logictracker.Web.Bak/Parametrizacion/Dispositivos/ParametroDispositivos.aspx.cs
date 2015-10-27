using System;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.NHibernate;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;
using System.Collections.Generic;

namespace Logictracker.Parametrizacion.Dispositivos
{
    public partial class ParametroDispositivosAlta : SecuredAbmPage<Dispositivo>
    {
        protected override string VariableName { get { return "PAR_DET_DISPOSITIVOS"; } }
        protected override string RedirectUrl { get { return "ParametroDispositivos.aspx"; } }
        protected override string GetRefference() { return "DET_DISPO"; }

        protected override bool AddButton { get { return false; } }
        protected override bool ListButton { get { return false; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void cbTipoDispositivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tipoDispositivo = DAOFactory.TipoDispositivoDAO.FindById(cbTipoDispositivo.Selected);
            grid.DataSource = tipoDispositivo.TiposParametro.Cast<TipoParametroDispositivo>().OrderBy(t=>t.Nombre);
            grid.DataBind();
        }

        protected void grid_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var tipoParametro = e.Row.DataItem as TipoParametroDispositivo;
            var txtValor = e.Row.FindControl("txtValor") as TextBox;
            var chkEdit = e.Row.FindControl("chkEdit") as CheckBox;

            if (tipoParametro.Editable)
            {
                txtValor.Attributes.Add("disabled", "true");

                var idtxt = txtValor.ClientID;
                chkEdit.Attributes.Add("onchange", string.Format("var ch=this.getElementsByTagName('input')[0]; $get('{0}').disabled = !ch.checked;", idtxt));
            }
            else
            {
                txtValor.Enabled = false;
                chkEdit.Visible = false;
                e.Row.ForeColor = Color.Gray;
            }
        }

        protected void cbDispositivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dispositivos = cbDispositivo.SelectedValues.Where(id => id > 0).Select(id => DAOFactory.DispositivoDAO.FindById(id));
            
            foreach (C1GridViewRow row in grid.Rows)
            {
                var parametro = DAOFactory.TipoParametroDispositivoDAO.FindById(Convert.ToInt32(grid.DataKeys[row.RowIndex].Value));
                var txtValor = row.FindControl("txtValor") as TextBox;
                var chkEdit = row.FindControl("chkEdit") as CheckBox;
                txtValor.Text = string.Empty;
                var valores = (from d in dispositivos
                               let detalle = d.DetallesDispositivo.Cast<DetalleDispositivo>().Where(det => det.TipoParametro.Id == parametro.Id).FirstOrDefault()
                               select detalle != null ? detalle.Valor : string.Empty);
                var refValue = valores.FirstOrDefault();
                if (valores.All(v => v == refValue))
                {
                    txtValor.Text = refValue ?? parametro.ValorInicial;
                    chkEdit.Checked = true;
                    chkEdit.Attributes.Add("checked", "true");
                    txtValor.Attributes.Remove("disabled");
                }  
                else
                {
                    chkEdit.Checked = false;
                    chkEdit.Attributes.Add("checked", "false");
                    txtValor.Attributes.Add("disabled", "false");
                }
            }
        }

        #region Overrides of SecuredAbmPage<Dispositivo>

        

        protected override void Bind(){}

        protected override void OnDelete() {}

        protected override void OnSave()
        {
            var rebootDevices = new List<Dispositivo>();
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    var dispositivos = cbDispositivo.SelectedValues.Where(id => id > 0).Select(id => DAOFactory.DispositivoDAO.FindById(id));

                    foreach (var dispositivo in dispositivos)
                    {
                        var reboot = false;

                        foreach (C1GridViewRow row in grid.Rows)
                        {
                            var chkEdit = row.FindControl("chkEdit") as CheckBox;
                            if (!chkEdit.Checked) continue;

                            var parametro = DAOFactory.TipoParametroDispositivoDAO.FindById(Convert.ToInt32(grid.DataKeys[row.RowIndex].Value));
                            var txtValor = row.FindControl("txtValor") as TextBox;

                            var valor = txtValor.Text;

                            var detalle =
                                dispositivo.DetallesDispositivo.Cast<DetalleDispositivo>().Where(det => det.TipoParametro.Id == parametro.Id).FirstOrDefault();

                            if (detalle == null)
                            {
                                detalle = new DetalleDispositivo {Dispositivo = dispositivo, Revision = dispositivo.MaxRevision + 1, TipoParametro = parametro};
                                dispositivo.AddDetalleDispositivo(detalle);
                            }

                            reboot |= parametro.RequiereReset && detalle.Valor != valor;
                            detalle.Valor = valor;
                        }
                        DAOFactory.DispositivoDAO.SaveOrUpdate(dispositivo);

                        if (reboot) rebootDevices.Add(dispositivo);
                    }

                    transaction.Commit();

                    foreach (var device in rebootDevices)
                    {
                        MessageSender.CreateReboot(device, null).Send();
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            cbDispositivo_SelectedIndexChanged(null, EventArgs.Empty);
        }

        #endregion
    }
}
