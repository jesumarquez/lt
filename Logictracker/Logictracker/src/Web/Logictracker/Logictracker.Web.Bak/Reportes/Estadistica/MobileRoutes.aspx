<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="MobileRoutes.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.EstadisticaMobileRoutes" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>
<%@ Register src="~/App_Controls/Pickers/NumberPicker.ascx" tagname="NumberPicker" tagprefix="uc2" %>
<%@ Register src="~/App_Controls/Pickers/TimePicker.ascx" tagname="TimePicker" tagprefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true"  />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" Width="150px" OnInitialBinding="DdlDistritoInitialBinding" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlBase" AddAllItem="true" runat="server" Width="150px" ParentControls="ddlDistrito" OnInitialBinding="DdlBaseInitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels"  VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" />
                <cwc:DateTimeRangeValidator ID="dtrVal" runat="server" StartControlID="dpDesde" EndControlID="dpHasta" MaxRange="1" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblSinReportar" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="SIN_REPORTAR" />
                <br />
                <uc3:TimePicker ID="tpSinReportar" runat="server" DefaultTimeMode="NotSet" IsValidEmpty="false" SelectedTime="00:01" Width="100" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownList runat="server" AddAllItem="true" ID="ddlTipoVehiculo" Width="150px" ParentControls="ddlBase" OnInitialBinding="DdlTipoVehiculoInitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlVehiculo" runat="server" Width="150px" OnInitialBinding="DdlVehiculoInitialBinding" ParentControls="ddlTipoVehiculo" UseOptionGroup="true" OptionGroupProperty="Estado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTipovehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDetencion" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DETENCION_MAYOR_A" />
                <br />
                <uc3:TimePicker ID="tpDetencion" runat="server" DefaultTimeMode="NotSet" SelectedTime="00:01" IsValidEmpty="false" Width="100" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistancia" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DISTANCIA_MAYOR_A" />
                <br />
                <uc2:NumberPicker ID="npDistancia" runat="server" IsValidEmpty="false" Mask="9999" MaximumValue="9999" Number="0100" Width="100" />
            </td>
            <td align="left">
                <cwc:ResourceCheckBox ID="chkCombustible" runat="server" ResourceName="Labels" VariableName="COMBUSTIBLE" Font-Bold="true" />
                <br />
                <cwc:ResourceCheckBox ID="chkDirecciones" runat="server" ResourceName="Labels" VariableName="VER_DIRECCIONES" Font-Bold="true" />
            </td>
        </tr>
    </table>
</asp:Content>
        
<asp:Content ID="cLbl" runat="server" ContentPlaceHolderID="DetalleSuperior" >      
    <table width="100%" cellpadding="5">
        <tr>
            <td align="center">
                <asp:Label  ID="lblTitle" runat="server" Font-Bold="true" Font-Size="x-small" Visible="false" />                    
            </td>
        </tr>
    </table> 
</asp:Content>
    
