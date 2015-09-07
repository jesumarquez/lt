<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.InsumoLista" Title="" Codebehind="InsumoLista.aspx.cs" %>

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
        </tr>
        <tr>
            <td>                
                <cwc:ResourceLabel ID="lblTipoInsumo" runat="server" ResourceName="Entities" VariableName="PARENTI60"/>
                <br />
                <cwc:TipoInsumoDropDownList ID="cbTipoInsumo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

