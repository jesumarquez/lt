<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Monitor.MonitorDeEntidades.MonitorEntidades" Codebehind="MonitorEntidades.aspx.cs" %> 

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Monitor De Entidades</title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/default.css"/>
    <script type="text/javascript">
        function getEntP(id, empresa, linea) 
        {
            return "<iframe width=\"400\" height=\"140\" style=\"border:none;\" src=\"InfoEntidad.aspx?c=" + id + "&e=" + empresa + "&l=" + linea + "&\" />";
        }
        function getPOIP(id, lin) 
        {
            return "<iframe width=\"250\" height=\"160\" style=\"border:none;\" src=\"InfoPOI.aspx?p=" + id + "&l=" + lin + "\" />";
        }
        function getDirP(lin, dir, lat, lon) 
        {
            return "<iframe width=\"350\" height=\"240\" style=\"border:none;\" src=\"InfoDir.aspx?d=" + dir + "&l=" + lin + "&la=" + lat + "&lo=" + lon + "\" />";
        }
        function getDimensions() 
        {
            var winWidth, winHeight;
            var d = document;
            if (typeof window.innerWidth != 'undefined') 
            {
                winWidth = window.innerWidth;
                winHeight = window.innerHeight;
            }
            else 
            {
                if (d.documentElement && typeof d.documentElement.clientWidth != 'undefined' && d.documentElement.clientWidth != 0) 
                {
                    winWidth = d.documentElement.clientWidth
                    winHeight = d.documentElement.clientHeight
                }
                else 
                {
                    if (d.body && typeof d.body.clientWidth != 'undefined') 
                    {
                        winWidth = d.body.clientWidth
                        winHeight = d.body.clientHeight
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
        var clockInterval = 200;
        function startClock(datetime) 
        {
            serverTime.setTime(datetime.getTime() + 5000);
            setInterval(clockTick, clockInterval * 1000);
        }
        function clockTick() 
        {
            serverTime.setTime(serverTime.getTime() + (clockInterval * 1000));
            $get('clock').innerHTML = PadLeft(serverTime.getHours(), 2, '0') + '<blink>:</blink>' + PadLeft(serverTime.getMinutes(), 2, '0');
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

            <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<a href='../../'><div class='logo_online'> </div></a>" tabPosition="top" />
            <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="225" split="false" minSize="225" maxSize="225" collapsible="true" title="Filtros" />
            <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="220" split="false" minSize="220" maxSize="220" collapsible="true" title="Opciones" />
            <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="20" split="false" />
            <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" South="rgSouth" West="rgWest" East="rgEast" runat="server" />
            <asp:Panel ID="pnlManager" runat="server" />
            
            <!--West: Filtros-->
            <asp:Panel ID="WestPanel" runat="server">
                <cc1:TabContainer ID="TabContainer2" runat="server" ActiveTabIndex="0" Height="500px">
                    <%--Tab: Filtros--%>
                    <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'../../Operacion/LorryGreen.png\' alt=\'Entidades\' title=\'Entidades\' />" >
                        <HeaderTemplate>
                            <img alt="\'Entidades\'" src="'../../Operacion/LorryGreen.png/'" title="\'Entidades\'" />
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>                            
                                    <%--Empresa, Linea, Tipo Vehiculo--%>                        
                                    <div class="header">
                                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                    </div>
                                    <cwc:LocacionDropDownList ID="cbLocacion" runat="server" AutoPostBack="True" Width="100%" OnSelectedIndexChanged="CbLocacionSelectedIndexChanged" />
                                    
                                    <div class="header">
                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                    </div>
                                    <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion" OnSelectedIndexChanged="CbPlantaSelectedIndexChanged" AddAllItem="true" />
                                    
                                    <asp:Panel ID="PanelEntidad" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="PARENTI79" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="CbTipoEntidadSelectedIndexChanged" ParentControls="cbPlanta" />
                                    
                                    <%--Entidad--%>
                                    <cwc:SelectAllExtender ID="selEntidad" runat="server" AutoPostBack="true" TargetControlId="PanelEntidad" ListControlId="cbEntidad" />
              
                                    <cwc:EntidadListBox ID="cbEntidad" runat="server" SelectionMode="Multiple" ParentControls="cbPlanta,cbTipoEntidad" Width="100%" Height="300px" AutoPostBack="True" OnSelectedIndexChanged="CbEntidadSelectedIndexChanged" UseOptionGroup="true" />
                                    <asp:Button ID="btCenter" runat="server" style="display: none;" OnClick="BtCenterClick" />
                                    
                                </ContentTemplate> 
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                </Triggers>                           
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    <%--Tab: Referencia Geográfica--%>
                    <cc1:TabPanel ID="tabGeoRef" runat="server" HeaderText="<img src=\'../../Operacion/office.png\' alt=\'Referencias Geograficas\' title=\'Referencias Geograficas\' />">
                        <ContentTemplate>                    
                            <asp:UpdatePanel ID="updPoi" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <%--Tipos Referencia Geográfica--%>
                                    <asp:Panel ID="PanelPoi" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="lblTipoPOI" runat="server" ResourceName="Entities" VariableName="PARENTI10" />
                                    </asp:Panel>
                                    <cwc:SelectAllExtender ID="selPoi" runat="server" AutoPostBack="true" TargetControlId="PanelPoi" ListControlId="cbPoi"  />
                                    <cwc:TipoReferenciaGeograficaListBox ID="cbPoi" runat="server" AutoPostBack="true"  SelectionMode="Multiple" RememberSessionValues="false" ParentControls="cbPlanta" OnSelectedIndexChanged="CbPoiSelectedIndexChanged" Width="100%" Height="400px" />                                                            
                                </ContentTemplate> 
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                </Triggers>                            
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </cc1:TabPanel>               
                    
                    <%--Tab: Clientes--%>
                    <cc1:TabPanel ID="tabRef" runat="server" HeaderText="<img src=\'../../Operacion/company.png\' alt=\'Clientes\' title=\'Clientes\' />" >
                        <ContentTemplate>
                            <asp:UpdatePanel ID="updCliente" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>                        
                                    <asp:Panel ID="panelHeaderCliente" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="lblCliente" runat="server" VariableName="CLIENT" ResourceName="Entities" />
                                    </asp:Panel>
                            
                                    <cwc:SelectAllExtender ID="selClientes" runat="server" AutoPostBack="true" TargetControlId="panelHeaderCliente" ListControlId="cbCliente"  />
                                    <br />
                                    <cwc:ResourceLabel ID="lblBuscar" runat="server" ResourceName="Labels" VariableName="BUSCAR" />
                                    <asp:TextBox ID="txtClientes" runat="server" width="150px" />
                                    <br /><br />
                                    <cwc:ClienteListBox ID="cbCliente" runat="server" Width="100%" Height="330px" SelectionMode="Multiple" RememberSessionValues="false" AutoPostBack="true" ParentControls="cbPlanta" OnSelectedIndexChanged="CbClienteSelectedIndexChanged" FilterControl="txtClientes" />
                                    
                                    <div  class="header" style="padding: 10px;">
                                        <cwc:ResourceCheckBox ID="chkCliente" runat="server" AutoPostBack="true" Checked="true" OnCheckedChanged="CbClienteSelectedIndexChanged" ResourceName="Entities" VariableName="CLIENT" />
                                        <div style="height: 10px;" />
                                        <cwc:ResourceCheckBox ID="chkPuntoEntrega" runat="server" AutoPostBack="true" Checked="false" OnCheckedChanged="CbClienteSelectedIndexChanged" ResourceName="Entities" VariableName="PARENTI44"  />
                                    </div>                  
                                </ContentTemplate>   
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                </Triggers>                          
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    
                    <%--Tab: Mensajes--%>
                    <cc1:TabPanel ID="TabPanel5" runat="server" HeaderText="<img src=\'../../Operacion/caution.png\' alt=\'Eventos\' title=\'Eventos\' />">
                        <ContentTemplate>                    
                            <asp:UpdatePanel ID="updEventos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <asp:Panel ID="PanelMensajes" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PAREVEN01" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:SelectAllExtender ID="selMensajes" runat="server" AutoPostBack="true" TargetControlId="PanelMensajes" ListControlId="cbMensajes"  />
                            
                                    <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="cbLocacion,cbPlanta" OnSelectedIndexChanged="CbTipoMensajeSelectedIndexChanged" />
                                    <cwc:MensajesListBox ID="cbMensajes" runat="server" ParentControls="cbPlanta,cbTipoMensaje" UseOptionGroup="true" Width="100%" Height="417px" AutoPostBack="true" SelectionMode="Multiple" OnSelectedIndexChanged="CbMensajeSelectedIndexChanged" />
                                </ContentTemplate> 
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPoi" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                </Triggers>                            
                            </asp:UpdatePanel>                        
                        </ContentTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>            
            </asp:Panel>
            <asp:Panel ID="EastPanel" runat="server">
                <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="500px" Width="220px">
                    <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="Buscar">
                        <ContentTemplate>
                            <div class="header"><cwc:ResourceLabel ID="ResourceLabel4" runat="server" VariableName="PARENTI79" ResourceName="Entities" /></div>
                                <asp:Panel ID="panelBuscarVehiculo" runat="server" CssClass="content" DefaultButton="btBuscar">
                                    Buscar <cwc:ResourceLabel ID="ResourceLabel14" runat="server" VariableName="PARENTI79" ResourceName="Entities" />
                                    <br />
                                    <div style="padding: 3px 0px 5px 8px;">
                                        <asp:TextBox runat="server" ID="txtEntidad" Width="120px" />
                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <asp:Button runat="server" ID="btBuscar" Text="" OnClick="BtBuscarClick" CssClass="btBuscar" />
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
                                                Ingrese la descripci&oacute;n del Punto de Inter&eacute;s que desea localizar<br />
                                                <br />
                                                <div style="padding: 3px 0px 5px 8px;">
                                                    <asp:TextBox ID="txtPoi" runat="server" Width="120px" />
                                                    <asp:Button runat="server" ID="btBuscarPoi" Text="" OnClick="BtBuscarPoiClick" CssClass="btBuscar" />
                                                </div>
                                            </asp:View>
                                            <asp:View ID="ViewPoiResult" runat="server">
                                                Se encontraron los siguientes resultados:
                                                <br />
                                                <asp:ListBox ID="cbPoiResult" runat="server" Width="100%" DataTextField="Descripcion" DataValueField="Id" Font-Size="X-Small" />
                                                <div style="text-align: right;">
                                                    <asp:Button runat="server" ID="btCancelPoi" Text="Cancelar" OnClick="BtCancelPoiClick" />
                                                    <asp:Button runat="server" ID="btSelectPoi" Text="Seleccionar" OnClick="BtSelectPoiClick" />
                                                </div>
                                            </asp:View>
                                        </asp:MultiView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>  
                            </asp:Panel>
                            <asp:Panel ID="panelPopup" runat="server" Style="width: 240px; height: 350px; position: absolute; bottom: 0px; right: 0px; border: solid 1px black; background-color: White; z-index: 99999; display: none;">
                                <div class="header" style="cursor: pointer;" onclick="HideEvents();">
                                    Eventos
                                </div>
                                <asp:Panel ID="panelPopupEvents" runat="server" ScrollBars="Auto" Style="height: 356px;" />
                                <asp:Panel ID="panelPopupDetail" runat="server" Style="height: 650px; display: none;">
                                    <iframe id="ifrPopupDetail" style="width: 240px; height: 650px; border: none;"></iframe>
                                </asp:Panel>
                            </asp:Panel>
                            
                            
                        </ContentTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>
            </asp:Panel>
            <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" Style="background-color: #666666; height: 20px; padding-top: 0px;">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ImageButton ID="connection_status" runat="server" ToolTip="[Conectado]" ImageUrl="../../Operacion/connected.png" Style="float: left; margin-right: 20px;" OnClick="ConnectionStatusClick" />
                    </ContentTemplate>  
                </asp:UpdatePanel>
                <div id='clock' style='width: auto; color: White; float: right; font-size: 14px;
                    font-weight: bold; padding-right: 10px; padding-left: 10px; background-color: #666666;
                    border-left: solid 1px #999999; border-right: solid 1px #999999;'>
                    00:00
                </div>
                <img src="../../Operacion/exclamation.png" onclick="ToggleEvents();" style="cursor: pointer; float: right;" />
                <asp:Label ID="lblInfo" runat="server" Text="Iniciando Monitor..." />
            </asp:Panel>
            <asp:Panel ID="CenterPanel" runat="server">
                <mon:Monitor ID="Monitor" runat="server" Width="800px" Height="500px" />
            </asp:Panel>
            
            <div>
                <div id="soundPlace" />
                <script type="text/javascript">
                <!--
                    function enqueueSounds(audiofiles) 
                    {
                        for (var i = 0; i < audiofiles.length && i < 5; i++) 
                        {
                            setTimeout('playSound("' + audiofiles[i] + '")', 2000 * i);
                        }
                    }
                    function playSound(audiofile) 
                    {
                        var place = $get("soundPlace");
                        place.innerHTML = '<embed src="' + audiofile + '" autostart="true" width="0" height="0" id="sound1" enablejavascript="true" />';
                    }
                //-->
                </script>
                
            </div>
        </div>
        <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progress" class="progress" />
            </ProgressTemplate>
        </asp:UpdateProgress>

        <script type="text/javascript">
            $addHandler(window, 'resize', Resize);
            Resize();
        </script>

    </form>
</body>
</html>