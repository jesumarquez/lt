<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MonitorSubEntidades.aspx.cs" Inherits="Logictracker.Monitor.MonitorDeSubEntidades.MonitorSubEntidades" %> 

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Monitor De SubEntidades</title>  
    <link rel="stylesheet" type="text/css" href="~/App_Styles/default.css"/>  
    <script type="text/javascript">
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
        
        var serverTime = new Date();
        var clockInterval = 200;
        function startClock(datetime) {
            serverTime.setTime(datetime.getTime() + 5000);
            setInterval(clockTick, clockInterval * 1000);
        }
        function clockTick() {
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
            <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="225" split="false" minSize="225" maxSize="225" title="Filtros" />
            <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="220" split="false" minSize="220" maxSize="220" title="Opciones" />
            <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="20" split="false" />
            <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" South="rgSouth" West="rgWest" East="rgEast" runat="server" />
            <asp:Panel ID="pnlManager" runat="server" />
            <!--West: Filtros-->
            <asp:Panel ID="WestPanel" runat="server">
                <cc1:TabContainer ID="TabContainer2" runat="server" ActiveTabIndex="0" Height="500px">
                    <%--Tab: Filtros--%>
                    <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'../../Operacion/LorryGreen.png\' alt=\'Entidades\' title=\'Entidades\' />" >
                        <HeaderTemplate>
                            <img alt="\'SubEntidades\'" src="'../../Operacion/LorryGreen.png/'" title="\'SubEntidades\'" />
                        </HeaderTemplate>
                        <ContentTemplate>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>                            
                                    <%--Empresa, Linea, Tipo Vehiculo--%>                        
                                    <div class="header">
                                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                    </div>
                                    <cwc:LocacionDropDownList ID="cbLocacion" runat="server" AutoPostBack="True" Width="100%" OnSelectedIndexChanged="CbLocacion_SelectedIndexChanged" />
                                    
                                    <div class="header">
                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                    </div>
                                    <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion" OnSelectedIndexChanged="CbPlanta_SelectedIndexChanged" AddAllItem="true" />
                                    
                                    <asp:Panel ID="PanelTipoEntidad" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="PARENTI76" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="CbTipoEntidad_SelectedIndexChanged" ParentControls="cbPlanta" />
                                    
                                    <asp:Panel ID="PanelEntidad" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="PARENTI79" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:EntidadDropDownList ID="cbEntidad" runat="server" AutoPostBack="true" Width="100%" AddAllItem="false" OnSelectedIndexChanged="CbEntidad_SelectedIndexChanged" ParentControls="cbTipoEntidad" />
                                    
                                    <asp:Panel ID="PanelSubEntidad" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="PARENTI81" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:SelectAllExtender ID="selSubEntidad" runat="server" AutoPostBack="true" TargetControlId="PanelSubEntidad" ListControlId="cbSubEntidad" />
                                    <cwc:SubEntidadListBox ID="cbSubEntidad" runat="server" SelectionMode="Multiple" ParentControls="cbPlanta,cbTipoEntidad,cbEntidad" Width="100%" Height="300px" AutoPostBack="True" OnSelectedIndexChanged="CbSubEntidad_SelectedIndexChanged" UseOptionGroup="true" />

                                </ContentTemplate> 
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />                                    
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbSubEntidad" EventName="SelectedIndexChanged" />                                    
                                    <%-- 
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                    --%>
                                </Triggers>                           
                            </asp:UpdatePanel>
                        </ContentTemplate>
                    </cc1:TabPanel>
                    
                    <%--Tab: Mensajes--%>
                    <%--
                    <cc1:TabPanel ID="TabPanel5" runat="server" HeaderText="<img src=\'../../Operacion/caution.png\' alt=\'Eventos\' title=\'Eventos\' />">
                        <ContentTemplate>                    
                            <asp:UpdatePanel ID="updEventos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <asp:Panel ID="PanelMensajes" runat="server" CssClass="header">
                                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PAREVEN01" ResourceName="Entities" />
                                    </asp:Panel>
                                    <cwc:SelectAllExtender ID="selMensajes" runat="server" AutoPostBack="true" TargetControlId="PanelMensajes" ListControlId="cbMensajes"  />
                            
                                    <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="cbLocacion,cbPlanta" OnSelectedIndexChanged="CbTipoMensaje_SelectedIndexChanged" />
                                    <cwc:MensajesListBox ID="cbMensajes" runat="server" ParentControls="cbPlanta,cbTipoMensaje" UseOptionGroup="true" Width="100%" Height="417px" AutoPostBack="true" SelectionMode="Multiple" OnSelectedIndexChanged="CbMensaje_SelectedIndexChanged" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbMensajes" EventName="SelectedIndexChanged" />
                                </Triggers>                            
                            </asp:UpdatePanel>                        
                        </ContentTemplate>
                    </cc1:TabPanel>
                    --%>
                </cc1:TabContainer>                
            </asp:Panel>
            <asp:Panel ID="EastPanel" runat="server">
                <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="500px" Width="220px">
                    <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="Buscar">
                        <ContentTemplate>
                            <div class="header"><cwc:ResourceLabel ID="ResourceLabel4" runat="server" VariableName="PARENTI81" ResourceName="Entities" /></div>
                            <asp:Panel ID="panelBuscarVehiculo" runat="server" CssClass="content" DefaultButton="btBuscar">
                                Buscar <cwc:ResourceLabel ID="ResourceLabel14" runat="server" VariableName="PARENTI81" ResourceName="Entities" />
                                <br />
                                <div style="padding: 3px 0px 5px 8px;">
                                    <asp:TextBox runat="server" ID="txtSubEntidad" Width="120px" />
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <asp:Button runat="server" ID="btBuscar" Text="" OnClick="BtBuscar_Click" CssClass="btBuscar" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>                                    
                                </div>
                            </asp:Panel>                            
                            <asp:Panel ID="panelPopup" runat="server" Style="width: 240px; height: 350px; position: absolute; bottom: 0px; right: 0px; border: solid 1px black; background-color: White; z-index: 99999; display: none;">
                                <div class="header" style="cursor: pointer;" onclick="HideEvents();">
                                    Eventos
                                </div>
                                <asp:Panel ID="panelPopupEvents" runat="server" ScrollBars="Auto" Style="height: 356px;" />
                                <asp:Panel ID="panelPopupDetail" runat="server" Style="height: 406px; display: none;">
                                    <iframe id="ifrPopupDetail" style="width: 240px; height: 406px; border: none;"></iframe>
                                </asp:Panel>
                            </asp:Panel>
                        </ContentTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>
            </asp:Panel>
            <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" Style="background-color: #666666; height: 20px; padding-top: 0px;">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table width="100%" border="0">
                            <tr>
                                <td align="left" valign="top">
                                    <asp:ImageButton ID="connection_status" runat="server" ToolTip="[Conectado]" ImageUrl="../../Operacion/connected.png" Style="float: left; margin-right: 20px;" OnClick="ConnectionStatusClick" />        
                                </td>
                                <td align="right" valign="top" width="10%">
                                    <cwc:RegionalInformationDisplayer ID="regionalInformation" runat="server" />        
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>  
                </asp:UpdatePanel>
                <asp:Label ID="lblInfo" runat="server" Text="Iniciando Monitor..." />
            </asp:Panel>
            <asp:Panel ID="CenterPanel" runat="server">            
                <asp:UpdatePanel ID="updCenterPanel" runat="server">
                    <ContentTemplate>
                        <cwc:BaloonTip ID="bln" runat="server" Visible="false"></cwc:BaloonTip>
                        <div id="divPlano" runat="server" style="position:relative;" />
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Timer ID="Timer" runat="server" OnTick="Timer_Tick" Enabled="true" Interval="30000" />
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
    </form>
</body>
</html>