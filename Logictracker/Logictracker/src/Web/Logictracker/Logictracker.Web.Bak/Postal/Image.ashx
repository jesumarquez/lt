<%@ WebHandler Language="C#" Class="Image" %>

using System.Web;
using Logictracker.DAL.Factories;

public class Image : IHttpHandler {

    private DAOFactory _daof;
    public DAOFactory DAOFactory { get { return _daof ?? (_daof = new DAOFactory()); } }
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "image/jpeg";

        var pieza = context.Request.QueryString["i"];
        if(pieza == null)
        {
            ShowNoImage(context);
            return;
        }

        var route = DAOFactory.RutaDAO.GetRouteByPieza(pieza);
        if (route == null || route.Foto == null)
        {
            ShowNoImage(context);
            return;
        }
        using(var mem = new System.IO.MemoryStream(route.Foto))
        {
            var bmp = System.Drawing.Image.FromStream(mem);
            bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            mem.Close();
        }
        
        
    }

    private void ShowNoImage(HttpContext context)
    {
        using (var bmp = System.Drawing.Image.FromFile(context.Server.MapPath("noimage.gif")))
        {
            bmp.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}