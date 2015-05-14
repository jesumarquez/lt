#region Usings

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

#endregion

namespace Logictracker.Culture
{
    public class ResourceImageHandler : IHttpHandler
    {
        #region Implementation of IHttpHandler

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/png";
            context.Response.Cache.SetExpires(DateTime.Now.AddMonths(1));
            context.Response.Cache.SetSlidingExpiration(true);
            context.Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            //context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
            //context.Response.AddHeader("Last-Modified", DateTime.UtcNow.ToLongDateString());

            var filename = Path.GetFileNameWithoutExtension(context.Request.Path);
            var image = CultureManager.GetMenuImage(filename);

            if (image == null) return;

            var mem = new MemoryStream();

            image.Save(mem, ImageFormat.Png);

            context.Response.OutputStream.Write(mem.ToArray(), 0, (int)mem.Length);

            image.Dispose();
        }

        public bool IsReusable
        {
            get { return true; }
        }

        #endregion
    }
}
