<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ControlDistribucion.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.ControlDistribucion" %>

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
                <cwc:ResourceLinkButton ID="lblCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI37" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbCentroDeCostos" />
                <br />
                <cwc:CentroDeCostosListBox ID="lbCentroDeCostos" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblSubCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI99" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbSubCentroDeCostos" />
                <br />
                <cwc:SubCentroDeCostosListBox ID="lbSubCentroDeCostos" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbVehiculo" />
                <br />
                <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="135px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbTransportista,lbCentroDeCostos,lbSubCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td valign="middle" align="left">
                <cwc:ResourceLabel ID="lblTipoDistribucion" runat="server" ResourceName="Labels" VariableName="TYPE" Font-Bold="true" />
                <br />
                <cwc:TipoDistribucionDropDownList ID="cbTipoDistribucion" runat="server" Width="135px" AddAllItem="true"  />
                <br />
                <cwc:ResourceCheckBox ID="chkVerOrdenManual" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_ORDEN_MANUAL" />
                <br />
                <cwc:ResourceCheckBox ID="chkVerDesvio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_DESVIOS" />
            </td>
            <td valign="top" align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="End" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" runat="server" />
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="server" />
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" runat="server" />
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="server" />
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="server" />

