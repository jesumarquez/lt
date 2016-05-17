<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ReporteDeEventos" Codebehind="ReporteDeEventos.aspx.cs" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="M2M_REPORTE_EVENTOS" /> 
    
    <style type="text/css">
        .filterpanel { padding: 20px;}
        #divResumen { background-color: #d9d9d9; border: solid 1px #cccccc;}
        #divResumen table { border: none; border-spacing: 0px; }
        #tblLayout { border-spacing: 0px;}
        .infobox { margin: 10px; padding: 10px; width: 140px; background-color: #f7f7f7; border: solid 1px #cccccc;}
        .infobox td { text-align: center; }
        .homeData { font-size: 12px; font-weight: bold; }
    </style>
    
    <div id="divResumen">
        <table>
            <tr class="filterpanel">
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" AddAllItem="true" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="FECHA" />
                    <br />
                    <cwc:DateTimePicker ID="dtFecha" runat="server" Mode="Date" />
                </td>
                <td width="50%" align="left">
                    <cwc:ResourceButton ID="btnBuscar" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnBuscar_OnClick" />
                </td>
            </tr>
        </table>
    </div>
    <div id="divEventos">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="EVENTOS" CssClass="homelabel" />
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="AbmTitledPanel3" runat="server" TitleVariableName="BOTONERA" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%" style="background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugePolicia" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugePolicia" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="20" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="10" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="EVENTOS_POLICIA" Font-Bold="true" Font-Underline="true" />
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="FECHA_SELECCIONADA" Font-Bold="true" />:
                                                            <asp:Label ID="lblPolicia" runat="server" CssClass="labelGaugeNaranja" />
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="ULTIMOS_7_DIAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblPoliciaSemanal" runat="server" CssClass="labelGaugeAmarillo" />
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
                                            <td align="center">
                                                <table width="100%" style="background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeBomberos" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeBomberos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="20" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="10" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="EVENTOS_BOMBEROS" Font-Bold="true" Font-Underline="true" />
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="FECHA_SELECCIONADA" Font-Bold="true" />:
                                                            <asp:Label ID="lblBomberos" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="ULTIMOS_7_DIAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblBomberosSemanal" runat="server" CssClass="labelGaugeAmarillo"/>
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
                                            <td align="center">
                                                <table width="100%" style="background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAmbulancia" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeAmbulancia" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="20" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="10" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="EVENTOS_AMBULANCIA" Font-Bold="true" Font-Underline="true" />
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="FECHA_SELECCIONADA" Font-Bold="true" />:
                                                            <asp:Label ID="lblAmbulancia" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="ULTIMOS_7_DIAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblAmbulanciaSemanal" runat="server" CssClass="labelGaugeAmarillo"/>
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
                    <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleVariableName="SENSOR_TEMPERATURA" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2" >
                                        <tr>
                                            <td align="center">
                                                <table width="100%" style="height:210px; background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center"> 
                                                            <c1:C1Gauge ID="gaugeExcesosTemperatura" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                               <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeExcesosTemperatura" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="20" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="10" Location="25">
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
                                                            <cwc:ResourceLabel ID="lblTitExcesosTemperatura" runat="server" ResourceName="Labels" VariableName="EVENTOS_TEMPERATURA" Font-Bold="true" Font-Underline="true" />
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="FECHA_SELECCIONADA" Font-Bold="true" />:
                                                            <asp:Label ID="lblExcesosTemperatura" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Labels" VariableName="ULTIMOS_7_DIAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblExcesosTemperaturaSemanal" runat="server" CssClass="labelGaugeAmarillo"/>                                                            
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2" >
                                        <tr>
                                            <td align="center">
                                                <table width="100%" style="height:210px; background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeTemperaturaMaxima" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="140px">
                                                                <Gauges>
                                                                    <c1:C1RadialGauge Maximum="50" Minimum="-50" Name="radialGaugeTemperaturaMaxima" StartAngle="-110" SweepAngle="220" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
                                                                        <Cap Radius="14">
                                                                            <Border Color="158, 189, 215" />
                                                                            <Filling BrushType="Gradient" Color="220, 231, 244" Color2="158, 189, 215" />
                                                                            <Gradient Direction="Vertical" />
                                                                            <MoreCircles>
                                                                                <c1:C1GaugeCapCircle Filling-BrushType="Gradient" Filling-Color="158, 189, 215" Filling-Color2="229, 236, 245" Gradient-Direction="Vertical" Radius="8" />
                                                                                <c1:C1GaugeCapCircle Filling-BrushType="Gradient" Filling-Color="220, 231, 244" Filling-Color2="194, 217, 240" Gradient-Direction="Vertical" Radius="7.5" />
                                                                            </MoreCircles>
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Cap>
                                                                        <Decorators>
                                                                            <c1:C1GaugeRange Alignment="In" AntiAliasing="LowQuality" Location="93">
                                                                                <Border LineStyle="None" />
                                                                                <ValueColors>
                                                                                    <c1:C1GaugeValueColor Color="213, 228, 241" Value="-50" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="-40" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="-30" />
                                                                                    <c1:C1GaugeValueColor Color="76, 133, 62" Value="-20" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="-10" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="0" />
                                                                                    <c1:C1GaugeValueColor Color="243, 230, 156" Value="10" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="20" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="30" />
                                                                                    <c1:C1GaugeValueColor Color="235, 100, 62" Value="40" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="50" />
                                                                                </ValueColors>
                                                                            </c1:C1GaugeRange>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="15" Location="93" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="12" Location="93" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Color="62, 106, 170" FontSize="11" Interval="10" Location="68">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeSector CenterRadius="17" CornerRadius="15" InnerRadius="-48" OuterRadius="102" StartAngle="-110" SweepAngle="220">
                                                                                <Border Color="141, 178, 227" Thickness="0.5" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="14" CornerRadius="12" InnerRadius="-45" OuterRadius="99" StartAngle="-110" SweepAngle="220">
                                                                                <Border Color="158, 189, 215" Thickness="0.5" />
                                                                                <Filling BrushType="Gradient" Color="20, 50, 88" Color2="191, 219, 255" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="12" CornerRadius="10" InnerRadius="-43" OuterRadius="97" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="149, 184, 216" Color2="213, 228, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="9" CornerRadius="7" InnerRadius="-40" OuterRadius="94" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="96, 136, 190" Color2="213, 228, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="8" CornerRadius="6" InnerRadius="-39" OuterRadius="93" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="White" Color2="213, 228, 241" />
                                                                                <Gradient Direction="RadialInner" />
                                                                            </c1:C1GaugeSector>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeSegment InnerRadius="150" SweepAngle="180" OuterRadius="93" StartAngle="-110">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="Transparent" Color2="White" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSegment>
                                                                            <c1:C1GaugeSegment InnerRadius="15" OuterRadius="7.5" StartAngle="-110" SweepAngle="180">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="Transparent" Color2="White" Opacity="0.3" Opacity2="0.1" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSegment>
                                                                        </CoverShapes>
                                                                        <Pointer Offset="-25" Shape="Custom" Length="115" Value="0" >
                                                                            <CustomShape EndRadius="1" EndWidth="2" StartWidth="12" StartRadius="3" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="BackwardDiagonal" />
                                                                            <Shadow Visible="True" Color="25, 70, 154" Opacity="0.4" />
                                                                        </Pointer>
                                                                    </c1:C1RadialGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="TEMPERATURA_MAXIMA" Font-Bold="true" />:
                                                            <asp:Label ID="lblTemperaturaMaxima" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>                                                
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="33%">
                                    <table width="98%" border="2" >
                                        <tr>
                                            <td align="center">
                                                <table width="100%" style="height:210px; background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeTemperaturaMaximaSemanal" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="140px">
                                                                <Gauges>
                                                                    <c1:C1RadialGauge Maximum="50" Minimum="-50" Name="radialGaugeTemperaturaMaximaSemanal" StartAngle="-110" SweepAngle="220" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
                                                                        <Cap Radius="14">
                                                                            <Border Color="158, 189, 215" />
                                                                            <Filling BrushType="Gradient" Color="220, 231, 244" Color2="158, 189, 215" />
                                                                            <Gradient Direction="Vertical" />
                                                                            <MoreCircles>
                                                                                <c1:C1GaugeCapCircle Filling-BrushType="Gradient" Filling-Color="158, 189, 215" Filling-Color2="229, 236, 245" Gradient-Direction="Vertical" Radius="8" />
                                                                                <c1:C1GaugeCapCircle Filling-BrushType="Gradient" Filling-Color="220, 231, 244" Filling-Color2="194, 217, 240" Gradient-Direction="Vertical" Radius="7.5" />
                                                                            </MoreCircles>
                                                                            <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                                                        </Cap>
                                                                        <Decorators>
                                                                            <c1:C1GaugeRange Alignment="In" AntiAliasing="LowQuality" Location="93">
                                                                                <Border LineStyle="None" />
                                                                                <ValueColors>
                                                                                    <c1:C1GaugeValueColor Color="213, 228, 241" Value="-50" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="-40" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="-30" />
                                                                                    <c1:C1GaugeValueColor Color="76, 133, 62" Value="-20" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="-10" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="0" />
                                                                                    <c1:C1GaugeValueColor Color="243, 230, 156" Value="10" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="20" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="30" />
                                                                                    <c1:C1GaugeValueColor Color="235, 100, 62" Value="40" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="50" />
                                                                                </ValueColors>
                                                                            </c1:C1GaugeRange>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="15" Location="93" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="12" Location="93" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Color="62, 106, 170" FontSize="11" Interval="10" Location="68">
                                                                            </c1:C1GaugeLabels>
                                                                        </Decorators>
                                                                        <FaceShapes>
                                                                            <c1:C1GaugeSector CenterRadius="17" CornerRadius="15" InnerRadius="-48" OuterRadius="102" StartAngle="-110" SweepAngle="220">
                                                                                <Border Color="141, 178, 227" Thickness="0.5" />
                                                                                <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="14" CornerRadius="12" InnerRadius="-45" OuterRadius="99" StartAngle="-110" SweepAngle="220">
                                                                                <Border Color="158, 189, 215" Thickness="0.5" />
                                                                                <Filling BrushType="Gradient" Color="20, 50, 88" Color2="191, 219, 255" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="12" CornerRadius="10" InnerRadius="-43" OuterRadius="97" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="149, 184, 216" Color2="213, 228, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="9" CornerRadius="7" InnerRadius="-40" OuterRadius="94" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="96, 136, 190" Color2="213, 228, 241" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSector>
                                                                            <c1:C1GaugeSector CenterRadius="8" CornerRadius="6" InnerRadius="-39" OuterRadius="93" StartAngle="-110" SweepAngle="220">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="White" Color2="213, 228, 241" />
                                                                                <Gradient Direction="RadialInner" />
                                                                            </c1:C1GaugeSector>
                                                                        </FaceShapes>
                                                                        <CoverShapes>
                                                                            <c1:C1GaugeSegment InnerRadius="150" SweepAngle="180" OuterRadius="93" StartAngle="-110">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="Transparent" Color2="White" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSegment>
                                                                            <c1:C1GaugeSegment InnerRadius="15" OuterRadius="7.5" StartAngle="-110" SweepAngle="180">
                                                                                <Border LineStyle="None" />
                                                                                <Filling BrushType="Gradient" Color="Transparent" Color2="White" Opacity="0.3" Opacity2="0.1" />
                                                                                <Gradient Direction="Vertical" />
                                                                            </c1:C1GaugeSegment>
                                                                        </CoverShapes>
                                                                        <Pointer Offset="-25" Shape="Custom" Length="115" Value="0" >
                                                                            <CustomShape EndRadius="1" EndWidth="2" StartWidth="12" StartRadius="3" />
                                                                            <Border Color="200, 240, 90, 40" />
                                                                            <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                                                            <Gradient Direction="BackwardDiagonal" />
                                                                            <Shadow Visible="True" Color="25, 70, 154" Opacity="0.4" />
                                                                        </Pointer>
                                                                    </c1:C1RadialGauge>
                                                                </Gauges>
                                                            </c1:C1Gauge>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <br />
                                                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="TEMPERATURA_MAXIMA_SEMANAL" Font-Bold="true" />:
                                                            <asp:Label ID="lblTemperaturaMaximaSemanal" runat="server" CssClass="labelGaugeNaranja"/>
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
</asp:Content>


