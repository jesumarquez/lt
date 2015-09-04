 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Monitor.popup.MonitorPopupMobileRoutePopup" Codebehind="MobileRoutePopup.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body style="background-color: White">
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td style="font-weight: bold;">
                    <asp:Label ID="lblEsquina" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblStartDate" runat="server" ResourceName="Labels" VariableName="INICIO" /></u>: 
                    <asp:Label ID="lblInicio" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblDuration" runat="server" ResourceName="Labels" VariableName="DURACION" /></u>: 
                    <asp:Label ID="lblDuracion" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblDistance" runat="server" ResourceName="Labels" VariableName="DISTANCIA" /></u>: 
                    <asp:Label ID="lblDistancia" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblMinSpeed" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_MINIMA" /></u>: 
                    <asp:Label ID="lblVelocidadMinima" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblMaxSpeed" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA" /></u>: 
                    <asp:Label ID="lblVelocidadMaxima" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblAverageSpeed" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" /></u>: 
                    <asp:Label ID="lblVelocidadPromedio" runat="server" />
                </td>
            </tr>
         </table>
    </div>
    </form>
</body>
</html>
