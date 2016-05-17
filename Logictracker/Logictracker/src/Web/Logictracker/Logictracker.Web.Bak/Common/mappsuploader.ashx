<%@ WebHandler Language="C#" Class="mappsuploader" %>

using System;
using System.Web;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using System.IO;

public class mappsuploader : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        try
        {
            String requestform = context.Request.Form["rf"];
            String imei = context.Request.Form["imei"];
            String filename = context.Request.Form["filename"];
            String exdata = context.Request.Form["exdata"];
            String b64image = context.Request.Form["data"];


            // para debug
            /*if (!String.IsNullOrEmpty(requestform))
            {
                doForm(context);
                return;
            }*/

            // valido
            if (String.IsNullOrEmpty(imei) || String.IsNullOrEmpty(imei) ||
                String.IsNullOrEmpty(imei) || String.IsNullOrEmpty(imei))
            {
                doForm(context);
                /*context.Response.ContentType = "text/plain";
                context.Response.Write("BAD REQUEST");*/
                return;
            }

            // localizo dispositivo
            var device = new DAOFactory().DispositivoDAO.GetByIMEI(imei);
            if (device == null)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("ERR1");
            }

            byte[] encodedDataAsBytes = System.Convert.FromBase64String(b64image);
            filename = device.Id.ToString("D4") + "-" + filename.Split('-')[1];
            var basePath = Path.Combine(Config.Directory.PicturesDirectory, device.Id.ToString("D4"));
            filename = Path.Combine(basePath, filename);
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            var file = new FileStream(filename, FileMode.Create);
            var writer = new BinaryWriter(file);
            writer.Write(encodedDataAsBytes);
            writer.Flush();
            writer.Close();
            context.Response.ContentType = "text/plain";
            //context.Response.Write("FILE=" + filename + ".");
            //context.Response.ContentType = "text/plain";
            context.Response.Write("OK");
        }
        catch (Exception e)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("EXCEPTION: " + e.Message + "\r\n");
            context.Response.Write(e.StackTrace);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    private void doForm(HttpContext context)
        
    {
        context.Response.ContentType = "text/html";
        context.Response.Write("<html><form method=POST>");
        context.Response.Write("<input name=imei value='353082050555912'></input>");
        //context.Response.Write("<input name=imei value='356459043688033'></input>");        
        context.Response.Write("<input name=filename value='353082050555912-ABCD.jpg'></input>");
        context.Response.Write("<input name=exdata value='123'></input>");
        context.Response.Write("<input name=data value='ABCD'></input>");
        context.Response.Write("<input type=submit name=btn></input>");
        context.Response.Write("</form></html>");
    }

}