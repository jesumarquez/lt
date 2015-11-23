<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.ConsumosLista" Title="" Codebehind="ConsumosLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
        <table>
        <tr>
            <td valign="top">
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="98%" AddAllItem="false" OnSelectedIndexChanged="FilterChangedHandler" />
                <br/>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
                <br/>
                <%--TRANSPORTISTA--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="98%" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbLinea" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td valign="top">
                <%--TIPO VEHICULO--%>
                <cwc:ResourceLabel ID="lblParenti17" runat="server" ResourceName="Entities" VariableName="PARENTI17" Font-Bold="true" />
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="98%" ParentControls="cbLinea,cbTransportista" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br/>
                <%--VEHICULO--%>
                <cwc:ResourceLabel ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" />
                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="cbMovil" runat="server" Width="98%" ParentControls="cbLinea,cbTransportista,cbTipoVehiculo" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <%--TIPO PROVEEDOR--%>
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI86" Font-Bold="true" />
                <asp:UpdatePanel ID="upTipoProveedor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TipoProveedorDropDownList ID="cbTipoProveedor" runat="server" Width="98%" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbEmpresa,cbLinea" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel>
                <br/>
                <%--PROVEEDOR--%>
                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI59" Font-Bold="true" />
                <asp:UpdatePanel ID="upProveedor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:ProveedorDropDownList ID="cbProveedor" runat="server" Width="98%" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbEmpresa,cbLinea,cbTipoProveedor" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoProveedor" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <%--DEPOSITO ORIGEN--%>
                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="DEPOSITO_ORIGEN" ListControlTargetID="cbDepositoOrigen" Font-Bold="true" />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:DepositoDropDownList ID="cbDepositoOrigen" runat="server" Width="98%" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel>
                <br/>
                <%--DEPOSITO DESTINO--%>
                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="DEPOSITO_DESTINO" Font-Bold="true" />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:DepositoDropDownList ID="cbDepositoDestino" runat="server" Width="98%" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
                <br/>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

