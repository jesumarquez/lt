using System;
using System.IO;
using Urbetrack.Toolkit;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Clear();
        Response.ContentType = "text/plain";
        // FILE = SERVER & "default.aspx?o=ini&f=" & FLEET & "&v=" & FileVersion( UMBIN ) & "&dt=hw6945&sn=" & serial & "&ds=" & FreeDiskSpace("\")
        var version = Request.QueryString["v"];
        var serial = Request.QueryString["sn"];
        var output = Request.QueryString["o"];
        var device_type = Request.QueryString["dt"];
        var fleet = Request.QueryString["f"];
        var free_disk_space = Request.QueryString["ds"];

        if (String.IsNullOrEmpty(version))
        {   
            Response.Write("# Device Version Required");
            return;
        }
        if (String.IsNullOrEmpty(serial))
        {
            serial = "undefined";
        }
        Response.Write("# Update Metadata For:" + serial + " Format:" + output + "\r\n");
        Response.Write("# Fleet:" + fleet + "\r\n");
        Response.Write("# Device Type:" + device_type + "\r\n");
        Response.Write("# Available Space:" + free_disk_space + "\r\n");
        var path = Request.PhysicalPath;
        path = path.Substring(0, path.LastIndexOf('\\'));
        var filename = path + @"\" + System.Web.Configuration.WebConfigurationManager.AppSettings["update_rules_file"];
        Response.Write("# Server Configuration File:" + filename + "\r\n");

        if (String.IsNullOrEmpty(filename) || (!File.Exists(filename)))
        {
            Response.Write("[Urbemobile]\r\n");
            Response.Write("UpdateService=Configuration Not Found.\r\n");
            Response.Write("State=UPTODATE\r\n");
            return;
        }

        var section = "All";
        if (!String.IsNullOrEmpty(serial) && IniFile.Get(filename, serial, "Active", "False") == "True")
        {
            Response.Write("# Device Custom Upgrade\r\n");
            section = serial;
        }
        
        var device_version = Convert.ToInt32(version);
        var condition_version = Convert.ToInt32(IniFile.Get(filename, section, "ConditionVersion", "0"));

        Response.Write("[Urbemobile]\r\n");

        if (device_version <= condition_version )
        {
            Response.Write("State=REQUPDATE\r\n");
            Response.Write("Source=" + IniFile.Get(filename, section, "Source", "error") + "\r\n");
            return;
        }
        Response.Write("State=UPTODATE\r\n");
    }
}
