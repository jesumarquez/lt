<%@ WebHandler Language="C#" Class="EditImage" %>

#region Usings

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Web;
using Logictracker.Web.BaseClasses.Handlers;

#endregion

/// <summary>
/// Handler for editing images.
/// </summary>
public class EditImage : BaseHandler
{
    #region Protected Properties

    /// <summary>
    /// Defines the handler as reusable.
    /// </summary>
    public override bool IsReusable { get { return true; } }
    
    #endregion

    #region Protected Methods

    /// <summary>
    /// Performs the handler tasks.
    /// </summary>
    /// <param name="context"></param>
    protected override void DoIt(HttpContext context)
    {
        context.Response.ContentType = "image/png";
        var imgName = context.Request.QueryString["file"];
        
        if (string.IsNullOrEmpty(imgName)) return;

        using (var img = Image.FromFile(context.Server.MapPath(imgName)))
        {
            var im = img;
            
            foreach (var key in context.Request.QueryString.AllKeys)
            {
                switch (key)
                {
                    case "angle": im = ChangeAngle(context, im); break;
                    case "write": im = WriteText(context, im); break;
                }
            }

            var mem = new MemoryStream();
            
            im.Save(mem, ImageFormat.Png);
            
            context.Response.OutputStream.Write(mem.ToArray(), 0, (int)mem.Length);
            
            im.Dispose();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Rotetas the specified image.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="im"></param>
    /// <returns></returns>
    private static Image ChangeAngle(HttpContext context, Image im)
    {
        var b = (Image)im.Clone();
        var angle = context.Request.QueryString["angle"];
        var degrees = !string.IsNullOrEmpty(angle) ? (float)Convert.ToDouble(angle) : 0;
        var gp = Graphics.FromImage(b);
        
        gp.Clear(Color.Transparent);
        gp.TranslateTransform((float)im.Width / 2, (float)im.Height / 2);
        gp.RotateTransform(degrees);
        gp.TranslateTransform(-(float)im.Width / 2, -(float)im.Height / 2);
        gp.DrawImage(im, 0, 0);
        
        return b;
    }
    
    /// <summary>
    /// Adds the specified text to the image.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="im"></param>
    /// <returns></returns>
    private static Image WriteText(HttpContext context, Image im)
    {
        var text = context.Request.QueryString["write"];
        
        float size, x, y;
        
        if (!float.TryParse(context.Request.QueryString["textsize"], NumberStyles.Any, CultureInfo.InvariantCulture, out size)) size = 10.0F;
        if (!float.TryParse(context.Request.QueryString["textx"], NumberStyles.Any, CultureInfo.InvariantCulture, out x)) x = (float)(im.Width / 2.0);
        if (!float.TryParse(context.Request.QueryString["texty"], NumberStyles.Any, CultureInfo.InvariantCulture, out y)) y = (float)(im.Height / 2.0);
        
        var gp = Graphics.FromImage(im);
        var f = new Font(FontFamily.GenericSansSerif, size, FontStyle.Bold, GraphicsUnit.Pixel);
        var fmt = StringFormat.GenericDefault;
        
        fmt.Alignment = StringAlignment.Center;
        fmt.LineAlignment = StringAlignment.Center;
        
        gp.DrawString(text, f, Brushes.White, x,y, fmt);
        
        return im;
    }

    #endregion
}