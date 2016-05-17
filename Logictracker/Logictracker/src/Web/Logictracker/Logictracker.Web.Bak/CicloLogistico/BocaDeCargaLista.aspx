<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="BocaDeCargaLista.aspx.cs" Inherits="Logictracker.CicloLogistico.ListaBocaDeCarga" Title="Untitled Page" %>
      
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
        </tr>
    </table>
</asp:Content>