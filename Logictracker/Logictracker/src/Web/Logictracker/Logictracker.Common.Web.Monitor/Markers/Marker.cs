#region Usings

using System;
using System.Globalization;

#endregion

namespace Logictracker.Web.Monitor.Markers
{
    [Serializable]
    public class Marker
    {
        string imageUrl;
        double latitud;
        double longitud;
        string offset = DrawingFactory.GetOffset(-10, -10);
        string size = DrawingFactory.GetSize(20, 20);

        public Marker(string id, string imageUrl, double latitud, double longitud)
            :this(id, imageUrl, latitud, longitud, null)
        {
        }
        public Marker(string id, string imageUrl, double latitud, double longitud, string size, string offset)
            : this(id, imageUrl, latitud, longitud, null, size, offset)
        {
        }
        public Marker(string id, string imageUrl, double latitud, double longitud, string popupContentHTML)
        {
            Id = id;
            this.imageUrl = imageUrl;
            this.latitud = latitud;
            this.longitud = longitud;
            PopupContentHTML = popupContentHTML;
        }
        public Marker(string id, string imageUrl, double latitud, double longitud, string popupContentHTML, string size,
                      string offset)
        {
            Id = id;
            this.imageUrl = imageUrl;
            this.latitud = latitud;
            this.longitud = longitud;
            PopupContentHTML = popupContentHTML;
            this.size = size;
            this.offset = offset;
        }

        public string Id { get; set; }

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }
        public double Latitud
        {
            get { return latitud; }
            set { latitud = value; }
        }
        public double Longitud
        {
            get { return longitud; }
            set { longitud = value; }
        }

        public string PopupContentHTML { get; set; }

        public string Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public virtual string Code()
        {
            return string.Format(
                @"new OL.M(new OL.LL({0},{1}), new OL.I('{2}', {3}, {4}))",
                longitud.ToString(CultureInfo.InvariantCulture), latitud.ToString(CultureInfo.InvariantCulture),
                imageUrl, size, offset);
        }
    }
}