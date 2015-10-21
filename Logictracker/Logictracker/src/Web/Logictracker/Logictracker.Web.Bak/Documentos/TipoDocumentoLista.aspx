<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TipoDocumentoLista.aspx.cs" Inherits="Logictracker.Documentos.Documentos_TipoDocumentoLista" Title="Untitled Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table><tr><td>
        <cwc:ResourceLabel ID="lblEmpresa" runat="server" VariableName="PARENTI01" ResourceName="Entities" /><br />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
    </td><td>
        <cwc:ResourceLabel ID="lblLinea" runat="server" VariableName="PARENTI02" ResourceName="Entities" /><br />
        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
    </td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>