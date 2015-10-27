﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TipoGeoRefLista.aspx.cs" Inherits="Logictracker.Parametrizacion.TipoGeoRefLista" Title="Untitled Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" AddAllItem="false" Width="200px" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" ParentControls="ddlLocacion" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" Width="200px" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>