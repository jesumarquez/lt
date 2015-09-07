<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Admin.AdminQueue" Codebehind="Queues.aspx.cs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <%--TITLEBAR--%>
        <cwc:TitleBar ID="TitleBar1" runat="server" ResourceName="Menu" VariableName="QUEUES_ADMIN">
            <cwc:ResourceButton ID="btnActualizar" runat="server" ResourceName="Controls" VariableName="BUTTON_REFRESH" OnClick="BtnSearchClick" />
        </cwc:TitleBar>
        <%--PROGRESSLABEL--%>
        <cwc:ProgressLabel ID="ProgressLabel1" runat="server">
        </cwc:ProgressLabel>
        <%--ERRORLABEL--%>
        <cwc:InfoLabel ID="infoLabel1" runat="server" />
        <asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <c1:C1GridView ID="gridResults" runat="server" SkinID="ListGrid">
                </c1:C1GridView>
            </ContentTemplate>
            <Triggers>
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