<asp:Content ID="cTotals" runat="server" ContentPlaceHolderID="DetalleInferior" >
    <asp:UpdatePanel ID="upTotals" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table id="tblSubtotals" runat="server" cellpadding="5" width="100%" style="font-size: x-small" visible="false">
                <tr align="left">
                    <td align="left">
                        <cwc:ResourceLabel ID="ResourceLabel9" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO" />: 
                        <asp:Label ID="lblTotalMovementTime" runat="server" />
                        <br /><br /><br />
                        <cwc:ResourceLabel ID="ResourceLabel10" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_DETENCION" />: 
                        <asp:Label ID="lblTotalStooppedTime" runat="server" />
                        (<asp:LinkButton ID="lnkStopEvents" runat="server" OnClick="LnkStopClick"/>)
                        <br /><br /><br />
                        <cwc:ResourceLabel ID="ResourceLabel14" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_INFRACCION" />: 
                        <asp:Label ID="lblTotalInfractionTime" runat="server" />
                        (<asp:LinkButton ID="lnkInfractionEvents" runat="server" OnClick="LnkInfractionClick"/>)
                        <br /><br /><br />
                        <cwc:ResourceLabel ID="ResourceLabel13" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="SIN_REPORTAR" />: 
                        <asp:Label ID="lblTotalNoReportTime" runat="server" />
                    </td>
                    <td align="center">
                        <c1:C1Gauge ID="gaugeTotalKm" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" ViewTag="640505324143004351" Width="140px">
                            <Gauges>
                                <c1:C1LinearGauge AxisLength="0.75" AxisStart="0.07" Maximum="500" Minimum="0" Name="linearGaugeTotalKm" Viewport-AspectRatio="0.4" ViewTag="650627358688755457" IsReversed="True" Orientation="Vertical">
                                    <Decorators>
                                        <c1:C1GaugeMarks Alignment="In" Interval="100" Length="40" Location="70" ViewTag="651190316563358808" Width="2">
                                            <Border LineStyle="None" />
                                            <Filling Color="96, 136, 190" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeMarks Alignment="In" Interval="50" Length="20" Location="70" ViewTag="651753275780104499" Width="1">
                                            <Border LineStyle="None" />
                                            <Filling Color="White" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeLabels Alignment="In" Color="62, 106, 170" FontSize="13" Interval="50" Location="28" ViewTag="651471795264041824">
                                        </c1:C1GaugeLabels>
                                    </Decorators>
                                    <FaceShapes>
                                        <c1:C1GaugeRectangle CornerRadius="5" Height="-1.28" Name="Face" Width="-1.08" CenterPointY="0.57">
                                            <Border Color="141, 178, 227" Thickness="1" />
                                            <Filling BrushType="Gradient" Color="191, 219, 255" Color2="101, 145, 205" />
                                            <Gradient Direction="Vertical" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeRectangle CornerRadius="3" Height="-1.24" CenterPointY="0.57">
                                            <Border Color="243, 248, 254" Thickness="1" />
                                            <Filling BrushType="Gradient" Color="235, 241, 250" Color2="219, 230, 242" />
                                            <Gradient Direction="Vertical" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeRectangle Width="-0.4">
                                            <Border LineStyle="None" />
                                            <Filling Color="193, 216, 242" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeRectangle Height="-1.01" Width="2" CenterPointX="0.71">
                                            <Border LineStyle="None" />
                                            <Filling Color="96, 136, 190" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeRectangle Height="-1.01" Width="-0.03" CenterPointX="0.775">
                                            <Border Color="96, 136, 190" />
                                            <Filling BrushType="Gradient" Color="21, 50, 88" Color2="194, 218, 241" />
                                            <Gradient Direction="Horizontal" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeRectangle CenterPointY="1.13" CornerRadius="1" Height="22" Width="-0.96">
                                            <Border LineStyle="None" />
                                            <Filling Color="193, 216, 242" />
                                        </c1:C1GaugeRectangle>
                                        <c1:C1GaugeCaption CenterPointY="1.13" Color="62, 106, 170" FontSize="12" Text="Km.">
                                        </c1:C1GaugeCaption>
                                    </FaceShapes>
                                    <CoverShapes>
                                        <c1:C1GaugeEllipse CenterPointY="-0.06" Width="-1.2" Height="-0.35">
                                            <Border LineStyle="None" />
                                            <Filling Color="White" Opacity="0.4" />
                                            <Clippings>
                                                <c1:C1GaugeClipping Operation="Replace" ShapeName="Face" />
                                            </Clippings>
                                        </c1:C1GaugeEllipse>
                                    </CoverShapes>
                                    <Pointer Alignment="In" FlipShape="True" Length="50" Offset="84" Shape="Custom">
                                        <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                        <Border Color="200, 240, 90, 40" />
                                        <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                        <Gradient Direction="Horizontal" />
                                        <Shadow Color="25, 70, 154" Opacity="0.4" Visible="True" />
                                    </Pointer>
                                </c1:C1LinearGauge>
                            </Gauges>
                        </c1:C1Gauge>
                    
                        <cwc:ResourceLabel ID="ResourceLabel11" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="KILOMETROS_RECORRIDOS" />: 
                        <asp:Label ID="lblTotalKilometers" runat="server" />
                    </td>
                    <td align="center">
                        <c1:C1Gauge ID="gaugeMinSpeed" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" ViewTag="640505324143004351" Width="140px">
                            <Gauges>
                                <c1:C1RadialGauge Maximum="120" Name="radialGaugeMinSpeed" StartAngle="-110" SweepAngle="220" ViewTag="639930353501108013" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
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
                                        <c1:C1GaugeRange Alignment="In" AntiAliasing="LowQuality" Location="93" ViewTag="685247798748123549">
                                            <Border LineStyle="None" />
                                            <ValueColors>
                                                <c1:C1GaugeValueColor Color="213, 228, 241" Value="0" ViewTag="685810748701544861" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="30" ViewTag="646408383518178919" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="60" ViewTag="644438052407375485" />
                                                <c1:C1GaugeValueColor Color="76, 133, 62" Value="60" ViewTag="644719527623749849" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="70" ViewTag="646689859674567049" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="80" ViewTag="645001003190904276" />
                                                <c1:C1GaugeValueColor Color="243, 230, 156" Value="80" ViewTag="645282479933345926" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="90" ViewTag="646971334896531733" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="100" ViewTag="645563955232075001" />
                                                <c1:C1GaugeValueColor Color="235, 100, 62" Value="100" ViewTag="645845430697773625" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="110" ViewTag="647252810247523796" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="120" ViewTag="646126906011193540" />
                                            </ValueColors>
                                        </c1:C1GaugeRange>
                                        <c1:C1GaugeMarks Alignment="In" Interval="10" Length="15" Location="93" ViewTag="647248702895735084" Width="2">
                                            <Border LineStyle="None" />
                                            <Filling Color="96, 136, 190" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeMarks Alignment="In" Interval="5" Length="12" Location="93" ViewTag="647811652849166397" Width="1">
                                            <Border LineStyle="None" />
                                            <Filling Color="White" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeLabels Color="62, 106, 170" FontSize="11" Interval="10" Location="68" ViewTag="648374602802587709">
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
                                    <Pointer Offset="-25" Shape="Custom" Value="0" Length="115">
                                        <CustomShape EndRadius="1" EndWidth="2" StartWidth="12" StartRadius="3" />
                                        <Border Color="200, 240, 90, 40" />
                                        <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                        <Gradient Direction="BackwardDiagonal" />
                                        <Shadow Visible="True" Color="25, 70, 154" Opacity="0.4" />
                                    </Pointer>
                                </c1:C1RadialGauge>
                            </Gauges>
                        </c1:C1Gauge>
                        
                        <cwc:ResourceLabel ID="ResourceLabel12" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_MINIMA" />: 
                        <asp:Label ID="lblVelocidadMinima" runat="server" />
                    </td>
                    <td align="center">
                        <c1:C1Gauge ID="gaugeAverageSpeed" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" ViewTag="640505324143004351" Width="140px">
                            <Gauges>
                                <c1:C1RadialGauge Maximum="120" Name="radialGaugeAverageSpeed" StartAngle="-110" SweepAngle="220" ViewTag="639930353501108013" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
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
                                        <c1:C1GaugeRange Alignment="In" AntiAliasing="LowQuality" Location="93" ViewTag="685247798748123549">
                                            <Border LineStyle="None" />
                                            <ValueColors>
                                                <c1:C1GaugeValueColor Color="213, 228, 241" Value="0" ViewTag="685810748701544861" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="30" ViewTag="646408383518178919" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="60" ViewTag="644438052407375485" />
                                                <c1:C1GaugeValueColor Color="76, 133, 62" Value="60" ViewTag="644719527623749849" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="70" ViewTag="646689859674567049" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="80" ViewTag="645001003190904276" />
                                                <c1:C1GaugeValueColor Color="243, 230, 156" Value="80" ViewTag="645282479933345926" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="90" ViewTag="646971334896531733" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="100" ViewTag="645563955232075001" />
                                                <c1:C1GaugeValueColor Color="235, 100, 62" Value="100" ViewTag="645845430697773625" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="110" ViewTag="647252810247523796" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="120" ViewTag="646126906011193540" />
                                            </ValueColors>
                                        </c1:C1GaugeRange>
                                        <c1:C1GaugeMarks Alignment="In" Interval="10" Length="15" Location="93" ViewTag="647248702895735084" Width="2">
                                            <Border LineStyle="None" />
                                            <Filling Color="96, 136, 190" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeMarks Alignment="In" Interval="5" Length="12" Location="93" ViewTag="647811652849166397" Width="1">
                                            <Border LineStyle="None" />
                                            <Filling Color="White" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeLabels Color="62, 106, 170" FontSize="11" Interval="10" Location="68" ViewTag="648374602802587709">
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
                                    <Pointer Offset="-25" Shape="Custom" Value="0" Length="115">
                                        <CustomShape EndRadius="1" EndWidth="2" StartWidth="12" StartRadius="3" />
                                        <Border Color="200, 240, 90, 40" />
                                        <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                        <Gradient Direction="BackwardDiagonal" />
                                        <Shadow Visible="True" Color="25, 70, 154" Opacity="0.4" />
                                    </Pointer>
                                </c1:C1RadialGauge>
                            </Gauges>
                        </c1:C1Gauge>
                    
                        <cwc:ResourceLabel ID="ResourceLabel15" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" />: 
                        <asp:Label ID="lblVelocidadPromedio" runat="server" />
                    </td>
                    <td align="center">
                        <c1:C1Gauge ID="gaugeMaxSpeed" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="160px" ImageFormat="Png" TextRenderingHint="AntiAlias" ViewTag="640505324143004351" Width="140px">
                            <Gauges>
                                <c1:C1RadialGauge Maximum="120" Name="radialGaugeMaxSpeed" StartAngle="-110" SweepAngle="220" ViewTag="639930353501108013" PointerOriginY="0.67" Radius="0.64" Viewport-AspectPinX="0.5" Viewport-AspectPinY="0.5" Viewport-AspectRatio="1.35">
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
                                        <c1:C1GaugeRange Alignment="In" AntiAliasing="LowQuality" Location="93" ViewTag="685247798748123549">
                                            <Border LineStyle="None" />
                                            <ValueColors>
                                                <c1:C1GaugeValueColor Color="213, 228, 241" Value="0" ViewTag="685810748701544861" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="30" ViewTag="646408383518178919" />
                                                <c1:C1GaugeValueColor Color="150, 184, 216" Value="60" ViewTag="644438052407375485" />
                                                <c1:C1GaugeValueColor Color="76, 133, 62" Value="60" ViewTag="644719527623749849" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="70" ViewTag="646689859674567049" />
                                                <c1:C1GaugeValueColor Color="50, 99, 42" Value="80" ViewTag="645001003190904276" />
                                                <c1:C1GaugeValueColor Color="243, 230, 156" Value="80" ViewTag="645282479933345926" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="90" ViewTag="646971334896531733" />
                                                <c1:C1GaugeValueColor Color="240, 164, 32" Value="100" ViewTag="645563955232075001" />
                                                <c1:C1GaugeValueColor Color="235, 100, 62" Value="100" ViewTag="645845430697773625" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="110" ViewTag="647252810247523796" />
                                                <c1:C1GaugeValueColor Color="155, 38, 32" Value="120" ViewTag="646126906011193540" />
                                            </ValueColors>
                                        </c1:C1GaugeRange>
                                        <c1:C1GaugeMarks Alignment="In" Interval="10" Length="15" Location="93" ViewTag="647248702895735084" Width="2">
                                            <Border LineStyle="None" />
                                            <Filling Color="96, 136, 190" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeMarks Alignment="In" Interval="5" Length="12" Location="93" ViewTag="647811652849166397" Width="1">
                                            <Border LineStyle="None" />
                                            <Filling Color="White" />
                                        </c1:C1GaugeMarks>
                                        <c1:C1GaugeLabels Color="62, 106, 170" FontSize="11" Interval="10" Location="68" ViewTag="648374602802587709">
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
                                    <Pointer Offset="-25" Shape="Custom" Value="0" Length="115">
                                        <CustomShape EndRadius="1" EndWidth="2" StartWidth="12" StartRadius="3" />
                                        <Border Color="200, 240, 90, 40" />
                                        <Filling BrushType="Gradient" Color="240, 90, 40" Color2="253, 207, 153" />
                                        <Gradient Direction="BackwardDiagonal" />
                                        <Shadow Visible="True" Color="25, 70, 154" Opacity="0.4" />
                                    </Pointer>
                                </c1:C1RadialGauge>
                            </Gauges>
                        </c1:C1Gauge>
                    
                        <cwc:ResourceLabel ID="ResourceLabel16" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA" />
                        <asp:Label ID="lblVelocidadMaxima" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
           
        </Triggers>
    </asp:UpdatePanel>    
    <br/>
    <asp:UpdatePanel ID="upEvents" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="text-align: center">
                <table border="0" width="100%">
                    <tr>
                        <td align="center">
                            <asp:Label ID="lblEventos" runat="server" Font-Bold="true" Font-Size="X-Small" Visible="false" />        
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <cwc:ToolBarButton ID="lnkEventos" runat="server" SkinID="Map" OnClick="LnkEventosClick" Visible="false"/>        
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <iframe id="ifEvents" runat="server" width="100%" src="MobileRoutes/MobileRoutesEvents.aspx?IsPrinting=false" visible="false"
                style="border-style: none" height="500px" />
        </ContentTemplate>
        <Triggers>
      
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="DetalleInferiorPrint" >
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table id="tblSubtotalsPrint" runat="server" cellpadding="5" width="100%" style="font-size: x-small" visible="false">
                <tr align="left">
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_MOVIMIENTO" />: 
                        <asp:Label ID="lblTotalMovementTimePrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_DETENCION" />: 
                        <asp:Label ID="lblTotalStooppedTimePrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="KILOMETROS_RECORRIDOS" />: 
                        <asp:Label ID="lblTotalKilometersPrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_MINIMA" />: 
                        <asp:Label ID="lblVelocidadMinimaPrint" runat="server" />
                    </td>
                </tr>
                <tr align="left">
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="SIN_REPORTAR" />: 
                        <asp:Label ID="lblTotalNoReportTimePrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TIEMPO_INFRACCION" />: 
                        <asp:Label ID="lblTotalInfractionTimePrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" />: 
                        <asp:Label ID="lblVelocidadPromedioPrint" runat="server" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA" />
                        <asp:Label ID="lblVelocidadMaximaPrint" runat="server" />                        
                    </td>              
                </tr>
            </table>
        </ContentTemplate>
        <Triggers>
           
        </Triggers>
    </asp:UpdatePanel>
    <br/>    
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="text-align: center">
                <asp:Label ID="lblEventosPrint" runat="server" Font-Bold="true" Font-Size="X-Small" Visible="false" />
            </div>
            <br />
            <iframe id="ifEventsPrint" runat="server" width="100%" src="MobileRoutes/MobileRoutesEvents.aspx?IsPrinting=true" visible="false"
                style="border-style: none" height="500px" />
        </ContentTemplate>
        <Triggers>
      
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>