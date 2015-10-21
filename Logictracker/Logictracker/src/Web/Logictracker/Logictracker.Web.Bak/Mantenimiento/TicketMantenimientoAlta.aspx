<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TicketMantenimientoAlta.aspx.cs" Inherits="Logictracker.Mantenimiento.TicketMantenimientoAlta" ValidateRequest="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    <%--TOOLBAR--%>
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
        <table id="tbTipoPunto" style="width: 98%; margin: auto; margin-top: 10px;" cellpadding="5">
            <tr>
                <td style="vertical-align: top; width: 100%;">
                    <cwc:TitledPanel ID="titPanelIncidencia" runat="server" Title="Incidencia" Height="150px">
                        <asp:UpdatePanel ID="updProblema" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblTaller" runat="server" ResourceName="Entities" VariableName="PARENTI35" />
                                        </td>
                                        <td>
                                            <cwc:TallerDropDownList ID="cbTaller" runat="server" Width="130px" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtInicio" runat="server" IsValidEmpty="false" Mode="DateTime" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtCodigo" runat="server" Width="125px" MaxLength="16" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Labels" VariableName="TICKET_MANT_ESTADO" />
                                        </td>
                                        <td>
                                            <cwc:EstadoTicketMantenimientoDropDownList ID="cbEstado" runat="server" Width="130px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                        </td>
                                        <td>
                                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="130px" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="updTipoVehiculo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="130px" ParentControls="cbEmpresa" AddAllItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />                            
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="updVehiculo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="130px" ParentControls="cbEmpresa,cbTipoVehiculo" AddNoneItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                    <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblResponsable" runat="server" ResourceName="Labels" VariableName="RESPONSABLE" />
                                        </td>
                                        <td>
                                            <asp:UpdatePanel ID="upEmpleado" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="130px" ParentControls="cbEmpresa" AddNoneItem="true" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblNivel" runat="server" ResourceName="Labels" VariableName="NIVEL_COMPLEJIDAD" />
                                        </td>
                                        <td>
                                            <cwc:NivelComplejidadDropDownList ID="cbNivel" runat="server" width="130px" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFechaTurno" runat="server" ResourceName="Labels" VariableName="FECHA_TURNO" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtTurno" runat="server" Mode="DateTime" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFechaRecepcion" runat="server" ResourceName="Labels" VariableName="FECHA_RECEPCION" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtRecepcion" runat="server" Mode="DateTime" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblVerificacion" runat="server" ResourceName="Labels" VariableName="FECHA_VERIFICACION" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtVerificacion" runat="server" Mode="DateTime" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFechaTrabajoTerminado" runat="server" ResourceName="Labels" VariableName="FECHA_TRABAJO_TERMINADO" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtTrabajoTerminado" runat="server" Mode="DateTime" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFechaEntrega" runat="server" ResourceName="Labels" VariableName="FECHA_ENTREGA" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtEntrega" runat="server" Mode="DateTime" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblFechaTrabajoAceptado" runat="server" ResourceName="Labels" VariableName="FECHA_TRABAJO_ACEPTADO" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtTrabajoAceptado" runat="server" Mode="DateTime" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td rowspan="2">
                                            <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                        </td>
                                        <td colspan="5" rowspan="2">
                                            <asp:TextBox ID="txtDescripcion" runat="server" Rows="4" TextMode="MultiLine" Width="98%" />
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblEntrada" runat="server" ResourceName="Labels" VariableName="ENTRADA" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtEntrada" runat="server" Enabled="false" Width="100px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblSalida" runat="server" ResourceName="Labels" VariableName="SALIDA" /> 
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtSalida" runat="server" Enabled="false" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%;">
                    <asp:UpdatePanel ID="upGridEstados" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TitledPanel ID="TitledPanel1" runat="server"  Title="Historia" ScrollBars="Auto">
                                <C1:C1GridView ID="gridEstados" DataKeyField="Id" runat="server" Width="100%" CellPadding="10" 
                                GridLines="Horizontal" OnRowDataBound="GridEstadosItemDataBound" AutoGenerateColumns="False" SkinID="SmallGrid">
                                    <Columns>
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="MODIFICADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Entities" VariableName="SOCUSUA01" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="ESTADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="NIVEL_COMPLEJIDAD" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_TURNO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_RECEPCION" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_VERIFICACION" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_TRABAJO_TERMINADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_ENTREGA" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_TRABAJO_ACEPTADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="DESCRIPCION" />
                                    </Columns>
                                </C1:C1GridView>
                            </cwc:TitledPanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="abmTabPresupuesto" runat="server" ResourceName="Labels" VariableName="PRESUPUESTO" >
        <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="DETALLE">
            <table cellpadding="5" style="width: 100%">
                <tr>
                    <td>
                        <table style="border-spacing: 5px;">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblPresupuesto" runat="server" ResourceName="Labels" VariableName="NRO_PRESUPUESTO" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPresupuesto" runat="server" Width="125px" MaxLength="16" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblMonto" runat="server" ResourceName="Labels" VariableName="MONTO" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMonto" runat="server" Width="125px" MaxLength="16" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblFechaPresupuesto" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                </td>
                                <td>
                                    <cwc:DateTimePicker ID="dtPresupuesto" runat="server" Mode="DateTime" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblEstadoPresupuesto" runat="server" ResourceName="Labels" VariableName="ESTADO" />
                                </td>
                                <td>
                                    <cwc:EstadoPresupuestoDropDownList ID="cbEstadoPresupuesto" runat="server" Width="130px" AutoPostBack="False" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblPrimerPresupuesto" runat="server" ResourceName="Labels" VariableName="PRIMER_PRESUPUESTO" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPrimerPresupuesto" runat="server" Width="125px" MaxLength="16" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblFechaPrimerPresupuesto" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                </td>
                                <td>
                                    <cwc:DateTimePicker ID="dtPrimerPresupuesto" runat="server" Mode="DateTime" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblFechaRecotizacion" runat="server" ResourceName="Labels" VariableName="FECHA_RECOTIZACION" />
                                </td>
                                <td>
                                    <cwc:DateTimePicker ID="dtRecotizacion" runat="server" Mode="DateTime" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblFechaAprobacion" runat="server" ResourceName="Labels" VariableName="FECHA_APROBACION" />
                                </td>
                                <td>
                                    <cwc:DateTimePicker ID="dtAprobacion" runat="server" Mode="DateTime" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                <td style="vertical-align: top; width: 100%;">
                    <asp:UpdatePanel ID="updHistoriaPresupuesto" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TitledPanel ID="TitledPanel3" runat="server" Title="Historia" ScrollBars="Auto">
                                <C1:C1GridView ID="gridPresupuesto" DataKeyField="Id" runat="server" Width="100%" CellPadding="10" 
                                GridLines="Horizontal" OnRowDataBound="GridPresupuestoItemDataBound" AutoGenerateColumns="False" SkinID="SmallGrid">
                                    <Columns>
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="MODIFICADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Entities" VariableName="SOCUSUA01" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="NRO_PRESUPUESTO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="MONTO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="ESTADO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="PRIMER_PRESUPUESTO" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_RECOTIZACION" />
                                        <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="FECHA_APROBACION" />
                                    </Columns>
                                </C1:C1GridView>
                            </cwc:TitledPanel>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            </table>
        </cwc:TitledPanel>    
    </cwc:AbmTabPanel>
</asp:Content>

