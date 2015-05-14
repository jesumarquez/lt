#region Usings

using System;
using System.Globalization;

#endregion

namespace Logictracker.Web.Monitor.Markers
{
    [Serializable]
    public class LabeledMarker: Marker
    {
        string label;

        public LabeledMarker(string id, string imageUrl, double latitud, double longitud)
            :this(id, imageUrl, latitud, longitud, null, null)
        {}
        public LabeledMarker(string id, string imageUrl, double latitud, double longitud, string label, string labelStyle)
            :base(id, imageUrl, latitud, longitud)
        {
            this.label = label;
            LabelStyle = labelStyle;
        }
        public LabeledMarker(string id, string imageUrl, double latitud, double longitud, string label, string labelStyle, string popupContentHTML)
            : base(id, imageUrl, latitud, longitud, popupContentHTML)
        {
            this.label = label;
            LabelStyle = labelStyle;
        }
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public string LabelStyle { get; set; }

        public override string Code()
        {
            if (label == null) return base.Code();
            return string.Format(@"new OL.LM(new OL.LL({0},{1}), new OL.I('{2}',{3}, {4}),{5}, {6})", 
                Longitud.ToString(CultureInfo.InvariantCulture), Latitud.ToString(CultureInfo.InvariantCulture), 
                ImageUrl, Size, Offset,
                !string.IsNullOrEmpty(Label) ? "'" + Label + "'" : "null", !string.IsNullOrEmpty(LabelStyle) ? "'" + LabelStyle + "'" : "null");
        }

    }
}