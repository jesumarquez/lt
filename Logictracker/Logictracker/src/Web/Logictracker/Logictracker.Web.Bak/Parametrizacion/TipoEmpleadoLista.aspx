<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TipoEmpleadoLista.aspx.cs" Inherits="Logictracker.Parametrizacion.TipoEmpleadoLista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" ParentControls="ddlDistrito" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>