<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.ControlDeCombustible.Despachos.ControlDeCombustible_Despachos_DespachosPorMovil" Codebehind="DespachosPorMovil.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
</head>
<body>
<form id="Form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <asp:UpdatePanel id="upGrid" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
          <C1:C1GridView ID="grid" runat="server" SkinID="ListGrid">
            </C1:C1GridView>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="grid" EventName="Sorted" />
    </Triggers>
</asp:UpdatePanel>
 </form>
</body>
</html>


 