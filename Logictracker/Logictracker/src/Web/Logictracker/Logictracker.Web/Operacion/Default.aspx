<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.MonitorOnline" Codebehind="Default.aspx.cs" %> 

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>

<%@ Register src="../App_Controls/Mensajeria.ascx" tagname="Mensajeria" tagprefix="uc1" %>
<%@ Register src="../App_Controls/Consultas.ascx" tagname="Consultas" tagprefix="uc2" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Monitor On-Line</title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
    <script type="text/javascript">
        function getCamP(cam) {
            window.open("Camaras/Default.aspx?i="+cam, "camara"+cam, "width=352,height=310,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no");
            return false;
        }
        function getMovP(id, empresa, linea) {
            return "<iframe width=\"400\" height=\"281\" style=\"border:none;\" src=\"InfoMovil.aspx?c=" + id + "&e=" + empresa + "&l=" + linea + "&\" />";
        }
        function getPOIP(id, lin) {
            return "<iframe width=\"250\" height=\"160\" style=\"border:none;\" src=\"InfoPOI.aspx?p=" + id + "&l=" + lin + "\" />";
        }
        function getPOIP(id, lin, emp) {
            return "<iframe width=\"250\" height=\"160\" style=\"border:none;\" src=\"InfoPOI.aspx?p=" + id + "&l=" + lin + "&e=" + emp + "\" />";
        }
        function getDirP(lin, dir, lat, lon) {
            return "<iframe width=\"350\" height=\"240\" style=\"border:none;\" src=\"InfoDir.aspx?d=" + dir + "&l=" + lin + "&la=" + lat + "&lo=" + lon + "\" />";
        }
        function getDimensions() {
            var winWidth, winHeight;
            var d = document;
            if (typeof window.innerWidth != 'undefined') 
            {
                winWidth = window.innerWidth;
                winHeight = window.innerHeight;
            }
            else 
            {
                if (d.documentElement &&
                typeof d.documentElement.clientWidth != 'undefined' &&
                d.documentElement.clientWidth != 0) 
                {
                    winWidth = d.documentElement.clientWidth;
                    winHeight = d.documentElement.clientHeight;
                }
                else 
                {
                    if (d.body &&
                    typeof d.body.clientWidth != 'undefined') 
                    {
                        winWidth = d.body.clientWidth;
                        winHeight = d.body.clientHeight;
                    }
                }
            }
            return { width: winWidth, height: winHeight };
        }
        function Resize() 
        {
            var size = getDimensions();
            var div = $get('progress');
            div.style.width = size.width + "px";
            div.style.height = size.height + "px";
        }
        var serverTime = new Date();
        var clockInterval = 10;
        function startClock(datetime) 
        {
            serverTime.setTime(datetime.getTime() + 5000);
            setInterval(clockTick, clockInterval * 1000);
        }
        function clockTick() 
        {
            serverTime.setTime(serverTime.getTime() + (clockInterval * 1000));
            $get('clock').innerHTML = PadLeft(serverTime.getHours(), 2, '0') + ':' + PadLeft(serverTime.getMinutes(), 2, '0');
        }
        function PadLeft(str, len, ch) 
        {
            var str2 = str + '';
            while (str2.length < len) str2 = ch + str2;
            return str2;
        }
    </script>

