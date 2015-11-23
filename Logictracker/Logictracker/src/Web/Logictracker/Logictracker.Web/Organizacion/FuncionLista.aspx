<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Organizacion.ListaFuncion"  Title="Funciones" Codebehind="FuncionLista.aspx.cs" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">

<cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Menu" VariableName="SOC_SISTEMAS" />
<br />
<cwc:SubSistemaDropDownList ID="cbSubSistema" runat="server" OnSelectedIndexChanged="FilterChangedHandler" Width="200px" AddAllItem="true" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>