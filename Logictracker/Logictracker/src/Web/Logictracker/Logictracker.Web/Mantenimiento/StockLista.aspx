<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.StockLista" Title="" Codebehind="StockLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--DEPOSITO--%>
                <cwc:ResourceLabel ID="lblDeposito" runat="server" ResourceName="Entities" VariableName="PARENTI87"/>
                <br />
                <cwc:DepositoDropDownList ID="cbDeposito" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--INSUMO--%>
                <cwc:ResourceLabel ID="lblInsumo" runat="server" ResourceName="Entities" VariableName="PARENTI58"/>
                <br />
                <cwc:InsumoDropDownList ID="cbInsumo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--DESDE--%>
                <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="DESDE"/>
                <br />
                <cwc:DateTimePicker ID="dtFecha" runat="server" Width="105px" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" OnDateChanged="FilterChangedHandler" AutoPostBack="true" />
            </td>
        </tr>
    </table>
</asp:Content>

