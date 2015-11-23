<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.ControlAcceso.CategoriaAccesoLista" Codebehind="CategoriaAccesoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

