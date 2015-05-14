using C1.Web.UI.Controls.C1GridView;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionGeoRefAlta : SecuredAbmPage<ReferenciaGeografica>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_POI"; } }
        protected override string RedirectUrl { get { return "GeoRefLista.aspx"; } }
        protected override string GetRefference() { return "DOMICILIO"; }
        protected override bool DeleteButton { get { return UserOk; } }
        protected override bool SaveButton { get { return UserOk; } }

        #endregion

        #region Protected Methods

        private bool? _userok;
        protected bool UserOk
        {
            get
            {
                if (!EditMode) _userok = true;

                if (!_userok.HasValue)
                {
                    var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
                    var pEmpresa = user.Empresas.Count > 0;
                    var pLinea = user.Lineas.Count > 0;
                    var eEmpresa = EditObject.Empresa != null;
                    var eLinea = EditObject.Linea != null;

                    _userok = (!pEmpresa && !pLinea) || (!pLinea && eEmpresa) || (pLinea && eLinea);
                }
                return _userok.Value;
            }
        }

        #region Page Events
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (EditMode)
            {
                cpColor.Color = EditObject.Color.HexValue;
            }
            BindTiposVehiculo();
            GetVelocidadesParent();
            if (!UserOk) ShowInfo("El nivel de acceso de este usuario no permite modificar este objeto");
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (UserOk)
            {
                var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
                cbEmpresa.AddAllItem = user.Empresas.Count == 0;
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsPostBack || cbLinea.Selected <= 0) return;

            var linea = DAOFactory.LineaDAO.FindById(cbLinea.Selected);

            if (linea.ReferenciaGeografica != null && linea.ReferenciaGeografica.Direccion != null)
                EditGeoRef1.SetCenter(linea.ReferenciaGeografica.Direccion.Latitud, linea.ReferenciaGeografica.Direccion.Longitud);
        } 
        
        #endregion

        protected void btBorrarPoly_Click(object sender, EventArgs e)
        {
            EditGeoRef1.BorrarGeocerca();
        }

        protected void btBorrarPunto_Click(object sender, EventArgs e)
        {
            EditGeoRef1.BorrarDireccion();
        }

        protected void SelectIcon2_SelectedIconChange(object sender, EventArgs e)
        {
            EditGeoRef1.SetIcono(SelectIcon2.Selected);
        }

        protected void cpColor_ColorChanged(object sender, EventArgs e)
        {
            var color = new RGBColor { HexValue = cpColor.Color };
            EditGeoRef1.Color = color.Color;
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id:cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoReferenciaGeografica.SetSelectedValue(EditObject.TipoReferenciaGeografica != null ? EditObject.TipoReferenciaGeografica.Id : cbTipoReferenciaGeografica.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtObservaciones.Text = EditObject.Observaciones;

            if (EditObject.Icono != null)
            {
                SelectIcon2.Selected = EditObject.Icono.Id;
                EditGeoRef1.SetIcono(Config.Directory.IconDir + EditObject.Icono.PathIcono, EditObject.Icono.Width, EditObject.Icono.Height, EditObject.Icono.OffsetX, EditObject.Icono.OffsetY);
            }

            if (EditObject.Color != null)
            {
                cpColor.Color = EditObject.Color.HexValue;
                EditGeoRef1.Color = EditObject.Color.Color;
            }

            chkIgnoraLogiclink.Checked = EditObject.IgnoraLogiclink;
            chkControlaVelocidad.Checked = EditObject.TipoReferenciaGeografica.ControlaVelocidad;
            chkControlaES.Checked = EditObject.TipoReferenciaGeografica.ControlaEntradaSalida;
            chkZonaRiesgo.Checked = EditObject.TipoReferenciaGeografica.EsZonaDeRiesgo;
            chkInhibeAlarma.Checked = EditObject.InhibeAlarma;
            chkInicio.Checked = EditObject.EsInicio;
            chkIntermedio.Checked = EditObject.EsIntermedio;
            chkFin.Checked = EditObject.EsFin;
            EditGeoRef1.Poligono = EditObject.Poligono;
            EditGeoRef1.Direccion = EditObject.Direccion;

            if (EditObject.Vigencia != null)
            {
                if (EditObject.Vigencia.Inicio.HasValue) txtFechaDesde.SelectedDate = EditObject.Vigencia.Inicio.Value.ToDisplayDateTime();

                if (EditObject.Vigencia.Fin.HasValue) txtFechaHasta.SelectedDate = EditObject.Vigencia.Fin.Value.ToDisplayDateTime();
            }

            BindVelocidades();

            panelVelocidades.Visible = EditObject.TipoReferenciaGeografica.ControlaVelocidad;
        }

        protected override void ValidateSave()
        {
            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");

            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            if (EditGeoRef1.Direccion == null && EditGeoRef1.Poligono == null) ThrowMustEnter("Entities", "PARENTI05");

            if (EditGeoRef1.Direccion != null && SelectIcon2.Selected <= 0) ThrowMustEnter("ICON");

            //Codigo Unico
            var byCode = DAOFactory.ReferenciaGeograficaDAO.FindByCodigo(cbEmpresa.SelectedValues, 
                                                                         cbLinea.SelectedValues,
                                                                         cbTipoReferenciaGeografica.SelectedValues, 
                                                                         codigo);
            ValidateDuplicated(byCode, "CODE");
        }

        /// <summary>
        /// Saves the Direccion
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = EditObject.Linea != null ? EditObject.Linea.Empresa : cbEmpresa.Selected> 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;

            EditObject.TipoReferenciaGeografica = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoReferenciaGeografica.Selected);
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Observaciones = txtObservaciones.Text.Trim();
            EditObject.Icono = SelectIcon2.Selected > 0 ? DAOFactory.IconoDAO.FindById(SelectIcon2.Selected) : null;
            EditObject.Color.HexValue = cpColor.Color;
            EditObject.InhibeAlarma = chkInhibeAlarma.Checked;
            EditObject.EsInicio = chkInicio.Checked;
            EditObject.EsIntermedio = chkIntermedio.Checked;
            EditObject.EsFin = chkFin.Checked;
            EditObject.IgnoraLogiclink = chkIgnoraLogiclink.Checked;

            if (EditObject.Vigencia == null) EditObject.Vigencia = new Vigencia();

            EditObject.Vigencia.Inicio = txtFechaDesde.SelectedDate.HasValue ? txtFechaDesde.SelectedDate.Value.ToDataBaseDateTime() : DateTime.UtcNow;
            EditObject.Vigencia.Fin = txtFechaHasta.SelectedDate.HasValue ? (DateTime?) txtFechaHasta.SelectedDate.Value.ToDataBaseDateTime() : null;

            var lastDir = EditObject.Direccion;
            var newDir = EditGeoRef1.Direccion;

            var lastPol = EditObject.Poligono;
            var newPol = EditGeoRef1.Poligono;

            var now = txtVigenciaDesde.SelectedDate.HasValue ? txtVigenciaDesde.SelectedDate.Value.ToDataBaseDateTime() : DateTime.UtcNow;

            var changedDir = lastDir == null ? newDir != null : !lastDir.Equals(newDir);
            var changedPol = lastPol == null ? newPol != null : !lastPol.Equals(newPol);

            if (changedDir || changedPol)
            {
                if (lastPol != null && lastPol.Vigencia == null) lastPol.Vigencia = new Vigencia();
                if (lastDir != null && lastDir.Vigencia == null) lastDir.Vigencia = new Vigencia();
                if (changedPol && lastPol != null) lastPol.Vigencia.Fin = now;
                if (changedDir && lastDir != null) lastDir.Vigencia.Fin = now;

                if(changedDir && EditObject.Direccion != null)
                {
                    if (EditObject.Direccion.Vigencia == null) EditObject.Direccion.Vigencia = new Vigencia();

                    EditObject.Direccion.Vigencia.Fin = now;
                }

                if (changedPol && EditObject.Poligono != null)
                {
                    if (EditObject.Poligono.Vigencia == null) EditObject.Poligono.Vigencia = new Vigencia();

                    EditObject.Poligono.Vigencia.Fin = now;
                }

                var newDireccion = EditObject.Direccion;
                var newPoligono = EditObject.Poligono;

                if (changedDir)
                {
                    if (newDir == null) newDireccion = null;
                    else
                    {
                        newDireccion = new Direccion {Vigencia = new Vigencia {Inicio = now}};

                        newDireccion.CloneData(newDir);
                    }
                }

                if (changedPol)
                {
                    if (newPol == null) newPoligono = null;
                    else
                    {
                        newPoligono = new Poligono {Vigencia = new Vigencia {Inicio = now}};
                        newPoligono.AddPoints(newPol.ToPointFList());
                        newPoligono.Radio = newPol.Radio;
                    }
                }

                EditObject.AddHistoria(newDireccion, newPoligono, now);
            }

            if (EditObject.TipoReferenciaGeografica.ControlaVelocidad) AddSpeedLimits();

            DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject);
        }


        #endregion

        protected void cbTipoReferenciaGeografica_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTipoReferenciaGeografica.Selected <= 0) return;

            var tipo = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoReferenciaGeografica.Selected);

            if (tipo.Icono != null)
            {
                SelectIcon2.Selected = tipo.Icono.Id;
                EditGeoRef1.SetIcono(Config.Directory.IconDir + tipo.Icono.PathIcono, tipo.Icono.Width, tipo.Icono.Height, tipo.Icono.OffsetX, tipo.Icono.OffsetY);
            }
            else
            {
                SelectIcon2.Selected = -1;
                EditGeoRef1.ClearIcono();
            }

            if (tipo.Color != null)
            {
                cpColor.Color = tipo.Color.HexValue;
                EditGeoRef1.Color = tipo.Color.Color;
            }

            chkControlaVelocidad.Checked = tipo.ControlaVelocidad;
            chkControlaES.Checked = tipo.ControlaEntradaSalida;
            chkZonaRiesgo.Checked = tipo.EsZonaDeRiesgo;
            chkInhibeAlarma.Checked = tipo.InhibeAlarma;
            chkInicio.Checked = tipo.EsInicio;
            chkIntermedio.Checked = tipo.EsIntermedio;
            chkFin.Checked = tipo.EsFin;

            panelVelocidades.Visible = chkControlaVelocidad.Checked;
            BindTiposVehiculo();
            GetVelocidadesParent();

            updComportamiento.Update();
        }

        #region Delete

        protected override void ValidateDelete()
        {
            if (!DAOFactory.ReferenciaGeograficaDAO.ValidateDelete(EditObject.Id)) throw new ApplicationException(CultureManager.GetError("DELETE_GEO_REF"));
        }

        protected override void OnDelete()
        {
            DAOFactory.ReferenciaGeograficaDAO.DeleteGeoRef(EditObject.Id);
        } 

        #endregion

        #region Velocidades

        /// <summary>
        /// Vehicle types custom item data bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridVelocidades_ItemDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType != C1GridViewRowType.DataRow) return;

            var tipo = e.Row.DataItem as TipoCoche;
            if (tipo == null) return;

            var own = string.Empty;

            if (cbLinea.Selected <= 0)
            {
                own = " (";

                if (cbEmpresa.Selected <= 0) own += (tipo.Empresa != null ? tipo.Empresa.RazonSocial : "Todos") + ", ";

                own += tipo.Linea != null ? tipo.Linea.Descripcion : "Todos";
                own += ")";
            }

            e.Row.Cells[0].Text = tipo.Descripcion + own;
        }

        private void GetVelocidadesParent()
        {
            var tipo = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoReferenciaGeografica.Selected);

            if (!tipo.ControlaVelocidad) return;

            var dic = tipo.VelocidadesMaximas.Cast<TipoReferenciaVelocidad>().ToDictionary(velo => velo.TipoVehiculo.Id, velo => velo.VelocidadMaxima);

            for (var i = 0; i < gridVelocidades.Rows.Count; i++)
            {
                var id = (int)gridVelocidades.DataKeys[i].Value;

                if (!dic.ContainsKey(id)) continue;

                var txt = gridVelocidades.Rows[i].FindControl("txtVelocidadMaxima") as TextBox;

                if (txt != null) txt.Text = dic[id].ToString();
            }
        }

        /// <summary>
        /// Vehicles types data binding.
        /// </summary>
        private void BindTiposVehiculo()
        {
            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);
            var tipos = DAOFactory.TipoCocheDAO.FindByEmpresasAndLineas(cbEmpresa.SelectedValues, cbLinea.SelectedValues, user);

            gridVelocidades.DataSource = tipos;
            gridVelocidades.DataBind();
        }

        /// <summary>
        /// Binds associated speed per vehicle T limits.
        /// </summary>
        private void BindVelocidades()
        {
            if (!EditMode) return;

            var dic = EditObject.VelocidadesMaximas.Cast<ReferenciaVelocidad>().ToDictionary(velo => velo.TipoVehiculo.Id, velo => velo.VelocidadMaxima);

            lblVelocidades.Visible = gridVelocidades.Rows.Count > 0;

            for (var i = 0; i < gridVelocidades.Rows.Count; i++)
            {
                var id = (int)gridVelocidades.DataKeys[i].Value;

                if (!dic.ContainsKey(id)) continue;

                var txt = gridVelocidades.Rows[i].FindControl("txtVelocidadMaxima") as TextBox;

                if (txt != null) txt.Text = dic[id].ToString();
            }
        }

        /// <summary>
        /// Adds speeds limits per vehicle T to the current geocerca.
        /// </summary>
        private void AddSpeedLimits()
        {
            EditObject.VelocidadesMaximas.Clear();

            for (var i = 0; i < gridVelocidades.Rows.Count; i++)
            {
                var id = (int)gridVelocidades.DataKeys[i].Value;
                var txt = gridVelocidades.Rows[i].FindControl("txtVelocidadMaxima") as TextBox;
                var velo = 0;

                if (txt != null && (!int.TryParse(txt.Text, out velo))) continue;

                var gv = new ReferenciaVelocidad { ReferenciaGeografica = EditObject, TipoVehiculo = DAOFactory.TipoCocheDAO.FindById(id), VelocidadMaxima = velo };

                EditObject.VelocidadesMaximas.Add(gv);
            }
        }

        #endregion
    }
}
