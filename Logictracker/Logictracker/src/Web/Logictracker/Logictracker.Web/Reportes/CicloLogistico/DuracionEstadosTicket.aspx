<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CicloLogistico.Reportes_CicloLogistico_DuracionEstadosTicket" Title="Untitled Page" Codebehind="DuracionEstadosTicket.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <table width="100%" style="font-size: x-small">
            <tr align="left">
                <td>
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AddAllItem="True" AutoPostBack="true" ParentControls="ddlLocacion" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                    <br />
                    <cwc:ClienteDropDownList ID="cbCliente" runat="server" AddAllItem="true" ParentControls="ddlPlanta" Width="200px" OnSelectedIndexChanged="CbCliente_OnSelectedIndexChanged" />
                    <br />
                    <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" />
                    <br />
                    <cwc:PuntoDeEntregaDropDownList ID="cbPuntoEntrega" runat="server" AddAllItem="true" ParentControls="ddlLocacion,ddlPlanta,cbCliente" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AddAllItem="true" ParentControls="ddlLocacion,ddlPlanta" Width="200px" />
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" />
                </td>       
                <td valign="top" align="left">
                    <br />
                    <cwc:ResourceCheckBox ID="chkIncluirCiclo0" runat="server" ResourceName="Labels" VariableName="CICLO_0" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkIncluirObra0" runat="server" ResourceName="Labels" VariableName="OBRA_0" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkIncluirEstados" runat="server" ResourceName="Labels" VariableName="INCLUIR_ESTADOS" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkIncluirSinVehiculo" runat="server" ResourceName="Labels" VariableName="INCLUIR_SIN_VEHICULO" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkIncluirComparacionCarga" runat="server" ResourceName="Labels" VariableName="INCLUIR_COMPARACION_CARGA" />
                </td>   
            </tr>
        </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" Runat="Server">
</asp:Content>

