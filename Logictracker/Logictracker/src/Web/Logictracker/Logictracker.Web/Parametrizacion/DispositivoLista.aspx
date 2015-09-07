<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.DispositivoLista" Title="Dispositivos" Codebehind="DispositivoLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI32" /><br />
    <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="200px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td>
    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="ddlLocacion" AddAllItem="true" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
<div>
    <cwc:ResourceCheckBox ID="chkSoloSinAsignar" runat="server" ResourceName="Labels" VariableName="SOLO_SIN_ASIGNAR" AutoPostBack="true" OnCheckedChanged="FilterChangedHandler" />
</div>
</asp:Content>