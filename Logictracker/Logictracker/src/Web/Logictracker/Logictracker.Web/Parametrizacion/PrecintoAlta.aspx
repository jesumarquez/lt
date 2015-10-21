<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.PrecintoAlta" Codebehind="PrecintoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>
