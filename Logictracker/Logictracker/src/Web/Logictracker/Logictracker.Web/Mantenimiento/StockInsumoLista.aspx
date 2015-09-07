<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.StockInsumoLista" Title="" Codebehind="StockInsumoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td rowspan="2">
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI87" Font-Bold="true"/>
                <br />
                <cwc:DepositoListBox ID="cbDeposito" runat="server" Width="100%" SelectionMode="Multiple" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td rowspan="2">
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI58" Font-Bold="true"/>
                <br />
                <cwc:InsumoListBox ID="cbInsumo" runat="server" Width="100%" SelectionMode="Multiple" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
        <tr>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

