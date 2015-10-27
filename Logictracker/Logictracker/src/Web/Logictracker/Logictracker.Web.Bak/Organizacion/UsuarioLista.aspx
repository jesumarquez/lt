<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="UsuarioLista.aspx.cs"
    Inherits="Logictracker.Organizacion.ListaUsuario" Title="Usuarios" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
    <br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

