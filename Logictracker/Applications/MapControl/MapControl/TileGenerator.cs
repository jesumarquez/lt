using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;

namespace RNR.Maps
{
    /// <summary>Helper methods to retrieve information from openstreetmap.org</summary>
    /// <remarks>See http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames for reference.</remarks>
    public static class TileGenerator
    {
        /// <summary>The maximum allowed zoom level.</summary>
        public const int MaxZoom = 18;

        /// <summary>The size of a tile in pixels.</summary>
        internal const double TileSize = 256;

        public const string TileFormatClarin = @"http://imgmapas.clarin.com/{0}/{1}/{2}.png";
        public const string TileFormatGoogle = @"http://mt1.google.com/vt/lyrs=m@135&hl=es&src=api&x={1}&s=&y={2}&z={0}&s=Galil";
        public const string TileFormatSatelital = @"http://khm0.google.com/kh/v=73&x={1}&s=&y={2}&z={0}&s=&token=81194";
        public const string TileFormatHibrido = @"http://mt1.google.com/vt/lyrs=h@135&hl=es&src=api&x={1}&s=&y={2}&z={0}&s=Galil";
        public const string TileFormatOpenStreetMap = @"http://tile.openstreetmap.org/{0}/{1}/{2}.png";

        public static string tileFormat = TileFormatClarin;
        public static string TileFormat
        {
            get 
            { 
                return tileFormat; 
            }
            set
            {
                tileFormat = value;                
            }
        }
        private const string TileCacheFolder = @"ImagesCache";

        /// <summary>Occurs when the download progress of Tile has changed.</summary>
        public static event EventHandler<DownloadProgressEventArgs> ImageDownloadingProgress;

        /// <summary>Gets or sets the folder used to store the downloaded tiles.</summary>
        /// <remarks>This must be set before any call to GetTileImage.</remarks>
        private static string cacheFolder;

        public static string CacheFolder { 
            get { 
                if (string.IsNullOrEmpty(cacheFolder)) {
                    cacheFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "RNR");
                    cacheFolder = Path.Combine(cacheFolder, TileCacheFolder);
                }
                return cacheFolder;
            }
            set {
                cacheFolder = value;
            }
        }

        /// <summary>Gets the number of Tiles requested to be downloaded.</summary>
        /// <remarks>This is not the number of active downloads.</remarks>
        public static int DownloadCount { get; private set; }

        /// <summary>Returns the latitude for the specified tile number.</summary>
        /// <param name="tileY">The tile number along the Y axis.</param>
        /// <param name="zoom">The zoom level of the tile index.</param>
        /// <returns>A decimal degree for the latitude, limited to aproximately +- 85.0511 degrees.</returns>
        internal static double GetLatitude(double tileY, int zoom)
        {
            // n = 2 ^ zoom
            // lat_rad = arctan(sinh(π * (1 - 2 * ytile / n)))
            // lat_deg = lat_rad * 180.0 / π
            double tile = Clip(1 - ((2 * tileY) / Math.Pow(2, zoom)), -1, 1); // Limit value we pass to sinh
            var atan = Math.Atan(Math.Sinh(Math.PI * tile)) * 180.0 / Math.PI;
            //Debug.WriteLine("GetLatitude TileY=" + tileY.ToString() + " Zoom=" + zoom.ToString() + " => tile.Clip=" + tile.ToString() + " => degrees=" + atan.ToString());
            return atan;
        }

        /// <summary>Returns the longitude for the specified tile number.</summary>
        /// <param name="tileX">The tile number along the X axis.</param>
        /// <param name="zoom">The zoom level of the tile index.</param>
        /// <returns>A decimal degree for the longitude, limited to +- 180 degrees.</returns>
        internal static double GetLongitude(double tileX, int zoom)
        {
            // n = 2 ^ zoom
            // lon_deg = xtile / n * 360.0 - 180.0
            double degrees = tileX / Math.Pow(2, zoom) * 360.0;
            var clip = Clip(degrees, 0, 360) - 180.0; // Make sure we limit its range
            //Debug.WriteLine("GetLongitude TileX=" + tileX.ToString() + " Zoom=" + zoom.ToString()+ " => degrees=" + degrees.ToString() + " => degrees.Clip=" + clip.ToString());
            return clip;
        }

        /// <summary>Returns the maximum size, in pixels, for the specifed zoom level.</summary>
        /// <param name="zoom">The zoom level to calculate the size for.</param>
        /// <returns>The size in pixels.</returns>
        internal static double GetSize(int zoom)
        {
            return Math.Pow(2, zoom) * TileSize;
        }

        /// <summary>Returns the tile index along the X axis for the specified longitude.</summary>
        /// <param name="longitude">The longitude coordinate.</param>
        /// <param name="zoom">The zoom level of the desired tile index.</param>
        /// <returns>The tile index along the X axis.</returns>
        /// <remarks>The longitude is not checked to be valid and, therefore, the output may not be a valid index.</remarks>
        internal static double GetTileX(double longitude, int zoom)
        {
            // n = 2 ^ zoom
            // xtile = ((lon_deg + 180) / 360) * n
            var xtile = ((longitude + 180.0) / 360.0) * Math.Pow(2, zoom);
            //Debug.WriteLine("GetTileX Longitude=" + longitude.ToString() + " Zoom=" + zoom.ToString() + " => xtile=" + xtile.ToString());
            return xtile;
        }

