<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    CodeFile="DistribuidorAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.Postal.DistribuidorAlta" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table>
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_ASIGNACION"
                    TitleResourceName="Labels">
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="USUARIO" />
                    <asp:TextBox ID="txtUsuario" runat="server" Width="100%" autocomplete="off" />
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" Width="100%" />
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="NOMBRE" />
                    <asp:TextBox ID="txtNombre" runat="server" MaxLength="128" Width="100%" />
                    <cwc:ResourceLabel ID="lblPassword" runat="server" ResourceName="Labels" VariableName="PASSWORD" />
                    <div>
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtClave" runat="server" Width="100%" TextMode="Password" Text=""
                                        autocomplete="off" />
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
                    <cwc:ResourceLabel ID="lblConfirmacion" runat="server" ResourceName="Labels" VariableName="CONFIRM_PASSWORD" />
                    <asp:TextBox ID="txtConfirmacion" runat="server" TextMode="Password" Width="150px" />
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
