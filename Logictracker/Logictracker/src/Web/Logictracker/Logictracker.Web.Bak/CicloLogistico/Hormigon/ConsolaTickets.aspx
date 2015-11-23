<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ConsolaTickets.aspx.cs" Inherits="Logictracker.CicloLogistico.Hormigon.ConsolaTickets" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.Tickets" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--BOCA DE CARGA--%>
                <cwc:ResourceLabel ID="lblBocaDeCarga" runat="server" ResourceName="Entities" VariableName="PARTICK04"/>
                <br />
                <cwc:BocaDeCargaDropDownList ID="cbBocaDeCarga" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--DESDE--%>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE"/>
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--HASTA--%>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA"/>
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
        </tr>
        <tr>            
            <td>
                <%--CLIENTE--%>
                <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT"/>
                <br />
                <cwc:ClienteDropDownList ID="cbCliente" runat="server" Width="100%" AddAllItem="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--PUNTO ENTREGA--%>
                <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44"/>
                <br />
                <cwc:PuntoDeEntregaDropDownList ID="cbPuntoEntrega" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbCliente" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
           <td>
                <%--PRODUCTO--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI63"/>
                <br />
                <cwc:ProductoDropDownList ID="cbProducto" runat="server" Width="100%" AddAllItem="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--ESTADO--%>
                <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Labels" VariableName="ESTADO"/>
                <br />
                <cwc:EstadosDropDownList ID="cbEstado" runat="server" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td></td>
        </tr>
    </table>
    
    <asp:UpdatePanel ID="updasdf" runat="server">
   <ContentTemplate>
    <asp:Panel ID="PanelModal" runat="server" style="display: none;">
        <div class="panelheader" style="padding-top: 10px;">Ticket de Ajuste</div>
        <asp:Panel ID="panelStartTicket" runat="server" CssClass="PopupPanel">
                <div style="margin-bottom: 30px; text-align: left;">
                    <div>Generar un ticket de ajuste para el pedido <strong><asp:Literal runat="server" ID="litAjustePedido"/></strong></div>
                    <div style="margin-top: 10px;">
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="CLIENT"/>:
                        <strong><asp:Label runat="server" ID="lblAjusteCliente"></asp:Label></strong>
                        
                        <br/>
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI44"/>:
                        <strong><asp:Label runat="server" ID="lblAjustePuntoEntrega"></asp:Label></strong>
                    </div>
                    <div style="margin-top: 10px;">    
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Entities" VariableName="Tickets"/>
                        <asp:Label runat="server" ID="lblAjusteTickets"></asp:Label>
                        
                        <br/>
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Entities" VariableName="Cantidad"/>
                        <asp:Label runat="server" ID="lblAjusteM3"></asp:Label>
                        
                    </div>
                    <div style="margin-top: 20px; text-align: right;">   
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="FECHA"/>
                        <cwc:DateTimePicker ID="dtTicketAjuste" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" />
                    </div>
                </div>
                <div style="text-align: right;">            
                    <cwc:ResourceButton ID="btCancelarAjuste" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClientClick="return false;" />
                    <cwc:ResourceButton ID="btAceptarAjuste" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="btAceptarAjuste_Click" />
                </div>
        </asp:Panel>
     
    </asp:Panel>
    <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
    <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" CancelControlID="btCancelarAjuste" ID="modalPanel" runat="server" BackgroundCssClass="disabled_back" >
    </AjaxToolkit:ModalPopupExtender>
    </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

