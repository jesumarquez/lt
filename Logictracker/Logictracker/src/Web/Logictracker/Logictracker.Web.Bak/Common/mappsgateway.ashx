<%@ WebHandler Language="C#" Class="mappsgateway" %>

using System;
using System.Web;
using Logictracker.Configuration;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class mappsgateway : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        try
        {
            String dgram = context.Request.Form["dgram"];
            /*context.Response.ContentType = "text/plain";
            context.Response.Write("DGRAM='" + dgram + "'");
            return;*/
            // para debug
            if (String.IsNullOrEmpty(dgram))
            {
                doForm(context);
                return;
            }

            UdpClient udpc = new UdpClient();
           
            udpc.Send(Encoding.ASCII.GetBytes(dgram), dgram.Length, Config.HttpGateway.Hostname, Config.HttpGateway.Port);
            IPEndPoint recevieAddress = new IPEndPoint(IPAddress.Any, 0);
            udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
            byte[] reply = udpc.Receive(ref recevieAddress);
            
            context.Response.ContentType = "text/plain";
            //context.Response.Write("FILE=" + filename + ".");
            //context.Response.ContentType = "text/plain";
            context.Response.Write(Encoding.ASCII.GetString(reply));
            
            try { 
				udpc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
				reply = udpc.Receive(ref recevieAddress);
				context.Response.Write(Encoding.ASCII.GetString(reply));
            } catch(Exception) { 
            }
            udpc.Close();
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
        context.Response.Write("<input name=dgram value='>RH,0,353082051266592,0,rt;ID=0;MSG=ABCD;*64<'></input>");        
        context.Response.Write("<input type=submit name=btn></input>");
        context.Response.Write("</form></html>");
    }

}