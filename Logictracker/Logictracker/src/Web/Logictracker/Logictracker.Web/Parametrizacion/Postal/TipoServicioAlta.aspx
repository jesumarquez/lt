<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.Postal.TipoServicioAlta" Title="Untitled Page" Codebehind="TipoServicioAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 50%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_ASIGNACION"
                    TitleResourceName="Labels" Height="115px">
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="200px" MaxLength="50" />
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescri" runat="server" Width="200px" MaxLength="50" />
                    <cwc:ResourceLabel ID="lblDescripcionCorta" runat="server" ResourceName="Labels"
                        VariableName="DESCRIPCION_CORTA" />
                    <asp:TextBox ID="txtDescriCorta" runat="server" Width="200px" MaxLength="5" />
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="CON_VALIDACION" />
                    <cwc:TipoValidacionDropDownList ID="ddlValidacion" runat="server" Width="200px" />
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CON_FOTO" />
                    <cwc:TipoValidacionDropDownList ID="ddlFoto" runat="server" Width="200px" />
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="CON_GPS" />
                    <cwc:TipoValidacionDropDownList ID="ddlGPS" runat="server" Width="200px" />
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="CON_LATERALES" />
                    <cwc:TipoValidacionDropDownList ID="ddlLaterales" runat="server" Width="200px" />
                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="CON_REFERENCIA" />
                    <cwc:TipoValidacionDropDownList ID="ddlReferencia" runat="server" Width="200px" />
                </cwc:AbmTitledPanel>
                <%-- Hack anti-postback--%>
                <asp:UpdatePanel ID="upHack" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlValidacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlFoto" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlGPS" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlLaterales" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlReferencia" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
