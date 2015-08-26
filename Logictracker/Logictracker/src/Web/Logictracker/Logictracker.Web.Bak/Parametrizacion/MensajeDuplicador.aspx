<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MensajeDuplicador.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionMensajeDuplicador" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
    <div>    
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="DUPLICAR_MENSAJES" />

    
    <table width="100%">
    
        <tr align="center" valign="bottom">
            <td style="width:300">    
        <%--EMPRESA--%>
        <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <asp:UpdatePanel runat="server"  UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <cwc:LocacionListBox ID="lbEmpresa" runat="server" Width="175px" SelectionMode="Multiple" AddAllItem="true"  />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        </td>
        <td>
        <%--LINEA--%>
        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="false">
            <ContentTemplate>
                <cwc:PlantaListBox ID="lbLinea" runat="server" Width="175px" AutoPostBack="True" SelectionMode="Multiple" ParentControls="lbEmpresa" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        </td>
        <td>
        <cwc:ResourceButton runat="server" ID="btnAceptar" OnClick="btnAceptar_Click" Width="75px" ResourceName="Controls"
                                    VariableName="BUTTON_ACCEPT"/>
                                    
        </td>
        
        </tr>
        </table>
   
    
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <asp:UpdatePanel runat="server" ID="upError" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <cwc:InfoLabel ID="infoLabel1" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAceptar" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    
    
    </div>
    </form>
</body>
</html>
