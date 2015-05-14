<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="ConfiguracionDispositivoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionConfiguracionDispositivoAlta" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    <table style="width: 50%; border-spacing: 10px;">
        <tr>

                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_GENERALES"
                    TitleResourceName="Labels" Height="115px">
                    <cwc:ResourceLabel ID="lblNombre" runat="server" ResourceName="Labels" VariableName="NAME" />
                    <asp:TextBox ID="tbNombre" runat="server" Width="225px" />
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="tbDescripcion" runat="server" Width="225px" />
                    <cwc:ResourceLabel ID="lblOrden" runat="server" ResourceName="Labels" VariableName="ORDER" />
                    <c1:C1NumericInput ID="npOrden" runat="server" MaxValue="999" MinValue="0" Value="0" Width="225px" Height="15px" DecimalPlaces="0" />
                    <cwc:ResourceLabel ID="lblConfiguracion" runat="server" ResourceName="Labels" VariableName="CONFIGURACION" />
                    <asp:TextBox ID="tbConfiguracion" runat="server" Width="100%" Height="500px" TextMode="MultiLine" Wrap="False" />
                </cwc:AbmTitledPanel>
 
        </tr>
    </table>
</asp:Content>

