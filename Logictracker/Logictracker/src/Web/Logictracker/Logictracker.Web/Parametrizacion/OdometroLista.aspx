<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.OdometroLista" Codebehind="OdometroLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="false" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" ParentControls="ddlDistrito" Width="175px" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>