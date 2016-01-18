<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Monitor" Codebehind="Monitor.aspx.cs" %>
<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
    <script type="text/javascript">
        function getPuntoEntrega(id)
        {
            return "<iframe width=\"300\" height=\"145\" style=\"border:none;\" src=\"InfoCiclo.aspx?id=" + id + "\" />";
        }
        function getDetencion(idEvento, idViaje) {
            return "<iframe width=\"500\" height=\"300\" style=\"border:none;\" src=\"InfoDetencion.aspx?idEvento=" + idEvento + "&idViaje=" + idViaje + "\" />";
        }
        function ticket(d, c, i, p, a, m, p2, a2, m2, t, t2) {
            $get('divTemplateTicket_icono').src = i;
            $get('divTemplateTicket_codigo').innerHTML = c;
            $get('divTemplateTicket_descripcion').innerHTML = d;
            $get('divTemplateTicket_programado').innerHTML = p;
            $get('divTemplateTicket_automatico').innerHTML = a;
            $get('divTemplateTicket_manual').innerHTML = m;
            $get('divTemplateTicket_programado2').innerHTML = p2;
            $get('divTemplateTicket_automatico2').innerHTML = a2;
            $get('divTemplateTicket_manual2').innerHTML = m2;
            $get('divTemplateTicket_title').innerHTML = t;
            $get('divTemplateTicket_title2').innerHTML = t2;
            return $get('divTemplateTicket').innerHTML;
        }
    </script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200"
            minSize="200" maxSize="500" title="<a href='../'><div class='logo_online'> </div></a>" tabPosition="top">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="270"
            split="false" minSize="260" maxSize="260" collapsible="true" title="Filtros">
        </cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="250"
            split="false" minSize="220" maxSize="220" collapsible="true" title="Opciones">
        </cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" runat="server">
        </cc1:LayoutManager>
        <asp:Panel ID="pnlManager" runat="server">
        </asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server" CssClass="ajax__tab_header">
            <asp:UpdatePanel ID="updTabCompleto" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <AjaxToolkit:TabContainer ID="tab" runat="server" CssClass="filterpanel">
                        <AjaxToolkit:TabPanel ID="tabFiltros" runat="server">
                            <HeaderTemplate>
                                <img src="../Operacion/LorryGreen.png" alt="Vehiculos" title="Vehiculos" style="padding-left: 10px;padding-right: 10px;" />
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:Panel runat="server" ID="panelFiltros" DefaultButton="btnSearch">
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
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
                                                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="100%" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="upLinea" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="CbLineaSelectedIndexChanged" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel16" runat="server" VariableName="PARENTI07" ResourceName="Entities" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel6" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa,cbLinea" AddAllItem="True" AddNoneItem="True" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" VariableName="PARENTI37" ResourceName="Entities" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa,cbLinea" AddAllItem="True" AddNoneItem="True" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="RESPONSABLE" ResourceName="Labels" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbTransportista,cbCentroDeCostos" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCostos" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" VariableName="PARENTI17" ResourceName="Entities" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel4" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        <div class="header">
                                                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                                        </div>
                                                    </th>
                                                    <td>
                                                        <asp:UpdatePanel runat="server" ID="UpdatePanel5" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <cwc:MovilDropDownList ID="cbVehiculo" runat="server" ParentControls="cbLinea,cbTipoVehiculo,cbTransportista,cbCentroDeCostos" Width="100%" AutoPostBack="True" AddAllItem="true" />
                                                            </ContentTemplate>
                                                            <Triggers>
                                                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged"/>
                                                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCostos" EventName="SelectedIndexChanged"/>
                                                            </Triggers>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="header">
                                                        <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                                                    </td>
                                                    <td>
                                                        <cwc:DateTimePicker runat="server" ID="dtDesde" Mode="DateTime" IsValidEmpty="False"></cwc:DateTimePicker>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="header">
                                                        <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                                                    </td>
                                                    <td>
                                                        <cwc:DateTimePicker runat="server" ID="dtHasta" Mode="DateTime" TimeMode="End" IsValidEmpty="False"></cwc:DateTimePicker>
                                                        <cwc:DateTimeRangeValidator runat="server" ID="dtvalidator" StartControlID="dtDesde" EndControlID="dtHasta" MaxRange="2.23:59"></cwc:DateTimeRangeValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="header">
                                                        <cwc:ResourceLabel ID="lblActivas" runat="server" ResourceName="Labels" VariableName="SOLO_ACTIVAS" />
                                                    </td>
                                                    <td>
                                                        <asp:CheckBox ID="chkActivas" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>                         
                                    </asp:UpdatePanel>
                                    
                                    <div style="text-align: center; padding-top: 10px;">
                                        <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtnSearchClick" ResourceName="Controls" VariableName="BUTTON_SEARCH" />
                                    </div>
                                    
                                </asp:Panel>
                            </ContentTemplate>
                        </AjaxToolkit:TabPanel>
                        <AjaxToolkit:TabPanel ID="tabTickets" runat="server">
                            <HeaderTemplate>
                                <img src="../Operacion/ticket.png" alt="Tickets" title="Tickets" style="padding-left: 10px;padding-right: 10px;"/>
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:Panel ID="PanelTickets" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="OPETICK01" ResourceName="Entities" />
                                </asp:Panel>
                                <div style="overflow-y: scroll;overflow-x: hidden; height:450px;">
                                    <asp:UpdatePanel ID="updTickets" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                        <ContentTemplate>
                                            <c1:C1GridView runat="server" ID="gridTickets" SkinID="ListGridNoGroupNoPage" ShowHeader="False" OnRowDataBound="GridTicketsRowDataBound" OnSelectedIndexChanging="GridTicketsSelectedIndexChanging">
                                                <Columns>
                                                    <c1:C1TemplateField>
                                                        <ItemTemplate>
                                                            <asp:HiddenField runat="server" ID="hidId"/>
                                                            <table class="ticket_template">
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label runat="server" ID="lblTipo" CssClass="tipo" />
                                                                        <asp:Label runat="server" ID="lblCodigo" CssClass="codigo" />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label runat="server" ID="lblVehiculo" CssClass="vehiculo" />
                                                                        <asp:Label runat="server" ID="lblDate" CssClass="date" />
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </ItemTemplate>
                                                    </c1:C1TemplateField>
                                                </Columns>
                                            </c1:C1GridView>
                                            <table width="100%" style="background-color: white">
                                                <tr>
                                                    <td align="center">
                                                        <cwc:ResourceLinkButton ID="lnkHistorico" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_HISTORICO" Visible="False" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>                            
                                    </asp:UpdatePanel> 
                                </div>
                            </ContentTemplate>
                        </AjaxToolkit:TabPanel>
                        <AjaxToolkit:TabPanel ID="TabPanel1" runat="server">
                            <HeaderTemplate>
                                <img src="../Operacion/caution.png" alt="Mensajes" title="Mensajes"  style="padding-left: 10px;padding-right: 10px;" />
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="updMensajes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                                    <ContentTemplate>
                                        <asp:Panel ID="PanelMensajes" runat="server" CssClass="header">
                                            <cwc:ResourceLabel ID="ResourceLabel9" runat="server" VariableName="PAREVEN01" ResourceName="Entities" />
                                        </asp:Panel>
                                        
                                        <cwc:SelectAllExtender ID="selMensajes" runat="server" AutoPostBack="true" TargetControlId="PanelMensajes" ListControlId="cbMensajes"  />
                                        <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="cbLinea" />
                                        
                                        <cwc:MensajesListBox ID="cbMensajes" runat="server" ParentControls="cbTipoMensaje" UseOptionGroup="true" Width="100%" Height="360px" AutoPostBack="true" SelectionMode="Multiple" />
                                        
                                    </ContentTemplate>                  
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                                <div style="text-align: center; padding-top: 10px;">
                                    <cwc:ResourceButton ID="btMensajes" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtMensajesClick" ResourceName="Labels" VariableName="MOSTRAR" />
                                </div>
                            </ContentTemplate>
                        </AjaxToolkit:TabPanel>
                        <AjaxToolkit:TabPanel ID="TabPanel2" runat="server">
                            <HeaderTemplate>
                                <img src="../images/centrar.bmp" alt="Buscar" title="Buscar" style="padding-left: 10px;padding-right: 10px;" />
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False">
                                    <ContentTemplate>
                                        <table id="tblBuscar">
                                            <tr>
                                                <th>
                                                    <div class="header">
                                                        <cwc:ResourceLabel ID="ResourceLabel20" runat="server" VariableName="RUTA" ResourceName="Labels" />
                                                    </div>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtBuscarRuta" runat="server" Width="100px"/>
                                                </td>
                                                <td>
                                                    <cwc:ResourceButton ID="btnBuscarRuta" runat="server" OnClick="BtnBuscarRutaClick" VariableName="BUTTON_SEARCH" ResourceName="Controls" CssClass="LogicButton_Big" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    <div class="header">
                                                        <cwc:ResourceLabel ID="ResourceLabel17" runat="server" VariableName="PARENTI18" ResourceName="Entities" />
                                                    </div>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtBuscarCliente" runat="server" Width="100px"/>
                                                </td>
                                                <td>
                                                    <cwc:ResourceButton ID="btnBuscarCliente" runat="server" OnClick="BtnBuscarClienteClick" VariableName="BUTTON_SEARCH" ResourceName="Controls" CssClass="LogicButton_Big"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    <div class="header">
                                                        <cwc:ResourceLabel ID="ResourceLabel18" runat="server" VariableName="PARENTI44" ResourceName="Entities" />
                                                    </div>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtBuscarPuntoEntrega" runat="server" Width="100px" />
                                                </td>
                                                <td>
                                                    <cwc:ResourceButton ID="btnBuscarPuntoEntrega" runat="server" OnClick="BtnBuscarPuntoEntregaClick" VariableName="BUTTON_SEARCH" ResourceName="Controls" CssClass="LogicButton_Big"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    <div class="header">
                                                        <cwc:ResourceLabel ID="ResourceLabel19" runat="server" VariableName="ENTREGA" ResourceName="Labels" />
                                                    </div>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtBuscarEntrega" runat="server" Width="100px"/>
                                                </td>
                                                <td>
                                                    <cwc:ResourceButton ID="btnBuscarEntrega" runat="server" OnClick="BtnBuscarEntregaClick" VariableName="BUTTON_SEARCH" ResourceName="Controls" CssClass="LogicButton_Big" />
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </AjaxToolkit:TabPanel>
                    </AjaxToolkit:TabContainer>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Panel ID="pnlLink" runat="server" BackColor="White">
                <table width="100%" border="0">
                    <tr>
                        <td align="center">
                            <br/>
                            <asp:UpdatePanel runat="server" ID="updLink">
                                <ContentTemplate>
                                    <cwc:ResourceLinkButton ID="lnkEstadoEntregas" runat="server" VariableName="IR_ESTADO_ENTREGAS" ResourceName="Labels" OnClientClick="window.open('../Reportes/Estadistica/ReporteDistribucion.aspx','Estado de Entregas')" />        
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <br/>
                            <br/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </asp:Panel>
        
        <asp:Panel ID="EastPanel" runat="server">
            <asp:UpdatePanel runat="server" ID="updEast" UpdateMode="Conditional">
                <ContentTemplate>                    
                        <div id="divTemplateTicket" style="display: none;">
                        <div class="data_popup">
                            <table>
                                <tr>
                                    <td rowspan="2" style="text-align: center; width: 40px;">
                                        <img id="divTemplateTicket_icono" alt=""/>
                                    </td>
                                    <td class="codigo">
                                        <span id="divTemplateTicket_codigo"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="descripcion">
                                        <span id="divTemplateTicket_descripcion"></span>
                                    </td>
                                </tr>
                            </table>
                            <table style="margin-top: 5px;">
                                <tr>
                                    <td class="title_big" colspan="2">
                                        <span id="divTemplateTicket_title"></span>
                                    </td>
                                    <td class="title_big" colspan="2">
                                        <span id="divTemplateTicket_title2"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel10" runat="server"  ResourceName="Labels" VariableName="TIME_PROGRAMMED" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_programado"></span>
                                    </td>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel11" runat="server"  ResourceName="Labels" VariableName="TIME_PROGRAMMED" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_programado2"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel12" runat="server"  ResourceName="Labels" VariableName="TIME_AUTOMATIC" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_automatico"></span>
                                    </td>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel13" runat="server"  ResourceName="Labels" VariableName="TIME_AUTOMATIC" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_automatico2"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel14" runat="server"  ResourceName="Labels" VariableName="TIME_MANUAL" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_manual"></span>
                                    </td>
                                    <td class="title">
                                        <cwc:ResourceLabel ID="ResourceLabel15" runat="server"  ResourceName="Labels" VariableName="TIME_MANUAL" />
                                    </td>
                                    <td>
                                        <span id="divTemplateTicket_manual2"></span>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="opciones">
                        <cwc:ResourceCheckBox runat="server" ID="chkRecorridoCalculado" ResourceName="Labels" VariableName="Calcular Recorrido"/>
                    </div>
                    <asp:Panel runat="server" ID="panelReferencia" Visible="False">
                        <table style="width: 100%;">
                            <tr>
                                <td colspan="2" class="Grid_Header">
                                    <cwc:ResourceLabel runat="server" ID="lblReferencia" ResourceName="Labels" VariableName="REFFERENCE" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20px; height: 20px; background-color: #228b22"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel23" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_REAL" />.<cwc:ResourceLabel ID="ResourceLabel24" runat="server"  ResourceName="Labels" VariableName="IDA" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20px; height: 20px;background-color: #0000FF"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel25" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_REAL" />.<cwc:ResourceLabel ID="ResourceLabel26" runat="server"  ResourceName="Labels" VariableName="VUELTA" />
                                </td>
                            </tr>
                            <tr runat="server" ID="trCalculadoIda">
                                <td style="width: 20px; height: 20px;background-color: #FF0000"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel27" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_CALCULADO" />.<cwc:ResourceLabel ID="ResourceLabel28" runat="server"  ResourceName="Labels" VariableName="IDA" />
                                </td>
                            </tr>
                            <tr runat="server" ID="trCalculadoVuelta">
                                <td style="width: 20px; height: 20px;background-color: #9400d3"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel29" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_CALCULADO" />.<cwc:ResourceLabel ID="ResourceLabel30" runat="server"  ResourceName="Labels" VariableName="VUELTA" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel runat="server" ID="panelReferenciaSimple" Visible="False">
                        <table style="width: 100%;">
                            <tr>
                                <td colspan="2" class="Grid_Header"><cwc:ResourceLabel runat="server" ID="ResourceLabel5" ResourceName="Labels" VariableName="REFFERENCE" /></td>
                            </tr>
                            <tr>
                                <td style="width: 20px; height: 20px; background-color: #228b22"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel31" runat="server" ResourceName="Labels" VariableName="RECORRIDO_REAL" />: 
                                    <asp:Label ID="lblKmReales" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 20px; height: 20px; background-color: #ff7f50"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel21" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_PROGRAMADO" />: 
                                    <asp:Label ID="lblKmProgramados" runat="server" />
                                </td>
                            </tr>
                            <tr runat="server" ID="trCalculado">
                                <td style="width: 20px; height: 20px;background-color: #FF0000"></td>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel32" runat="server"  ResourceName="Labels" VariableName="RECORRIDO_CALCULADO" />: 
                                    <asp:Label ID="lblKmCalculados" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="pnlPosicionar" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <table width="100%" border="0">
                        <tr>
                            <th colspan="3">
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblPosicionar" runat="server" ResourceName="Labels" VariableName="POSICIONAR" ForeColor="White" Font-Bold="True" />
                                </div>
                            </th>
                        </tr>
                        <tr>
                            <th align="left">
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblOrdenProg" runat="server" ResourceName="Labels" VariableName="ORDEN_PROG" ForeColor="White" />
                                </div>
                            </th>
                            <td align="left">
                                <asp:TextBox ID="txtOrdenProg" runat="server" Width="75px" />
                            </td>
                            <td align="right">
                                <cwc:ResourceButton ID="btnOrdenProg" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnOrdenProgOnClick" CssClass="LogicButton_Big" />
                            </td>
                        </tr>
                        <tr>
                            <th align="left">
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblOrdenReal" runat="server" ResourceName="Labels" VariableName="ORDEN_REAL" ForeColor="White" />
                                </div>
                            </th>
                            <td align="left">
                                <asp:TextBox ID="txtOrdenReal" runat="server" Width="75px" />
                            </td>
                            <td align="right">
                                <cwc:ResourceButton ID="btnOrdenReal" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnOrdenRealOnClick" CssClass="LogicButton_Big" />
                            </td>
                        </tr>
                        <tr>
                            <th align="left">
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" ForeColor="White" />
                                </div>
                            </th>
                            <td align="left">
                                <asp:TextBox ID="txtPuntoEntrega" runat="server" Width="75px" />
                            </td>
                            <td align="right">
                                <cwc:ResourceButton ID="btnPuntoEntrega" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnPuntoEntregaOnClick" CssClass="LogicButton_Big" />
                            </td>
                        </tr>
                        <tr>
                            <th align="left">
                                <div class="header">
                                    <cwc:ResourceLabel ID="lblEntrega" runat="server" ResourceName="Labels" VariableName="ENTREGA" ForeColor="White" />
                                </div>
                            </th>
                            <td align="left">
                                <asp:TextBox ID="txtEntrega" runat="server" Width="75px" />
                            </td>
                            <td align="right">
                                <cwc:ResourceButton ID="btnEntrega" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnEntregaOnClick" CssClass="LogicButton_Big" />
                            </td>
                        </tr>
                    </table>
                    <br/>
                    <table width="100%" border="0">
                        <tr>
                            <td align="center">
                                <cwc:ResourceButton ID="btnUltima" runat="server" OnClick="BtnUltimaEntregaOnClick" VariableName="ULTIMA" ResourceName="Labels" CssClass="LogicButton_Big" />
                            </td>
                            <td align="center">
                                <cwc:ResourceButton ID="btnSiguiente" runat="server" OnClick="BtnSiguienteEntregaOnClick" VariableName="SIGUIENTE" ResourceName="Labels" CssClass="LogicButton_Big" />
                            </td>
                            <td align="center">
                                <cwc:ResourceButton ID="btnVehiculo" runat="server" OnClick="BtnVehiculoOnClick" VariableName="PARENTI03" ResourceName="Entities" CssClass="LogicButton_Big" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div style="overflow-y: scroll;overflow-x: scroll; height: 250px">
                <asp:UpdatePanel ID="pnlDetalleEntregas" runat="server" >
                    <ContentTemplate>
                        <table width="100%" border="0">
                            <tr>
                                <td align="left">
                                    <C1:c1GridView ID="gridEntregas" runat="server" OnRowDataBound="GridEntregasRowDataBound" SkinID="SmallGrid" ScrollSettings="AllowColMoving: true;" >
                                        <Columns>
                                            <C1:C1TemplateField HeaderText="Orden" >
                                                <ItemStyle HorizontalAlign="Right" />
                                            </C1:C1TemplateField>
                                            <C1:C1TemplateField HeaderText="Estado" />
                                            <C1:C1TemplateField HeaderText="Entrega" />
                                            <C1:C1TemplateField HeaderText="Descripción" />
                                        </Columns>
                                    </C1:c1GridView>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server">
            <mon:Monitor ID="monitor" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
        
        <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progress" class="progress"></div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>
