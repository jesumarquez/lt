 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Logictracker.Operacion.Simulador.OperacionSimuladorDefault" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register Src="~/App_Controls/Pickers/DateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/Pickers/NumberPicker.ascx" TagName="NumberPicker" TagPrefix="uc3" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <script type="text/javascript" src="geplugin-helpers.js"></script>
    <script type="text/javascript" src="Simulator.js"></script>
    <script type="text/javascript" src="SimRecorrido.js"></script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />

    <asp:Panel ID="pnlManager" runat="server" />
    <cwc:ResourceLayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server"
        ResourceName="Menu" VariableName="OPE_SIMULADOR" />
    <cwc:ResourceLayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server"
        initialSize="250" collapsible="true" split="false" ResourceName="Labels" VariableName="FILTROS" />
    <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" West="rgWest" Center="rgCenter"
        runat="server" />
    <asp:Panel ID="CenterPanel" runat="server">
        <div style="z-index: 1; position: relative">
            <%--PROGRESSLABEL--%>
            <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
        </div>
        <%--ERRORLABEL--%>
        <cwc:InfoLabel ID="infoLabel1" runat="server" />
        <div id="map3d" style="height: 500px; width: 100%;">
        </div>
        <table class="simulator_toolbar">
            <tr>
                <td style="width: 25%">
                    <table style="width: 80%">
                        <tr>
                            <td>
                                <table style="width: 80%">
                                    <tr>
                                        <td>
                                            <div class="simulator_button_minus" onclick="simulador.slower();">
                                            </div>
                                        </td>
                                        <td>
                                            <div id="simulator_speed" class="simulator_speed">
                                                x1</div>
                                        </td>
                                        <td>
                                            <div class="simulator_button_plus" onclick="simulador.faster();">
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td class="simulator_text">
                                <input type="checkbox" id="manualMove" />
                                Navegacion Manual
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 50%">
                    <table style="margin: auto;">
                        <tr>
                            <td>
                                <div id="btplay" class="simulator_button_play" onclick="simulador.playPause();">
                                </div>
                            </td>
                            <td>
                                <div class="simulator_button_stop" onclick="simulador.reset();">
                                </div>
                            </td>
                            <td style="padding: 0px 10px 0px 10px;">
                                <div id="backtimebar" style="background-color: #666666; height: 10px; width: 200px;">
                                    <div id="timebar" style="background-color: #AAAAAA; height: 10px; width: 1px;">
                                    </div>
                                </div>
                            </td>
                            <td class="simulator_text">
                                <div id="time" class="simulator_data" style="border: solid 1px #444444; padding: 1px 3px 1px 3px;
                                    background-color: #333333;">
                                    00:00:00</div>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="width: 25%">
                    <table style="width: 80%">
                        <tr>
                            <td class="simulator_text" style="width: 50%">
                                Velocidad: <span id="speed" class="simulator_data">0</span> km/h
                            </td>
                            <td class="simulator_text" style="width: 50%">
                                Distancia: <span id="distance" class="simulator_data">0</span> km
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </asp:Panel>
    
    <asp:Panel ID="WestPanel" runat="server" CssClass="filters" Height="100%" ScrollBars="Auto">
        <div class="x-panel-body">
        <table>
            <tr>
                <td style="width: 95px" class="header">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                </td>
                <td>
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" OnInitialBinding="ddlDistrito_InitialBinding" />
                </td>
            </tr>
            <tr>
                <td class="header">
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                </td>
                <td>
                    <asp:UpdatePanel ID="upBase" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="100%" AutoPostBack="true" ParentControls="ddlDistrito" OnInitialBinding="ddlPlanta_PreBind" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="header">
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                </td>
                <td>
                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="100%" ParentControls="ddlPlanta"
                                OnInitialBinding="ddlTipoVehiculo_PreBind" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="header">
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                </td>
                <td>
                    <asp:UpdatePanel ID="upMovil" runat="server">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="100%" ParentControls="ddlDistrito,ddlPlanta,ddlTipoVehiculo,ddlEmpleado"
                                OnInitialBinding="ddlMovil_PreBind" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    </td>
                    <td>
                        <cwc:DateTimePicker runat="server" ID="dtDesde" Mode="DateTime" IsValidEmpty="False"></cwc:DateTimePicker>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    </td>
                    <td>
                        <cwc:DateTimePicker runat="server" ID="dtHasta" Mode="DateTime" IsValidEmpty="False"></cwc:DateTimePicker>
                        <cwc:DateTimeRangeValidator ID="DateTimeRangeValidator1" runat="server" StartControlID="dtDesde" EndControlID="dtHasta" MaxRange="23:59"></cwc:DateTimeRangeValidator>
                    </td>
                </tr>
            </table>
            <table>
            <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Menu" VariableName="PAR_TIPO_MENSAJE" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upTipoMensaje" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoMensajeDropDownList ID="ddlTipo" runat="server" AutoPostBack="true" Width="140px" AddAllItem="true" ParentControls="ddlDistrito,ddlPlanta"
                                    OnInitialBinding="ddlTipo_PreBind" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLinkButton ID="lnkMessages" runat="server" CssClass="MonitorPais_Link" Font-Bold="true" OnClick="lnkMessages_Click" ResourceName="Labels"
                            VariableName="MENSAJES" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upMessages" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cwc:MensajesListBox ID="lbMessages" runat="server" SelectionMode="Multiple" Width="140px" ParentControls="ddlDistrito, ddlPlanta, ddlTipo" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlTipo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="lnkMessages" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            <div class="x-panel-body" style="text-align: center; padding-top: 10px;">
                    <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="btnSearch_Click" 
                            ResourceName="Controls" VariableName="BUTTON_SEARCH" />
            </div>
       
        </div>
        
        <a href="../../Home.aspx">
            <div class="Logo"></div>
        </a>
        
        <div id="debug">
        </div>
    </asp:Panel>
    <%--HACK PARA QUE NO HAGA POSTBACK--%>
    <asp:UpdatePanel ID="upQualityMonitor" runat="server" UpdateMode="Conditional">
        <ContentTemplate />
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function getDimensions() {
            var winWidth, winHeight;
            var d = document;
            if (typeof window.innerWidth != 'undefined') {
                winWidth = window.innerWidth;
                winHeight = window.innerHeight;
            } else {
                if (d.documentElement &&
                typeof d.documentElement.clientWidth != 'undefined' &&
                d.documentElement.clientWidth != 0) {
                    winWidth = d.documentElement.clientWidth;
                    winHeight = d.documentElement.clientHeight;
                } else {
                    if (d.body &&
                    typeof d.body.clientWidth != 'undefined') {
                        winWidth = d.body.clientWidth;
                        winHeight = d.body.clientHeight;
                    }
                }
            }
            return { width: winWidth, height: winHeight };
        }

        window.onresize = function() {
            $get('<%=WestPanel.ClientID%>').style.height = getDimensions().height - 27 + 'px';
            $get('map3d').style.height = getDimensions().height - 27 - 32 + 'px';
        };

        window.onresize();
    </script>

    </form>
</body>
</html>
