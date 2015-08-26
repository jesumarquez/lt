<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="TicketSoporteLista.aspx.cs" Inherits="Logictracker.Soporte.SoporteTicketSoporteLista" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table width="100%">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" AutoPostBack="true" runat="server" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="ESTADO"/>                
                <br />
                <asp:DropDownList ID="cbEstado" AutoPostBack="true" runat="server" Width="180px" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESDE"/>
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" IsValidEmpty="true" Mode="DateTime" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="HASTA"/>
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" IsValidEmpty="true" Mode="DateTime" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
