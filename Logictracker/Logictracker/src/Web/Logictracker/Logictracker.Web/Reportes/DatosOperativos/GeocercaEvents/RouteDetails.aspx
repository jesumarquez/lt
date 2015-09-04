<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.DatosOperativos.GeocercaEvents.EstadisticaGeocercaEventsRouteDetails" Codebehind="RouteDetails.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
         
        <table style="font-size: x-small; width: 100%; background-color: White; border: solid 1px Black">
            <tr align="center" style="height: 25px; background-color: #f7f7f7;">
                <td style="font-weight: bold; border: solid 1px Black;">
                    <asp:Label ID="lblTitle" runat="server" />
                </td>
            </tr>
            <tr style="height: 5px"><td></td></tr>
            <tr>
                <td>
                    <table id="tblResults" runat="server">
                        <tr style="height: 20px">
                            <td>
                                <strong><cwc:ResourceLabel ID="lblStart" runat="server" ResourceName="Labels" VariableName="INICIO" />: </strong>
                                <asp:Label ID="lblInicio" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="EN_MOVIMIENTO" />: </strong>
                                <asp:Label ID="lblMovimiento" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="KILOMETROS" />: </strong>
                                <asp:Label ID="lblKilometros" runat="server" />
                            </td>
                        </tr>
                        <tr style="height: 20px">
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="FIN" />: </strong>
                                <asp:Label ID="lblFin" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="EN_DETENCION" />: </strong>
                                <asp:Label ID="lblDetencion" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" />: </strong>
                                <asp:Label ID="lblVelocidadPromedio" runat="server" />
                            </td>
                        </tr>
                        <tr style="height: 20px">
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="DURACION" />: </strong>
                                <asp:Label ID="lblDuracion" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="EN_INFRACCION" />: </strong>
                                <asp:Label ID="lblInfraccion" runat="server" />
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA" />: </strong>
                                <asp:Label ID="lblVelocidadMaxima" runat="server" />
                            </td>
                        </tr>
                         <tr style="height: 20px">
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <strong><cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="SIN_REPORTAR" />: </strong>
                                <asp:Label ID="lblSinReportar" runat="server" />
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    
                    <%--ERRORLABEL--%>
                    <cwc:InfoLabel ID="infoLabel1" runat="server" />
                </td>
            </tr>
            <tr style="height: 5px"><td></td></tr>
            <tr>
                <td>
                    <strong><cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="LINKS" />: </strong>
                    <asp:UpdatePanel ID="upLinkHistorico" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:ResourceLinkButton ID="lnkMonitorHistorico" runat="server" onclick="LnkMonitorHistoricoClick" ResourceName="Menu"
                                VariableName="OPE_MON_HISTORICO" />
                            <a>, </a>
                            <cwc:ResourceLinkButton ID="lnkResumenDeRuta" runat="server" onclick="LnkResumenDeRutaClick" ResourceName="Menu"
                                VariableName="STAT_RESUMEN_RUTA" />
                            <a>, </a>
                            <cwc:ResourceLinkButton ID="lnkReporteDeEventos" runat="server" onclick="LnkReporteDeEventosClick" ResourceName="Menu"
                                VariableName="DOP_REP_EVENTOS" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lnkMonitorHistorico" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lnkResumenDeRuta" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="lnkReporteDeEventos" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>                    
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
