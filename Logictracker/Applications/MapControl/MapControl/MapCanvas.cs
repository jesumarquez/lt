using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Collections.Specialized;
using Urbetrack.QuadTree;
using System.Windows.Data;

namespace RNR.Maps
{
    /// <summary>Visor de mapas via Clarin.</summary>
    public sealed partial class MapCanvas : Canvas
    {
        /// <summary>Obtiene o establece la latitud sobre la que el mapa esta centrado.</summary>
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.RegisterAttached("Latitude", typeof(double), typeof(MapCanvas), new PropertyMetadata(double.PositiveInfinity, OnLatitudeLongitudePropertyChanged));

        /// <summary>Obtiene o establece la longitud sobre la que el mapa esta centrado.</summary>
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.RegisterAttached("Longitude", typeof(double), typeof(MapCanvas), new PropertyMetadata(double.PositiveInfinity, OnLatitudeLongitudePropertyChanged));

        /// <summary>
        /// Obtiene o Establece la altura o zoom desde donde se visualiza el mapa.
        /// </summary>
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(int), typeof(MapCanvas), new UIPropertyMetadata(0, OnZoomPropertyChanged, OnZoomPropertyCoerceValue));


        /// <summary>
        /// Obtiene o Establece la carpeta donde esta el Repositorio del QTree 
        /// </summary>
        public static readonly DependencyProperty RepositoryProperty =
            DependencyProperty.Register("Repository", typeof(Repository), typeof(MapCanvas), new PropertyMetadata(null, OnRepositoryPropertyChanged));



        /// <summary>
        /// Obtiene o Establece la carpeta donde esta el Repositorio del QTree 
        /// </summary>
        public static readonly DependencyProperty ScrollLockProperty =
            DependencyProperty.Register("ScrollLock", typeof(bool), typeof(MapCanvas), new PropertyMetadata(false, OnScrollLockPropertyChanged));

        /// <summary>
        /// Obtiene el Rectangulo que delimita el area visible del mapa.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty;
        private static readonly DependencyPropertyKey ViewportKey =
            DependencyProperty.RegisterReadOnly("Viewport", typeof(Rect), typeof(MapCanvas), new PropertyMetadata());


        /// <summary>
        /// Obtiene la Latitud sobre la cual esta ubicado el cursor.
        /// </summary>
        public static readonly DependencyProperty ActualCursorLatitudeProperty;
        public static readonly DependencyPropertyKey ActualCursorLatitudePropertyKey =
            DependencyProperty.RegisterReadOnly("ActualCursorLatitude", typeof(double), typeof(MapCanvas), new PropertyMetadata());

        /// <summary>
        /// Obtiene la Longitud sobre la cual esta ubicado el cursor.
        /// </summary>
        public static readonly DependencyProperty ActualCursorLongitudeProperty;
        public static readonly DependencyPropertyKey ActualCursorLongitudePropertyKey =
            DependencyProperty.RegisterReadOnly("ActualCursorLongitude", typeof(double), typeof(MapCanvas), new PropertyMetadata());


        private TilePanel _tilePanel = new TilePanel();
        private Canvas _qtPanel = new Canvas();
        private Image _cache = new Image();
        private int _updateCount;
        private bool _mouseCaptured;
        private Point _previousMouse;
        private MapOffset _offsetX;
        private MapOffset _offsetY;
        private TranslateTransform _translate = new TranslateTransform();

