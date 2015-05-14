using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urbetrack.QuadTree;
using RNR.Maps;

namespace Compumap7
{
    /// <summary>
    /// Interaction logic for C7MainWindow.xaml
    /// </summary>
    public partial class C7MainWindow : Window
    {
        public C7MainWindow()
        {
            if (Directory.Exists(@"ImageCache"))
                Directory.Delete(@"ImageCache", true);
            // Very important we set the CacheFolder before doing anything so the MapCanvas knows where
            // to save the downloaded files to.
            TileGenerator.CacheFolder = @"ImageCache";

            InitializeComponent();
            tileCanvas.Loaded += tileCanvas_Loaded;
            tileCanvas.QuadTreeEvent += QuadTreeEvent;
        }

        void QuadTreeEvent(object sender, MapCanvas.QuadTreeEventCode Evt, MapCanvas.QuadTreeCell Cell)
        {
            StatusAnnotation.Text = Cell.ToString();
            if (Evt == MapCanvas.QuadTreeEventCode.Click){
                Cell.Class = velocidad;
                Cell.RectangleHost.Tag = new MapCanvas.QuadTreeCell
                            {
                                Class = Cell.Class,
                                Latitude = Cell.Latitude,
                                Longitude = Cell.Longitude,
                                MetaData = Cell.MetaData,
                                RectangleHost = Cell.RectangleHost
                            };
                tileCanvas.Repository.SetPositionClass((float) Cell.Latitude, (float)Cell.Longitude, Cell.Class);
            }
        }

        void tileCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //-34.606367,-58.435936
            //tileCanvas.Center(-34.606367, -58.435936, 15);
            tileCanvas.Center(-34.61389, -58.42561, 15);
            //tileCanvas.Center(-45.8001313, -67.4828725, 15);
        }

        private void MouseButtonEventHandler(Object sender, MouseButtonEventArgs e) 
        {
            this.Title = "BOTON";
        }

        private void MouseButtonEventHandler2(Object sender, MouseButtonEventArgs e)
        {
            // Open document
            var dirname = @"C:\QTREE_REPO";
            var repo = new Repository();
            var so = new StorageGeometry();

            repo.Open<GR2>(dirname, ref so);
            tileCanvas.Repository = repo;
        }

        private void MouseEventHandler(Object sender,MouseEventArgs e)
        {
            this.Title = "ENTRO";
        }

        protected void MostrarCapaQuadTree(object sender, RoutedEventArgs e)
        {
           
        }

        protected void OcultarCapaQuadTree(object sender, RoutedEventArgs e)
        {
          
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            if (b == null) throw new InvalidOperationException("el sender debe ser un boton");
            if (b.Name == "open") AbrirQT(sender, e);
            if (b.Name == "new") NuevoQT(sender, e);
            if (b.Name == "close") CerrarQT(sender, e);
            if (b.Name == "opengg") AbrirGG(sender, e);
            if (b.Name == "newgg") NuevoGG(sender, e);
            if (b.Name == "closegg") CerrarGG(sender, e);
        }

        private void AbrirQT(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
                          {
                              FileName = "transaction",
                              DefaultExt = ".log;",
                              Filter = "Registro de Cambios (transaction.log)|transaction.log;",
                              Title = "Abrir Base de Datos ...",
                              Multiselect = false,
                              DereferenceLinks = true
                          };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result != true) return;
            // Open document
            var filename = dlg.FileName;
            var dir = System.IO.Path.GetDirectoryName(filename);
            var repo = new Repository();
            var so = new StorageGeometry();
            
