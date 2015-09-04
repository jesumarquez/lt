<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Reportes.CicloLogistico.KPIEstadoEntregas" Codebehind="KPIEstadoEntregas.aspx.cs" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="server">
    <asp:UpdatePanel ID="pnlUpd" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <br />
                        <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <br />
                        <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList id="ddlPlanta" runat="server" Width="200px" ParentControls="ddlEmpresa" AddAllItem="true" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
    
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentReport">
    <asp:UpdatePanel ID="CenterPanel" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel ID="pnlUpdate" runat="server">
                <ContentTemplate>
                    <table width="100%" border="0">
                        <tr>
                            <td align="center" valign="top"> 
                                <c1:C1GridView ID="gridTransportistas" runat="server" OnRowDataBound="GridTransportistasOnRowDataBound" AutoGenerateColumns="false" Width="100%" Visible="true" SkinID="ListGridNoGroupNoPage" >
                                    <Columns>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblTransportista" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblRutas" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblEntregas" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Right" ForeColor="green" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblRealizados" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <c1:C1Gauge ID="gaugeCompletados" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="25px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="50px">
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
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="green" HorizontalAlign="Right" />
                                            <ItemTemplate >
                                                <asp:Label ID="lblCompletados" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="Gold" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblVisitados" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="blue" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblEnSitio" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="gray" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblEnZona" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="red" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblNoCompletados" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="orange" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblNoVisitados" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle ForeColor="orange" HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblPendientes" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                    </Columns>
                                </c1:C1GridView>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top">
                                <asp:Panel ID="SouthPanel" runat="server" ScrollBars="Vertical" Height="100%" style="background-color: #CCCCCC;">
                                    <div id="content"></div>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>

                    <div id="item_template" style="display: none;">        
                        <div class="vehiculo_mimico_kpi">
                            <table>
                                <tr>
                                    <td class="vehiculo">
                                        <div class="vehiculo_info" style="background-image: url({{{ICON}}}); cursor:pointer;" onclick="window.open('../../CicloLogistico/Monitor.aspx?t=D&i={{{ID_RUTA}}}&c=0','Monitor Ciclo');"><span class="{{{VEHICLE_STATE}}}">{{{VEHICLE}}}</span></div>
                                    </td>
                                    <td>
                                        <div class="mimico">
                                            <div class="bars">
                                                {{{STATE_BARS}}}
                                            </div>
                                        </div>          
                                    </td>
                                </tr>
                            </table>    
                        </div>
                    </div>

                    <script type="text/javascript" src="mimicoKpi.js"></script>

                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Timer runat="server" ID="timer" Interval="60000" OnTick="OnTick" />

</asp:Content>    
    
            

