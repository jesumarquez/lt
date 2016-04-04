 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Monitor.MonitorHistorico.MonitorHistorico" Codebehind="monitorHistorico.aspx.cs" %>

<%@ Import Namespace="Logictracker.Culture"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Src="../../App_Controls/Pickers/DateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="uc1" %>
<%@ Register Src="../../App_Controls/Pickers/TimePicker.ascx" TagName="TimePicker" TagPrefix="uc2" %>
<%@ Register Src="../../App_Controls/Pickers/NumberPicker.ascx" TagName="NumberPicker" TagPrefix="uc3" %>
    

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body id="monitor">
    <script type="text/javascript">
        function gMP(a, b, c, d, e, f, g, h) {
            return "<iframe width=\"350px\" height=\"140px\" style=\"border:none;\" src=\"../popup/MobileRoutePopup.aspx?latitude="
                 + a + "&longitude=" + b + "&inicio=" + c + "&duracion=" + d + "&distancia=" + e + "&velocidadMinima=" + f
                 + "&velocidadMaxima=" + g + "&velocidadPromedio=" + h + "\" />";
        }

        function gMSP(id) {
            return "<iframe width=\"400px\" height=\"100px\" style=\"border:none;\" src=\"../popup/MobileEventPopup.aspx?id=" + id + "\" />";
        }

        function gFP(a) {
            return "<table width=\"150px\"><tr><td style=\"font-weight: bold;\">" + a + "</td></tr></table>";
        }

        function gPOIP(a) {
            return "<table width=\"200px\"><tr><td style=\"font-weight: bold;\">" + a + "</td></tr></table>";
        }

        function gCP(a, b, c) {
            return "<table width=\"300px\"><tr><td style=\"font-weight: bold;\">" + a +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "DATE") %></u>: " + b +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "VELOCIDAD") %></u>: " + c +
            "km/h</td></tr></table>";
        }
    </script>

    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />        
        
        <asp:Panel ID="pnlManager" runat="server" />
        
        <cwc:ResourceLayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" ResourceName="Menu" VariableName="OPE_MON_HISTORICO" />
        
        <cwc:ResourceLayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="260" collapsible="true" split="false" ResourceName="Labels" VariableName="FILTROS" />
            
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" West="rgWest" Center="rgCenter" runat="server" />
        
        <asp:Panel ID="CenterPanel" runat="server">
            <div style="z-index: 99999999; width: 100%; position: absolute;">
                <%--PROGRESSLABEL--%>
                <cwc:ProgressLabel ID="ProgressLabel1" runat="server"/>
            </div>
            
            <%--ERRORLABEL--%>
            <cwc:InfoLabel ID="infoLabel1" runat="server" />
            
            <mon:Monitor ID="Monitor" runat="server" />
        </asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server" CssClass="filters" Height="100%" ScrollBars="Auto">
            <cc1:TabContainer ID="TabContainer2" runat="server" ActiveTabIndex="0" Height="290px">
                <%--Tab: Filtros--%>
                <cc1:TabPanel ID="tabFiltros" runat="server" HeaderText="<img src=\'../../Operacion/LorryGreen.png\' alt=\'Vehiculos\' title=\'Vehiculos\' />" >
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td style="width: 95px" class="header">
                                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                        <ContentTemplate>
                                            <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" OnInitialBinding="DdlDistritoInitialBinding" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="header">
                                    <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upBase" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="100%" AutoPostBack="true" ParentControls="ddlDistrito" OnInitialBinding="DdlPlantaPreBind" AddAllItem="true" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="header">
                                    <cwc:ResourceLabel ID="lblResponsable" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="RESPONSABLE" />
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upResponsable" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cwc:EmpleadoDropDownList ID="ddlEmpleado" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="ddlDistrito,ddlPlanta" OnInitialBinding="DdlEmpleadoPreBind" />
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
                                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="100%" ParentControls="ddlPlanta"
                                                OnInitialBinding="DdlTipoVehiculoPreBind" AddAllItem="true" />
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
                                                OnInitialBinding="DdlMovilPreBind" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlEmpleado" EventName="SelectedIndexChanged" />
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
                                    <asp:UpdatePanel ID="updRange" runat="server">
                                        <ContentTemplate>
                                            <cwc:DateTimeRangeValidator runat="server" ID="dtvalidator" StartControlID="dtDesde" EndControlID="dtHasta" MaxRange="23:59"></cwc:DateTimeRangeValidator>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                        <table>
                            <tr>
                                <td class="header">
                                    <cwc:ResourceLabel ID="lblStopped" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="FIN_VIAJE" />
                                    <cwc:BaloonTip ID="bFinViaje" runat="server" ResourceName="Labels" VariableName="BALOON_FIN_VIAJE" />
                                </td>
                                <td class="header">
                                    <cwc:ResourceLabel ID="lblStoppedDistance" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DISTANCIA" />
                                    <cwc:BaloonTip ID="bDistancia" runat="server" ResourceName="Labels" VariableName="BALOON_DISTANCIA" />
                                </td>
                                <td class="header">
                                    <cwc:ResourceLabel ID="lblStoppedTime" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DETENCION" />
                                    <cwc:BaloonTip ID="bDetencion" runat="server" ResourceName="Labels" VariableName="BALOON_DETENCION" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <uc2:TimePicker ID="tpStopped" runat="server" Width="80" DefaultTimeMode="NotSet" />
                                </td>
                                <td>
                                    <uc3:NumberPicker ID="npDistance" runat="server" Mask="999" MaximumValue="999" Number="100" Width="80" />
                                </td>
                                
                                <td>
                                    <uc3:NumberPicker ID="npStoppedEvent" runat="server" Mask="99" MaximumValue="60" Number="1" Width="80" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
                <%--Tab: Mensajes--%>
                <cc1:TabPanel ID="tabMensajes" runat="server" HeaderText="<img src=\'../../Operacion/caution.png\' alt=\'Eventos\' title=\'Eventos\' />">
                    <ContentTemplate>
                        <asp:Panel ID="PanelMensajes" runat="server" CssClass="header">
                            <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PAREVEN01" ResourceName="Entities" />
                        </asp:Panel>
                        <cwc:SelectAllExtender ID="selMensajes" runat="server" AutoPostBack="true" TargetControlId="PanelMensajes" ListControlId="lbMessages"  />

                        <asp:UpdatePanel ID="upTipoMensaje" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoMensajeDropDownList ID="ddlTipo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="ddlDistrito,ddlPlanta"
                                    OnInitialBinding="DdlTipoPreBind" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="upMessages" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cwc:MensajesListBox ID="lbMessages" runat="server" AutoPostBack="false" SelectionMode="Multiple" Height="207px" Width="100%" ParentControls="ddlDistrito, ddlPlanta, ddlTipo" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlTipo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="btnPosicionarTicket" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
                <%--Tab: Referencia Geográfica--%>
                <cc1:TabPanel ID="tabGeoRef" runat="server" HeaderText="<img src=\'../../Operacion/office.png\' alt=\'Referencias Geograficas\' title=\'Referencias Geograficas\' />">
                    <ContentTemplate>
                        <asp:Panel ID="panelGeoRef" runat="server" CssClass="header">
                            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="PARENTI05" ResourceName="Entities" />
                        </asp:Panel>
                        <cwc:SelectAllExtender ID="SelectAllExtender1" runat="server" AutoPostBack="true" TargetControlId="panelGeoRef" ListControlId="lbPuntosDeInteres"  />
                        
                        <asp:UpdatePanel ID="upPuntosDeInteres" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cwc:TipoReferenciaGeograficaListBox ID="lbPuntosDeInteres" runat="server" Height="227px" Width="100%" ParentControls="ddlPlanta"
                                    SelectionMode="Multiple" AutoPostBack="false" RememberSessionValues="false" Monitor="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </ContentTemplate>
                </cc1:TabPanel>
                <!--Tab: Tickets-->
                <cc1:TabPanel ID="tabTickets" runat="server" HeaderText="<img src=\'../../Operacion/ticket.png\' alt=\'Tickets\' title=\'Tickets\' />">
                    <ContentTemplate>
                        <table id="tblTickets" style="font-size: x-small; margin: auto;">
                            <tr>
                                <td style="width: 90px">
                                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DATE" />
                                </td>
                                <td>
                                    <uc1:DateTimePicker ID="dtDia" runat="server" Width="100" DefaultTimeMode="Start" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Controls" VariableName="BUTTON_SEARCH" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBuscar" runat="server" Width="140px"/>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <br />
                                    <cwc:ResourceButton ID="btnBuscarTickets" runat="server" ToolTip="Buscar" Width="110px" OnClick="BtnSearchTicketsClick"
                                        ResourceName="Controls" VariableName="BUTTON_SEARCH_TICKETS" />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top">
                                    <br />
                                    <cwc:ResourceLabel ID="ResourceLinkButton1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TICKETS" />
                                </td>
                                <td valign="top">
                                    <br />
                                    <asp:UpdatePanel ID="upTickets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                        <ContentTemplate>
                                            <cwc:TicketListBox ID="lstTicket" runat="server" Width="160px" ParentControls="ddlDistrito, ddlPlanta, ddlMovil" AutoPostBack="false" SelectionMode="Single" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="ddlMovil" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="btnBuscarTickets" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2">
                                    <br />
                                    <cwc:ResourceButton ID="btnPosicionarTicket" runat="server" ToolTip="Posicionar" OnClick="BtnPosicionarTicketClick"
                                        ResourceName="Controls" VariableName="BUTTON_VIEW_TICKETS" Width="150px" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
            
            <div class="x-panel-body" style="text-align: center; padding-top: 10px;">
                <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtnSearchClick" 
                            ResourceName="Controls" VariableName="BUTTON_SEARCH" />
            
                
            <div style="margin-top: 10px;">

                <asp:UpdatePanel ID="upQualityMonitor" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLinkButton ID="lnkQualityMonitor" runat="server" Visible="false" CssClass="LogicLinkButton"
                            OnClick="LnkQualityMonitorClick" ResourceName="Labels" VariableName="VER_MONITOR_CALIDAD" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnPosicionarTicket" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div style="margin-top: 10px;margin-bottom: 10px;">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLinkButton ID="lnkSimulator" runat="server" Visible="false" CssClass="LogicLinkButton"
                            OnClick="LnkSimulatorClick" ResourceName="Labels" VariableName="VER_SIMULADOR" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnPosicionarTicket" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
            </div>
            
            <a href="../../Home.aspx">
                <div class="Logo"></div>
            </a>
            
        </asp:Panel>

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
            };

            window.onresize();
        </script>
    </form>
</body>
</html>