<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="ZonaAccesoLista.aspx.cs" Inherits="Logictracker.Parametrizacion.ZonaAccesoLista" %>

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
                <cwc:ResourceLabel id="lblTipoZonaAcceso" runat="server" ResourceName="Entities" VariableName="PARENTI91" />
                <br />
                <cwc:TipoZonaAccesoDropDownList id="ddlTipoZonaAcceso" runat="server" Width="200px" ParentControls="ddlLocacion,ddlPlante" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>