<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DetalleVehiculo.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.ResumenVehicular.AccidentologiaResumenVehicularDetalleVehiculo" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
        <table width="100%">
            <tr align="center" valign="bottom">
                <td>
                    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    
                    <%--ERRORLABEL--%>
                    <cwc:InfoLabel ID="infoLabel1" runat="server" />
                    
                    <asp:Label ID="lblInterno" runat="server" Font-Bold="true" Font-Size="Small" />
                    
                    <br />
                    <br />
                    
                    <table style="font-size:x-small; border-style:solid; border-width:1px; background-color:White" width="100%">
                        <tr align="left">
                            <td>
                                <cwc:ResourceLabel ID="lblRecorrido" runat="server" ResourceName="Labels" VariableName="RECORRIDO"
                                    Font-Bold="true" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblTotalRecorrido" runat="server" ResourceName="Labels" VariableName="TOTAL" /></u>: 
                                <asp:Label ID="lblTotal" runat="server" />
                            </td>
                            <td colspan="3">
                                <u><cwc:ResourceLabel ID="lblPromedioDiario" runat="server" ResourceName="Labels" VariableName="PROMEDIO_DIARIO" /></u>: 
                                <asp:Label ID="lblDistanciaPromedio" runat="server" />
                            </td>
                        </tr>
                        <tr align="left">
                            <td>
                                <cwc:ResourceLabel ID="lblDia" runat="server" ResourceName="Labels" VariableName="DIAS" Font-Bold="true" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblTotales" runat="server" ResourceName="Labels" VariableName="TOTALES" /></u>: 
                                <asp:Label ID="lblDias" runat="server" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblConActividad" runat="server" ResourceName="Labels" VariableName="CON_ACTIVIDAD" /></u>: 
                                <asp:Label ID="lblActivo" runat="server" />
                            </td>
                            <td colspan="2">
                                <u><cwc:ResourceLabel ID="lblSinActividad" runat="server" ResourceName="Labels" VariableName="SIN_ACTIVIDAD" /></u>: 
                                <asp:Label ID="lblInactivo" runat="server" />
                            </td>
                        </tr>
                        <tr align="left">
                            <td>
                                <cwc:ResourceLabel ID="lblVelocidad" runat="server" ResourceName="Labels" VariableName="VELOCIDAD"
                                    Font-Bold="true" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblMaxima" runat="server" ResourceName="Labels" VariableName="MAXIMA_ALCANZADA" /></u>: 
                                <asp:Label ID="lblAlcanzada" runat="server" />
                            </td>
                            <td colspan="3">
                                <u><cwc:ResourceLabel ID="lblPromedio" runat="server" ResourceName="Labels" VariableName="PROMEDIO" /></u>: 
                                <asp:Label ID="lblVelocidadPromedio" runat="server" />
                            </td>
                        </tr>
                        <tr align="left">
                            <td>
                                <cwc:ResourceLabel ID="lblTiempos" runat="server" ResourceName="Labels" VariableName="TIEMPOS" Font-Bold="true" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblEnMovimiento" runat="server" ResourceName="Labels" VariableName="MOVIMIENTO" /></u>: 
                                <asp:Label ID="lblMovimiento" runat="server" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblDetenido" runat="server" ResourceName="Labels" VariableName="DETENCION" /></u>: 
                                <asp:Label ID="lblDetencion" runat="server" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblNoReportado" runat="server" ResourceName="Labels" VariableName="SIN_REPORTAR" /></u>: 
                                <asp:Label ID="lblSinReportar" runat="server" />
                            </td>
                        </tr>
                        <tr align="left">
                            <td>
                                <cwc:ResourceLabel ID="lblEnInfraccion" runat="server" ResourceName="Labels" VariableName="INFRACCIONES"
                                    Font-Bold="true" />
                            </td>
                            <td>
                                <u><cwc:ResourceLabel ID="lblCantidad" runat="server" ResourceName="Labels" VariableName="CANTIDAD" /></u>: 
                                <asp:Label ID="lblInfracciones" runat="server" />
                            </td>
                            <td colspan="3">
                                <u><cwc:ResourceLabel ID="lblTiempo" runat="server" ResourceName="Labels" VariableName="TIEMPO" /></u>: 
                                <asp:Label ID="lblInfraccion" runat="server" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
