<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="DepartamentoLista.aspx.cs" Inherits="Logictracker.Parametrizacion.DepartamentoLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table><tr><td>
        <cwc:ResourceLabel ID="lblDistritto" runat="server" VariableName="PARENTI01" ResourceName="Entities" /><br />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
    </td><td>
        <cwc:ResourceLabel ID="lblBase" runat="server" VariableName="PARENTI02" ResourceName="Entities" /><br />
        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
    </td></tr></table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

