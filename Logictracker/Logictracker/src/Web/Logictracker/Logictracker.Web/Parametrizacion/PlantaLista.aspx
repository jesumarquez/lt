<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.PlantaLista" Title="Bases" Codebehind="PlantaLista.aspx.cs" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
    <br />
    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" Width="200px" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>