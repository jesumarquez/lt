<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.Parametrizacion_FirmwareAlta"
    MasterPageFile="~/MasterPages/AbmPage.master" Title="Firmware Alta - Modificación" Codebehind="FirmwareAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 50%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_GENERALES"
                    TitleResourceName="Labels" Height="115px">
                    <cwc:ResourceLabel ID="lblNombre" runat="server" ResourceName="Labels" VariableName="NAME" />
                    <asp:TextBox ID="tbNombre" runat="server" Width="225px" />
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="tbDescripcion" runat="server" Width="225px" />
                    <cwc:ResourceLabel ID="lblFirma" runat="server" ResourceName="Labels" VariableName="FIRMA" />
                    <asp:TextBox ID="tbFirma" runat="server" Width="225px" />
                    <cwc:ResourceLabel ID="lblBinario" runat="server" ResourceName="Labels" VariableName="BINARIO" />
                    <asp:FileUpload ID="fuBinario" runat="server" />
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
