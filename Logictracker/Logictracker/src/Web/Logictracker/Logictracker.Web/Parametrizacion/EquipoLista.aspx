<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionEquipoLista" Title="Untitled Page" Codebehind="EquipoLista.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbEmpresa" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" /><br />
    <cwc:ClienteDropDownList ID="cbCliente" runat="server" AutoPostBack="true" Width="175px" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />      
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>