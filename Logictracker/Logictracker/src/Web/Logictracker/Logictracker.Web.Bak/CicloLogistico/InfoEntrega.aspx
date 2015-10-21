 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoEntrega.aspx.cs" Inherits="Logictracker.CicloLogistico.InfoEntrega" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <style type="text/css">
        body { background-color: White; }
    </style>
    <form id="form1" runat="server">
    <div>
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <asp:Label ID="lblEntrega" runat="server" Font-Bold="True" />
                    <br/><br/>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="lblTitViaje" runat="server" ResourceName="Menu" VariableName="CLOG_DISTRIBUCION" />:</b>
                    <asp:Label ID="lblViaje" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI03" />:</b>
                    <asp:Label ID="lblVehiculo" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="ESTADO" />:</b>
                    <asp:Label ID="lblEstado" runat="server" />
                </td>
            </tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DIRECCION" />:</b>
                    <asp:Label ID="lblDireccion" runat="server" />
                </td>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="ENTRADA" />:</b>
                    <asp:Label ID="lblEntrada" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="MANUAL" />:</b>
                    <asp:Label ID="lblManual" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b><cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="SALIDA" />:</b>
                    <asp:Label ID="lblSalida" runat="server" />
                    <br/><br/>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <cwc:ResourceLinkButton ID="lnkMonitorCiclo" runat="server" OnClick="LnkMonitorCicloOnClick" ResourceName="Labels" VariableName="VER_MONITOR_CICLO" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
