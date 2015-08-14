<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MimicoEntregas.aspx.cs" Inherits="Logictracker.Operacion.Mimico.MimicoEntregas" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mímico Entregas</title>
    <script type="text/javascript" src="mimicoEntregas.js"></script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    <div>
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<a href='../../'><div class='logo_online'> </div></a>" tabPosition="top"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="300" split="false" minSize="300" maxSize="300" collapsible="true" title="Filtros"></cc1:LayoutRegion>        
        <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="20" split="false"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" South="rgSouth" runat="server"></cc1:LayoutManager>
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server">
            <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="600px" >
                
                <%--Tab: Filtros--%>
                <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'../LorryGreen.png\' alt=\'Vehículos\' title=\'Vehículos\' />" >
                    <ContentTemplate>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                            
                                <%--Empresa, Linea, Tipo Vehiculo--%>
                        
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                </div>
                                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="100%" />
                                
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblLinea" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                </div>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" />
                                
                                <asp:Panel ID="PanelVehiculo" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                </asp:Panel>
                                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbLinea" />
                                
                                <%--Vehiculo--%>
                                <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="PanelVehiculo" ListControlId="cbVehiculo"  />
                                <cwc:MovilListBox ID="cbVehiculo" runat="server" SelectionMode="Multiple" ParentControls="cbTipoVehiculo" Width="100%" Height="200px" AutoPostBack="True" />
                                
                                <asp:Panel ID="PanelEstado" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="lblEstadoRuta" runat="server" VariableName="ESTADO_RUTA" ResourceName="Labels" />
                                </asp:Panel>
                                <cwc:SelectAllExtender ID="selEstado" runat="server" AutoPostBack="true" TargetControlId="PanelEstado" ListControlId="cbEstadoRuta" />
                                <cwc:EstadoViajeDistribucionListBox ID="cbEstadoRuta" runat="server" Width="100%" Height="55px" SelectionMode="Multiple" />

                                <div class="header">
                                    <table width="100%">
                                        <tr>
                                            <td align="left">
                                                <cwc:ResourceLabel ID="lblFecha" runat="server" VariableName="FECHA" ResourceName="Labels" />
                                            </td>
                                            <td align="left">
                                                <cwc:DateTimePicker ID="dtFecha" runat="server" Mode="Date" TimeMode="Start" />
                                            </td>
                                            <td align="center">
                                                <cwc:ResourceButton ID="btnBuscar" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtnBuscarClick" ResourceName="Controls" VariableName="BUTTON_SEARCH" />
                                            </td>
                                        </tr>                                        
                                    </table>
                                </div>

                                <div>
                                    <table width="100%">
                                        <tr>
                                            <td width="30%">
                                                <cwc:ResourceLabel ID="lblCompletadas" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_COMPLETADO" />
                                            </td>
                                            <td class="verde" width="10%">
                                                &nbsp;
                                            </td>
                                            <td width="50%">
                                                <cwc:ResourceLabel ID="lblVisitadas" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_VISITADO" />
                                            </td>
                                            <td class="amarillo" width="10%">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblEnSitio" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_ENSITIO" />
                                            </td>
                                            <td class="azul">
                                                &nbsp;
                                            </td>
                                            <td>
                                                <cwc:ResourceLabel ID="lblEnZona" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_ENZONA" />
                                            </td>
                                            <td class="gris">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr >
                                            <td>
                                                <cwc:ResourceLabel ID="lblNoCompletadas" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_NOCOMPLETADO" />
                                            </td>
                                            <td class="rojo">
                                                &nbsp;
                                            </td>             
                                            <td>
                                                <cwc:ResourceLabel ID="lblNoVisitadas" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_SINVISITAR" /> - <cwc:ResourceLabel ID="lblPendientes" runat="server" ResourceName="Labels" VariableName="ENTREGA_STATE_PENDIENTE" /> 
                                            </td>
                                            <td class="naranja">
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </div>

                            </ContentTemplate>                         
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>

            </cc1:TabContainer>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server" ScrollBars="Vertical" Height="100%"  style="background-color: #CCCCCC;">
            <div id="viewmode">
                <table align="right"><tr>
                    <td><div id="fullmode" class="fullmodeselected" title="Vista Completa" onclick="FullMode(true);"></div></td>
                    <td><div id="simplemode" class="simplemodeunselected" title="Vista Simple" onclick="FullMode(false);"></div></td>
                </tr></table>
            </div>
            
            <div id="content"></div>           
        </asp:Panel>
        
        <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" style="background-color: #666666; height: 20px;padding-top: 0px;">
            <div id='clock'><cwc:OnlineClock ID="olclock" runat="server"></cwc:OnlineClock></div>
            <div id="div_mensajes"></div>
        </asp:Panel>

        <%--BEGIN TEMPLATES--%>
        
        <div id="item_template" style="display: none;">
            <div class="vehiculo_mimico_entregas">
                <table>
                    <tr>
                        <td class="vehiculo">
                            <div class="vehiculo_info" style="background-image: url({{{ICON}}});" ><span class="{{{VEHICLE_STATE}}}">{{{VEHICLE}}}</span></div>                                                
                        </td>
                        <td>
                            <div class="mimico">
                                <div class="bars">
                                    {{{STATE_BARS}}}
                                </div>                    
                                <div class="completed_bar" style="width: {{{COMPLETED_BAR}}}"></div>
                    
                                <div class="marker" style="left: {{{COMPLETED}}};">
                                    <div class="marker_line"></div>
                                    <div class="marker_icon"></div>
                                    <div class="marker_text">{{{COMPLETED_TEXT}}}</div>
                                </div>
                                <!-- <div style="position: absolute; left: 0px; top: 8px;background-color: red; height: 20px; width: 40%;"></div> -->
                            </div>          
                        </td>
                    </tr>
                </table>    
            </div>
            <%--<div id="state_time_template" style="display: none;">
                <div class="horario" style="{{{POSITION_PROP}}}: {{{POSITION}}};width: {{{SIZE}}};" title="{{{TITLE}}}"><span class="programado">{{{PROG_TIME}}}</span> <span class="real">{{{REAL_TIME}}}</span></div>
            </div>
            
            <div id="state_bar_template" style="display: none;">
                <div class="tramodiv {{{STATE}}}" style="left: {{{POSITION}}};width: {{{SIZE}}};" title="{{{TITLE}}}">{{{TEXT}}}</div>
            </div>--%>

        </div>

        <%--END TEMPLATES--%>
    </div>
    
    <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div id="progress" class="progress">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    </form>
</body>
</html>
 