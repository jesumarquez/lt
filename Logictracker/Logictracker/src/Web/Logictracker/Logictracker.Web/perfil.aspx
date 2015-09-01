<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="True"
    codeBehind="perfil.aspx.cs" Inherits="Logictracker._perfil" Title="" %>

<%@ Register Src="App_Controls/altaEntidad.ascx" TagName="altaEntidad" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Culture" Assembly="Logictracker.Web.CustomWebControls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="upPerfil" runat="server">
        <ContentTemplate>
            <%--ERRORLABEL--%>
            <cwc:InfoLabel ID="infoLabel1" runat="server" />
            <%--TITLEBAR--%>
            <cwc:TitleBar ID="TitleBar1" runat="server" ResourceName="User" VariableName="PROFILE_TITLE">
                <asp:Label ID="lblUsuario" runat="server" CssClass="UserName" />
            </cwc:TitleBar>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="ViewMenu" runat="server">
                    <table style="width: 100%" cellspacing="20px">
                        <tr>
                            <td class="PerfilItem" style="width: 50%; padding-left: 25%; text-align: center;">
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/userinfo.png"
                                    OnClick="lnkDatos_Click" />
                                <br />
                                <cwc:ResourceLinkButton ID="lnkDatos" runat="server" OnClick="lnkDatos_Click" ResourceName="User"
                                    VariableName="PROFILE_EDIT_USER_DATA" />
                            </td>
                            <td class="PerfilItem" style="width: 50%; padding-right: 25%; text-align: center;">
                                <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="~/images/culture.png"
                                    OnClick="lnkConfiguracionRegional_Click" />
                                <br />
                                <cwc:ResourceLinkButton ID="lnkConfiguracionRegional" runat="server" OnClick="lnkConfiguracionRegional_Click"
                                    ResourceName="Labels" VariableName="CONFIG_REGIONAL" />
                            </td>
                        </tr>
                        <tr>
                            <td class="PerfilItem" style="width: 50%; padding-left: 25%; text-align: center;">
                                <asp:ImageButton ID="ImageButton3" runat="server" ImageUrl="~/images/pass.png" OnClick="lnkContraseña_Click" />
                                <br />
                                <cwc:ResourceLinkButton ID="lnkContraseña" runat="server" OnClick="lnkContraseña_Click"
                                    ResourceName="User" VariableName="PROFILE_CHANGE_PASS" />
                            </td>
                            <td class="PerfilItem" style="width: 50%; padding-right: 25%; text-align: center;">
                                <asp:ImageButton ID="ImageButton4" runat="server" ImageUrl="~/images/theme.png" OnClick="lnkTheme_Click" />
                                <br />
                                <cwc:ResourceLinkButton ID="lnkTheme" runat="server" OnClick="lnkTheme_Click" ResourceName="User"
                                    VariableName="PROFILE_THEMES" />
                            </td>
                        </tr>
                    </table>
                </asp:View>
                <asp:View ID="ViewDatos" runat="server">
                    <div class="UserProfileTitle">
                        <asp:Image ID="imgUserInfo" runat="server" ImageUrl="~/images/userinfo.png" ImageAlign="Middle" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="User" VariableName="PROFILE_EDIT_USER_DATA" CssClass="UserName" />
                    </div>
                    <div class="UserProfileContent">
                        <uc1:altaEntidad ID="AltaEntidad" runat="server" />
                        <br />
                        <br />
                        <br />
                        <div class="UserProfileButtons">
                            <cwc:ResourceButton ID="btCancelar" runat="server" CssClass="LogicButton_Big" OnClick="btCancelar_Click"
                                        ResourceName="Controls" VariableName="BUTTON_CANCEL"  />
                            <cwc:ResourceButton ID="btGuardar" runat="server" CssClass="LogicButton_Big" OnClick="btGuardar_Click"
                                        ResourceName="Controls" VariableName="BUTTON_SAVE" />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewPass" runat="server">
                    
                    <div class="UserProfileTitle">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/pass.png" ImageAlign="Middle" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="User" VariableName="PROFILE_CHANGE_PASS" CssClass="UserName"></cwc:ResourceLabel>
                    </div>
                    
                    <div class="UserProfileContent">
                    
                    
                    <table cellpadding="5" style="width: 100%;">
                        <tr>
                            <td style="padding-left: 50px;">
                                <cwc:ResourceLabel ID="lblPassword" runat="server" ResourceName="Labels" VariableName="PASSWORD" />
                            </td>
                            <td>
                                <div>
                                    <table style="width: 100%;"  cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtClave" runat="server" Width="100%" TextMode="Password" Text=""
                                                    autocomplete="off" CssClass="LogicTextbox" />
                                            </td>
                                            <td>
                                                <div>
                                                    <style type="text/css">
                                                        .strength_border
                                                        {
                                                            border: solid 1px #CCCCCC;
                                                            background-color: White;
                                                            width: 80px;
                                                        }
                                                        .strength_bar
                                                        {
                                                            background-color: Blue;
                                                        }
                                                        .strength_bar_red
                                                        {
                                                            background-color: Red;
                                                        }
                                                        .strength_bar_green
                                                        {
                                                            background-color: Green;
                                                        }
                                                    </style>
                                                    <AjaxToolkit:PasswordStrength ID="passStrength" runat="server" TargetControlID="txtClave"
                                                        StrengthIndicatorType="BarIndicator" BarBorderCssClass="strength_border" BarIndicatorCssClass="strength_bar"
                                                        DisplayPosition="RightSide" MinimumSymbolCharacters="1" MinimumNumericCharacters="1"
                                                        PreferredPasswordLength="6" MinimumLowerCaseCharacters="1" MinimumUpperCaseCharacters="0"
                                                        StrengthStyles="strength_bar_red;strength_bar;strength_bar_green">
                                                    </AjaxToolkit:PasswordStrength>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                        </tr>
                        <tr>
                            <td>
                                <cwc:ResourceLabel ID="lblConfirmacion" runat="server" ResourceName="Labels" VariableName="CONFIRM_PASSWORD" />
                            </td>
                            <td>
                                <asp:TextBox ID="txtConfirmacion" runat="server" TextMode="Password" Width="100%" CssClass="LogicTextbox" />
                            </td>
                            <div>
                            </div>
                            
                        </tr>
                    </table>
                        <div class="UserProfileButtons">
                            <cwc:ResourceButton ID="btCancelarPass" runat="server" CssClass="LogicButton_Big" OnClick="btCancelarPass_Click"
                                    ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                            <cwc:ResourceButton ID="btAceptarPass" runat="server" CssClass="LogicButton_Big" OnClick="btAceptarPass_Click"
                                    ResourceName="Controls" VariableName="BUTTON_SAVE" />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="View1" runat="server">
                    
                    <div class="UserProfileTitle">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/images/theme.png" ImageAlign="Middle" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="User" VariableName="PROFILE_THEMES" CssClass="UserName"></cwc:ResourceLabel>
                    </div>
                    
                    <div class="UserProfileContent">
                        <table cellpadding="5" style="width: 100%;">
                            <tr>
                                <td style="padding-left: 50px;">
                                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" CssClass="InputLabel" ResourceName="User"
                                        VariableName="PROFILE_SELECTTHEME"></cwc:ResourceLabel>
                                </td>
                                <td>
                                    <asp:DropDownList ID="cbTheme" runat="server" Width="100%" CssClass="LogicCombo">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                        
                        <div class="UserProfileButtons">
                            <cwc:ResourceButton ID="btCancelarTheme" runat="server" CssClass="LogicButton_Big" OnClick="btCancelarTheme_Click"
                                        ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                            <cwc:ResourceButton ID="btAceptarTheme" runat="server" CssClass="LogicButton_Big" OnClick="btAceptarTheme_Click"
                                        ResourceName="Controls" VariableName="BUTTON_SAVE" />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ConfiguracionRegional" runat="server">
                    
                    <div class="UserProfileTitle">
                        <asp:Image ID="Image3" runat="server" ImageUrl="~/images/culture.png" ImageAlign="Middle" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="CONFIG_REGIONAL" CssClass="UserName"></cwc:ResourceLabel>
                    </div>
                    
                    <div class="UserProfileContent">
                        <table cellpadding="5" style="width: 100%;">
                            <tr>
                                <td style="padding-left: 50px;">
                                    <cwc:ResourceLabel ID="lblUsoHorario" runat="server" ResourceName="Labels" VariableName="USO_HORARIO"
                                        Font-Bold="true" />
                                </td>
                                <td>
                                    <cwc:TimeZoneDropDownList ID="ddlUsoHorario" runat="server" Width="300px" CssClass="LogicCombo" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-left: 50px;">
                                    <cwc:ResourceLabel ID="lblCultura" runat="server" ResourceName="Labels" VariableName="CULTURA"
                                        Font-Bold="true" />
                                </td>
                                <td>
                                    <cwc:CultureSelector ID="cultureSelector" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left" style="padding-left: 50px;" colspan="2">
                                    <asp:CheckBox ID="cbPredeterminados" runat="server" />
                                    <cwc:ResourceLabel ID="lblCulturaPredeterminada" runat="server" ResourceName="Labels"
                                        VariableName="ESTABLECER_PREDETERMINADO" />
                                </td>
                            </tr>
                            </table>
                        
                        <div class="UserProfileButtons">
                            <cwc:ResourceButton ID="rbCancelConfRegional" runat="server" CssClass="LogicButton_Big"
                                        ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="rbCancelConfRegional_Click" />
                            <cwc:ResourceButton ID="rbAceptConfRegional" runat="server" CssClass="LogicButton_Big"
                                        ResourceName="Controls" VariableName="BUTTON_SAVE" OnClick="rbAceptConfRegional_Click" />
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btAceptarTheme" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
