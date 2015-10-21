<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="LoginsHistory.aspx.cs" Inherits="Logictracker.Reportes.Auditoria.Reportes_Auditoria_LoginsHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
<table width="100%">
    <tr>
        <td>
            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
            <br /> 
            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" Width="75px" />
        </td>
        <td align="left">
            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
            <br />
            <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" Width="75px" />
       </td>
    </tr>
</table>
</asp:Content>


