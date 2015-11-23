<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="SubCentroCostoLista.aspx.cs" Inherits="Logictracker.Parametrizacion.SubCentroCostoLista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" runat="server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" OnSelectedIndexChanged="FilterChangedHandler"  ParentControls="cbEmpresa" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblParenti04" runat="server" ResourceName="Entities" VariableName="PARENTI04" />
                <br />
                <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" Width="200px" OnSelectedIndexChanged="FilterChangedHandler"  ParentControls="cbEmpresa,cbLinea" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblParenti37" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                <br />
                <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" Width="200px" OnSelectedIndexChanged="FilterChangedHandler"  ParentControls="cbEmpresa,cbLinea,cbDepartamento" AddAllItem="true" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>
