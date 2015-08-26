<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ParametrosDispositivoLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionParametrosDispositivoLista" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" ResourceName="Entities" VariableName="TIPODISPOSITIVO"></cwc:ResourceLabel><br />
    <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="200px" onselectedindexchanged="FilterChangedHandler" />   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
    <cwc:ResourceButton ID="btnResetDevices" runat="server" Width="150px" ResourceName="Labels" VariableName="RESET_DEVICES" OnClick="btnResetDevices_Click" />
</asp:Content>