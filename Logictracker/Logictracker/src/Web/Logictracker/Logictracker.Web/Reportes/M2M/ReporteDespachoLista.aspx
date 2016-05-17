<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.M2M.ReporteDespachoLista" Title="Reporte de Despachos" Codebehind="ReporteDespachoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="False" runat="server" Width="150px" />
                <br />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="150px" AutoPostBack="true" ParentControls="cbEmpresa" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" ListControlTargetID="cbTransportista" />
                <br />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistasListBox ID="cbTransportista" runat="server" Width="150px" SelectionMode="Multiple" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" Font-Bold="true" ForeColor="Black" ListControlTargetID="cbTipoVehiculo" />
                <br />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox ID="cbTipoVehiculo" runat="server" Width="150px" SelectionMode="Multiple" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="cbVehiculo" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="cbVehiculo" runat="server" Width="150px" SelectionMode="Multiple" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea,cbTransportista,cbTipoVehiculo" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblFechaDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaDesde" runat="server" IsValidEmpty="false" TimeMode="Start" />                
                <br/>
                <br />
                <cwc:ResourceLabel ID="lblFechaHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaHasta" runat="server" IsValidEmpty="false" TimeMode="End" />
            </td>
        </tr>
    </table>
    
</asp:Content>


