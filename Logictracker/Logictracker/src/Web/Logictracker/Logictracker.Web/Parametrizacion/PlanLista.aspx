<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="PlanLista.aspx.cs" Inherits="Logictracker.Parametrizacion.PlanLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="EMPRESA"/>
                <br />
                <cwc:EmpresaTelefonicaDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
            <td>
                <%--TIPO DE LINEA--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="TIPO_LINEA"/>
                <br />
                <cwc:TipoLineaTelefonicaDropDownList ID="cbTipoLinea" runat="server" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

