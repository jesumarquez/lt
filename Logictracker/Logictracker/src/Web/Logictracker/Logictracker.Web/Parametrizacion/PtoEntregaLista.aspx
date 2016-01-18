<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="True" Inherits="Logictracker.Parametrizacion.ParametrizacionPtoEntregaLista" Title="Untitled Page" Codebehind="PtoEntregaLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" AutoPostBack="true" Width="200px" AddAllItem="false" onselectedindexchanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" AutoPostBack="true" Width="200px" ParentControls="ddlDistrito" AddAllItem="true" onselectedindexchanged="FilterChangedHandler"  />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="PARENTI18" /><br />
                <cwc:ClienteDropDownList ID="ddlCliente" runat="server" AutoPostBack="true" Width="200px" ParentControls="ddlDistrito,ddlBase" onselectedindexchanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblActivarPaginacion" Text="Paginar Consulta" runat="server" ResourceName="Entities" VariableName="Paginar Consulta" /><br />
                <cwc:ResourceCheckBox ID="CheckBoxActivarPaginacion" runat="server" Checked="true" AutoPostBack="true" Width="200px"  OnCheckedChanged="CheckBoxActivarPaginacion_CheckedChanged" />
    
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>