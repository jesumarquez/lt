 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="monitorUltimaPosicion.aspx.cs" Inherits="Logictracker.Monitor.MonitorUltimaPosicion.Monitor_UltimaPosicion" %>
<%@ Import Namespace="Logictracker.Culture"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
    <style type="text/css">
        html, body
        {
            height: 100%;
        }
        form
        {
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body>
    <script type="text/javascript">
        function gPP(a, b, c, d, e, f) {
            return "<table width=\"300px\"><tr><td style=\"font-weight: bold;\">" + a +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Entities", "PARENTI03") %></u>: " + b +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Entities", "PARENTI04") %></u>: " + c +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Entities", "PARENTI08") %></u>: " + d +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "DATE") %></u>: " + e +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "VELOCIDAD") %></u>: " + f +
            "km/h</td></tr></table>";
        }

        function gPOIP(a) {
            return "<table><tr><td style=\"font-weight: bold;\">" + a + "</td></tr></table>";
        }
    </script>

    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <table id="tblNoData" runat="server" width="100%" style="font-size: small; font-weight: bold;">
            <tr align="center">
                <td>
                    <cwc:ResourceLabel ID="lblNoData" runat="server" ResourceName="SystemMessages" VariableName="NO_RESULT_FOR_CURRENT_FILTERS" />
                </td>
            </tr>
        </table>
        <mon:Monitor ID="Monitor" runat="server" Height="100%" Width="100%" />
    </form>    
</body>
</html>
