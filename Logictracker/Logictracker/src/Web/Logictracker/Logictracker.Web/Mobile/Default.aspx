<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Logictracker.Mobile.Mobile_Default" Theme="" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Culture" Assembly="Logictracker.Web.CustomWebControls" %>    

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Login</title>    
</head>
<body style="background-color:#E7AA00;">
    <form id="form1" runat="server" defaultfocus="txtUsuario" defaultbutton="btLogin"> 
        <asp:Literal ID="litStyle" runat="server"></asp:Literal>  
        <div id="divBack" runat="server">
            
            <asp:Panel ID="panLogin" runat="server" Width="100%" HorizontalAlign="Center">
           <%-- <p>&nbsp;</p>--%>
            <asp:Image runat="server" ImageAlign="Middle" ImageUrl="~/App_Themes/Naranja Vehicular/img/mobile_Logiclogo.jpg"/>
            <%--ERRORLABEL--%>
            <div>
            <asp:Label ID="infoLabel1" runat="server" SkinID="Blank"></asp:Label>
                </div>
                <table align="center" cellspacing="0" style="">
                    <tr>
                        <td style="height: 12px;" >
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="User" VariableName="LOGIN_USERNAME"></cwc:ResourceLabel></td>
                        <td><asp:TextBox ID="txtUsuario" runat="server" MaxLength="255"  TabIndex="1"/></td>
                    </tr>
                    <tr>
                        <td style="height: 12px;" ><cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="User" VariableName="LOGIN_PASSWORD"></cwc:ResourceLabel></td>
                        <td>
                            <asp:TextBox ID="txtPassword" runat="server" MaxLength="255" TabIndex="1" TextMode="Password" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right" colspan="2" style="height: 12px;">
                            <cwc:ResourceButton ID="btLogin" runat="server" OnClick="btLogin_Click" ResourceName="Controls" VariableName="BUTTON_LOGIN" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom" align="right" colspan="2" style="height: 12px;">
                            <cwc:CultureSelector ID="cultureSelector" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            
            <asp:Panel ID="panPerfil" runat="server" Width="100%" Visible="false">
                <p>&nbsp;</p>
                <table align="center" cellspacing="0" style="text-align: right;">
                    <tr>
                    <td style="height: 68px;" ><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="User" VariableName="LOGIN_SELECTROLE"></cwc:ResourceLabel></td>
                    <td><asp:DropDownList ID="cbPerfiles" runat="server" Visible="true" /></td></tr>
                    <tr>
                        <td valign="bottom" align="right" colspan="2" style="height: 24px;">
                            <cwc:ResourceButton ID="btSelPerfil" runat="server" OnClick="btSelPerfil_Click" ResourceName="Controls" VariableName="BUTTON_SELECT" />
                            <cwc:ResourceButton ID="btCancel" runat="server" OnClick="btCancel_Click" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
