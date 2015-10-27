<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CicloLogistico.ResumenDeRutas" Codebehind="ResumenDeRutas.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td valign="top">
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="130px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion" AddAllItem="true" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbTransportista" />
                <br />
                <cwc:TransportistasListBox ID="lbTransportista" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblDepartamento" runat="server" ResourceName="Entities" VariableName="PARENTI04" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbDepartamento" />
                <br />
                <cwc:DepartamentoListBox ID="lbDepartamento" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI37" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbCentroDeCostos" />
                <br />
                <cwc:CentroDeCostosListBox ID="lbCentroDeCostos" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento" SelectionMode="Multiple" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblSubCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI99" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbSubCentroDeCostos" />
                <br />
                <cwc:SubCentroDeCostosListBox ID="lbSubCentroDeCostos" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento,lbCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbVehiculo" />
                <br />
                <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbTransportista,lbDepartamento,lbCentroDeCostos,lbSubCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblRuta" runat="server" ResourceName="Labels" VariableName="RUTA" Font-Bold="true" />
                <br />
                <asp:TextBox ID="txtRuta" runat="server" Width="90px" />
                <br/>
                <cwc:ResourceCheckBox ID="chkVerRecepcion" runat="server" ResourceName="Labels" VariableName="VER_RECEPCION" Font-Bold="True" />
                <br/>
                <cwc:ResourceCheckBox ID="chkVerKm" runat="server" ResourceName="Labels" VariableName="VER_KM" Font-Bold="True" />
                </td>
            <td valign="top" align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" />
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
            <td>
                <cwc:ResourceLabel ID="rlblTotalVehiculos" runat="server" ResourceName="Menu" VariableName="PAR_VEHICULOS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblTotalVehiculos" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblRutasConVehiculo" runat="server" ResourceName="Labels" VariableName="CON_VEHICULO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasConVehiculo" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblRutasSinVehiculo" runat="server" ResourceName="Labels" VariableName="SIN_VEHICULO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasSinVehiculo" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblRutasIniciadas" runat="server" ResourceName="Labels" VariableName="INICIADAS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasIniciadas" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblRutasSinIniciar" runat="server" ResourceName="Labels" VariableName="SIN_INICIAR" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasSinIniciar" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblRutasFinalizadas" runat="server" ResourceName="Labels" VariableName="FINALIZADAS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasFinalizadas" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblRutasEnCurso" runat="server" ResourceName="Labels" VariableName="EN_CURSO" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblRutasEnCurso" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>   
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="server" />
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="server" />

