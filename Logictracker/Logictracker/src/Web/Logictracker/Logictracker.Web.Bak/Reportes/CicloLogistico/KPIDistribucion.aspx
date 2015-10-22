<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="KpiDistribucion.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.KpiDistribucion" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
<div id="tablerocontrol">
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="KPI_DISTRIBUCION" /> 
    <div id="divResumen">
        <table>
            <tr>
                <td class="filterpanel">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="160px" AddAllItem="false" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="True" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="160px" AddAllItem="true" ParentControls="ddlEmpresa" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="True" />
                    <br />
                    <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" Width="160px" AddAllItem="true" ParentControls="ddlEmpresa,ddlPlanta" />
                </td>
                <td class="filterpanel">
                    <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="ddlMovil" Font-Bold="True" ForeColor="Black" />
                    <br />
                    <cwc:MovilListBox ID="ddlMovil" runat="server" Width="160px" AddAllItem="false" ParentControls="ddlEmpresa,ddlPlanta,ddlTransportista" SelectionMode="Multiple" />
                </td>
                <td >
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <table class="infobox">
                                                <tr>
                                                    <td>
                                                        <cwc:ResourceLabel ID="lblFlotaActiva" runat="server" ResourceName="Labels" VariableName="FLOTA_ACTIVA" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblActiva" runat="server" Text="0"  class="homeData" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table class="infobox">
                                                <tr>
                                                    <td>
                                                        <cwc:ResourceLabel ID="lblFlotaInactiva" runat="server" ResourceName="Labels" VariableName="FLOTA_INACTIVA"  />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblInactiva" runat="server" Text="0"  CssClass="homeData" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>
                                            <table class="infobox">
                                                <tr>
                                                    <td>
                                                        <cwc:ResourceLabel ID="lblFlotaMantenimiento" runat="server" ResourceName="Labels" VariableName="FLOTA_MANTENIMIENTO" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblMantenimiento" runat="server" Text="0" CssClass="homeData" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="right" valign="middle">
                                            <asp:Button ID="btnCalcular" runat="server" Text="Calcular" OnClick="BtnCalcular_OnClick" />
                                        </td>
                                    </tr>
                                </table>                
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>    
    
    <!--ESTADISTICAS HOY -->            
    <div id="divHoy" runat="server" Visible="False">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleVariableName="KILOMETROS" TitleResourceName="Labels">
                        <table width="100%" border="0">                                    
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeKilometros" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeKilometros" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="1000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="1000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="KM_TOTALES" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmTotales" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="KM_EN_VIAJE" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmEnViaje" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr valign="middle">
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeKmMensual" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeKmMensual" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="2000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="2000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Labels" VariableName="KM_TOTALES_MENSUAL" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmMensual" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel17" runat="server" ResourceName="Labels" VariableName="KM_EN_VIAJE_MENSUAL" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmEnViajeMensual" runat="server" CssClass="labelGaugeAmarillo"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                                            
                                </td>                           
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeKmPromedio" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeKmPromedio" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="1000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="1000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>                                                        
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel18" runat="server" ResourceName="Labels" VariableName="KM_TOTAL_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmPromedio" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel19" runat="server" ResourceName="Labels" VariableName="KM_EN_VIAJE_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmEnViajePromedio" runat="server" CssClass="labelGaugeAmarillo"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                                            
                                </td>
                            </tr>
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeProductivos" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeProductivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="500" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="500" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="KM_PRODUCTIVOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmProductivos" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="KM_IMPRODUCTIVOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmImproductivos" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr valign="middle">
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeProductivosMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeProductivosMes" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="2000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="2000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel20" runat="server" ResourceName="Labels" VariableName="KM_PRODUCTIVOS_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmProductivosMensual" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel21" runat="server" ResourceName="Labels" VariableName="KM_IMPRODUCTIVOS_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmImproductivosMensual" runat="server" CssClass="labelGaugeAmarillo"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                                            
                                </td>                           
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeKmProductivosPromedio" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeKmProductivosPromedio" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="500" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="500" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>                                                        
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel22" runat="server" ResourceName="Labels" VariableName="KM_PRODUCTIVOS_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmProductivosPromedio" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel26" runat="server" ResourceName="Labels" VariableName="KM_IMPRODUCTIVOS_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmImproductivosPromedio" runat="server" CssClass="labelGaugeAmarillo"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                                            
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleVariableName="TIEMPO_MOVIMIENTO" TitleResourceName="Labels">
                        <table width="100%" border="0">                                    
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMovimiento" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeMovimiento" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="200" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="200" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO_HORAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblTiempoMovimiento" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMovimientoMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeMovimientoMensual" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="1000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="1000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel27" runat="server" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblTiempoMovimientoMensual" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMovimientoPromedio" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugePromedio" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="200" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="200" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel32" runat="server" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO_PROMEDIO_DIARIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblTiempoMovimientoPromedio" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="TitledPanel6" runat="server" TitleVariableName="PARTICK01" TitleResourceName="Entities">
                        <table width="100%" border="0">                                    
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEntregas" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeEntregas" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="100" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel23" runat="server" ResourceName="Labels" VariableName="ENTREGAS_TOTAL" Font-Bold="true" />:
                                                            <asp:Label ID="lblEntregas" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel34" runat="server" ResourceName="Labels" VariableName="ENTREGAS_REALIZADAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblRealizadas" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEntregasMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeEntregasMensual" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="2000" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="2000" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel31" runat="server" ResourceName="Labels" VariableName="ENTREGAS_TOTAL_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblEntregasMensual" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel35" runat="server" ResourceName="Labels" VariableName="ENTREGAS_REALIZADAS_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblRealizadasMensual" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEntregasPromedio" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeEntregasPromedio" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="100" Location="25">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="232, 177, 42" />
                                                                                <Filling BrushType="Gradient" Color="255, 215, 106" Color2="255, 229, 155" />
                                                                                <Gradient Direction="Horizontal" />
                                                                                <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                            </c1:C1GaugePointer>
                                                                        </MorePointers>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeRectangle CornerRadius="5" Height="-1.1" Name="Face" Width="-1.1">
                                                                                <Border Color="141, 178, 227" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CornerRadius="3" Height="-0.95" Width="-1.05">
                                                                                <Border Color="243, 248, 254" Thickness="1" />
                                                                                <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle Height="-0.5">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="193, 216, 242" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.75" Height="2" Width="-1.005">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeRectangle>
                                                                            <c1:C1GaugeRectangle CenterPointY="0.85" Height="3" Width="-1.005">
                                                                                <Border Color="96, 136, 190" />
                                                                                <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeRectangle>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeEllipse CenterPointY="0.05" Width="-1.2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" Opacity="0.4" />
                                                                                <Clippings>
                                                                                    <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                                                                </Clippings>
                                                                            </c1:C1GaugeEllipse>
                                                                        </CoverShapes>
                                                                        <Pointer Alignment="In" FlipShape="True" Length="70" Offset="92" Shape="Custom" Value="0">
                                                                            <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="Horizontal" />
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Pointer>
                                                                    </c1:C1LinearGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel33" runat="server" ResourceName="Labels" VariableName="ENTREGAS_TOTAL_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblEntregasPromedio" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel36" runat="server" ResourceName="Labels" VariableName="ENTREGAS_REALIZADAS_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblRealizadasPromedio" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
             <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="TitledPanel7" runat="server" TitleVariableName="PRODUCTIVIDAD" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="50%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <br />                                          
                                                            <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="TIEMPO_MINIMO_DIA" Font-Bold="true" />:
                                                            <asp:Label ID="lblMinimo" runat="server"/>
                                                            <br /><br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel37" runat="server" ResourceName="Labels" VariableName="TIEMPO_MAXIMO_DIA" Font-Bold="true" />:
                                                            <asp:Label ID="lblMaximo" runat="server"/>
                                                            <br /><br />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="50%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <br />                                          
                                                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="TIEMPO_MINIMO_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblMinimoMes" runat="server"/>
                                                            <br /><br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="TIEMPO_MAXIMO_MES" Font-Bold="true" />:
                                                            <asp:Label ID="lblMaximoMes" runat="server"/>
                                                            <br /><br />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
    </div>
    
    <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="300000" OnTick="Timer_OnTick" />
</div>
</asp:Content>


