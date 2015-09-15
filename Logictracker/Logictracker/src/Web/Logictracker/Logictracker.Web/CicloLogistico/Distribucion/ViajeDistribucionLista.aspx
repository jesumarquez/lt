<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ViajeDistribucionLista.aspx.cs" Inherits="Logictracker.CicloLogistico.Distribucion.ViajeDistribucionLista" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="False" OnSelectedIndexChanged="FilterChanged" />
                <br/>
                <br/>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="True"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChanged" />
            </td>
            <td>
                <%--DEPARTAMENTO--%>
                <cwc:ResourceLinkButton ID="lblDepto" runat="server" ResourceName="Entities" VariableName="PARENTI04" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbDepartamento" />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:DepartamentoListBox ID="cbDepartamento" runat="server" Width="100%" OnSelectedIndexChanged="FilterChanged" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--CENTRO COSTO--%>
                <cwc:ResourceLinkButton ID="lblCentroCosto" runat="server" ResourceName="Entities" VariableName="PARENTI37" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbCentroDeCosto" />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:CentroDeCostosListBox ID="cbCentroDeCosto" runat="server" Width="100%" OnSelectedIndexChanged="FilterChanged" ParentControls="cbEmpresa,cbLinea,cbDepartamento" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--TRANSPORTISTA--%>
                <cwc:ResourceLinkButton ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbTransportista" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TransportistasListBox ID="cbTransportista" runat="server" Width="100%" OnSelectedIndexChanged="FilterChanged" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--VEHICULO--%>
                <cwc:ResourceLinkButton ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbMovil" />
                <br />
                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="cbMovil" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea,cbDepartamento,cbCentroDeCosto,cbTransportista" OnSelectedIndexChanged="FilterChanged" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbCentroDeCosto" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
                <br/>
                <br/>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
                <cwc:DateTimeRangeValidator ID="dtvalidator" runat="server" StartControlID="dtpDesde" EndControlID="dtpHasta" MaxRange="23:59" />
            </td>
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
                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="ENTREGAS" />:
                            <asp:Label ID="lblCliente" runat="server" />
                        </div>
                        <div style="text-align: right;">            
                            <cwc:ResourceButton ID="btOpenedCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" />
                            <cwc:ResourceButton ID="btOpenedCerrarTicket" runat="server" ResourceName="Controls" VariableName="BUTTON_CERRAR_CLOG" OnClick="BtOpenedCerrarTicketClick" />
                        </div>
                    </div>
                </asp:Panel>
        
                <asp:Panel ID="panelStartTicket" runat="server">
                    <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                        <div style="margin-bottom: 10px;text-align: center;">
                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="TICKET_INIT" />
                        </div>
                        <div style="text-align: right;">            
                            <cwc:ResourceButton ID="btCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" />
                            <cwc:ResourceButton ID="btIniciar" runat="server" ResourceName="Controls" VariableName="BUTTON_START_CLOG" OnClick="BtIniciarClick" />
                        </div>
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="panelAsociarVehiculo" runat="server">
                    <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                        <div style="margin-bottom: 10px;text-align: center;">
                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                            <cwc:MovilDropDownList ID="cbVehiculoAsociar" runat="server" AutoPostBack="False" />
                        </div>
                        <div style="text-align: right;">            
                            <cwc:ResourceButton ID="btCancelarAsociacion" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" />
                            <cwc:ResourceButton ID="btAceptarAsociacion" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtAceptarAsociacionClick" />
                        </div>
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="panelCombinarViaje" runat="server">
                    <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                        <div style="margin-bottom: 10px;text-align: center;">
                            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="OPETICK01" />
                            <asp:DropDownList ID="cbViaje" runat="server" AutoPostBack="False" />
                        </div>
                        <div style="text-align: right;">            
                            <cwc:ResourceButton ID="btCancelarCombinar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" />
                            <cwc:ResourceButton ID="btAceptarCombinar" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtAceptarCombinarClick" />
                        </div>
                    </div>
                </asp:Panel>
                
            </asp:Panel>
    
            <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
    
            <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" CancelControlID="btCancelar" ID="mpePanel" runat="server" BackgroundCssClass="disabled_back" >
            </AjaxToolkit:ModalPopupExtender>
        
        </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>