        /// <summary>Inicializa los miembros static y registra los command bindings.</summary>
        static MapCanvas()
        {
            ViewportProperty = ViewportKey.DependencyProperty; // Need to set it here after ViewportKey has been initialized.
            ActualCursorLatitudeProperty = ActualCursorLatitudePropertyKey.DependencyProperty;
            ActualCursorLongitudeProperty = ActualCursorLongitudePropertyKey.DependencyProperty;

            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(ComponentCommands.MoveDown, (sender, e) => Pan(sender, e.Command, 0, -1)));
            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(ComponentCommands.MoveLeft, (sender, e) => Pan(sender, e.Command, 1, 0)));
            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(ComponentCommands.MoveRight, (sender, e) => Pan(sender, e.Command, -1, 0)));
            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(ComponentCommands.MoveUp, (sender, e) => Pan(sender, e.Command, 0, 1)));

            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(NavigationCommands.DecreaseZoom, (sender, e) => ((MapCanvas)sender).Zoom--));
            CommandManager.RegisterClassCommandBinding(
                typeof(MapCanvas), new CommandBinding(NavigationCommands.IncreaseZoom, (sender, e) => ((MapCanvas)sender).Zoom++));
        }

        /// <summary>Inicializa el mapa</summary>
        public MapCanvas()
        {
            _offsetX = new MapOffset(_translate.GetType().GetProperty("X"), this.OnOffsetChanged);
            _offsetY = new MapOffset(_translate.GetType().GetProperty("Y"), this.OnOffsetChanged);

            _tilePanel.RenderTransform = _translate;
            this.Background = Brushes.Transparent; // Register all mouse clicks
            this.Children.Add(_cache);
            this.Children.Add(_tilePanel);
            this.Children.Add(_qtPanel);
            this.ClipToBounds = true;
            this.Focusable = true;
            this.FocusVisualStyle = null;
            this.SnapsToDevicePixels = true;
        }

        public double ActualCursorLatitude
        {
            get { return (double)this.GetValue(ActualCursorLatitudeProperty); }
            private set { this.SetValue(ActualCursorLatitudePropertyKey, value); }
        }

        public double ActualCursorLongitude
        {
            get { return (double)this.GetValue(ActualCursorLongitudeProperty); }
            private set { this.SetValue(ActualCursorLongitudePropertyKey, value); }
        }

        /// <summary>Gets the visible area of the map in latitude/longitude coordinates.</summary>
        public Rect Viewport
        {
            get { return (Rect)this.GetValue(ViewportProperty); }
            private set { this.SetValue(ViewportKey, value); }
        }

        /// <summary>Gets or sets the zoom level of the map.</summary>
        public int Zoom
        {
            get { return (int)this.GetValue(ZoomProperty); }
            set { this.SetValue(ZoomProperty, value); }
        }

        // <summary>Gets or sets the zoom level of the map.</summary>
        public Repository Repository
        {
            get { return (Repository)this.GetValue(RepositoryProperty); }
            set { this.SetValue(RepositoryProperty, value); }
        }

        public bool ScrollLock
        {
            get { return (bool)this.GetValue(ScrollLockProperty); }
            set { this.SetValue(ScrollLockProperty, value); }
        }

        /// <summary>Gets the value of the Latitude attached property for a given depencency object.</summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The Latitude coordinate of the specified element.</returns>
        public static double GetLatitude(DependencyObject obj)
        {
            return (double)obj.GetValue(LatitudeProperty);
        }

        /// <summary>Gets the value of the Longitude attached property for a given depencency object.</summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The Longitude coordinate of the specified element.</returns>
        public static double GetLongitude(DependencyObject obj)
        {
            return (double)obj.GetValue(LongitudeProperty);
        }

        /// <summary>Sets the value of the Latitude attached property for a given depencency object.</summary>
        /// <param name="obj">The element to which the property value is written.</param>
        /// <param name="value">Sets the Latitude coordinate of the specified element.</param>
        public static void SetLatitude(DependencyObject obj, double value)
        {
            obj.SetValue(LatitudeProperty, value);
        }

        /// <summary>Sets the value of the Longitude attached property for a given depencency object.</summary>
        /// <param name="obj">The element to which the property value is written.</param>
        /// <param name="value">Sets the Longitude coordinate of the specified element.</param>
        public static void SetLongitude(DependencyObject obj, double value)
        {
            obj.SetValue(LongitudeProperty, value);
        }

        /// <summary>Centers the map on the specified coordinates.</summary>
        /// <param name="latitude">The latitude cooridinate.</param>
        /// <param name="longitude">The longitude coordinates.</param>
        /// <param name="zoom">The zoom level for the map.</param>
        public void Center(double latitude, double longitude, int zoom)
        {
            this.BeginUpdate();
            this.Zoom = zoom;
            var tilex = TileGenerator.GetTileX(longitude, this.Zoom);
            _offsetX.CenterOn(tilex);
            var tiley = TileGenerator.GetTileY(latitude, this.Zoom);
            _offsetY.CenterOn(tiley);

            Debug.WriteLine(String.Format("CenterZoom: ty={0} tx={1} z={2} lat={3} lon={4}",
                tilex,tiley,
                zoom, latitude, longitude));
            this.EndUpdate();
        }

        /// <summary>Centers the map on the specified coordinates, calculating the required zoom level.</summary>
        /// <param name="latitude">The latitude cooridinate.</param>
        /// <param name="longitude">The longitude coordinates.</param>
        /// <param name="size">The minimum size that must be visible, centered on the coordinates.</param>
        public void Center(double latitude, double longitude, Size size)
        {
            double left = TileGenerator.GetTileX(longitude - (size.Width / 2.0), 0);
            double right = TileGenerator.GetTileX(longitude + (size.Width / 2.0), 0);
            double top = TileGenerator.GetTileY(latitude - (size.Height / 2.0), 0);
            double bottom = TileGenerator.GetTileY(latitude + (size.Height / 2.0), 0);

            double height = (top - bottom) * TileGenerator.TileSize;
            double width = (right - left) * TileGenerator.TileSize;
            int zoom = Math.Min(TileGenerator.GetZoom(this.ActualHeight / height), TileGenerator.GetZoom(this.ActualWidth / width));
            // todo, hacer un combinador o algo asi.
            Debug.WriteLine(String.Format("CenterSize: aw={0} ah={1} l={2} r={3} t={4} b={5} h={6} w={7} z={8} lat={9} lon={10}", 
                (int)this.ActualWidth, (int)this.ActualHeight,
                left, right, top,
                bottom, height, width, 
                zoom,latitude, longitude));
            
            this.Center(latitude, longitude, zoom);
        }
               
        /// <summary>Creates a static image of the current view.</summary>
        /// <returns>An image of the current map.</returns>
        public ImageSource CreateBaseImage()
        {
            Debug.WriteLine(String.Format("CreateBaseImage: {0} x {1}", (int)this.ActualWidth, (int)this.ActualHeight));
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);
            bitmap.Render(_tilePanel);
            bitmap.Freeze();
            return bitmap;
        }

        /// <summary>Calculates the coordinates of the specifed point.</summary>
        /// <param name="point">A point, in pixels, relative to the top left corner of the control.</param>
        /// <returns>A Point filled with the Latitude (Y) and Longitude (X) of the specifide point.</returns>
        public Point GetLocation(Point point)
        {
            Point output = new Point();
            output.X = TileGenerator.GetLongitude((_offsetX.Pixels + point.X) / TileGenerator.TileSize, this.Zoom);
            output.Y = TileGenerator.GetLatitude((_offsetY.Pixels + point.Y) / TileGenerator.TileSize, this.Zoom);
            return output;
        }

        /// <summary>Calculates the coordinates of the specifed point.</summary>
        /// <param name="point">A point, in pixels, relative to the top left corner of the control.</param>
        /// <returns>A Point filled with the Latitude (Y) and Longitude (X) of the specifide point.</returns>
        public void Invalidate()
        {
            Invalidate(false);
        }
        public void Invalidate(bool cacheToo)
        {
            if (cacheToo)
            {
                if (!string.IsNullOrEmpty(TileGenerator.CacheFolder))
                {
                    Directory.Delete(TileGenerator.CacheFolder, true);                    
                }
            }
            this.BeginUpdate();
            _tilePanel.ForceUpdate = true;
            this.EndUpdate();
        }

        /// <summary>Tries to capture the mouse to enable dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (ScrollLock) return;
            this.Focus(); // Make sure we get the keyboard
            if (this.CaptureMouse())
            {
                _mouseCaptured = true;
                _previousMouse = e.GetPosition(null);
            }
        }

        /// <summary>Releases the mouse capture and stops dragging of the map.</summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (ScrollLock) return;
            this.ReleaseMouseCapture();
            _mouseCaptured = false;
        }

        /// <summary>Drags the map, if the mouse was succesfully captured.</summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point position = e.GetPosition(null);

            if (_mouseCaptured)
            {
                this.BeginUpdate();
                _offsetX.Translate(position.X - _previousMouse.X);
                _offsetY.Translate(position.Y - _previousMouse.Y);
                _previousMouse = position;
                this.EndUpdate();
            }
            else
            {
                var location = GetLocation(position);
                ActualCursorLatitude = location.Y;
                ActualCursorLongitude = location.X;
            }
        }

        /// <summary>Alters the zoom of the map, maintaing the same point underneath the mouse at the new zoom level.</summary>
        /// <param name="e">The MouseWheelEventArgs that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            int newZoom = TileGenerator.GetValidZoom(this.Zoom + (e.Delta / Mouse.MouseWheelDeltaForOneLine));
            Point mouse = e.GetPosition(this);

            this.BeginUpdate();
            _offsetX.ChangeZoom(newZoom, mouse.X);
            _offsetY.ChangeZoom(newZoom, mouse.Y);
            this.Zoom = newZoom; // Set this after we've altered the offsets
            this.EndUpdate();
        }

        /// <summary>Notifies child controls that the size has changed.</summary>
        /// <param name="sizeInfo">
        /// The packaged parameters (SizeChangedInfo), which includes old and new sizes, and which dimension actually changes.
        /// </param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.BeginUpdate();
            _offsetX.ChangeSize(sizeInfo.NewSize.Width);
            _offsetY.ChangeSize(sizeInfo.NewSize.Height);
            _tilePanel.Width = sizeInfo.NewSize.Width;
            _tilePanel.Height = sizeInfo.NewSize.Height;
            this.EndUpdate();
        }

        private static bool IsKeyboardCommand(RoutedCommand command)
        {
            foreach (var gesture in command.InputGestures)
            {
                var key = gesture as KeyGesture;
                if (key != null)
                {
                    if (Keyboard.IsKeyDown(key.Key))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void OnLatitudeLongitudePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Search for a MapControl parent
            MapCanvas canvas = null;
            FrameworkElement child = d as FrameworkElement;
            while (child != null)
            {
                canvas = child as MapCanvas;
                if (canvas != null)
                {
                    break;
                }
                child = child.Parent as FrameworkElement;
            }
            if (canvas != null)
            {
                canvas.RepositionChildren();
            }
        }

        private static void OnRepositoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MapCanvas)d).OnRepositoryChanged();
        }

        private static void OnScrollLockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void OnZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MapCanvas)d).OnZoomChanged();
        }

        private static object OnZoomPropertyCoerceValue(DependencyObject d, object baseValue)
        {
            return TileGenerator.GetValidZoom((int)baseValue);
        }

        private static void Pan(object sender, ICommand command, double x, double y)
        {
            if (!IsKeyboardCommand((RoutedCommand)command)) // Move a whole square instead of a pixel if it wasn't the keyboard who sent it
            {
                x *= TileGenerator.TileSize;
                y *= TileGenerator.TileSize;
            }
            MapCanvas instance = (MapCanvas)sender;
            instance._offsetX.AnimateTranslate(x);
            instance._offsetY.AnimateTranslate(y);
            instance.Focus();
        }

        private void OnOffsetChanged(object sender, EventArgs e)
        {
            this.BeginUpdate();
            MapOffset offset = (MapOffset)sender;
            offset.Property.SetValue(_translate, offset.Offset, null);
            this.EndUpdate();
        }

        private void OnZoomChanged()
        {
            this.BeginUpdate();
            _offsetX.ChangeZoom(this.Zoom, this.ActualWidth / 2.0);
            _offsetY.ChangeZoom(this.Zoom, this.ActualHeight / 2.0);
            _tilePanel.Zoom = this.Zoom;
            this.EndUpdate();
        }

        private void OnRepositoryChanged()
        {
            _tilePanel.ForceUpdate = true;
            if (Repository != null)
            {
                if (this.Zoom != Repository.ZoomLevel)
                {
                    this.Zoom = Repository.ZoomLevel;
                    return;
                }
            }
            OnZoomChanged();
        }

        private void BeginUpdate()
        {
            _updateCount++;
        }

        private void EndUpdate()
        {
            System.Diagnostics.Debug.Assert(_updateCount != 0, "Must call BeginUpdate first");
            if (--_updateCount == 0)
            {
                if (LoadingQuadtree) return;
                _tilePanel.LeftTile = _offsetX.Tile;
                _tilePanel.TopTile = _offsetY.Tile;
                if (_tilePanel.RequiresUpdate)
                {
                    _cache.Visibility = Visibility.Visible; // nuestro la cache durante la actualizcion.
                    _tilePanel.Update(_tilePanel.ForceUpdate); 
                    // limpio el update forzoso.
                    _tilePanel.ForceUpdate = false;
                    this.RegenerateQuadTree(true);
                    this.RepositionChildren();
                    _cache.Visibility = Visibility.Hidden;
                    _cache.Source = this.CreateBaseImage(); // Save our image for later
                }

                // Update viewport
                Point topleft = this.GetLocation(new Point(0, 0));
                Point bottomRight = this.GetLocation(new Point(this.ActualWidth, this.ActualHeight));
                this.Viewport = new Rect(topleft, bottomRight);
            }
        }

        /// <summary>Processes all messages in the CurrentDispatcher's queue.</summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.threading.dispatcherframe.aspx" />
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => frame.Continue = false), DispatcherPriority.Background, null);
            Dispatcher.PushFrame(frame);
        }

        private void RegenerateQuadTree(bool processEvents)
        {
            if (Repository == null) return;

            // ENTROPY_RANDOM_FAILURE (tarde o temprano causara una falla)
            // aqui hago una cochinada... me fastidio el wpf por las propiedades atachadas que ubican los 
            // objetos sobre el mapa (latitud y longitud), disparan los eventos de cambios que a su vez disparan 
            // el reposicionamiento de los childens.
            // tons.. para evitar este molesto problema uso la vieja pero poco confiable tecnica de una bandera 
            // ortogonal.
            // esta es una de las muchas cosas que hay que sacar para alcanzar la meta de una tecnica global y
            // determinista que permita comprobar la fiabilidad de un sistema cualquiera sea su aplicacion.
            LoadingQuadtree = true;

            var tl = GetLocation(new Point(0, 0));
            var br = GetLocation(new Point(this.ActualWidth, this.ActualHeight));

            var hres = Repository.HorizontalResolution;
            var vres = Repository.VerticalResolution;

            // ubicacion del primer cuadradiro que pongo
            var xlon = br.X - (br.X % hres);
            var ylat = br.Y - (br.Y % vres);
            
            // cantidad de filas y columnas visibles.
            var xcnt = (br.X - tl.X) / hres;
            var ycnt = (br.Y - tl.Y) / vres;

            Debug.WriteLine(String.Format("ViewPort Coords: top={0} left={1} bottom={2} right={3} x-cells={4} y-cells={5}",
                                        tl.Y, tl.X, br.Y, br.X, xcnt, ycnt));

            //var cache=                  
            
            for (double x = -2; x < Math.Abs(xcnt) + 1; ++x)
            {
                if (processEvents)
                {
                    // procesamos los eventos wpf para que la aplicacion no muestre señales de
                    // agarrotarse, esto le da confort al programa.
                    DoEvents(); 
                }
                for (double y = -1; y < Math.Abs(ycnt) +1 ; ++y)
                {
                    // Calculo la posicion del cuadradito  del canvas.
                    var lat = ylat + (y * vres);
                    var lon = xlon - (x * hres);
                    //Debug.WriteLine("AddQtRect Lat=" + lat.ToString() + " Lon=" + lon.ToString());
                    var Class = Repository.GetPositionClass((float)((float)lat + ((float)vres / 2.0) - 0.0002), (float)((float)lon + ((float)hres / 2.0) - 0.0002));
                    AddQuadtreeRectangle(lat, lon, vres, hres, this.Zoom, Class);
                }
            }
            LoadingQuadtree = false;
        }

        private bool LoadingQuadtree;
        private static BooleanToHiddenVisibility bool2VisConverter = new BooleanToHiddenVisibility();
        private static QuadTreeCellToColor quadTreeCellToColor = new QuadTreeCellToColor();

        public class QuadTreeCell
        {
            public double Latitude;
            public double Longitude;
            public int Class;
            public string MetaData;
            public Rectangle RectangleHost;

            public override string ToString()
            {
                return string.Format("Lat={0} Lon={1} Class={2} MetaData='{3}'",
                    Latitude, Longitude, Class, MetaData);
            }
        }
        
        private void AddQuadtreeRectangle(double lat, double lon, double vres, double hres, int zoom, int value)
        {
            Rectangle newRectangle = new Rectangle();
            //double width = (tile * TileGenerator.TileSize) - (_size / 2.0);
            var visibleBind = new Binding();
            visibleBind.ElementName = "checkViewTextBox";
            visibleBind.Path = new PropertyPath(CheckBox.IsCheckedProperty);
            visibleBind.Converter = bool2VisConverter;
            BindingOperations.SetBinding(newRectangle, Rectangle.VisibilityProperty, visibleBind);
            newRectangle.Width = 13.5;
            newRectangle.Height = 14.73;
            newRectangle.MouseLeftButtonUp += new MouseButtonEventHandler(QuadTreeCellLeftClick);
            newRectangle.MouseEnter += new MouseEventHandler(QuadTreeCellHover);
            newRectangle.MouseRightButtonUp += new MouseButtonEventHandler(QuadTreeCellRightClick);
            
            var colorBind = new Binding();
            colorBind.Path = new PropertyPath(Rectangle.TagProperty);
            colorBind.RelativeSource = RelativeSource.Self;
            colorBind.Converter = quadTreeCellToColor;
            BindingOperations.SetBinding(newRectangle, Rectangle.FillProperty, colorBind);
            
            // Create a SolidColorBrush and use it to
            // paint the rectangle.
            
            //newRectangle.Fill = myBrush;

            newRectangle.Stroke = Brushes.Black;
            newRectangle.StrokeThickness = 0.5;            
            newRectangle.Tag = new QuadTreeCell
                            {
                                Class = value,
                                Latitude = lat,
                                Longitude = lon,
                                MetaData = string.Format("z={0}", zoom),
                                RectangleHost = newRectangle
                            };

            //_qtPanel.Children.Add(newRectangle);
            Children.Add(newRectangle);
            SetLatitude(newRectangle, lat);
            SetLongitude(newRectangle, lon);
        }

        public enum QuadTreeEventCode
        {
            MouseHover,
            Click,
            RigthClick
        }

        public delegate void QuadTreeEventHandler(object sender, QuadTreeEventCode evt, QuadTreeCell Args);

        public event QuadTreeEventHandler QuadTreeEvent;

        void QuadTreeCellHover(object sender, MouseEventArgs e)
        {
            var rect = sender as Rectangle;
            if (QuadTreeEvent != null) QuadTreeEvent(this, QuadTreeEventCode.MouseHover, rect.Tag as QuadTreeCell); 
        }

        void QuadTreeCellLeftClick(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (QuadTreeEvent != null) QuadTreeEvent(this, QuadTreeEventCode.Click, rect.Tag as QuadTreeCell); 
        }

        void QuadTreeCellRightClick(object sender, MouseButtonEventArgs e)
        {
            var rect = sender as Rectangle;
            if (QuadTreeEvent != null) QuadTreeEvent(this, QuadTreeEventCode.RigthClick, rect.Tag as QuadTreeCell); 
        }

        private void RepositionChildren()
        {
            if (LoadingQuadtree) return;
            foreach (UIElement element in this.Children)
            {
                double latitude = GetLatitude(element);
                double longitude = GetLongitude(element);
                if (latitude != double.PositiveInfinity && longitude != double.PositiveInfinity)
                {
                    double x = (TileGenerator.GetTileX(longitude, this.Zoom) - _offsetX.Tile) * TileGenerator.TileSize;
                    double y = (TileGenerator.GetTileY(latitude, this.Zoom) - _offsetY.Tile) * TileGenerator.TileSize;
                    Canvas.SetLeft(element, x);
                    Canvas.SetTop(element, y);
                    element.RenderTransform = _translate;
                }
                else
                {
                    Debug.WriteLine("Descartando Objeto por Type=" + element.GetType().ToString());
                }
            }
        }
    }
}