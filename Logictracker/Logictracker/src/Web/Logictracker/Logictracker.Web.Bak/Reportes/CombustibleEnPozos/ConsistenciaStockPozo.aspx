<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ConsistenciaStockPozo.aspx.cs" 
         Inherits="Logictracker.Reportes.CombustibleEnPozos.ReportesCombustibleEnPozosConsistenciaStockPozo" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
<table width="100%">
    <tr>
        <td>
            <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
            <br />
            <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="200px" AddAllItem="true" />
            <br />
            <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
            <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" Width="200px" ParentControls="ddlLocation" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td>
            <cwc:ResourceLabel ID="lblEquipo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI19" />
            <asp:UpdatePanel ID="upEquipo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <cwc:EquipoDropDownList runat="server" AddAllItem="true" ID="ddlEquipo" Width="200px" ParentControls="ddlPlanta" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
            <asp:UpdatePanel ID="upTanque" runat="server">
                <ContentTemplate>
                    <cwc:TanqueDropDownList ID="ddlTanque" runat="server" ParentControls="ddlEquipo"
                                     AllowBaseBinding="false" AllowEquipmentBinding="true"  Width="200px" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td>
            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
            <br /> 
            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" Width="75px" />
            <br />
            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
            <br />
            <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" Width="75px" />
       </td>
    </tr>
</table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
   <br />
    <C1:C1GridView ID="gridSubTotales" runat="server" Width="100%" AutoGenerateColumns="False" CellPadding="10" BorderWidth="1" 
        OnItemDataBound="gridSubtotales_ItemDataBound" AllowSorting="false" AllowPaging="false" Visible="false" GridLines="None">
        <Columns>
            <c1h:C1ResourceBoundColumn DataField="Fecha" ResourceName="Labels" VariableName="FECHA" SortDirection="Ascending" />
            <c1h:C1ResourceBoundColumn DataField="StockInicial" ResourceName="Labels" VariableName="STOCK_INICIAL" DataFormatString="{0:00}" />
            <c1h:C1ResourceBoundColumn DataField="Ingresos" ResourceName="Labels" VariableName="INGRESOS" />
            <c1h:C1ResourceBoundColumn DataField="IngresosPorConciliacion" ResourceName="Labels" VariableName="INGRESOS_CONCILIACION" />
            <c1h:C1ResourceBoundColumn DataField="Egresos" ResourceName="Labels" VariableName="EGRESOS" />
            <c1h:C1ResourceBoundColumn DataField="EgresosPorConciliacion" ResourceName="Labels" VariableName="EGRESOS_CONCILIACION" />
            <c1h:C1ResourceBoundColumn DataField="StockFinal" ResourceName="Labels" VariableName="VARIACION_STOCK"/>
            <c1h:C1ResourceBoundColumn DataField="StockSonda" ResourceName="Labels" VariableName="VARIACION_SONDA" DataFormatString="{0:00}" />
            <c1h:C1ResourceBoundColumn DataField="DiferenciaDeStock" ResourceName="Labels" VariableName="DIFERENCIA" />
        </Columns>
    </C1:C1GridView>
</asp:Content>

