<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ConciliacionesLista.aspx.cs" Inherits="Logictracker.Reportes.CombustibleEnPozos.ReportesCombustibleEnPozosConciliacionesLista" %>

<%@ Register Src="~/App_Controls/Pickers/DateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="120px" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="120px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti19" runat="server" ResourceName="Entities" VariableName="PARENTI19" /><br />
    <cwc:EquipoDropDownList ID="cbEquipo" runat="server" Width="120px" ParentControls="cbEmpresa, cbLinea" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti36" runat="server" ResourceName="Entities" VariableName="PARENTI36" /><br />
    <cwc:TanqueDropDownList ID="cbTanque" runat="server" Width="120px" AllowBaseBinding="false" AllowEquipmentBinding="true" ParentControls="cbEquipo" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Width="75px" /><br />
    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" />
</td><td>
    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Width="75px" /><br />
    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="End" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>