</head>
<body id="monitor">
    <form id="form1" runat="server">
    <div>
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200"
            minSize="200" maxSize="500" title="<a href='../'><div class='logo_monitor_online'> </div></a>" tabPosition="top">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="260"
            split="false" minSize="260" maxSize="260" collapsible="true" title="Filtros">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="220"
            split="false" minSize="220" maxSize="220" collapsible="true" title="Opciones">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="22"
            split="false"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter"
            South="rgSouth" West="rgWest" East="rgEast" runat="server">
        </cc1:LayoutManager>
        <asp:Panel ID="pnlManager" runat="server">
        </asp:Panel>
        
        <!--West: Filtros-->
        <asp:Panel ID="WestPanel" runat="server">     
        
            <cc1:TabContainer ID="TabContainer2" runat="server" ActiveTabIndex="0" Height="440px" >
          
                <%--Tab: Filtros--%>
                <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'LorryGreen.png\' alt=\'Vehiculos\' title=\'Vehiculos\' />" >
                    <HeaderTemplate>
                        <img alt="\'Vehiculos\'" src="'LorryGreen.png/'" title="\'Vehiculos\'" />
                    </HeaderTemplate>
                    <ContentTemplate>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <%--Empresa, Linea, Tipo Vehiculo--%>
                                <table id="tbFilters">
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:LocacionDropDownList ID="cbLocacion" runat="server" AutoPostBack="True" Width="100%" OnSelectedIndexChanged="CbLocacionSelectedIndexChanged" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion" OnSelectedIndexChanged="CbPlantaSelectedIndexChanged" AddAllItem="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel9" runat="server" VariableName="PARENTI04" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="CbDepartamentoSelectedIndexChanged" ParentControls="cbLocacion,cbPlanta" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel10" runat="server" VariableName="PARENTI37" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion,cbPlanta,cbDepartamento" OnSelectedIndexChanged="CbCentroDeCostosSelectedIndexChanged" AddAllItem="True" AddNoneItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel11" runat="server" VariableName="PARENTI99" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:SubCentroDeCostosDropDownList ID="cbSubCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="CbSubCentroDeCostosSelectedIndexChanged" ParentControls="cbLocacion,cbPlanta,cbDepartamento,cbCentroDeCostos" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" VariableName="PARENTI07" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion,cbPlanta" OnSelectedIndexChanged="CbTransportistaSelectedIndexChanged" AddAllItem="True" AddNoneItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="PARENTI17" ResourceName="Entities" />
                                        </div>
                                        </th>
                                        <td>
                                            <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="CbTipoVehiculoSelectedIndexChanged" ParentControls="cbPlanta" />
                                        </td>
                                    </tr>
                                </table>
                                
                                <asp:Panel ID="PanelVehiculo" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                </asp:Panel>
                                
                                <%--Vehiculo--%>
                                <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="PanelVehiculo" ListControlId="cbVehiculo" />
          
                                <cwc:MovilListBox ID="cbVehiculo" runat="server" SelectionMode="Multiple" ParentControls="cbPlanta,cbTipoVehiculo,cbTipoEmpleado,cbCentroDeCostos,cbTransportista,cbSubCentroDeCostos,cbDepartamento" Width="100%" Height="180px" AutoPostBack="True" OnSelectedIndexChanged="CbVehiculoSelectedIndexChanged" UseOptionGroup="true" HideWithNoDevice="True" HideInactive="True" onchange="onCall=true;" />
                                <asp:Button ID="btCenter" runat="server" style="display: none;" OnClick="BtCenterClick" />
                                
                                <%--<div class="header" style="text-align: center;">
                                    <cwc:ResourceLinkButton ID="btnCochesConTickets" runat="server" AutoPostBack="true" OnClick="BtnCochesConTickets_OnClick" ResourceName="Labels" VariableName="CON_TICKETS_ACTIVOS" ForeColor="#FFFFFF" Font-Underline="false" />
                                </div>--%>
                                
                            </ContentTemplate> 
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="lstTicket" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="LnkOn" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="LnkOff" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="lnkNA" EventName="Click" />
                            </Triggers>                           
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
                <%--Tab: Referencia Geográfica--%>
                <cc1:TabPanel ID="tabGeoRef" runat="server" HeaderText="<img src=\'office.png\' alt=\'Referencias Geograficas\' title=\'Referencias Geograficas\' />">
                    <ContentTemplate>
                        <asp:UpdatePanel ID="updPoi" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <%--Tipos Referencia Geográfica--%>
                                <asp:Panel ID="PanelPoi" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="lblTipoPOI" runat="server" ResourceName="Entities" VariableName="PARENTI10" />
                                </asp:Panel>
                                <cwc:SelectAllExtender ID="selPoi" runat="server" AutoPostBack="true" TargetControlId="PanelPoi" ListControlId="cbPoi"  />
                                <cwc:TipoReferenciaGeograficaListBox ID="cbPoi" runat="server" AutoPostBack="true"  SelectionMode="Multiple" RememberSessionValues="false" ParentControls="cbPlanta" OnSelectedIndexChanged="CbPoiSelectedIndexChanged" Width="100%" Height="380px" Monitor="True" />
                                
                                <div class="header" style="padding: 10px; height: 40px;">
                                    <cwc:ResourceCheckBox ID="chkTickets" runat="server" AutoPostBack="true" Checked="false" OnCheckedChanged="CbVehiculoSelectedIndexChanged" ResourceName="Labels" VariableName="TICKETS_ACTIVOS" />
                                    <br/><br/>
                                    <cwc:ResourceCheckBox ID="chkTalleres" runat="server" AutoPostBack="true" Checked="false" OnCheckedChanged="ChkTalleresCheckedChanged" ResourceName="Securables" VariableName="VIEW_TALLER" />
                                </div>
                            
                            </ContentTemplate> 
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                            </Triggers>                            
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
                
                <%--Tab: Clientes--%>
                <cc1:TabPanel ID="tabRef" runat="server" HeaderText="<img src=\'company.png\' alt=\'Clientes\' title=\'Clientes\' />" >
                    <ContentTemplate>
                    <asp:UpdatePanel ID="updCliente" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                        
                            <asp:Panel ID="panelHeaderCliente" runat="server" CssClass="header">
                                <cwc:ResourceLabel ID="lblCliente" runat="server" VariableName="CLIENT" ResourceName="Entities" />
                            </asp:Panel>
                        
                            <cwc:SelectAllExtender ID="selClientes" runat="server" AutoPostBack="true" TargetControlId="panelHeaderCliente" ListControlId="cbCliente"  />
                            
                            <div>
                                <asp:TextBox ID="txtClientes" runat="server" width="245px" CssClass="LogicTextbox" />
                                <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender3" runat="server"
                                    ResourceName="Labels" VariableName="BUSCAR" TargetControlID="txtClientes"
                                    WatermarkCssClass="LogicWatermark" />
                            </div>
                            <cwc:ClienteListBox ID="cbCliente" runat="server" Width="100%" Height="300px" RememberSessionValues="false" AutoPostBack="true" ParentControls="cbPlanta" OnSelectedIndexChanged="CbClienteSelectedIndexChanged" FilterControl="txtClientes" />
                                
                            <div  class="header" style="padding: 10px;">
                                <cwc:ResourceCheckBox ID="chkCliente" runat="server" AutoPostBack="true" Checked="true" OnCheckedChanged="CbClienteSelectedIndexChanged" ResourceName="Entities" VariableName="CLIENT" />
                                <div style="height: 10px;"></div>
                                <cwc:ResourceCheckBox ID="chkPuntoEntrega" runat="server" AutoPostBack="true" Checked="false" OnCheckedChanged="CbClienteSelectedIndexChanged" ResourceName="Entities" VariableName="PARENTI44"  />
                            </div>
                            
                        </ContentTemplate>   
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                        </Triggers>                          
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
                
                <%--Tab: Mensajes--%>
                <cc1:TabPanel ID="TabPanel5" runat="server" HeaderText="<img src=\'caution.png\' alt=\'Eventos\' title=\'Eventos\' />">
                    <ContentTemplate>                    
                        <asp:UpdatePanel ID="updEventos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <asp:Panel ID="PanelMensajes" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PAREVEN01" ResourceName="Entities" />
                                </asp:Panel>
                                <cwc:SelectAllExtender ID="selMensajes" runat="server" AutoPostBack="true" TargetControlId="PanelMensajes" ListControlId="cbMensajes"  />
                        
                                <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="cbLocacion,cbPlanta"
                                    OnSelectedIndexChanged="CbTipoMensajeSelectedIndexChanged" />
                            
                                <cwc:MensajesListBox ID="cbMensajes" runat="server" ParentControls="cbPlanta,cbTipoMensaje" UseOptionGroup="true"
                                    Width="100%" Height="350px" AutoPostBack="true" SelectionMode="Multiple" OnSelectedIndexChanged="CbMensajeSelectedIndexChanged" />
                            
                            </ContentTemplate> 
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                            </Triggers>                            
                        </asp:UpdatePanel>                        
                    </ContentTemplate>
                </cc1:TabPanel>
                
                <%--Tab: Tickets--%>
                <cc1:TabPanel ID="TabPanel6" runat="server" HeaderText="<img src=\'ticket.png\' alt=\'Tickets\' title=\'Tickets\' />" >
                    <ContentTemplate>                    
                        <asp:UpdatePanel ID="updTickets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <asp:Panel ID="PanelTickets" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="OPETICK01" ResourceName="Entities" />
                                </asp:Panel>
                                <cwc:SelectAllExtender ID="selTickets" runat="server" AutoPostBack="true" TargetControlId="PanelTickets" ListControlId="lstTicket" />
                                
                                <div>
                                    <asp:TextBox ID="txtTickets" runat="server" width="245px" CssClass="LogicTextbox" />
                                    <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender4" runat="server" ResourceName="Labels" VariableName="BUSCAR" TargetControlID="txtTickets" WatermarkCssClass="LogicWatermark" />
                                </div>
                                <asp:ListBox ID="lstTicket" runat="server" style="overflow: auto; font-size:xx-small;" Width="100%" Height="310px" AutoPostBack="true" SelectionMode="Multiple" OnSelectedIndexChanged="LstTicketsSelectedIndexChanged" />
                                <div align="center">
                                    <cwc:ResourceButton ID="btnBuscarTickets" runat="server" CssClass="LogicButton_Big" VariableName="BUSCAR_VIAJES" ResourceName="Labels" OnClick="BtnBuscarTicketsOnClick"/>
                                </div>
                            </ContentTemplate> 
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnBuscarTickets" EventName="Click" />
                            </Triggers>                            
                        </asp:UpdatePanel>                        
                    </ContentTemplate>
                </cc1:TabPanel>
                
            </cc1:TabContainer>
            
            <asp:UpdatePanel runat="server" ID="pnlTotalizador" UpdateMode="Conditional">
                <ContentTemplate>
                    
                    <div class="header" style="text-align: center;">
                        <cwc:ResourceLabel ID="lblTotalizador" runat="server" VariableName="TOTALIZADOR" ResourceName="Labels" Font-Bold="True" />
                        <cwc:TotalizadorDropDownList ID="cbTotalizador" runat="server" OnSelectedIndexChanged="CbTotalizadorSelectedIndexChanged" ParentControls="cbLocacion,cbPlanta,cbDepartamento,cbCentroDeCostos,cbSubCentroDeCostos,cbTransportista,cbTipoVehiculo" />
                    </div>
                    <div style="background-color: #CCCCCC; text-align: center;">
                        <asp:Label ID="lblTotal" runat="server" Text="TOTAL: " Font-Bold="True" />
                        <br/>
                        <asp:LinkButton ID="lnkOn" runat="server" AutoPostBack="true" OnClick="LnkOnClick" ForeColor="#3E9A00" Font-Bold="True" Font-Underline="false">
                            <cwc:ResourceLabel ID="lnkOnRL" runat="server" VariableName="COUNTER_ON" ResourceName="Labels" />
                        </asp:LinkButton>
                        <br/>
                        <asp:LinkButton ID="lnkOff" runat="server" AutoPostBack="true" OnClick="LnkOnClick" Text="OFF: " ForeColor="#FF3333" Font-Bold="True" Font-Underline="false">
                            <cwc:ResourceLabel ID="lnkOffRL" runat="server" VariableName="COUNTER_OFF" ResourceName="Labels" />
                        </asp:LinkButton>
                        <br/>
                        <asp:LinkButton ID="lnkNA" runat="server" AutoPostBack="true" OnClick="LnkOnClick" Text="N/A: " ForeColor="#000000" Font-Bold="True" Font-Underline="false">
                            <cwc:ResourceLabel ID="lnkNARL" runat="server" VariableName="COUNTER_NOT_AVAILABLE" ResourceName="Labels" />
                        </asp:LinkButton>
                    </div>                    
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbCentroDeCostos" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbSubCentroDeCostos" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbTotalizador" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            
        </asp:Panel>
        <asp:Panel ID="EastPanel" runat="server">
            <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="500px" Width="220px">
                <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="Buscar">
                    <ContentTemplate>
                        
                        <div class="header">
                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                        </div>
                        <asp:Panel ID="panelBuscarVehiculo" runat="server" CssClass="content" DefaultButton="btBuscar">
                            <div style="text-align: center; padding: 5px; padding-top:10px;">
                                <asp:UpdatePanel runat="server" ID="updVehiculo" RenderMode="Inline" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:AutoCompleteTextBox ID="autoVehiculo" runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetVehiculos" Width="90%" ParentControls="cbLocacion,cbPlanta" />
                                        <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender1" runat="server"
                                            ResourceName="Entities" VariableName="PARENTI03" TargetControlID="autoVehiculo"
                                            WatermarkCssClass="LogicWatermark" />     
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged"/>
                                        <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div style="text-align: right;">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ResourceButton runat="server" ID="btBuscar" CssClass="LogicButton_Big" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtBuscarClick" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        
                       <div class="header">Referencia Geogr&aacute;fica</div>
                       <asp:Panel ID="panelBuscarPOI" runat="server" CssClass="content" DefaultButton="btBuscarPoi">
                             <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:MultiView ID="MultiViewPoi" runat="server" ActiveViewIndex="0">
                                        <asp:View ID="ViewPoiSearch" runat="server">
                                            <asp:Label ID="lblPoiResult" runat="server" Text=""></asp:Label>
                                            <div style="text-align: center; padding: 5px; padding-top:10px;">
                                                <asp:TextBox ID="txtPoi" runat="server" Width="90%" MaxLength="64" CssClass="LogicTextbox"></asp:TextBox>
                                                <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender2" runat="server"
                                                    ResourceName="Entities" VariableName="PARENTI05" TargetControlID="txtPoi"
                                                    WatermarkCssClass="LogicWatermark" />
                                            </div>
                                            <div style="text-align: right;">
                                                <cwc:ResourceButton runat="server" ID="btBuscarPoi" CssClass="LogicButton_Big"
                                                    ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtBuscarPoiClick" />
                                            </div>
                                            
                                            
                                            <%--Ingrese la descripci&oacute;n del Punto de Inter&eacute;s que desea localizar<br />--%>

                                        </asp:View>
                                        <asp:View ID="ViewPoiResult" runat="server">
                                            Se encontraron los siguientes resultados:
                                            <br />
                                            <asp:ListBox ID="cbPoiResult" runat="server" Width="100%" Height="200px" Font-Size="X-Small"></asp:ListBox>
                                            <div style="text-align: right;">
                                                <asp:Button runat="server" ID="btCancelPoi" Text="Cancelar" CssClass="LogicButton_Big" OnClick="BtCancelPoiClick" />
                                                <asp:Button runat="server" ID="btSelectPoi" Text="Seleccionar" CssClass="LogicButton_Big" OnClick="BtSelectPoiClick" />
                                            </div>
                                        </asp:View>
                                    </asp:MultiView>
                                </ContentTemplate>
                            </asp:UpdatePanel>  
                        </asp:Panel>
                        
                        <div class="header">
                            <cwc:ResourceLabel ID="ResourceLabel12" runat="server" VariableName="ENTREGA" ResourceName="Labels" />
                        </div>
                        <asp:Panel ID="panel1" runat="server" CssClass="content" DefaultButton="btnBuscarEntrega">
                            <div style="text-align: center; padding: 5px; padding-top:10px;">
                                <asp:UpdatePanel runat="server" ID="UpdatePanel3" RenderMode="Inline" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtEntrega" runat="server" Width="90%" MaxLength="64" CssClass="LogicTextbox" />
                                        <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender5" runat="server"
                                            ResourceName="Labels" VariableName="ENTREGA" TargetControlID="txtEntrega"
                                            WatermarkCssClass="LogicWatermark" />     
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged"/>
                                        <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                            <div style="text-align: right;">
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ResourceButton runat="server" ID="btnBuscarEntrega" CssClass="LogicButton_Big" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnBuscarEntregaClick" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="Mensajer&iacute;a">
                <ContentTemplate>
                    <uc1:Mensajeria ID="Mensajeria1" runat="server" ParentControls="cbPlanta,cbTipoVehiculo" />
                    </ContentTemplate>
                </cc1:TabPanel>
                <cc1:TabPanel ID="TabPanel3" runat="server" HeaderText="Consultas">
                <ContentTemplate>
                    <uc2:Consultas ID="Consultas1" runat="server" OnDireccionAdded="Consultas1DireccionAdded" OnDireccionRemoved="Consultas1DireccionRemoved" OnDireccionSelected="Consultas1DireccionSelected" OnClear="Consultas1Clear" OnDireccionSaved="Consultas1DireccionSaved" ParentControls="cbPlanta" />
                </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
        </asp:Panel>
        <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" Style="background-color: #666666;
            height: 22px; padding-top: 0px;">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:ImageButton ID="connection_status" runat="server" ToolTip="[Conectado]" ImageUrl="connected.png"
                        Style="float: left; margin-right: 20px;" OnClick="ConnectionStatusClick" />
                </ContentTemplate>  
            </asp:UpdatePanel>
            <div id="clock" style="width: auto; color: White; float: right; font-size: 14px;
                font-weight: bold; padding-right: 10px; padding-left: 10px; background-color: #666666;
                border-left: solid 1px #999999; border-right: solid 1px #999999;">
                00:00</div>
            <img src="exclamation.png" onclick="ToggleEvents();" style="cursor: pointer; float: right;" />
            <asp:Label ID="lblInfo" runat="server" Text="Iniciando Monitor..."></asp:Label>
        </asp:Panel>
        <asp:Panel ID="CenterPanel" runat="server">
            <mon:Monitor ID="Monitor" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
        <asp:Panel ID="panelPopup" runat="server" Style="width: 240px; height: 500px; position: absolute;
            bottom: 20px; right: 0px; border: solid 1px black; background-color: White; z-index: 99999;
            display: none;">
            <div class="header" style="cursor: pointer;" onclick="HideEvents();">
                Eventos</div>
            <asp:Panel ID="panelPopupEvents" runat="server" ScrollBars="Auto" Style="height: 100%">
            </asp:Panel>
            <asp:Panel ID="panelPopupDetail" runat="server" Style="height: 490px; display: none;">
                <iframe id="ifrPopupDetail" style="width: 240px; height: 490px; border: none;"></iframe>
            </asp:Panel>
        </asp:Panel>
        <div>
        <div id="soundPlace"></div>           

            <script type="text/javascript">
            <!--
                function enqueueSounds(audiofiles) {
                    for(var i = 0; i < audiofiles.length && i < 5; i++)
                    {
                        setTimeout('playSound("' + audiofiles[i] + '")', 2000*i);
                    }
                }
                function playSound(audiofile) {
                    if ($get('FlashSound').playSound) { $get('FlashSound').playSound(audiofile); }
                    else { $get('FlashSound2').playSound(audiofile); }
                    //var place = $get("soundPlace");
                    //place.innerHTML = '<embed src="'+audiofile+'" autostart="true" width="0" height="0" id="sound1" enablejavascript="true" />';                   
                }
            //-->
            </script>
            
            </div>
    </div>
    <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div id="progress" class="progress">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <script type="text/javascript">
        $addHandler(window, 'resize', Resize);
        Resize();
    </script>

    </form>
</body>
</html>
 