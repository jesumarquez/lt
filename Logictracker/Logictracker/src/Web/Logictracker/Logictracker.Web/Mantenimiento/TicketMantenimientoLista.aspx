<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TicketMantenimientoLista.aspx.cs" Inherits="Logictracker.Mantenimiento.TicketMantenimientoLista" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table width="100%">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblTaller" runat="server" ResourceName="Entities" VariableName="PARENTI35"/>
                <br />
                <cwc:TallerDropDownList ID="cbTaller" AutoPostBack="true" runat="server" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AutoPostBack="true" runat="server" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" AutoPostBack="true" runat="server" ParentControls="cbEmpresa" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03"/>
                <br />
                <cwc:MovilDropDownList ID="cbVehiculo" AutoPostBack="true" runat="server" ParentControls="cbEmpresa,cbLinea" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESDE"/>
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" IsValidEmpty="False" Mode="DateTime" TimeMode="Start" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="HASTA"/>
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" IsValidEmpty="False" Mode="DateTime" TimeMode="End" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
