<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="LineaTelefonicaLista.aspx.cs" Inherits="Logictracker.Parametrizacion.LineaTelefonicaLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="EMPRESA"/>
                <br />
                <cwc:EmpresaTelefonicaDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" onselectedindexchanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>

