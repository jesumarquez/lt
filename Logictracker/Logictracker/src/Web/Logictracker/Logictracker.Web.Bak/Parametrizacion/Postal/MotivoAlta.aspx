<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="MotivoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.Postal.MotivoAlta" Title="Untitled Page" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table>
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_ASIGNACION" TitleResourceName="Labels">
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" autocomplete="off" />
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="100%" autocomplete="off" />
                    <cwc:ResourceLabel ID="lblOrden" runat="server" ResourceName="Labels" VariableName="ORDER" />
                    <c1:C1NumericInput ID="npOrden" runat="server" Height="17px" DecimalPlaces="0" MinValue="0"/>
                    <div /><cwc:ResourceCheckBox ID="cbEntrega" runat="server" ResourceName="Labels" VariableName="ES_ENTREGA" />
                    <div /><cwc:ResourceCheckBox ID="cbDevolucion" runat="server" ResourceName="Labels" VariableName="ES_DEVOLUCION" />
                    <div /><cwc:ResourceCheckBox ID="cbGestion" runat="server" ResourceName="Labels" VariableName="ES_GESTION" />
                </cwc:AbmTitledPanel>
                <AjaxToolkit:MutuallyExclusiveCheckBoxExtender ID="mutexAction" runat="server" Key="action" TargetControlID="cbEntrega" />
                <AjaxToolkit:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender1" runat="server" Key="action" TargetControlID="cbDevolucion" />
                <AjaxToolkit:MutuallyExclusiveCheckBoxExtender ID="MutuallyExclusiveCheckBoxExtender2" runat="server" Key="action" TargetControlID="cbGestion" />
            </td>
        </tr>
    </table>
</asp:Content>
