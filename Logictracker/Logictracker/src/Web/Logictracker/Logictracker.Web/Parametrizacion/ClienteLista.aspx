<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ClienteLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ClienteLista" Title="Clientes" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" runat="server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="false" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="200px" ParentControls="ddlDistrito" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" runat="server">
</asp:Content>
