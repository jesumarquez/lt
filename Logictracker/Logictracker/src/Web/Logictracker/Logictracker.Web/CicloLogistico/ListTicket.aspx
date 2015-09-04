<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.ListaTicket" Title="Untitled Page" Codebehind="ListTicket.aspx.cs" %>
      
<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="false" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--TRANSPORTISTA--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <br />
                        <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="100%" AddAllItem="true" 
                            OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--VEHICULO--%>
                <cwc:ResourceLabel ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="cbMovil" runat="server" Width="100%" AddAllItem="true"
                            ParentControls="cbLinea,cbTransportista" OnSelectedIndexChanged="FilterChangedHandler" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <%--BOCA DE CARGA--%>
                <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Entities" VariableName="PARTICK04" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:BocaDeCargaDropDownList ID="cbBocaDeCarga" runat="server" Width="100%" AddAllItem="true" AddNoneItem="true" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="bottom" align="right" rowspan="2">
                <%--<cwc:ResourceButton ID="btnBuscar" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="btnBuscar_Click" />--%>
            </td>
        </tr><tr>
            <td>
                <%--ESTADO--%>
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="STATE" />
                <br />
                <cwc:EstadoTicketDropDownList ID="cbEstado" runat="server" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" Width="100%" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td></td>
            
        </tr>
    </table>
   
   <asp:UpdatePanel ID="updasdf" runat="server">
   <ContentTemplate>
    <asp:Panel ID="PanelModal" runat="server" style="display: none;">
        <asp:Panel ID="panelOpenedTicket" runat="server">
            <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                <div style="margin-bottom: 10px;text-align: center;">
                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ForeColor="Red" ResourceName="Labels" VariableName="TICKET_OPENED" />
                </div>
                <div style="margin-bottom: 30px;text-align: center;">
                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="SystemMessages" VariableName="TICKET_OPENED" />
                </div>
                <div style="margin-bottom: 30px; text-align: left;">
                    <cwc:ResourceLabel ID="ResourceLabel11" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="TICKET" />: 
                    <asp:Label ID="lblCodigoTicket" runat="server" Font-Size="Small" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel9" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="FECHA" />:
                    <asp:Label ID="lblFecha" runat="server" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel7" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="CLIENT" />:
                    <asp:Label ID="lblCliente" runat="server" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI44" />:
                    <asp:Label ID="lblPuntoEntrega" runat="server" />
                </div>
                <div style="text-align: right;">            
                    <cwc:ResourceButton ID="btOpenedCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="btCancelar_Click" />
                    <cwc:ResourceButton ID="btOpenedCerrarTicket" runat="server" ResourceName="Controls" VariableName="BUTTON_CERRAR_CLOG" OnClick="btOpenedCerrarTicket_Click" />
                </div>
            </div>
        </asp:Panel>
        
        <asp:Panel ID="panelStartTicket" runat="server">
            <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                <div style="margin-bottom: 10px;text-align: center;">
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TICKET_INIT" />
                </div>
                <div style="margin-bottom: 30px;text-align: center;">
                    <cwc:ResourceLabel ID="lblInit" runat="server" ResourceName="SystemMessages" VariableName="TICKET_INIT" />
                </div>
                <div style="margin-bottom: 30px; text-align: center;">
                    <cwc:DateTimePicker ID="dtHora" runat="server" Mode="DateTime" TimeMode="None" />
                    <cwc:ResourceButton ID="btNow" runat="server" ResourceName="Controls" VariableName="BUTTON_NOW" OnClick="btNow_Click" />
                </div>
                <div style="text-align: right;">            
                    <cwc:ResourceButton ID="btCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="btCancelar_Click" />
                    <cwc:ResourceButton ID="btIniciar" runat="server" ResourceName="Controls" VariableName="BUTTON_START_CLOG" OnClick="btIniciar_Click" />
                </div>
            </div>
        </asp:Panel>
     
    </asp:Panel>
    <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
    <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" CancelControlID="btCancelar" ID="mpePanel" runat="server" BackgroundCssClass="disabled_back" >
    </AjaxToolkit:ModalPopupExtender>
    </ContentTemplate>
   </asp:UpdatePanel>
   
   <asp:UpdatePanel ID="updDocumentos" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <%--POPUP DE DOCUMENTOS VENCIDOS--%>
                <asp:Panel ID="panelModalDocumentos" runat="server" style="display: none;">                
                    <asp:Panel ID="panelDocumentos" runat="server">
                        <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                            <div style="margin-bottom: 10px;text-align: center;">
                                <cwc:ResourceLabel ID="ResourceLabel17" runat="server" Font-Bold="true" ResourceName="SystemMessages" VariableName="TICKET_DOCUMENTOS_VENCIDOS" />
                                <br /><br />
                            </div>                
                            <div style="margin-bottom: 30px;">
                                <asp:BulletedList ID="cbDocumentosVencidos" runat="server">
                                </asp:BulletedList>
                            </div>
                            <div style="text-align: right;">            
                                <cwc:ResourceButton ID="btCancelarDocumentos" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                                &nbsp;&nbsp;&nbsp;
                                <cwc:ResourceButton ID="btAceptarDocumentos" runat="server" ResourceName="Controls" VariableName="BUTTON_START_CLOG" OnClick="btAceptarDocumentos_Click" />
                            </div>
                        </div>
                    </asp:Panel>             
                </asp:Panel>
                <asp:Panel ID="panelBobo2" runat="server"></asp:Panel>
                <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo2" PopupControlID="panelModalDocumentos" CancelControlID="btCancelarDocumentos" ID="ModalPopupExtenderDocumentos" runat="server" BackgroundCssClass="disabled_back" ></AjaxToolkit:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>
</asp:Content>
