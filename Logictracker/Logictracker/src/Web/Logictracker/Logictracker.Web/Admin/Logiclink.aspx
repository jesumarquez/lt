<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Admin.Logiclink" Title="" Codebehind="Logiclink.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <cwc:PlantaDropDownList ID="ddlBase" runat="server" AddAllItem="true" ParentControls="ddlEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Estado" VariableName="Estado" Font-Bold="true" />
                <br />
                <cwc:EstadoArchivoDropDownList ID="ddlEstadoArchivo" runat="server" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" OnDateChanged="FilterChangedHandler" IsValidEmpty="false" AutoPostBack="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="Now" Mode="DateTime" OnDateChanged="FilterChangedHandler" IsValidEmpty="false" AutoPostBack="true" />
            </td>         
        </tr>
    </table>
</asp:Content>

