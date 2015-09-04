<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.MonitorEstadoEntregas" Codebehind="MonitorEstadoEntregas.aspx.cs" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="c1" Namespace="C1.Web.UI.Controls.C1Gauge" Assembly="C1.Web.UI.Controls.3, Version=3.5.20113.220, Culture=neutral, PublicKeyToken=9b75583953471eea" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
    <script type="text/javascript">
        function getEntregaP(id)
        {
            return "<iframe width=\"350\" height=\"210\" style=\"border:none;\" src=\"InfoEntrega.aspx?id=" + id + "\" />";
        }
    </script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <cwc:ResourceLayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" ResourceName="Menu" VariableName="MONITOR_ESTADO_ENTREGAS" tabPosition="top" ></cwc:ResourceLayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="260" split="false" minSize="260" maxSize="260" collapsible="true" title="Filtros"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="220" split="false" minSize="260" maxSize="220" collapsible="true" title="Estados"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" runat="server"></cc1:LayoutManager>
        
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server"  >
             <asp:UpdatePanel ID="updTabCompleto" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="panelFiltros" DefaultButton="btnSearch">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <table id="tbFilters" style="background-color: #D4D0C8;">
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="100%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="CbLineaSelectedIndexChanged"  />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PARENTI04" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa,cbLinea" AddAllItem="true"  />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" VariableName="PARENTI37" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLinea,cbDepartamento" AddAllItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="PARENTI99" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:SubCentroDeCostosDropDownList ID="cbSubCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLinea,cbDepartamento,cbCentroDeCostos" AddAllItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="PARENTI07" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbLinea" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th valign="top">
                                            <div class="header">
                                                <cwc:ResourceLabel ID="lblVehiculo" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                                <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="lblVehiculo" ListControlId="cbVehiculo" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:MovilListBox ID="cbVehiculo" runat="server" ParentControls="cbLinea,cbTransportista,cbDepartamento,cbCentroDeCostos,cbSubCentroDeCostos" Width="100%" Height="85px" SelectionMode="Multiple" HideWithNoDevice="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th valign="top">
                                            <div class="header">
                                                <cwc:ResourceLabel ID="lblEstado" runat="server" VariableName="ESTADO_ENTREGA" ResourceName="Labels" />
                                                <cwc:SelectAllExtender ID="SelectAllExtender1" runat="server" AutoPostBack="true" TargetControlId="lblEstado" ListControlId="cbEstado" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:EstadoEntregaDistribucionListBox ID="cbEstado" runat="server" Width="100%" Height="120px" SelectionMode="Multiple" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th valign="top">
                                            <div class="header">
                                                <cwc:ResourceLabel ID="lblEstadoRuta" runat="server" VariableName="ESTADO_RUTA" ResourceName="Labels" />
                                                <cwc:SelectAllExtender ID="SelectAllExtender2" runat="server" AutoPostBack="true" TargetControlId="lblEstadoRuta" ListControlId="cbEstadoRuta" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:EstadoViajeDistribucionListBox ID="cbEstadoRuta" runat="server" Width="100%" Height="60px" SelectionMode="Multiple" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="header">
                                            <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker runat="server" ID="dtFecha" Mode="Date" TimeMode="Start" IsValidEmpty="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtnSearchClick" ResourceName="Controls" VariableName="BUTTON_SEARCH" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <br/>
                                            <cwc:ResourceLinkButton ID="lnkEstadoEntregas" runat="server" VariableName="IR_ESTADO_ENTREGAS" ResourceName="Labels" OnClientClick="window.open('../Reportes/Estadistica/ReporteDistribucion.aspx','Estado de Entregas')" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <br/>
                                            <asp:UpdatePanel ID="upDistrGlobal" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:ResourceLinkButton ID="lnkDistribucionGlobal" runat="server" Visible="false" CssClass="LogicLinkButton" OnClick="LnkDistribucionGlobal" ResourceName="Labels" VariableName="VER_DISTRIBUCION_GLOBAL" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                            <br/>
                                            <br/>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>                         
                        </asp:UpdatePanel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server">
            <mon:Monitor ID="monitorPuntos" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
        
        <asp:Panel ID="EastPanel" runat="server">
            <asp:UpdatePanel ID="pnlEstados" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table width="100%" style="background-color: #D4D0C8;">
                        <tr>
                            <td align="center" colspan="2">
                                <br/>
                                <c1:C1Gauge ID="gaugeCompletados" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="75px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="150px">
                                    <Gauges>
                                        <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="100" Minimum="0" Name="linearGaugeCompletados" Viewport-AspectRatio="3">
                                            <Decorators>
                                                <c1:C1GaugeMarks Alignment="In" Interval="25" Length="50" Location="75" Width="2">
                                                    <Border LineStyle="None" />
                                                    <Filling Color="96, 136, 190" />
                                                </c1:C1GaugeMarks>
                                                <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                    <Border LineStyle="None" />
                                                    <Filling Color="White" />
                                                </c1:C1GaugeMarks>
                                                <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="25" Location="25">
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
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="lbl1" runat="server" ResourceName="Labels" VariableName="REALIZADOS" Font-Bold="true" />:
                                <asp:Label ID="lblCompletados" runat="server" CssClass="labelGaugeVerde"/>
                                <br/><br/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="COMPLETADOS" Font-Bold="true" />:
                                <asp:Label ID="lblComp" runat="server" CssClass="labelGaugeVerde"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="VISITADOS" Font-Bold="true" />:
                                <asp:Label ID="lblVisitados" runat="server" CssClass="labelGaugeAmarillo"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="EN_SITIO" Font-Bold="true" />:
                                <asp:Label ID="lblEnSitio" runat="server" CssClass="labelGaugeAzul"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="EN_ZONA" Font-Bold="true" />:
                                <asp:Label ID="lblEnZona" runat="server" CssClass="labelGaugeGris"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="NO_COMPLETADOS" Font-Bold="true" />:
                                <asp:Label ID="lblNoCompletados" runat="server" CssClass="labelGaugeRojo"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="NO_VISITADOS" Font-Bold="true" />:
                                <asp:Label ID="lblSinVisitar" runat="server" CssClass="labelGaugeNaranja"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="PENDIENTES" Font-Bold="true" />:
                                <asp:Label ID="lblPendientes" runat="server" CssClass="labelGaugeNaranja"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <br/>
                                <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="TOTAL" Font-Bold="true" />:
                                <asp:Label ID="lblTotal" runat="server" Font-Bold="True" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="RUTAS" Font-Bold="true" />:
                                <asp:Label ID="lblRutas" runat="server" Font-Bold="True" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <br/>
                                <a href='../'>
                                    <div class='Logo'></div>
                                </a>
                                <br/><br/>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </asp:Panel>
        
        <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progress" class="progress"></div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>
