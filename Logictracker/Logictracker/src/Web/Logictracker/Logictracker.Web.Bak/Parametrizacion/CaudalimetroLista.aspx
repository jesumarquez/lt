<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master"  AutoEventWireup="true" CodeFile="CaudalimetroLista.aspx.cs" Inherits="Logictracker.Parametrizacion.Parametrizacion_CaudalimetroLista" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI19" /><br />
    <cwc:EquipoDropDownList ID="cbEquipo" runat="server" Width="200px" ParentControls="cbEmpresa, cbLinea" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />   
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>


