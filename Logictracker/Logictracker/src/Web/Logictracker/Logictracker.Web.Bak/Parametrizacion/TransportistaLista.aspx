<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TransportistaLista.aspx.cs" Inherits="Logictracker.Parametrizacion.TransportistaLista" Title="Transportistas" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistritto" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblBase" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                <br />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlDistrito" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>