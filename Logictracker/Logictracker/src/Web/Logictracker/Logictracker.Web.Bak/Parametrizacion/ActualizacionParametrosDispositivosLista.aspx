<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ActualizacionParametrosDispositivosLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionActualizacionParametrosDispositivosLista" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" ResourceName="Entities" VariableName="TIPODISPOSITIVO" /><br />
    <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="200px" onselectedindexchanged="FilterChangedHandler" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>