<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ConsistenciaStock.aspx.cs" Inherits="Logictracker.Reportes.ControlDeCombustible.ControlDeCombustibleConsistenciaStock" Title="Consistencia" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
  
<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="150px" ParentControls="ddlLocation" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
                    <asp:UpdatePanel ID="upTanque" runat="server">
                        <ContentTemplate>
                            <cwc:TanqueDropDownList ID="ddlTanque" AllowBaseBinding="true" AllowEquipmentBinding="false" runat="server" ParentControls="ddlPlanta" Width="150px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTanque" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" Width="90px" />
               </td>
               <td>
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" Width="90px" />
               </td>
            </tr>
        </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <br />
    <C1:C1GridView ID="gridSubTotales" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="10" BorderWidth="1" 
        AllowSorting="false" AllowPaging="false" Visible="false" OnItemDataBound="gridSubtotales_ItemDataBound">
        <Columns>
            <c1h:C1ResourceBoundColumn DataField="Fecha" ResourceName="Labels" VariableName="FECHA" SortDirection="Ascending" DataFormatString="{0:dd/MM/yyyy}" />
            <c1h:C1ResourceBoundColumn DataField="StockInicial" ResourceName="Labels" VariableName="STOCK_INICIAL" DataFormatString="{0:00} lit" />
            <c1h:C1ResourceBoundColumn DataField="Ingresos" HeaderText="Ingreso" ResourceName="Labels" VariableName="INGRESOS"  DataFormatString="{0:00} lit" />
            <c1h:C1ResourceBoundColumn DataField="Egresos" HeaderText="Egreso" ResourceName="Labels" VariableName="EGRESOS" DataFormatString="{0:00} lit"  />
            <c1h:C1ResourceBoundColumn DataField="StockFinal" ResourceName="Labels" VariableName="VARIACION_STOCK" DataFormatString="{0:00} lit" />
            <c1h:C1ResourceBoundColumn DataField="StockSonda" HeaderText="Stock Sonda" ResourceName="Labels" VariableName="VARIACION_SONDA" DataFormatString="{0:00} lit" />
            <c1h:C1ResourceBoundColumn DataField="Diferencia" ResourceName="Labels" VariableName="DIFERENCIA" DataFormatString="{0:00} lit"  />
        </Columns>
    </C1:C1GridView>
</asp:Content>
