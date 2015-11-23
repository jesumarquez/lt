<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="TableroDeControl.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.TableroDeControl" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="tablerocontrol">
    <script type="text/javascript">
        function showDiv(div)
        {
            var divHoy = document.getElementById('divHoy');
            var divAyer = document.getElementById('divAyer');
            var divMes = document.getElementById('divMes');            
            
            divHoy.style.display = div == 'divHoy' ? 'block' : 'none';
            divAyer.style.display = div == 'divAyer' ? 'block' : 'none';
            divMes.style.display = div == 'divMes' ? 'block' : 'none';
        }
    </script>

    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="EST_TABLERO_CONTROL" /> 
    <div id="divResumen">
        <table>
            <tr>
                <td class="filterpanel">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" AddAllItem="false" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa" />
                </td>
                <td >
                    <table>
                        <tr>
                            <td>
                                <table class="infobox">
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblHoraActual" runat="server" ResourceName="Labels" VariableName="HORA_ACTUAL"  />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblHora" runat="server" CssClass="homeData" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
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
                                                        <asp:Label ID="lblActiva" runat="server" class="homeData" />
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
                                                        <asp:Label ID="lblInactiva" runat="server" CssClass="homeData" />
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
                                                        <asp:Label ID="lblMantenimiento" runat="server" CssClass="homeData" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td align="right" valign="middle">
                                            <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="BtnActualizar_OnClick" />
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
    
    <table border="0">
        <tr>
            <td align="left">
                <div id="btShowHoy" onclick="showDiv('divHoy');">
                    <cwc:ResourceLabel ID="ResourceLabel20" runat="server" ResourceName="Labels" VariableName="HOY" />
                </div>        
            </td>
            <td align="left">
                <div id="btShowAyer" onclick="showDiv('divAyer');">
                    <cwc:ResourceLabel ID="ResourceLabel21" runat="server" ResourceName="Labels" VariableName="AYER" />
                </div>
            </td>
            <td align="left">
                <div id="btShowMes" onclick="showDiv('divMes');">
                    <cwc:ResourceLabel ID="ResourceLabel22" runat="server" ResourceName="Labels" VariableName="MES" />
                </div>
            </td>
        </tr>
    </table>    
    
    <!--ESTADISTICAS HOY -->            
    <div id="divHoy">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="ESTADISTICAS_HOY" CssClass="homelabel"/>
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleVariableName="PRODUCTIVIDAD" TitleResourceName="Labels">
                        <table width="100%" border="0">                                    
                            <tr>
                                <td align="center" width="30%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMovDet" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeMovDet" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_MOVIMIENTO" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnMovimiento" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="MOVILES_DETENIDOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetenidos" runat="server" CssClass="labelGaugeAmarillo" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" width="40%">
                                    <table width="98%" border="2">
                                        <tr valign="middle">
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeBaseGeocerca" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeBaseGeocerca" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                        <td align="center" rowspan="2">
                                                            <table id="tblLabels" runat="server" border="0" width="98%">
                                                                <tr>
                                                                    <td align="left">
                                                                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DETALLE" />:
                                                                    </td>                                                                    
                                                                </tr>
                                                                <tr>
                                                                    <td> &nbsp; </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_BASE" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnBase" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel17" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_GEOCERCA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnGeocerca" runat="server" CssClass="labelGaugeAmarillo"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>                                                            
                                </td>                           
                                <td align="center" width="30%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMas1Hora" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeMas1Hora" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel18" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_BASE_MAS_1_HORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnBaseMas1Hora" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel19" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_GEOCERCA_MAS_1_HORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnGeocercaMas1Hora" runat="server" CssClass="labelGaugeAmarillo"/>
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
                <td>
                    <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleVariableName="CICLO_LOGISTICO" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeActivos" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeActivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="MOVILES_ACTIVOS_AHORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblActivosAhora" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnHoraHoy" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel31" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_HORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnHoraHoy" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeDemoradosHoy" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel32" runat="server" ResourceName="Labels" VariableName="MOVILES_DEMORADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblDemoradosHoy" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAdelantadosHoy" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeActivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel33" runat="server" ResourceName="Labels" VariableName="MOVILES_ADELANTADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblAdelantadosHoy" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeInactivosBase" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel23" runat="server" ResourceName="Labels" VariableName="MOVILES_INACTIVOS_BASE" Font-Bold="true" />:
                                                            <asp:Label ID="lblInactivosBase" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnPlanta" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel36" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_PLANTA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnPlanta" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnCliente" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel37" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_CLIENTE" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnCliente" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnViaje" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeActivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel38" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_VIAJE" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnViaje" runat="server" CssClass="labelGaugeNaranja"/>
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
    
    <!--ESTADISTICAS AYER -->
    <div id="divAyer" style="display: none;">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="ESTADISTICAS_AYER" CssClass="homelabel" />
                </td>
            </tr>
            <tr>
                <td align="center" valign="top">
                    <cwc:TitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="PRODUCTIVIDAD" TitleResourceName="Labels">
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
                                                            <cwc:ResourceLabel ID="lblTitMovimiento" runat="server" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblMovimiento" runat="server"/>
                                                            <br /><br /><br />
                                                            <cwc:ResourceLabel ID="lblTitDetencion" runat="server" ResourceName="Labels" VariableName="TIEMPO_DETENCION_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetencion" runat="server"/>
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
                                                            <c1:C1Gauge ID="gaugeDet1" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeDet1" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="25" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="50" Location="25">
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
                                                            <cwc:ResourceLabel ID="lblTitDetencion1" runat="server" ResourceName="Labels" VariableName="DETENCIONES_MAYORES_A_1" Font-Bold="true" />:
                                                            <asp:Label ID="lblMayor1" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="lblTitDetencion15" runat="server" ResourceName="Labels" VariableName="DETENCIONES_MAYORES_A_15" Font-Bold="true" />:
                                                            <asp:Label ID="lblMayor15" runat="server" CssClass="labelGaugeAmarillo"/>
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
                    <cwc:TitledPanel ID="TitledPanel3" runat="server" TitleVariableName="CICLO_LOGISTICO" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center"> 
                                                            <c1:C1Gauge ID="gaugeTickets" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeTickets" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="25" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="50" Location="25">
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
                                                            <cwc:ResourceLinkButton ID="lnkTickets" runat="server" ResourceName="Labels" VariableName="CANTIDAD_TICKETS" Font-Bold="true" OnClick="LnkTickets_OnClick" />:
                                                            <asp:Label ID="lblTickets" runat="server" CssClass="labelGaugeNaranja"/>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnHoraAyer" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel28" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_HORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnHoraAyer" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeDemoradosAyer" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel29" runat="server" ResourceName="Labels" VariableName="MOVILES_DEMORADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblDemoradosAyer" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="25%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAdelantadosAyer" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeActivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="10" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="5" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="5" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel30" runat="server" ResourceName="Labels" VariableName="MOVILES_ADELANTADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblAdelantadosAyer" runat="server" CssClass="labelGaugeNaranja"/>
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
                    <cwc:TitledPanel ID="TitledPanel4" runat="server" TitleVariableName="ACCIDENTOLOGIA" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAverageSpeed" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="140px">
                                                                <Gauges>
                                                                    <c1:C1RadialGauge Maximum="120" Name="radialGaugeAverageSpeed" StartAngle="-110" SweepAngle="220" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
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
                                                                                    <c1:C1GaugeValueColor Color="213, 228, 241" Value="0" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="30" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="60" />
                                                                                    <c1:C1GaugeValueColor Color="76, 133, 62" Value="60" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="70" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="80" />
                                                                                    <c1:C1GaugeValueColor Color="243, 230, 156" Value="80" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="90" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="100" />
                                                                                    <c1:C1GaugeValueColor Color="235, 100, 62" Value="100" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="110" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="120" />
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
                                                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO_FLOTA" Font-Bold="true" />:
                                                            <asp:Label ID="lblVelPromedio" runat="server" CssClass="labelGaugeNaranja"/>
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
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeMaxSpeed" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="140px">
                                                                <Gauges>
                                                                    <c1:C1RadialGauge Maximum="120" Name="radialGaugeMaxSpeed" StartAngle="-110" SweepAngle="220" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
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
                                                                                    <c1:C1GaugeValueColor Color="213, 228, 241" Value="0" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="30" />
                                                                                    <c1:C1GaugeValueColor Color="150, 184, 216" Value="60" />
                                                                                    <c1:C1GaugeValueColor Color="76, 133, 62" Value="60" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="70" />
                                                                                    <c1:C1GaugeValueColor Color="50, 99, 42" Value="80" />
                                                                                    <c1:C1GaugeValueColor Color="243, 230, 156" Value="80" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="90" />
                                                                                    <c1:C1GaugeValueColor Color="240, 164, 32" Value="100" />
                                                                                    <c1:C1GaugeValueColor Color="235, 100, 62" Value="100" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="110" />
                                                                                    <c1:C1GaugeValueColor Color="155, 38, 32" Value="120" />
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
                                                                        <Pointer Offset="-25" Shape="Custom" Length="115" Value="0">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA_FLOTA" Font-Bold="true" />:
                                                            <asp:Label ID="lblVelMax" runat="server" CssClass="labelGaugeNaranja"/>
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
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <br />
                                                            <c1:C1Gauge ID="gaugeInfracciones" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInfracciones" Viewport-AspectRatio="3">
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
                                                            <br />
                                                            <cwc:ResourceLinkButton ID="lnkInfracciones" runat="server" ResourceName="Labels" VariableName="CANTIDAD_INFRACCIONES" Font-Bold="true" OnClick="LnkInfracciones_OnClick" />:
                                                            <asp:Label ID="lblInfracciones" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="INFRACCIONES_VEHICULOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblInfVehiculos" runat="server" CssClass="labelGaugeAmarillo"/>
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
                    <cwc:TitledPanel ID="AbmTitledPanel3" runat="server" TitleVariableName="LOGISTICA" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" width="50%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td align="center">
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeKm" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeKm" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="100" Location="25">
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
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="KM_RECORRIDOS_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblKmRecorridos" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br /><br />
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
                                                            <c1:C1Gauge ID="gaugeEntradas" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeEntradas" Viewport-AspectRatio="3">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="ENTRADAS_BASE" Font-Bold="true" />:
                                                            <asp:Label ID="lblEntradasBase" runat="server" CssClass="labelGaugeNaranja"/>
                                                            <br /><br />
                                                            <cwc:ResourceLinkButton ID="lnkCercas" runat="server" ResourceName="Labels" VariableName="ENTRADAS_GEOCERCA" Font-Bold="true" OnClick="LnkCercas_OnClick" />:
                                                            <asp:Label ID="lblEntradasGeocerca" runat="server" CssClass="labelGaugeAmarillo"/>
                                                            <br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="MINUTOS_PROMEDIO_GEOCERCA" Font-Bold="true" />:
                                                            <asp:Label ID="lblPromedioGeocerca" runat="server"/>
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
                    <cwc:TitledPanel ID="TitledPanel5" runat="server" TitleVariableName="COSTOS" TitleResourceName="Labels">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="MINUTOS_DETENIDO_ON" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetenidoOn" runat="server"/>
                                                            <br /><br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel24" runat="server" ResourceName="Labels" VariableName="MINUTOS_DETENIDO_ON_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetenidoOnPromedio" runat="server"/>
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
                                                            <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="MINUTOS_DETENIDO_OFF" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetenidoOff" runat="server"/>  
                                                            <br /><br /><br />
                                                            <cwc:ResourceLabel ID="ResourceLabel25" runat="server" ResourceName="Labels" VariableName="MINUTOS_DETENIDO_OFF_PROMEDIO" Font-Bold="true" />:
                                                            <asp:Label ID="lblDetenidoOffPromedio" runat="server"/>
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
    
    <!--ESTADISTICAS MES -->
    <div id="divMes" style="display: none;">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="ResourceLabel26" runat="server" ResourceName="Labels" VariableName="ESTADISTICAS_MES" CssClass="homelabel"/>
                </td>
            </tr>
            <tr>
                <td>
                    <cwc:TitledPanel ID="TitledPanel7" runat="server" TitleVariableName="CICLO_LOGISTICO" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" valign="top" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeEnHoraMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="50" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel27" runat="server" ResourceName="Labels" VariableName="MOVILES_EN_HORA" Font-Bold="true" />:
                                                            <asp:Label ID="lblEnHoraMes" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeDemoradosMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeInactivosBase" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="50" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel34" runat="server" ResourceName="Labels" VariableName="MOVILES_DEMORADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblDemoradosMes" runat="server" CssClass="labelGaugeNaranja"/>                                              
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td align="center" valign="top" width="33%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAdelantadosMes" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
                                                                <Gauges>
                                                                    <c1:C1LinearGauge AxisLength="0.85" AxisStart="0.08" Maximum="30" Minimum="0" Name="linearGaugeActivos" Viewport-AspectRatio="3">
                                                                        <Decorators>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="100" Length="50" Location="75" Width="2">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="96, 136, 190" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeMarks Alignment="In" Interval="50" Length="25" Location="74" Width="1">
                                                                                <Border LineStyle="None" />
                                                                                <Filling Color="White" />
                                                                            </c1:C1GaugeMarks>
                                                                            <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="14" Interval="50" Location="25">
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
                                                            <cwc:ResourceLabel ID="ResourceLabel35" runat="server" ResourceName="Labels" VariableName="MOVILES_ADELANTADOS" Font-Bold="true" />:
                                                            <asp:Label ID="lblAdelantadosMes" runat="server" CssClass="labelGaugeNaranja"/>
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


