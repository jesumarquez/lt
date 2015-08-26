using System;
using System.Web.UI;
using Logictracker.Configuration;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BaseControls;
using Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces;

namespace Logictracker.App_Controls
{
    public partial class EditEntityGeoRef : BaseUserControl
    {
        public int TipoReferenciaGeograficaId
        {
            get { return cbTipoReferenciaGeografica.Selected; }
        }
        public Poligono Poligono
        {
            get { return EditGeoRef1.Poligono; }
        }
        public Direccion Direccion
        {
            get { return EditGeoRef1.Direccion; }
        }
        public int IconId
        {
            get { return SelectIcon2.Selected; }
        }
        public string Color
        {
            get { return cpColor.Color; }
        }
        public string ParentControls
        {
            get { return (string)ViewState["ParentControls"]; }
            set { ViewState["ParentControls"] = SelectIcon2.ParentControls = value; }
        }
        protected int IdGeoRef
        {
            get { return (int)(ViewState["IdGeoRef"] ?? -1); }
            set { ViewState["IdGeoRef"] = value; }
        }
        public bool EditMode { get { return IdGeoRef > 0; } }
        private ReferenciaGeografica editObject;
        protected ReferenciaGeografica EditObject
        {
            get
            {
                if (editObject == null)
                    editObject = EditMode ? DAOFactory.ReferenciaGeograficaDAO.FindById(IdGeoRef) : new ReferenciaGeografica { Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                return editObject;
            }
        }
        protected int IdEmpresa
        {
            get { return (int)(ViewState["IdEmpresa"] ?? -1); }
            set { ViewState["IdEmpresa"] = value; }
        }
        protected int IdLinea
        {
            get { return (int)(ViewState["IdLinea"] ?? -1); }
            set { ViewState["IdLinea"] = value; }
        }

        protected int SelectedId
        {
            get { return (int)(ViewState["Selected"] ?? -1); }
            set { ViewState["Selected"] = value; }
        }

        private void LoadParents()
        {
            if (string.IsNullOrEmpty(ParentControls)) return;

            foreach (var parent in ParentControls.Split(','))
            {
                var control = FindParent(parent.Trim(), Page.Controls);
                if (control == null) continue;
                var iab = control as IAutoBindeable;
                var ipc = control as IParentControl;
                if (iab != null)
                {
                    if (iab.Type == typeof(Empresa)) IdEmpresa = iab.Selected;
                    else if (iab.Type == typeof(Linea)) IdLinea = iab.Selected;
                }
                else if (ipc != null)
                {
                    foreach (var bindeable in ipc.ParentControls)
                    {
                        if (bindeable.Type == typeof(Empresa)) IdEmpresa = bindeable.Selected;
                        else if (bindeable.Type == typeof(Linea)) IdLinea = bindeable.Selected;
                    }
                }
            }
        }

        private static Control FindParent(string parent, ControlCollection controls)
        {
            if (controls == null) return null;

            foreach (Control control in controls)
            {
                if (!string.IsNullOrEmpty(control.ID) && control.ID.Equals(parent)) return control;
                var cnt = FindParent(parent, control.Controls);
                if (cnt != null) return cnt;
            }
            return null;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            cbTipoReferenciaGeografica.ParentControls = ParentControls;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoadParents();
            base.OnLoad(e);
        }
        protected void SelectIcon2_SelectedIconChange(object sender, EventArgs e)
        {
            var icono = DAOFactory.IconoDAO.FindById(SelectIcon2.Selected);
            EditGeoRef1.SetIcono(Config.Directory.IconDir + icono.PathIcono, icono.Width, icono.Height, icono.OffsetX, icono.OffsetY);
        }

        protected void cpColor_ColorChanged(object sender, EventArgs e)
        {
            var color = new RGBColor { HexValue = cpColor.Color };
            EditGeoRef1.Color = color.Color;
        }
        protected void btBorrarPoly_Click(object sender, EventArgs e)
        {
            EditGeoRef1.BorrarGeocerca();
        }

        protected void btBorrarPunto_Click(object sender, EventArgs e)
        {
            EditGeoRef1.BorrarDireccion();
        }
        protected void Bind(ReferenciaGeografica georef)
        {
            IdGeoRef = georef != null ? georef.Id : -1;

            if (georef != null && georef.Icono != null)
            {
                SelectIcon2.Selected = georef.Icono.Id;
                EditGeoRef1.SetIcono(Config.Directory.IconDir + georef.Icono.PathIcono, georef.Icono.Width, georef.Icono.Height, georef.Icono.OffsetX, georef.Icono.OffsetY);
            }
            else SelectIcon2.Selected = -1;
            if (georef != null && georef.Color != null)
            {
                cpColor.Color = georef.Color.HexValue;
                EditGeoRef1.Color = georef.Color.Color;
            }
            EditGeoRef1.Poligono = georef != null ? georef.Poligono : null;
            EditGeoRef1.Direccion = georef != null ? georef.Direccion : null;

            if (georef != null) cbTipoReferenciaGeografica.SetSelectedValue(georef.TipoReferenciaGeografica.Id);
        }
        public void SetLinea(int idLinea)
        {
            if (!IsPostBack && idLinea > 0)
            {
                var linea = DAOFactory.LineaDAO.FindById(idLinea);
                if (linea.ReferenciaGeografica != null && linea.ReferenciaGeografica.Direccion != null)
                    EditGeoRef1.SetCenter(linea.ReferenciaGeografica.Direccion.Latitud,
                                          linea.ReferenciaGeografica.Direccion.Longitud);
            }
        }

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
            updColor.Update();
            updTipoGeoRef.Update();
            updSelectIcon.Update();
        }
        public void SetReferencia(ReferenciaGeografica georef)
        {
            Bind(georef);
        }
        public ReferenciaGeografica GetNewGeoRefference()
        {
            if (!EditMode)
            {
                EditObject.Linea = IdLinea > 0 ? DAOFactory.LineaDAO.FindById(IdLinea) : null;
                EditObject.Empresa = EditObject.Linea != null
                                               ? EditObject.Linea.Empresa
                                               : IdEmpresa > 0 ? DAOFactory.EmpresaDAO.FindById(IdEmpresa) : null;
            }
            EditObject.TipoReferenciaGeografica = DAOFactory.TipoReferenciaGeograficaDAO.FindById(cbTipoReferenciaGeografica.Selected);

            if (!EditMode) EditObject.Descripcion = EditGeoRef1.Direccion != null ? EditGeoRef1.Direccion.Descripcion : "Sin Descripcion";
            EditObject.Icono = IconId > 0 ? DAOFactory.IconoDAO.FindById(IconId) : null;
            EditObject.Color.HexValue = Color;
            EditObject.InhibeAlarma = EditObject.TipoReferenciaGeografica.InhibeAlarma;
            EditObject.EsInicio = EditObject.TipoReferenciaGeografica.EsInicio;
            EditObject.EsIntermedio = EditObject.TipoReferenciaGeografica.EsIntermedio;
            EditObject.EsFin = EditObject.TipoReferenciaGeografica.EsFin;

            var lastDir = EditObject.Direccion;
            var newDir = EditGeoRef1.Direccion;

            var lastPol = EditObject.Poligono;
            var newPol = EditGeoRef1.Poligono;

            var now = txtVigenciaDesde.SelectedDate.HasValue ? txtVigenciaDesde.SelectedDate.Value : DateTime.UtcNow;
            var ChangedPolygon = EditObject.Poligono == null ? EditGeoRef1.Poligono != null : !EditObject.Poligono.Equals(EditGeoRef1.Poligono);
            var ChangedDireccion = EditObject.Direccion == null ? EditGeoRef1.Direccion != null : !EditObject.Direccion.Equals(EditGeoRef1.Direccion);

            if (ChangedDireccion || ChangedPolygon)
            {
                if (lastPol != null && lastPol.Vigencia == null) lastPol.Vigencia = new Vigencia();
                if (lastDir != null && lastDir.Vigencia == null) lastDir.Vigencia = new Vigencia();
                if (ChangedPolygon && lastPol != null) lastPol.Vigencia.Fin = now;
                if (ChangedDireccion && lastDir != null) lastDir.Vigencia.Fin = now;

                var hist = EditObject.GetHistoria(now);
                if (hist != null && hist.Vigencia == null) hist.Vigencia = new Vigencia();
                if (hist != null) hist.Vigencia.Fin = now;

                if (ChangedDireccion && EditObject.Direccion != null)
                {
                    if (EditObject.Direccion.Vigencia == null) EditObject.Direccion.Vigencia = new Vigencia();
                    EditObject.Direccion.Vigencia.Fin = now;
                }
                if (ChangedPolygon && EditObject.Poligono != null)
                {
                    if (EditObject.Poligono.Vigencia == null) EditObject.Poligono.Vigencia = new Vigencia();
                    EditObject.Poligono.Vigencia.Fin = now;
                }


                var newDireccion = EditObject.Direccion;
                var newPoligono = EditObject.Poligono;
                if (ChangedDireccion)
                {
                    if (newDir == null) newDireccion = null;
                    else
                    {
                        newDireccion = new Direccion { Vigencia = new Vigencia { Inicio = now } };
                        newDireccion.CloneData(newDir);
                    }
                }
                if (ChangedPolygon)
                {
                    if (newPol == null) newPoligono = null;
                    else
                    {
                        newPoligono = new Poligono { Vigencia = new Vigencia { Inicio = now } };
                        newPoligono.AddPoints(newPol.ToPointFList());
                        newPoligono.Radio = newPol.Radio;
                    }
                }

                if (newDireccion == null && newPoligono == null) return null;

                EditObject.AddHistoria(newDireccion, newPoligono, now);
            }
            else if (EditObject.Direccion == null && EditObject.Poligono == null) return null;

            if (EditObject.TipoReferenciaGeografica.ControlaVelocidad)
            {
                EditObject.VelocidadesMaximas.Clear();

                foreach (TipoReferenciaVelocidad maxima in EditObject.TipoReferenciaGeografica.VelocidadesMaximas)
                {
                    var gv = new ReferenciaVelocidad
                                 {
                                     ReferenciaGeografica = EditObject,
                                     TipoVehiculo = maxima.TipoVehiculo,
                                     VelocidadMaxima = maxima.VelocidadMaxima
                                 };

                    EditObject.VelocidadesMaximas.Add(gv);
                }
            }

            return EditObject;

        }
    }
}