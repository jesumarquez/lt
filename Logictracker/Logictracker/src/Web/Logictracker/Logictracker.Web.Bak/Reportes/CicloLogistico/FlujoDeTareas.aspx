<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="FlujoDeTareas.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.FlujoDeTareas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="130px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbTransportista" />
                <br />
                <cwc:TransportistasListBox ID="lbTransportista" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblDepartamento" runat="server" ResourceName="Entities" VariableName="PARENTI04" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbDepartamento" />
                <br />
                <cwc:DepartamentoListBox ID="lbDepartamento" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblCentroDeCosto" runat="server" ResourceName="Entities" VariableName="PARENTI37" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbCentroDeCosto" />
                <br />
                <cwc:CentroDeCostosListBox ID="lbCentroDeCosto" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblSubProyecto" runat="server" ResourceName="Entities" VariableName="PARENTI99" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbSubCentroDeCosto" />
                <br />
                <cwc:SubCentroDeCostosListBox ID="lbSubCentroDeCosto" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento,lbCentroDeCosto" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbVehiculo" />
                <br />
                <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento,lbCentroDeCosto,lbSubCentroDeCosto,lbTransportista" SelectionMode="Multiple" />
            </td>
            <td valign="top" align="left">
                <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="FECHA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFecha" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                <br/>
                <cwc:ResourceCheckBox ID="chkVerEventos" runat="server" ResourceName="Labels" VariableName="VER_EVENTOS" Font-Bold="True" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" runat="server" />
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="server" />
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" runat="server">
    <table width="100%">
        <tr>
            <td align="center" valign="top" style="width: 50%">
                <table id="tbl_totales" runat="server" cellpadding="5" class="abmpanel" style="width: 80%" visible="false">  
                    <tr>
                        <td class="panelheader" colspan="5">
                            <cwc:ResourceLabel ID="lblResTotales" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <cwc:ResourceLabel ID="lblResTotal" runat="server" ResourceName="Labels" VariableName="TOTAL" Font-Bold="True" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPorc" runat="server" Text="%" Font-Bold="True" /> 
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="lblResCantidad" runat="server" ResourceName="Labels" VariableName="CANTIDAD" Font-Bold="True" /> 
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="lblResPromedio" runat="server" ResourceName="Labels" VariableName="PROMEDIO" Font-Bold="True" /> 
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel runat="server" ID="lblResKmTotales" ResourceName="Labels" VariableName="KM_TOTALES" Font-Bold="True" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalKm" runat="server" /> 
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblCantKm" />
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPromKm" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel runat="server" ID="lblResTiempoTotales" ResourceName="Labels" VariableName="TIEMPO_TOTAL" Font-Bold="True" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalTiempo" runat="server" /> 
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblCantTiempo" />
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblPromTiempo" />
                        </td>
                    </tr>
                    <tr style="background-color: yellow">
                        <td>
                            <cwc:ResourceLabel runat="server" ID="lblResTotalTraslados" ResourceName="Labels" VariableName="TOTAL_TRASLADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalTraslados" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPorcTraslados" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblCantTraslados" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPromTraslados" runat="server" /> 
                        </td>
                    </tr>
                    <tr style="background-color: orangered">
                        <td>
                            <cwc:ResourceLabel runat="server" ID="lblResTotalParadas" ResourceName="Labels" VariableName="TOTAL_INACTIVIDAD" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalParadas" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPorcParadas" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblCantParadas" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPromParadas" runat="server" /> 
                        </td>
                    </tr>
                            <tr style="background-color: lightgreen">
                        <td>
                            <cwc:ResourceLabel runat="server" ID="lblResTotalDespacho" ResourceName="Labels" VariableName="TOTAL_TAREAS" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotalDespachos" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPorcDespachos" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblCantDespachos" runat="server" /> 
                        </td>
                        <td>
                            <asp:Label ID="lblPromDespachos" runat="server" /> 
                        </td>
                    </tr>
                </table>
            </td>
            <td align="center" valign="top" style="width: 50%">
                <table id="tbl2" runat="server" cellpadding="5" class="abmpanel" style="width: 70%" visible="false">  
                    <tr>
                        <td class="panelheader" colspan="3">
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="rlblTotal" runat="server" ResourceName="Labels" VariableName="TOTAL" Font-Bold="True" />
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="%" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="rlblEntregas" runat="server" ResourceName="Labels" VariableName="ENTREGAS" Font-Bold="True" />
                        </td>
                        <td>
                            <asp:Label ID="lblTotal" runat="server" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr style="background-color: chartreuse">
                        <td>
                            <cwc:ResourceLabel ID="rlblRealizadas" runat="server" ResourceName="Labels" VariableName="REALIZADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantRealizadas" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcRealizadas" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #008000">
                        <td>
                            <cwc:ResourceLabel ID="rlblCompletadas" runat="server" ResourceName="Labels" VariableName="COMPLETADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantCompletadas" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcCompletadas" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #FFFF00">
                        <td>
                            <cwc:ResourceLabel ID="rlblVisitadas" runat="server" ResourceName="Labels" VariableName="VISITADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantVisitadas" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcVisitadas" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #00A2E8">
                        <td>
                            <cwc:ResourceLabel ID="rlblEnSitio" runat="server" ResourceName="Labels" VariableName="EN_SITIO" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantEnSitio" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcEnSitio" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: grey;">
                        <td>
                            <cwc:ResourceLabel ID="rlblEnZona" runat="server" ResourceName="Labels" VariableName="EN_ZONA" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantEnZona" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcEnZona" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #FF0000">
                        <td>
                            <cwc:ResourceLabel ID="rlblNoCompletadas" runat="server" ResourceName="Labels" VariableName="NO_COMPLETADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantNoCompletadas" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcNoCompletadas" runat="server" />
                        </td>
                    </tr>
                    <tr style="background-color: #FF4500">
                        <td>
                            <cwc:ResourceLabel ID="rlblNoVisitadas" runat="server" ResourceName="Labels" VariableName="NO_VISITADOS" />
                        </td>
                        <td>
                            <asp:Label ID="lblCantNoVisitadas" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblPorcNoVisitadas" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="server" />
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="server" />

