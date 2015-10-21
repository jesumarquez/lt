<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.ParametrizacionCaudalimetroAlta" Codebehind="CaudalimetroAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <asp:UpdatePanel ID="upAlarmas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:AbmTitledPanel ID="panAlarmas" runat="server" TitleResourceName="Labels" TitleVariableName="CONFIGURACION_ALARMAS">
                            <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                            <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="50" />
                            <cwc:ResourceLabel ID="lblCaudalMAX" runat="server" ResourceName="Labels" VariableName="CAUDAL_MAXIMO" />
                            <c1:C1NumericInput runat="server" ID="npCaudalMaximo" Width="150px" MaxValue="999999"
                                NullText="" ShowNullText="true" DecimalPlaces="0" Height="17px" />
                            <cwc:ResourceLabel ID="lblTiempoSinReportar" runat="server" ResourceName="Labels"
                                VariableName="TIEMPO_SIN_REPORTAR" />
                            <c1:C1NumericInput runat="server" ID="npSinReportar" Width="150px" MaxValue="999999"
                                NullText="" ShowNullText="true" DecimalPlaces="0" Height="17px" />
                        </cwc:AbmTitledPanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
            </td>
        </tr>
    </table>
</asp:Content>
