<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="CocheLista.aspx.cs" Inherits="Logictracker.Parametrizacion.CocheLista" Title="Coches" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server"> 
<table><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="TipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" /><br />
    <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" AddAllItem="true" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbLinea" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti07" runat="server" ResourceName="Entities" VariableName="PARENTI07" /><br />
    <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" AddAllItem="true" AddNoneItem="true" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbLinea" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti37" runat="server" ResourceName="Entities" VariableName="PARENTI37" /><br />
    <cwc:CentroDeCostosDropDownList ID="ddlCentroCostos" runat="server" AddAllItem="true" AddNoneItem="true" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbLinea" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
 <cwc:ResourceCheckBox ID="chkOcultarConDispo" runat="server" ResourceName="Controls" VariableName="OCULTAR_CON_DISPO" AutoPostBack="true"  OnCheckedChanged="FilterChangedHandler" />
 <br />
 <cwc:ResourceCheckBox ID="chkOcultarSinDispo" runat="server" ResourceName="Controls" VariableName="OCULTAR_SIN_DISPO" AutoPostBack="true"  OnCheckedChanged="FilterChangedHandler" />
</asp:Content>
