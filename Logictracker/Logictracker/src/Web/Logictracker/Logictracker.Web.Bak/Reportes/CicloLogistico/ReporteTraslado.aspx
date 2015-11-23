<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ReporteTraslado.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.ReporteTraslado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="135px" />
                <br />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbTransportista" />
                <br />
                <cwc:TransportistasListBox ID="lbTransportista" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbVehiculo" />
                <br />
                <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbTransportista,lbCentroDeCostos,lbSubCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td valign="top" align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="End" />
                <br/>
                <cwc:ResourceCheckBox ID="chkVerDesvios" runat="server" ResourceName="Labels" VariableName="VER_DESVIOS" Font-Bold="True" />
            </td>
            <td>
                <cwc:BaloonTip ID="bProductividad" runat="server" ResourceName="Labels" VariableName="BALOON_PROD_VIAJE" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" runat="server" />
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="server" />
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" runat="server">
    <table id="tbl_totales" runat="server" cellpadding="5" class="abmpanel" style="width: 40%" visible="false">  
        <tr>
            <td class="panelheader" colspan="4">
                <cwc:ResourceLabel ID="lblResTotales" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblTotalRutas" runat="server" ResourceName="Labels" VariableName="TOTAL_RUTAS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblTotalRutas" runat="server" />
            </td>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblKmTotales" runat="server" ResourceName="Labels" VariableName="KM_REALES" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblKmTotales" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblMinutosTotales" runat="server" ResourceName="Labels" VariableName="TIEMPO_REAL" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblTiempoTotal" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblPromedioKm" runat="server" ResourceName="Labels" VariableName="KM_PROMEDIO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblPromedioKm" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblPromedioMin" runat="server" ResourceName="Labels" VariableName="TIEMPO_PROMEDIO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblPromedioMin" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblKmProgramados" runat="server" ResourceName="Labels" VariableName="PROGRAMADOS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblKmProgramados" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblMinProgramados" runat="server" ResourceName="Labels" VariableName="PROGRAMADO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblMinProgramados" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblKmProductivos" runat="server" ResourceName="Labels" VariableName="PRODUCTIVOS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblKmProductivos" runat="server" />
            </td>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblKmImproductivos" runat="server" ResourceName="Labels" VariableName="IMPRODUCTIVOS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblKmImproductivos" runat="server" />
            </td>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>   
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="server" />
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="server" />

