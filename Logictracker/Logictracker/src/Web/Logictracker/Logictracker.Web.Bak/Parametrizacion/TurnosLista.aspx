﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TurnosLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionTurnosLista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" />
</td><td>
    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
    <cwc:PlantaDropDownList ID="ddlBase" runat="server" ParentControls="ddlDistrito" Width="175px" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>