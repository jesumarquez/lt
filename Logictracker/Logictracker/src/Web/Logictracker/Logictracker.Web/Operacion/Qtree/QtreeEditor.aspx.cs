using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Qtree;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Base;
using Logictracker.Web.Monitor.ContextMenu;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Culture;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.Operacion.Qtree
{
    public partial class Parametrizacion_QtreeEditor : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }

        protected override string GetRefference() { return "QTREE_EDITOR"; }

        public enum QtreeEditorState { None, Info, Edit, ColorPick, CreatingQtree}

        protected const int MinZoomLevel = 4;
        protected const int InitialZoomLevel = 10;
        protected const int QtreeMinZoomLevel = 11;
        protected static string LayerGeocercas { get; set; }

        protected Bounds Bounds
        {
            get { return ViewState["Bounds"] as Bounds; }
            set { ViewState["Bounds"] = value; }
        }

        protected Bounds NewQtreeBounds
        {
            get { return ViewState["NewQtreeBounds"] as Bounds; }
            set { ViewState["NewQtreeBounds"] = value; }
        }

        protected string QtreeName
        {
            get { return (string) ViewState["QtreeName"]??"qtree"; }
            set { ViewState["QtreeName"] = value;}
        }

        protected string QtreeDirectory 
        { 
            get 
            {
                if (Format == QtreeFormats.Gte) return Config.Qtree.QtreeGteDirectory;
                if (Format == QtreeFormats.Torino) return Config.Qtree.QtreeTorinoDirectory;
                return Config.Qtree.QtreeDirectory;
            } 
        }
        protected string SelectedQtreeDirectory 
        { 
            get 
            {
                if (SelectedFormat == QtreeFormats.Gte) return Config.Qtree.QtreeGteDirectory;
                if (SelectedFormat == QtreeFormats.Torino) return Config.Qtree.QtreeTorinoDirectory;
                return Config.Qtree.QtreeDirectory;
            } 
        }
    

        protected string QtreeFullPath { get { return Path.Combine(QtreeDirectory, QtreeName); } }

        protected QtreeEditorState CurrentState
        {
            get { return (QtreeEditorState)(ViewState["CurrentState"]?? QtreeEditorState.Info); }
            set { ViewState["CurrentState"] = value; }
        }

        protected QtreeEditorState LastState
        {
            get { return (QtreeEditorState)(ViewState["LastState"]?? QtreeEditorState.Info); }
            set { ViewState["LastState"] = value; }
        }

        protected double HorizontalResolution
        {
            get { return (double)(ViewState["HorizontalResolution"] ?? 5600 / 10000000); }
            set { ViewState["HorizontalResolution"] = value; }
        }

        protected double VerticalResolution
        {
            get { return (double)(ViewState["VerticalResolution"] ?? 5600 / 10000000); }
            set { ViewState["VerticalResolution"] = value; }
        }

        protected QtreeFormats Format
        {
            get { return (QtreeFormats)(ViewState["Format"] ?? QtreeFormats.Gte); }
            set { ViewState["Format"] = value; }
        }

        protected QtreeFormats SelectedFormat
        {
            get { return (QtreeFormats) Enum.Parse(typeof(QtreeFormats), cbQtreeFormat.SelectedValue); }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            LayerGeocercas = CultureManager.GetLabel("LAYER_GEOCERCAS");

            InitializeMap();

            if (!IsPostBack)
            {
                RegisterExtJsStyleSheet();
                InitializeDirectories();
                BindFormats();
                BindQtrees();
                InitializeQtree();
            }        
        }

        private void InitializeDirectories()
        {
            if (!Directory.Exists(Config.Qtree.QtreeDirectory)) Directory.CreateDirectory(Config.Qtree.QtreeDirectory);
            if (!Directory.Exists(Config.Qtree.QtreeGteDirectory)) Directory.CreateDirectory(Config.Qtree.QtreeGteDirectory);
            if (!Directory.Exists(Config.Qtree.QtreeTorinoDirectory)) Directory.CreateDirectory(Config.Qtree.QtreeTorinoDirectory);
        }

        private void InitializeMap()
        {
            Monitor.Click += Monitor_Click;
            Monitor.ContextMenuPostback += Monitor_ContextMenuPostback;
            Monitor.MapMove += Monitor_MapMove;
            Monitor.DrawPolygon += Monitor_DrawPolygon;
            Monitor.DrawSquare += Monitor_DrawSquare;
            Monitor.DrawPolygonMethod = EventMethods.PostBack;
            Monitor.ClickMethod = EventMethods.PostBack;
            if (!IsPostBack)
            {
                Monitor.PostbackOnMoveZoom = 0;
                RegisterExtJsStyleSheet();
                Monitor.ImgPath = Config.Monitor.GetMonitorImagesFolder(this);
                Monitor.GoogleMapsScript = Config.Map.GoogleMapsKey;
                Monitor.DefaultMarkerIcon = "salida.gif";
                Monitor.AddLayers(LayerFactory.GetGoogleStreet(CultureManager.GetLabel("LAYER_GSTREET"), MinZoomLevel),
                                  //LayerFactory.GetCompumap(CultureManager.GetLabel("LAYER_COMPUMAP"), Config.Map.CompumapTiles, MinZoomLevel),
                                  LayerFactory.GetOpenStreetMap(CultureManager.GetLabel("LAYER_OSM")),
                                  LayerFactory.GetGoogleSatellite(CultureManager.GetLabel("LAYER_GSAT"), MinZoomLevel),
                                  LayerFactory.GetGoogleHybrid(CultureManager.GetLabel("LAYER_GHIBRIDO"), MinZoomLevel),
                                  LayerFactory.GetGooglePhysical(CultureManager.GetLabel("LAYER_GFISICO"), MinZoomLevel),
                                  LayerFactory.GetVector(LayerGeocercas, true),
                                  LayerFactory.GetVector("Vertices", true),
                                  LayerFactory.GetMarkers("Vertices 2", true));

                var ctx = ControlFactory.GetContextMenu();
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.None, "Opciones", "", "olContextMenuTitle", true));
                ctx.AddItem(ContextMenuItem.Separator);
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, "Ver información", "Info", "olContextMenuItem", true));
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, "Seleccionar color", "ColorPick", "olContextMenuItem", true));
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, "Bloquear/Desbloquear", "Lock", "olContextMenuItem", true));
                ctx.AddItem(new ContextMenuItem(ContextMenuItemBehaviourTypes.Postback, "Repintar", "Paint", "olContextMenuItem", true));
            
                Monitor.AddControls(ControlFactory.GetLayerSwitcher(),
                                    ControlFactory.GetNavigation(),
                                    ControlFactory.GetPanZoomBar(),
                                    ControlFactory.GetToolbar(true, false, true, false, false, false, false),
                                    ctx);

                Monitor.SetDefaultCenter(-34.6134981326759, -58.4255323559046);
                Monitor.SetCenter(-34.6134981326759, -58.4255323559046, InitialZoomLevel);
            }
        }

        private void InitializeQtree()
        {
            Format = SelectedFormat;
            SelectQtree(QtreeName);
            if (!Directory.Exists(QtreeFullPath)) return;
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                HorizontalResolution = qtree.HorizontalResolution;
                VerticalResolution = qtree.VerticalResolution;
                lblQtreeName.Text = QtreeName;
                lblPosicionLat.Text = qtree.Top.ToString("0.0000000");
                lblPosicionLon.Text = qtree.Left.ToString("0.0000000");
                //lblGridSize.Text = string.Format("{0} x {1}", qtree.Width, qtree.Height);
                lblCellWidth.Text = (qtree.HorizontalResolution * 10000000).ToString();
                lblCellHeight.Text = (qtree.VerticalResolution * 10000000).ToString();
                //lblSectorCount.Text = qtree.FileSectorCount.ToString("0");
            }
            Monitor.ZoomTo(InitialZoomLevel);
            ChangeState(QtreeEditorState.None);
        }

        private void BindFormats()
        {
            cbQtreeFormat.Items.Add(new ListItem(QtreeFormats.Gte.ToString()));
            cbQtreeFormat.Items.Add(new ListItem(QtreeFormats.Torino.ToString()));
        }
        private void BindQtrees()
        {
            var qtrees = Directory.GetDirectories(SelectedQtreeDirectory).OfType<string>().Select(d => Path.GetFileName(d)).Where(d => !d.StartsWith(".")).ToList();
            cbQtree.DataSource = qtrees;
            cbQtree.DataBind();
            SelectQtree(QtreeName);
        }
        private void SelectQtree(string name)
        {
            var item = cbQtree.Items.FindByValue(name);
            if (item != null) item.Selected = true;
        }

        private void ChangeQtree(string newQtreeName)
        {
            QtreeName = newQtreeName;
            InitializeQtree();
        }

        private void DrawPoint(double latitud, double longitud)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                var changed = qtree.Paint(latitud, longitud, SelectedBrushSize, SelectedNivel, LockCells);
                AddLeaves(changed);          
            }
        }

        private void DrawPolygon(List<PointF> points)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                var changed = qtree.PaintPolygon(points, SelectedNivel, LockCells);
                AddLeaves(changed);
            }
        }

        private QLeaf GetQleaf(double latitud, double longitud)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                return qtree.GetQLeaf(latitud, longitud);
            }
        }

        private void AddLeaves(IEnumerable<QLeaf> leaves)
        {
            foreach (var leaf in leaves) Monitor.AddGeometries(LayerGeocercas, GetPolygon(leaf));
        }

        public Polygon GetPolygon(QLeaf leaf)
        {
            var id = GetPolygonId(leaf.Index.Y, leaf.Index.X);

            var pol = new Polygon(id, GetColor(leaf.Valor, leaf.Locked));
            pol.AddPoint(new Point("0", leaf.Posicion.Longitud, leaf.Posicion.Latitud));
            pol.AddPoint(new Point("1", leaf.Posicion.Longitud + HorizontalResolution, leaf.Posicion.Latitud));
            pol.AddPoint(new Point("2", leaf.Posicion.Longitud + HorizontalResolution, leaf.Posicion.Latitud - VerticalResolution));
            pol.AddPoint(new Point("3", leaf.Posicion.Longitud, leaf.Posicion.Latitud - VerticalResolution));
            pol.AddPoint(new Point("4", leaf.Posicion.Longitud, leaf.Posicion.Latitud));
            return pol;
        }

        protected string GetColor(int nivel, bool locked)
        {
            var color = lvlSel.GetColorForLevel(nivel);
            return StyleFactory.GetPolygonFromColor(color, locked ? Color.Red : Color.DarkGray);
        }

        private static string GetPolygonId(long y, long x) { return string.Concat(y, ',', x); }

        private int SelectedNivel { get { return lvlSel.SelectedLevel; } }

        private int SelectedBrushSize { get { int bs; return int.TryParse(Slider1.Text, out bs) ? bs : 1; } }

        private bool LockCells { get { return chkLock.Checked; } }

        private void ChangeState(QtreeEditorState newState)
        {
            if (CurrentState == newState) return;

            if (CurrentState == QtreeEditorState.CreatingQtree)
            {
                txtNewQtreeName.Text = txtCellWidth.Text = txtCellHeight.Text = string.Empty;
                panelCrearQtree.Visible = false;
                NewQtreeBounds = null;
                Monitor.ClearLayer(LayerGeocercas);
            }

            var invalidLastState = CurrentState == QtreeEditorState.ColorPick || CurrentState == QtreeEditorState.CreatingQtree;

            LastState = invalidLastState ? QtreeEditorState.Info : CurrentState;
            CurrentState = newState;

            btInfo.BackColor = newState == QtreeEditorState.Info ? Color.Orange : Color.Transparent;
            btEdit.BackColor = newState == QtreeEditorState.Edit ? Color.Orange : Color.Transparent;
            btPickColor.BackColor = newState == QtreeEditorState.ColorPick ? Color.Orange : Color.Transparent;
       
            if (CurrentState != QtreeEditorState.Info)
            {
                panelInfo.Visible = false;
                updInfo.Update();
            }
            if (CurrentState == QtreeEditorState.CreatingQtree)
            {
                Monitor.ClearLayer(LayerGeocercas);
                SetStateEnabled(0);
                panelCrearQtree.Visible = true;
                panelCrearGte.Visible = SelectedFormat == QtreeFormats.Gte;
            }
            updStates.Update();
        }

        private void SetStateEnabled(int zoom)
        {
            var enable = zoom >= QtreeMinZoomLevel && CurrentState != QtreeEditorState.CreatingQtree;

            btInfo.Enabled = 
                btEdit.Enabled = 
                btPickColor.Enabled = enable;

            if (!enable)
            {
                if(CurrentState != QtreeEditorState.CreatingQtree) ChangeState(QtreeEditorState.None);
                btInfo.ImageUrl = "information_dis.png";
                btEdit.ImageUrl = "pencil_dis.png";
                btPickColor.ImageUrl = "colorpicker_dis.png";
            }
            else
            {
                btInfo.ImageUrl = "information.png";
                btEdit.ImageUrl = "pencil.png";
                btPickColor.ImageUrl = "colorpicker.png";
                if (CurrentState == QtreeEditorState.None) ChangeState(QtreeEditorState.Edit);
            }
            updStates.Update();
        }

        private void ShowLeafInfo(double latitud, double longitud)
        {
            //using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            //{
            //    //long numSector, numByte;
            //    //bool lowBits;
            //    //var fn = qtree.GetFileNameAndIndexes((float)latitud, (float)longitud, out numSector, out numByte, out lowBits);
            //    //var leaf = qtree.GetQLeaf(latitud, longitud);
            //    //lblArchivo.Text = Path.GetFileName(fn);
            //    //lblSector.Text = numSector.ToString();
            //    //lblByte.Text = numByte + (lowBits ? "L" : "H");
            //    //lblInfoNivel.Text = leaf.Valor.ToString();
            //    //lblLock.Text = leaf.Locked ? "Bloqueado" : "Desbloqueado";
            //}
            panelInfo.Visible = true;
            updInfo.Update();
        }
        protected void ShowQTree(Box box)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                for (var i = box.Bottom; i < box.Top; i++)
                    for (var j = box.Left; j < box.Right; j++)
                    {
                        var latlon = qtree.GetCenterLatLon(new QIndex { Y = i, X = j });
                        var leaf = qtree.GetQLeaf(latlon.Latitud, latlon.Longitud);
                        if (leaf == null) continue;
                        Monitor.AddGeometries(LayerGeocercas, GetPolygon(leaf));
                    }
            }
        }
        private void SelectColor(double latitud, double longitud)
        {
            lvlSel.SelectedLevel = GetQleaf(latitud, longitud).Valor;
        }
        private void ToggleLock(double latitud, double longitud)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                var leaf = qtree.GetQLeaf(latitud, longitud);
                leaf = qtree.SetLock(latitud, longitud, !leaf.Locked);
                if(leaf != null) AddLeaves(new []{leaf});  
            }
        }

        protected void cbQtreeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindQtrees();
        }

        void Monitor_DrawPolygon(object sender, MonitorDrawPolygonEventArgs e)
        {
            if(CurrentState == QtreeEditorState.Edit) DrawPolygon(e.Points);
        }

        void Monitor_DrawSquare(object sender, MonitorDrawSquareEventArgs e)
        {
            if (CurrentState == QtreeEditorState.Edit)
            {
                if (!Directory.Exists(QtreeFullPath)) return;
                var points = new List<PointF>
                                 {
                                     new PointF((float) e.Bounds.Left, (float) e.Bounds.Top),
                                     new PointF((float) e.Bounds.Right, (float) e.Bounds.Top),
                                     new PointF((float) e.Bounds.Right, (float) e.Bounds.Bottom),
                                     new PointF((float) e.Bounds.Left, (float) e.Bounds.Bottom),
                                     new PointF((float) e.Bounds.Left, (float) e.Bounds.Top)
                                 };
                DrawPolygon(points);
            }
            else if(CurrentState == QtreeEditorState.CreatingQtree)
            {
                NewQtreeBounds = e.Bounds;
                var pol = new Polygon("square", StyleFactory.GetPolygonFromColor(Color.Red, Color.DarkRed));
                pol.AddPoint(new Point("0", e.Bounds.Left, e.Bounds.Top));
                pol.AddPoint(new Point("1", e.Bounds.Right, e.Bounds.Top));
                pol.AddPoint(new Point("2", e.Bounds.Right, e.Bounds.Bottom));
                pol.AddPoint(new Point("3", e.Bounds.Left, e.Bounds.Bottom));
                pol.AddPoint(new Point("4", e.Bounds.Left, e.Bounds.Top));
                Monitor.AddGeometries(LayerGeocercas, pol);
            }
        }

        protected void btState_Click(object sender, CommandEventArgs e)
        {
            var value = Convert.ToInt32(e.CommandArgument);
            var state = QtreeEditorState.Info;
            switch(value)
            {
                case 0: state = QtreeEditorState.Info; break;
                case 1: state = QtreeEditorState.Edit; break;
                case 2: state = QtreeEditorState.ColorPick; break;
            }
            ChangeState(state);
        }
        #region AutoGen

        protected void btAutoGen_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(QtreeFullPath)) return;
            Box box = null;
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                qtree.AutoGenerator.Niveles = autoGenConfig.GetNiveles();
                qtree.Generate();
                if (Bounds != null) box = qtree.GetBoxFromBounds(Bounds.Top, Bounds.Bottom, Bounds.Left, Bounds.Right);
            }
            if (Bounds != null) ShowQTree(box);
        }
        #endregion

        protected void btNuevo_Click(object sender, EventArgs e)
        {
            ChangeQtree(cbQtree.SelectedValue);
        }

        protected void btCrear_Click(object sender, EventArgs e)
        {
            ChangeState(QtreeEditorState.CreatingQtree);
        }
        protected void btAceptarNuevo_Click(object sender, EventArgs e)
        {
            var qtreeName = txtNewQtreeName.Text.Trim();
            if (string.IsNullOrEmpty(qtreeName)) return;
            if (Directory.Exists(Path.Combine(SelectedQtreeDirectory, qtreeName))) return;

            QtreeName = qtreeName;
            Format = SelectedFormat;
            if (SelectedFormat == QtreeFormats.Gte)
            {
                ulong w, h;
                if (!ulong.TryParse(txtCellWidth.Text, out w)) return;
                if (!ulong.TryParse(txtCellHeight.Text, out h)) return;
                if (NewQtreeBounds == null) return;

                using (var q = new GteQtree())
                {
                    q.Create(QtreeFullPath, NewQtreeBounds.Top, NewQtreeBounds.Left, NewQtreeBounds.Bottom, NewQtreeBounds.Right, w, h);
                }
            }
            else if(SelectedFormat == QtreeFormats.Torino)
            {
                using (var q = new TorinoQtree())
                {
                    q.Create(QtreeFullPath);
                }
            }
            BindQtrees();
            InitializeQtree();

            ChangeState(LastState);
        }
        protected void btCancelarNuevo_Click(object sender, EventArgs e)
        {
        
            ChangeState(LastState);
        }


        void Monitor_MapMove(object sender, MapMoveEventArgs e)
        {
            SetStateEnabled(e.Zoom);

            if (e.Zoom < QtreeMinZoomLevel || CurrentState == QtreeEditorState.CreatingQtree || !Directory.Exists(QtreeFullPath))
            {
                Monitor.ClearLayer(LayerGeocercas);
                Bounds = null;
                return;
            }

            Box[] toDelete;
            var newBounds = SetBounds(e.Bounds, out toDelete);

            foreach (var del in toDelete) DeleteOutOfBounds(del);
            foreach(var b in newBounds) ShowQTree(b);
        }

        void Monitor_ContextMenuPostback(object sender, PostbackEventArgs e)
        {
            if (!Directory.Exists(QtreeFullPath)) return;
            if (e.CommandArguments == "Info" && btInfo.Enabled)
            {
                ChangeState(QtreeEditorState.Info);
                ShowLeafInfo(e.Latitud, e.Longitud);
            }
            else if (e.CommandArguments == "ColorPick" && btPickColor.Enabled)
            {
                SelectColor(e.Latitud, e.Longitud); 
            }
            else if (e.CommandArguments == "Lock" && btEdit.Enabled)
            {
                ToggleLock(e.Latitud, e.Longitud);
            }
            else if (e.CommandArguments == "Paint" && btEdit.Enabled)
            {
                using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
                {
                    var leaf = qtree.GetQLeaf(e.Latitud, e.Longitud);
                    Monitor.AddGeometries(LayerGeocercas, GetPolygon(leaf));
                }
            }
        }
    

        void Monitor_Click(object sender, MonitorClickEventArgs e)
        {
            if (!Directory.Exists(QtreeFullPath)) return;
            switch (CurrentState)
            {
                case QtreeEditorState.Info:
                    ShowLeafInfo(e.Latitud, e.Longitud);
                    break;
                case QtreeEditorState.Edit:
                    DrawPoint(e.Latitud, e.Longitud);
                    break;
                case QtreeEditorState.ColorPick:
                    SelectColor(e.Latitud, e.Longitud);
                    ChangeState(LastState);
                    break;
            }
        }
        protected Box[] SetBounds(Bounds newBounds, out Box[] toDelete)
        {
            using (var qtree = BaseQtree.Open(QtreeFullPath, Format))
            {
                var oldBounds = Bounds;
                Bounds = newBounds;

                toDelete = new Box[0];

                var newBox = qtree.GetBoxFromBounds(newBounds.Top, newBounds.Bottom, newBounds.Left, newBounds.Right);

                if (oldBounds == null) return new[] { newBox };

                var oldBox = qtree.GetBoxFromBounds(oldBounds.Top, oldBounds.Bottom, oldBounds.Left, oldBounds.Right);

                var bounds = new List<Box>(4);
                var boundsToDelete = new List<Box>(4);

                if (newBounds.Top > oldBounds.Top)
                {
                    bounds.Add(new Box(newBox.Top, oldBox.Top, newBox.Left, newBox.Right));
                    boundsToDelete.Add(new Box(newBox.Bottom, oldBox.Bottom, oldBox.Left, oldBox.Right));
                }
                if (newBounds.Bottom < oldBounds.Bottom)
                {
                    bounds.Add(new Box(oldBox.Bottom, newBox.Bottom, newBox.Left, newBox.Right));
                    boundsToDelete.Add(new Box(oldBox.Top, newBox.Top, oldBox.Left, oldBox.Right));
                }
                if (newBounds.Left < oldBounds.Left)
                {
                    bounds.Add(new Box(Math.Min(oldBox.Top, newBox.Top), Math.Max(newBox.Bottom, oldBox.Bottom), newBox.Left, oldBox.Left));
                    boundsToDelete.Add(new Box(Math.Min(oldBox.Top, newBox.Top), Math.Max(newBox.Bottom, oldBox.Bottom), newBox.Right, oldBox.Right));
                }

                if (newBounds.Right > oldBounds.Right)
                {
                    bounds.Add(new Box(Math.Min(oldBox.Top, newBox.Top), Math.Max(newBox.Bottom, oldBox.Bottom), oldBox.Right, newBox.Right));
                    boundsToDelete.Add(new Box(Math.Min(oldBox.Top, newBox.Top), Math.Max(newBox.Bottom, oldBox.Bottom), oldBox.Left, newBox.Left));
                }
                toDelete = boundsToDelete.ToArray();
        
                return bounds.ToArray();
            }
        }
     
        protected void DeleteOutOfBounds(Box box)
        {
            for (var i = box.Bottom; i < box.Top; i++)
                for (var j = box.Left; j < box.Right; j++)
                    Monitor.RemoveGeometries(LayerGeocercas, GetPolygonId(i, j));
        }
   
    }
    public class SquareStyle
    {
        public int Nivel { get; set; }
        public string Descripcion { get; set; }
        public int TipoVia { get; set; }
        public Color BorderColor { get; set; }
        public Color BackColor { get; set; }
    }
}