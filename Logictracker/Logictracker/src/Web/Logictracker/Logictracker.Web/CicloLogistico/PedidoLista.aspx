<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.ListaPedido" Title="Untitled Page" Codebehind="PedidoLista.aspx.cs" %>
      
<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
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
                <%--BOCA DE CARGA--%>
                <cwc:ResourceLabel ID="lblBocaDeCarga" runat="server" ResourceName="Entities" VariableName="PARTICK04"/>
                <br />
                <cwc:BocaDeCargaDropDownList ID="cbBocaDeCarga" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--DESDE--%>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE"/>
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--HASTA--%>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA"/>
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
        </tr>
        <tr>            
            <td>
                <%--CLIENTE--%>
                <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT"/>
                <br />
                <cwc:ClienteDropDownList ID="cbCliente" runat="server" Width="100%" AddAllItem="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--PUNTO ENTREGA--%>
                <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44"/>
                <br />
                <cwc:PuntoDeEntregaDropDownList ID="cbPuntoEntrega" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbCliente" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
           <td>
                <%--PRODUCTO--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI63"/>
                <br />
                <cwc:ProductoDropDownList ID="cbProducto" runat="server" Width="100%" AddAllItem="true" ParentControls="cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--ESTADO--%>
                <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Labels" VariableName="ESTADO"/>
                <br />
                <cwc:EstadosDropDownList ID="cbEstado" runat="server" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td></td>
        </tr>
    </table>
</asp:Content>
