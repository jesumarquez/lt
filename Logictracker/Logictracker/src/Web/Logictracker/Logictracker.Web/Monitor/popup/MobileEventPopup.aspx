 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Monitor.popup.Monitor_MonitorHistorico_popup_MobileEventPopup" Codebehind="MobileEventPopup.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title />
</head>
<body style="background-color: White">
    <form id="form1" runat="server">
    <div>
        <table width="100%">
            <tr>
                <td style="font-weight: bold;">
                    <asp:Label ID="lblCorner" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblVehicle" runat="server" ResourceName="Entities" VariableName="PARENTI03" /></u>: 
                    <asp:Label ID="lblCoche" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblDate" runat="server" ResourceName="Labels" VariableName="DATE" /></u>: 
                    <asp:Label ID="lblFecha" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <u><cwc:ResourceLabel ID="lblMessage" runat="server" ResourceName="Entities" VariableName="PAREVEN01" /></u>: 
                    <asp:Label ID="lblMensaje" runat="server" />
                </td>
            </tr>
            <tr>
                <td>                    
                    <asp:Label ID="lblLink" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