        /// <summary>Returns the tile index along the Y axis for the specified latitude.</summary>
        /// <param name="latitude">The latitude coordinate.</param>
        /// <param name="zoom">The zoom level of the desired tile index.</param>
        /// <returns>The tile index along the Y axis.</returns>
        /// <remarks>The latitude is not checked to be valid and, therefore, the output may not be a valid index.</remarks>
        internal static double GetTileY(double latitude, int zoom)
        {
            // n = 2 ^ zoom
            // ytile = (1 - (log(tan(lat_rad) + sec(lat_rad)) / π)) / 2 * n
            double radians = latitude * Math.PI / 180.0;
            double log = Math.Log(Math.Tan(radians) + (1.0 / Math.Cos(radians)));
            var ytile = (1.0 - (log / Math.PI)) * Math.Pow(2, zoom - 1);
            //Debug.WriteLine("GetTileY Latitude=" + latitude.ToString() + " Zoom=" + zoom.ToString() + " => ytile=" + ytile.ToString());
            return ytile;
        }

        /// <summary>Returns a Tile for the specified area.</summary>
        /// <param name="zoom">The zoom level of the desired tile.</param>
        /// <param name="x">Tile index along the X axis.</param>
        /// <param name="y">Tile index along the Y axis.</param>
        /// <returns>
        /// If any of the indexes are outside the valid range of tile numbers for the specified zoom level,
        /// null will be returned.
        /// </returns>
        internal static BitmapImage GetTileImage(int zoom, int x, int y)
        {
            if (string.IsNullOrEmpty(CacheFolder))
            {
                throw new InvalidOperationException("Se debe establecer la carpeta de la cache antes de trasladar la ubicacion de una imagen.");
            }

            double tileCount = Math.Pow(2, zoom) - 1;
            if (x < 0 || y < 0 || x > tileCount || y > tileCount) // Bounds check
            {
                return null;
            }

            string localName = Path.Combine(
                CacheFolder,
                zoom.ToString(CultureInfo.InvariantCulture));
            localName = Path.Combine(localName,
                x.ToString(CultureInfo.InvariantCulture));
            localName = Path.Combine(localName,
                y.ToString(CultureInfo.InvariantCulture) + ".png");

            if (File.Exists(localName))
            {
                FileStream file = null;
                try
                {
                    file = File.OpenRead(localName);
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = file;
                    bitmap.EndInit();

                    bitmap.Freeze();
                    return bitmap;
                }
                catch (NotSupportedException) // Error creando el bitmap (corrupcion de archivo?)
                {
                    // se intenta descargar otra vez el archivo
                }
                catch (IOException) // O un error abriendo el archivo
                {
                    // se intenta descargar otra vez el archivo
                }
                finally
                {
                    if (file != null)
                    {
                        file.Dispose();
                        file = null;
                    }
                }
            }
            DownloadCount++;
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, TileFormat, zoom, x, y));
            BitmapImage image = new BitmapImage(uri);
            image.DownloadProgress += OnImageDownloadProgress;
            image.DownloadCompleted += OnImageDownloadCompleted;
            image.DownloadFailed += OnImageDownloadFailed;
            return image;
        }

        /// <summary>Returns a valid value for the specified zoom.</summary>
        /// <param name="zoom">The zoom level to validate.</param>
        /// <returns>A value in the range of 0 - MaxZoom inclusive.</returns>
        public static int GetValidZoom(int zoom)
        {
            return (int)Clip(zoom, 0, MaxZoom);
        }

        /// <summary>Returns the closest zoom level less than or equal to the specified map size.</summary>
        /// <param name="size">The size in pixels.</param>
        /// <returns>The closest zoom level for the specified size.</returns>
        internal static int GetZoom(double size)
        {
            return (int)Math.Log(size, 2);
        }

        private static double Clip(double value, double minimum, double maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }
            if (value > maximum)
            {
                return maximum;
            }
            return value;
        }

        private static void OnImageDownloadFailed(object sender, EventArgs e)
        {

        }        

        private static void OnImageDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            BitmapImage image = (BitmapImage)sender;
            if (e.Progress == 100)
            {
                image.DownloadProgress -= OnImageDownloadProgress;
                DownloadCount--;
            }

            var callback = ImageDownloadingProgress;
            if (callback != null)
            {
                callback(null, e);
            }
        }

        /// <summary>Tries to save the image to the disk for future access.</summary>
        /// <param name="sender">The source of the event, a BitmapImage</param>
        /// <param name="e">Not used by this method.</param>
        private static void OnImageDownloadCompleted(object sender, EventArgs e)
        {
            BitmapImage image = (BitmapImage)sender;
            image.DownloadCompleted -= OnImageDownloadCompleted;

            // Save our PNG
            string filename = Path.Combine(CacheFolder, image.UriSource.LocalPath.TrimStart('/'));
            FileStream file = null;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                file = File.Create(filename);
                Debug.WriteLine(String.Format("GetImage: uri={0}", image.UriSource.AbsoluteUri.ToString()));

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(file);
                /*var pngFrame = encoder.Frames[0];
                InPlaceBitmapMetadataWriter pngInplace = pngFrame.CreateInPlaceBitmapMetadataWriter();
                if (pngInplace.TrySave() == true)
                { 
                    Debug.Print("Imagen [{0}] fecha [{1}]", image.UriSource.AbsoluteUri.ToString(), pngInplace.DateTaken.ToString());
                    //pngInplace.SetQuery("/Text/Description", "Have a nice day."); 
                } */
                //pngInplace.DateTaken
            }
            catch (IOException) // Couldn't save the file
            {
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }
    }
}
