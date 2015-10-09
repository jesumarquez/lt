using System;
using System.Linq;
using System.Web.UI.HtmlControls;
using Logictracker.Configuration;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using System.IO;
using System.Collections.Generic;

namespace Logictracker.Common.Pictures
{
    public partial class ShowPictures : BasePage
    {
        protected override InfoLabel LblInfo { get { return null; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadPictures();
        }

        protected void lblNotFoundRefreshLink_Click(object sender, EventArgs e)
        {
            LoadPictures();
        }

        private void LoadPictures()
        {
            panelGallery.Visible = false;
            panelNotFound.Visible = true;

            var evento = Request.QueryString["e"];
            int idEvento;
            if (evento == null || !int.TryParse(evento, out idEvento))
            {
                var d = Request.QueryString["d"];
                var f = Request.QueryString["f"];
                var t = Request.QueryString["t"];
                int idDisp;
                DateTime desde;
                DateTime hasta;

                if (d != null && int.TryParse(d, out idDisp)
                 && f != null && DateTime.TryParse(f, out desde)
                 && t != null && DateTime.TryParse(t, out hasta))
                {
                    LoadPictures(idDisp, desde, hasta);
                }

                return;
            }
            var logEvento = DAOFactory.LogMensajeDAO.FindById(idEvento);
            DateTime from;
            DateTime to;
            if (logEvento.FechaFin.HasValue)
            {
                from = logEvento.Fecha;
                to = logEvento.FechaFin.Value;
            }
            else
            {
                var segs = logEvento.Accion != null && logEvento.Accion.SegundosFoto > 0 ? logEvento.Accion.SegundosFoto : 0;
                from = logEvento.Fecha.AddSeconds(-segs);
                to = logEvento.Fecha;
            }
            LoadPictures(logEvento.Dispositivo.Id, from, to);
        }

        private void LoadPictures(int idDisp, DateTime from, DateTime to)
        {
			var dir = Path.Combine(Config.Directory.PicturesDirectory, idDisp.ToString("D4"));
            if (!Directory.Exists(dir))
            {
                return;
            }
            var pre = new List<string>();
            var tmp = from;
            while (tmp <= to)
            {
                pre.Add(tmp.ToString("ddMMyy"));
                tmp = tmp.AddDays(1);
            }
            var files = pre.Distinct().SelectMany(p => Directory.GetFiles(dir, string.Format("{0:D4}-{1}*.jp*g", idDisp, p))).OrderBy(f => GetDateFromFileName(f));
            var root = Server.MapPath("~");
            foreach (var file in files)
            {
                var dateTime = GetDateFromFileName(file);
                if (!dateTime.HasValue) continue;
                if (dateTime < from || dateTime > to) continue;

                var relPath = "~/" + file.Substring(root.Length);
                AddPicture(ResolveUrl(relPath), dateTime.Value);

                panelGallery.Visible = true;
                panelNotFound.Visible = false;
            }
            litDir.Text = ResolveUrl("~/" + dir.Substring(root.Length));
        }

        private static DateTime? GetDateFromFileName(string fileName)
        {
            try
            {
                var fileParts = Path.GetFileNameWithoutExtension(fileName).Split('-');
                var timePart = fileParts[1];

                var day = Convert.ToInt32(timePart.Substring(0, 2));
                var month = Convert.ToInt32(timePart.Substring(2, 2));
                var year = 2000 + Convert.ToInt32(timePart.Substring(4, 2));
                var hour = Convert.ToInt32(timePart.Substring(6, 2));
                var minute = Convert.ToInt32(timePart.Substring(8, 2));
                var second = Convert.ToInt32(timePart.Substring(10, 2));

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch
            {
                return null;
            }
        }

        private void AddPicture(string filename, DateTime dateTime)
        {
            var li = new HtmlGenericControl("li");
            li.Attributes.Add("value", Path.GetFileName(filename));
            li.Attributes.Add("time", dateTime.ToDisplayDateTime().ToString("dd-MM-yyyy HH:mm:ss"));
            var img = new HtmlGenericControl("img");
            img.Attributes.Add("src", filename);
            img.Attributes.Add("width", "133");
            img.Attributes.Add("height", "100");
            img.Attributes.Add("alt", "");
            li.Controls.Add(img);
            thumbs.Controls.Add(li);
        }
    }
}