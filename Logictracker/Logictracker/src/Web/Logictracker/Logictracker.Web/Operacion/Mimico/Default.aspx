<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.Mimico.Default" Codebehind="Default.aspx.cs" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mimico</title>
    <script type="text/javascript" src="mimico.js"></script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    <div>
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<a href='../../'><div class='logo_online'> </div></a>" tabPosition="top"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="300" split="false" minSize="300" maxSize="300" collapsible="true" title="Filtros"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="200" split="false" minSize="200" maxSize="200" collapsible="true" title="Resumen"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="20" split="false"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" South="rgSouth" runat="server"></cc1:LayoutManager>
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server">
            <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="465px" >
                
                <%--Tab: Filtros--%>
                <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'../LorryGreen.png\' alt=\'Vehiculos\' title=\'Vehiculos\' />" >
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
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" />
                                
                                <asp:Panel ID="PanelVehiculo" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                </asp:Panel>
                                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbLinea" />
                                
                                <%--Vehiculo--%>
                                <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="PanelVehiculo" ListControlId="cbVehiculo"  />
                                
                                <cwc:MovilListBox ID="cbVehiculo" runat="server" SelectionMode="Multiple" ParentControls="cbTipoVehiculo" Width="100%" Height="300px" AutoPostBack="True" OnSelectedIndexChanged="CbVehiculoSelectedIndexChanged" UseOptionGroup="true" />
                                
                                <asp:Timer ID="timer" runat="server" Interval="20000" Enabled="true" OnTick="TimerTick"></asp:Timer>
                            </ContentTemplate>                         
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>

            </cc1:TabContainer>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server" ScrollBars="Vertical" Height="100%">
            <div id="zoom">
                <div id="changezoom" class="zoomx1" title="Zoom" onclick="Zoom();"></div>
            </div>
            <div id="viewmode">
                <table align="right"><tr>
                    <td><div id="fullmode" class="fullmodeselected" title="Vista Completa" onclick="FullMode(true);"></div></td>
                    <td><div id="simplemode" class="simplemodeunselected" title="Vista Simple" onclick="FullMode(false);"></div></td>
                </tr></table>
            </div>
            
            <div id="content"></div>           
        </asp:Panel>
        
        <asp:Panel ID="EastPanel" runat="server">
            <asp:UpdatePanel ID="updResumen" runat="server" >
            <ContentTemplate>
            
            <table style="font-size: 12px; border-spacing: 10px;">
            
            <tr><td style="font-weight: bold;"> Total: </td><td style="font-weight: bold;"><asp:Label ID="lblTotal" runat="server" ></asp:Label></td></tr>

            <tr><td> En Servicio: </td><td><asp:Label ID="lblEnServicio" runat="server" ></asp:Label></td></tr>
            <tr><td> Sin Servicio: </td><td><asp:Label ID="lblSinServicio" runat="server" ></asp:Label></td></tr>
            <tr><td> En Manteniemiento: </td><td><asp:Label ID="lblEnMantenimiento" runat="server" ></asp:Label></td></tr>            
            <tr><td></td><td></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkEnPlanta" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                     En Planta:
                </td><td>  <asp:Label ID="lblEnPlanta" runat="server" ></asp:Label></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkEnCliente" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                    En Cliente: 
                </td><td> <asp:Label ID="lblEnCliente" runat="server" ></asp:Label></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkEnViaje" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                    En Viaje:
                </td><td>  <asp:Label ID="lblEnViaje" runat="server" ></asp:Label></td></tr>
            <tr><td></td><td></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkEnHora" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                     En Hora: 
                </td><td><asp:Label ID="lblEnHora" runat="server" ></asp:Label></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkDemorados" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                     Demorados: 
                </td><td><asp:Label ID="lblDemorados" runat="server" ></asp:Label></td></tr>
            <tr><td>
                    <asp:CheckBox runat="server" ID="chkAdelantados" Checked="True" AutoPostBack="True" OnCheckedChanged="ChkReload"/>
                     Adelantados: 
                </td><td><asp:Label ID="lblAdelantados" runat="server" ></asp:Label></td></tr>
         
            </table>
            
            </ContentTemplate>
            </asp:UpdatePanel>
            
            
        </asp:Panel>
        
        <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" style="background-color: #666666; height: 20px;padding-top: 0px;">
            <div id='clock'><cwc:OnlineClock ID="olclock" runat="server"></cwc:OnlineClock></div>
            <div id="div_mensajes"></div>
        </asp:Panel>

        <%--BEGIN TEMPLATES--%>
        
        <div id="item_template" style="display: none;">
            <div class="vehiculo_mimico">                   
                <table><tr><td class="vehiculo">
                    <div class="vehiculo_info" style="background-image: url({{{ICON}}});" ><span class="{{{VEHICLE_STATE}}}">{{{VEHICLE}}}</span></div>                                                
                </td><td>
                <div class="mimico">
                    
                    <div>
                        {{{STATE_TIMES}}}
                    </div> 
                    
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
                </td></tr></table>    
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
 