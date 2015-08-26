<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="ParametrosDispositivoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionParametrosDispositivoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
 
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">

    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_TIPO_PARAMETRO_DISPOSITIVO" TitleResourceName="Labels">
           <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="TIPODISPOSITIVO" />
            <asp:UpdatePanel ID="upTipoDispositivo" runat="server">
                <ContentTemplate>
                    <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="100%" OnInitialBinding="ddlTipoDispositivo_PreBind" />
                </ContentTemplate>
            </asp:UpdatePanel>
           <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="NAME" />
            <asp:TextBox ID="txtNombre" runat="server" Width="100%" />
            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
            <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" />
            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="TIPO_DATO" />
            <asp:TextBox ID="txtTipoDato" runat="server" Width="100%" />
            <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="CONSUMIDOR" />
            <asp:TextBox ID="txtConsumidor" runat="server" Width="100%" MaxLength="1" />
            <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="VALOR_INICIAL" />
            <asp:TextBox ID="txtValorInicial" runat="server" Width="100%" MaxLength="255" />
            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="EDITABLE" />
            <asp:CheckBox ID="chbEditable" runat="server" />
            <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="REQUIRES_RESET" />
            <asp:CheckBox ID="chkReset" runat="server" />
            </cwc:AbmTitledPanel>
        </td>
    </tr>
</table>
</asp:Content>