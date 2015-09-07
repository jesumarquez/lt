<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionDetalleDispositivosLista"
    Title="Dispositivos" Codebehind="DetalleDispositivosLista.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
    
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Entities" VariableName="PARENTI32" /><br />
    <cwc:TipoDispositivoDropDownList ID="ddlTipo" runat="server" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" />
</td><td rowspan="3">
    <asp:Panel runat="server" ID="panDispositivos">
        <cwc:ResourceLabel ID="lblDispositivos" runat="server" ResourceName="Entities" VariableName="PARENTI08" /><br />
    </asp:Panel>
    <cwc:DispositivoListBox ID="lbDispositivos" runat="server" Width="175px" AutoPostBack="true" ParentControls="cbEmpresa, cbLinea, ddlTipo" Height="125px" SelectionMode="Multiple" OnSelectedIndexChanged="lbDispositivos_SelectedIndexChanged" />
    <cwc:SelectAllExtender ID="selDispositivos" runat="server" TargetControlId="panDispositivos" ListControlID="lbDispositivos"/>
</td><td rowspan="3">
    <asp:Panel runat="server" ID="panParametros">
        <cwc:ResourceLabel ID="lblParametros" runat="server" ResourceName="Labels" VariableName="PARAMETERS" /><br />
    </asp:Panel>
    <cwc:TipoParametroDispositivoListBox ID="lbParametros" runat="server" Width="175px" ParentControls="ddlTipo" Height="125px" SelectionMode="Multiple" OnSelectedIndexChanged="lbDispositivos_SelectedIndexChanged" AutoPostBack="true" /> 
    <cwc:SelectAllExtender ID="selParametros" runat="server" TargetControlId="panParametros" ListControlID="lbParametros"/>
</td></tr><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler"  />
</td></tr><tr><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddAllItem="true"  OnSelectedIndexChanged="FilterChangedHandler" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>