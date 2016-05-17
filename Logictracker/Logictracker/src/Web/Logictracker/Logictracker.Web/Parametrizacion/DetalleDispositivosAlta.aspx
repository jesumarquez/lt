<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionDetalleDispositivosAlta" Title="Dispositivos" Codebehind="DetalleDispositivosAlta.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 25%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels">
                <cwc:ResourceLabel ID="lblType" runat="server" ResourceName="Labels" VariableName="TYPE" />
                <asp:Label ID="lblTipo" runat="server"/>
                <cwc:ResourceLabel ID="lblFirmware" runat="server" ResourceName="Labels" VariableName="FIRMWARE" />
                <asp:Label ID="lblFirm" runat="server" />
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="IMEI" />
                <asp:Label ID="lblIMEI" runat="server"/>
                <cwc:ResourceLabel ID="lblCode" runat="server" ResourceName="Labels" VariableName="CODE" />
                <asp:Label ID="lblCodigo" runat="server"/>
    </cwc:AbmTitledPanel>
    </td>
    <td style="width: 50%; vertical-align: top;">
    </td>
    </tr>
    <tr>
        <td colspan="2">
            <cwc:TitledPanel ID="panDetalles" runat="server" TitleResourceName="Labels" TitleVariableName="PARAMETERS" >
                 <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
                     <ContentTemplate>
                        <C1:C1GridView id="gridParametros" runat="server" Width="100%"  SkinID="SmallGrid" autogeneratecolumns="False"
                            CellPadding="10" OnRowDataBound="grid_ItemDataBound">
                            <Columns>
                                <c1h:C1ResourceBoundColumn DataField="Param" SortDirection="Ascending" ResourceName="Labels" VariableName="PARAMETRO" />
                                <c1h:C1ResourceBoundColumn DataField="Descripcion" ResourceName="Labels" VariableName="DESCRIPCION" />
                                <c1h:C1ResourceBoundColumn DataField="TipoParam" ResourceName="Labels" VariableName="TYPE" />
                                <c1h:C1ResourceBoundColumn DataField="Consumidor" ResourceName="Labels" VariableName="CONSUMIDOR" />
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR">
                                    <ItemTemplate> 
                                        <asp:Label ID="lblValor" runat="server" />
                                        <asp:TextBox ID="txtbValor" runat="server" Width="900px" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                <C1:C1BoundField DataField="IDDetail" Visible="false" />
                            </Columns>
                         </C1:C1GridView>
                     </ContentTemplate>
                     <Triggers>
 <%--                       <asp:AsyncPostBackTrigger ControlID="gridParametros" EventName="SortingCommand" />--%>
                     </Triggers>
                 </asp:UpdatePanel>
               </cwc:TitledPanel>
        </td>
        </tr>
    </table>
</asp:Content>