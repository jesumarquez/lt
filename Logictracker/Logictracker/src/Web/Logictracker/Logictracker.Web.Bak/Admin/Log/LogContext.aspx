<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LogContext.aspx.cs" Inherits="Logictracker.Admin.Log.AdminLogContext" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Labels" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.ToolBar" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body>
    <form id="form2" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" />
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
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
    
    </form>
    
</body>
</html>
