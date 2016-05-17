<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConductoresPorDia.aspx.cs"
    Inherits="Logictracker.Reportes.Estadistica.ResumenVehicular.ReportesEstadisticaResumenVehicularConductoresPorDia" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" />
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
    <table width="100%" cellpadding="10">
        <tr align="center" valign="bottom">
            <td>
                <br />
                <asp:Label ID="lblInterno" runat="server" Font-Bold="true" Font-Size="Small" />
                <br />
            </td>
        </tr>
    </table>
    <asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <c1:C1GridView ID="grid" runat="server" SkinID="ListGrid">
            </c1:C1GridView>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upPrint" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <C1:C1GridView ID="gridPrint" runat="server" SkinID="PrintGrid">
            </C1:C1GridView> 
        </ContentTemplate>
        <Triggers>
        </Triggers>
        </asp:UpdatePanel>  
    </form>
</body>
</html>
