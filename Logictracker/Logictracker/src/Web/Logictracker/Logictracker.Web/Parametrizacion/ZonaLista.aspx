<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ZonaLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ZonaLista" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ReferenciasGeograficas" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel id="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList id="ddlPlanta" runat="server" Width="200px" ParentControls="ddlLocacion" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel id="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI93" />
                <br />
                <cwc:TipoZonaDropDownList id="cbTipoZona" runat="server" Width="200px" ParentControls="ddlPlanta" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>