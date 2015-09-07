<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionTanqueLista" Codebehind="TanqueLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI19" /><br />
                <cwc:EquipoDropDownList ID="cbEquipo" runat="server" Width="175px" ParentControls="cbEmpresa, cbLinea" AddAllItem="true" AddNoneItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>
