<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Logictracker.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" id="login" >
<head runat="server">
    <title>Login</title>
</head>
<body runat="server" id="loginBody">
    <form id="form1" runat="server" defaultfocus="txtUsuario" defaultbutton="btLogin"> 
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />   
    
    <asp:Panel ID="panLogin" runat="server" Height="228px" Width="100%" HorizontalAlign="Center">
    
        <div class="loginform">
            <table align="center" cellspacing="20" style="text-align: right;">
                <tr>
                    <td style="height: 24px;" ><cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="User" VariableName="LOGIN_USERNAME"></cwc:ResourceLabel></td>
                    <td><asp:TextBox ID="txtUsuario" runat="server" MaxLength="255"  TabIndex="1" Width="200px" CssClass="LogicTextbox" /></td>
                </tr>
                <tr>
                    <td style="height: 24px;" ><cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="User" VariableName="LOGIN_PASSWORD"></cwc:ResourceLabel></td>
                    <td>
                        <asp:TextBox ID="txtPassword" runat="server" MaxLength="255" TabIndex="3" Width="200px" TextMode="Password" CssClass="LogicTextbox" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="loginbutton">
            <cwc:ResourceButton ID="btLogin" runat="server" OnClick="BtLoginClick" ResourceName="Controls" VariableName="BUTTON_LOGIN" CssClass="LogicButton_Big" />
            <cwc:InfoLabel ID="infoLabel1" runat="server" SkinID="Blank" />
        </div>
        
        <div class="flags">
            <cwc:CultureSelector ID="cultureSelector" runat="server" />
        </div>
        
    </asp:Panel>
            
    <asp:Panel ID="panPerfil" runat="server" Width="100%" Visible="false">
        
        <div class="perfilform">
            <table align="center" cellspacing="20" style="text-align: right;">
                <tr>
                <td style="height: 68px;" ><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="User" VariableName="LOGIN_SELECTROLE"></cwc:ResourceLabel></td>
                <td><asp:DropDownList ID="cbPerfiles" runat="server" Visible="true" Width="250px" CssClass="LogicCombo" /></td></tr>
            </table>
        </div>
        
        <div class="selectbutton">
            <cwc:ResourceButton ID="btSelPerfil" runat="server" OnClick="BtSelPerfilClick" ResourceName="Controls" VariableName="BUTTON_SELECT" CssClass="LogicButton_Big" />
        </div>
        <div class="cancelbutton">
            <cwc:ResourceButton ID="btCancel" runat="server" OnClick="BtCancelClick" ResourceName="Controls" VariableName="BUTTON_CANCEL" CssClass="LogicButton_Big" />
        </div>
    </asp:Panel>
            
    </form>
</body>
</html>
