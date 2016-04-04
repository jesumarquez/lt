<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Documentos.DocumentoLista" Title="Untitled Page" Codebehind="DocumentoLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="200px" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList id="cbLinea" runat="server" Width="150px" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" AutoPostBack="True" ParentControls="cbEmpresa"></cwc:PlantaDropDownList> 
            </td>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI25" />
                <br />
                <cwc:TipoDocumentoDropDownList ID="cbTipoDocumento" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="True" />
            </td>
             <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
                <br/>
                <br/>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
                <cwc:DateTimeRangeValidator ID="dtvalidator" runat="server" StartControlID="dtpDesde" EndControlID="dtpHasta" MaxRange="23:59" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>