            //repo.Open<GeoGrillas>(dir, ref so);
            repo.Open<GR2>(dir, ref so);
            tileCanvas.Repository = repo;
        }

        private void NuevoQT(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = "transaction",
                DefaultExt = ".log;",
                Filter = "Registro de Cambios (transaction.log)|transaction.log;",
                Title = "Crear Base de Datos ...",
                DereferenceLinks = true
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result != true) return;
            // Open document
            var filename = dlg.FileName;
            var dir = System.IO.Path.GetDirectoryName(filename);
            var repo = new Repository();
           // so.Lat_Grid = 1.0/60.0/30.0;
            var so = new StorageGeometry()
                {
                    Signature = "GR2",
                    Lat_Grid = 0,
                    Lon_Grid = 0,
                    Lat_GridCount = 0,
                    Lon_GridCount = 0,
                    Lat_OffSet = 0,
                    Lon_OffSet = 0,
                    FileSectorCount = 0,
                };
            repo.Init<GR2>(dir, so);
            tileCanvas.Repository = repo;
        }

        private void CerrarQT(object sender, RoutedEventArgs e)
        {
            if (tileCanvas.Repository != null)
                    tileCanvas.Repository.Close();
            tileCanvas.Repository = null;
        }

        private void AbrirGG(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "transaction",
                DefaultExt = ".log;",
                Filter = "Registro de Cambios (transaction.log)|transaction.log;",
                Title = "Abrir Base de Datos ...",
                Multiselect = false,
                DereferenceLinks = true
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result != true) return;
            // Open document
            var filename = dlg.FileName;
            var dir = System.IO.Path.GetDirectoryName(filename);
            var repo = new Repository();
            var so = new StorageGeometry();

            //repo.Open<GeoGrillas>(dir, ref so);
            repo.Open<GeoGrillas>(dir, ref so);
            tileCanvas.Repository = repo;
        }

        private void NuevoGG(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                FileName = "transaction",
                DefaultExt = ".log;",
                Filter = "Registro de Cambios (transaction.log)|transaction.log;",
                Title = "Crear Base de Datos ...",
                DereferenceLinks = true
            };

            // Show open file dialog box
            var result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result != true) return;
            // Open document
            var filename = dlg.FileName;
            var dir = System.IO.Path.GetDirectoryName(filename);
            var repo = new Repository();
            // so.Lat_Grid = 1.0/60.0/30.0;
            var so = new StorageGeometry()
            {
                Signature = "GeoGrillas",
                Lat_Grid = 5600,
                Lon_Grid = 5600,
                Lat_GridCount = 2000,
                Lon_GridCount = 2500,
                Lat_OffSet = -345300000,
                Lon_OffSet = -585500000,
                FileSectorCount = 79,
            };
            repo.Init<GeoGrillas>(dir, so);
            tileCanvas.Repository = repo;
        }

        private void CerrarGG(object sender, RoutedEventArgs e)
        {
            if (tileCanvas.Repository != null)
                tileCanvas.Repository.Close();
            tileCanvas.Repository = null;
        }


        private void CambiarMapaBase(object sender, RoutedEventArgs e)
        {
            var b = sender as RadioButton;
            if (b == null) throw new InvalidOperationException("el sender debe ser un boton");
            if (b.Name == "clarin") TileGenerator.TileFormat = TileGenerator.TileFormatClarin;
            if (b.Name == "google") TileGenerator.TileFormat = TileGenerator.TileFormatGoogle;
            if (b.Name == "satelital") TileGenerator.TileFormat = TileGenerator.TileFormatSatelital;
            if (b.Name == "hibrido") TileGenerator.TileFormat = TileGenerator.TileFormatHibrido;
            if (b.Name == "osmap") TileGenerator.TileFormat = TileGenerator.TileFormatOpenStreetMap;
            if (tileCanvas != null) tileCanvas.Invalidate(true);
        }

        private int velocidad = 1;
        private void CambiarVelocidad(object sender, RoutedEventArgs e)
        {
            var b = sender as RadioButton;
            if (b == null) throw new InvalidOperationException("el sender debe ser un boton");
            if (b.Name == "c1") velocidad = 1;
            if (b.Name == "c2") velocidad = 2;
            if (b.Name == "c3") velocidad = 3;
            if (b.Name == "c4") velocidad = 4;
            if (b.Name == "c5") velocidad = 5;
        }

    }

    public class BooleanToHiddenVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility rv = Visibility.Visible;
            try
            {
                var p = (bool)(parameter == null ? true : bool.Parse(parameter.ToString()));
                var x = bool.Parse(value.ToString());
                if (x == p)
                {
                    rv = Visibility.Visible;
                }
                else
                {
                    rv = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
            }
            return rv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }
}
