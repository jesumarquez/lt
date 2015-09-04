<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ResumenDeEntidades" Codebehind="ResumenDeEntidades.aspx.cs" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:UpdatePanel ID="pnlUpd" runat="server">
    <ContentTemplate>

    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="M2M_RESUMEN_ENTIDADES" /> 
    
    <style type="text/css">        
        #divResumen { background-color: #d9d9d9; border: solid 1px #cccccc; }
        .homeData { font-size: 12px; font-weight: bold; }
    </style>
    
    <div id="divResumen">
        <table border="0">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" AddAllItem="true" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" />
                    <br />
                    <cwc:TipoEntidadDropDownList ID="ddlTipoEntidad" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa, ddlPlanta" />
                </td>
            </tr>
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dtDesde" runat="server" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dtHasta" runat="server" />
                </td>
                <td>
                    <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" OnClick="BtnActualizar_OnClick" />
                </td>
            </tr>
        </table>
    </div>
    
    <div id="divHoy">
        <br />
        <table width="100%" border="0">
            <tr>
                <td align="center" valign="top" width="50%">
                    <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleVariableName="CLASIFICACION" TitleResourceName="Labels">
                        <table width="100%" border="0">                                    
                            <tr>
                                <td align="center" width="30%">
                                    <table width="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" border="0" style="background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeActInact" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
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
                                                                        <MorePointers>
                                                                            <c1:C1GaugePointer Alignment="In" FlipShape="True" Length="70" Offset="90" Shape="Custom" Value="0">
                                                                                <CustomShape EndRadius="1" EndWidth="3" StartRadius="2" StartWidth="9" />
                                                                                <Border Color="87, 150, 87" />
                                                                                <Filling BrushType="Gradient" Color="87, 150, 87" Color2="87, 150, 87" />
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
                                                        <td align="center">
                                                            <cwc:ResourceLinkButton ID="lnkActivos" runat="server" ResourceName="Labels" VariableName="ENTIDADES_ACTIVAS" Font-Bold="true" ForeColor="Black" OnClick="LnkActivosOnClick" />:
                                                            <asp:Label ID="lblActivas" runat="server" CssClass="labelGaugeVerde"/>
                                                            <br /><br />
                                                            <cwc:ResourceLinkButton ID="lnkInactivos" runat="server" ResourceName="Labels" VariableName="ENTIDADES_INACTIVAS" Font-Bold="true" ForeColor="Black" OnClick="LnkInactivosOnClick" />:
                                                            <asp:Label ID="lblInactivas" runat="server" CssClass="labelGaugeAmarillo" />
                                                            <br /><br />
                                                            <cwc:ResourceLinkButton ID="lnkTotal" runat="server" ResourceName="Labels" VariableName="TOTAL_ENTIDADES" Font-Bold="true" ForeColor="Black" OnClick="LnkTotalOnClick" />:
                                                            <asp:Label ID="lblTotal" runat="server" Font-Bold="true" />
                                                            <br /><br />
                                                            <cwc:ResourceLinkButton ID="lnkAlarmas" runat="server" ResourceName="Labels" VariableName="ENTIDADES_CON_ALARMA" Font-Bold="true" ForeColor="Black" OnClick="LnkAlarmasOnClick" />:
                                                            <asp:Label ID="lblConAlarma" runat="server" CssClass="labelGaugeNaranja" />
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
                    <asp:Timer ID="Timer" runat="server" Interval="300000" OnTick="BtnActualizar_OnClick" />
                </td>
                <td align="center" valign="top" width="50%">
                    <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleVariableName="ALARMAS" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" valign="top">
                                    <table width="98%" height="98%" border="2">
                                        <tr>
                                            <td>
                                                <table width="100%" style="background-color:#D4D0C8;">
                                                    <tr>
                                                        <td align="center">
                                                            <c1:C1Gauge ID="gaugeAlarmas" runat="server" EnableAjax="True" ImageRenderMethod="HttpHandler" BackImageLayout="None" Height="100px" ImageFormat="Png" TextRenderingHint="AntiAlias" Width="200px">
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
                                                        <td align="center">
                                                            <cwc:ResourceLabel ID="ResourceLabel23" runat="server" ResourceName="Labels" VariableName="ALARMAS" Font-Bold="true" />:
                                                            <asp:Label ID="lblAlarmas" runat="server" CssClass="labelGaugeNaranja"/>                                              
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
                <td width="50%" valign="top">
                    <table width="100%" border="0">
                        <tr>
                            <td align="center">
                                <cwc:TitledPanel ID="pnlEntidades" runat="server" TitleVariableName="PAR_ENTIDAD" TitleResourceName="Menu" >
                                    <table width="100%" border="0">
                                        <tr>
                                            <td align="center" valign="top"> 
                                                <asp:Label ID="lblTitEntidades" runat="server" Visible="false" />
                                                <cwc:ResourceLabel ID="lblSinEntidades" runat="server" ResourceName="Labels" VariableName="SIN_ENTIDADES" Visible="false" />
                                                <c1:C1GridView ID="gridEntidades" runat="server" OnRowDataBound="GridEntidadesOnRowDataBound" AutoGenerateColumns="false" Width="100%">
                                                    <Columns>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblTipoEntidad" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lblDescripcion" runat="server" OnClick="LblDescripcionEntidadOnClick" style="text-decoration:none; color:black;" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFechaUltimoReporte" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblValorUltimoReporte" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Image ID="imgEstado" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnMonitorEntidades" runat="server" OnClick="BtnMonitorEntidadesOnClick" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnMonitorSubEntidades" runat="server" OnClick="BtnMonitorSubEntidadesOnClick" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                    </Columns>
                                                </c1:C1GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </cwc:TitledPanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <cwc:TitledPanel ID="pnlDetalles" runat="server" TitleVariableName="DETALLES" TitleResourceName="Labels" Visible="false" >
                                    <table width="100%" border="0">
                                        <tr>
                                            <td align="center" valign="top">
                                                <asp:Label ID="lblTitDetalles" runat="server" Visible="false" />
                                                <cwc:ResourceLabel ID="lblSinDetalles" runat="server" ResourceName="Labels" VariableName="SIN_DETALLE" />
                                                <c1:C1GridView ID="gridDetalles" runat="server" OnRowDataBound="GridDetallesOnRowDataBound" AutoGenerateColumns="false" Width="100%">
                                                    <Columns>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblDescripcion" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblValor" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>                                                        
                                                    </Columns>
                                                </c1:C1GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </cwc:TitledPanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <cwc:TitledPanel ID="pnlSubEntidades" runat="server" TitleVariableName="PAR_SUBENTIDAD" TitleResourceName="Menu" Visible="false" >
                                    <table width="100%" border="0">
                                        <tr>
                                            <td align="center" valign="top">
                                                <cwc:ResourceLabel ID="lblSinSubEntidades" runat="server" ResourceName="Labels" VariableName="SIN_SUBENTIDAD" />
                                                <c1:C1GridView ID="gridSubEntidades" runat="server" OnRowDataBound="GridSubEntidadesOnRowDataBound" AutoGenerateColumns="false" Width="100%">
                                                    <Columns>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblEntidad" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lblDescripcion" runat="server" OnClick="LblDescripcionSubEntidadOnClick" style="text-decoration:none; color:black;" />                                                                
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblFechaUltimoReporte" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblValorUltimoReporte" runat="server" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:Image ID="imgEstado" runat="server" />
                                                            </ItemTemplate>
                                                        </c1:C1TemplateField>
                                                        <c1:C1TemplateField>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="btnMonitorSubEntidades" runat="server" OnClick="BtnMonitorSubEntidades2OnClick" />
                                                            </ItemTemplate>                                            
                                                        </c1:C1TemplateField>
                                                    </Columns>
                                                </c1:C1GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </cwc:TitledPanel>
                            </td>
                        </tr>                    
                    </table>                    
                </td>
                <td width="50%" valign="top" align="center">
                    <cwc:TitledPanel ID="pnlAlarmas" runat="server" TitleVariableName="DETALLE_ALARMAS" TitleResourceName="Labels">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center" valign="top">
                                    <asp:Label ID="lblTitAlarma" runat="server" Visible="false" />
                                    <cwc:ResourceLabel ID="lblSinAlarmas" runat="server" ResourceName="Labels" VariableName="SIN_ALARMA" />
                                    <c1:C1GridView ID="gridAlarmas" runat="server" OnRowDataBound="GridAlarmasOnRowDataBound" AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <c1:C1TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblFecha" runat="server" />
                                                </ItemTemplate>                                            
                                            </c1:C1TemplateField>
                                            <c1:C1TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEntidad" runat="server" />
                                                </ItemTemplate>                                            
                                            </c1:C1TemplateField>
                                            <c1:C1TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSubentidad" runat="server" />
                                                </ItemTemplate>                                            
                                            </c1:C1TemplateField>
                                            <c1:C1TemplateField>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTexto" runat="server" />
                                                </ItemTemplate>                                            
                                            </c1:C1TemplateField>
                                        </Columns>
                                    </c1:C1GridView>
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
    </div>

    </ContentTemplate>
</asp:UpdatePanel>
</asp:Content>


