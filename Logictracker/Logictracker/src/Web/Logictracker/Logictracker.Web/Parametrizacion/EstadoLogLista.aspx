<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.Parametrizacion_EstadoLogLista" Title="Estados Logisticos" Codebehind="EstadoLogLista.aspx.cs" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="175px" />
</td><td>
    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" AutoPostBack="True" ParentControls="cbEmpresa" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>