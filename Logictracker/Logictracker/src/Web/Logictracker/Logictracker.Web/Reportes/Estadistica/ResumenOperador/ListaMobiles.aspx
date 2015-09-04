<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ResumenOperador.Reportes_Estadistica_ResumenOperador_ListaMobiles" Codebehind="ListaMobiles.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
    <table width="100%" style="font-size: x-small">
        <tr align="center" valign="top">
            <td>
                <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
                <%--ERRORLABEL--%>
                <cwc:InfoLabel ID="infoLabel1" runat="server" />
                <asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>
                        <c1:C1GridView ID="grid" runat="server" SkinID="ListGrid">
                        </c1:C1GridView>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
