<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.StockVehicularLista" Title="" Codebehind="StockVehicularLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblZona" runat="server" ResourceName="Entities" VariableName="PARENTI89"/>
                <br />
                <cwc:ZonaDropDownList ID="cbZona" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--COCHE--%>
                <cwc:ResourceLabel ID="lblCoche" runat="server" ResourceName="Entities" VariableName="PARENTI17"/>
                <br />
                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

