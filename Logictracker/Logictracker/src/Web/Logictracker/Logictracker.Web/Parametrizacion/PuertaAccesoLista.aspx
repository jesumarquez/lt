<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.PuertaAccesoLista" Title="Untitled Page" Codebehind="PuertaAccesoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AutoPostBack="true" />
</td><td>
    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AutoPostBack="true" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler"/>
</td></tr></table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

