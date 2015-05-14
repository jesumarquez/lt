<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="CocheAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.CocheAlta" Title="Coches" enableEventValidation="false" %>
 
<%@ Register src="~/App_Controls/DocumentList.ascx" tagname="DocumentList" tagprefix="uc1" %>
<%@ Register src="~/App_Controls/TicketsList.ascx" tagname="TicketsList" tagprefix="uc2" %>
<%@ Register src="~/App_Controls/ConsumosList.ascx" tagname="ListaConsumos" tagprefix="uc3" %>
<%@ Register src="~/App_Controls/TicketsMantenimientoList.ascx" tagname="TicketsMantenimientoList" tagprefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_VEHICULO" DefaultButton="btDummy" >
        <asp:Button ID="btDummy" runat="server" style="display: none;" OnClientClick="return false;" />
        
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="350px">
                        
                        <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="98%" AddAllItem="true" />
            
                        <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" >
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" ParentControls="ddlEmpresa" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />        
                        <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoDeVehiculoDropDownList ID="cbTipoCoche" runat="server" Width="98%" ParentControls="cbLinea" OnSelectedIndexChanged="CbTipoCocheSelectedIndexChanged" AutoPostBack="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
            
                        <cwc:ResourceLabel ID="lblInterno" runat="server" ResourceName="Labels" VariableName="INTERNO" />
                        <asp:TextBox ID="txtInterno" runat="server" Width="98%" MaxLength="32" />
                        
                        <cwc:ResourceLabel ID="lblPatente" runat="server" ResourceName="Labels" VariableName="PATENTE" />        
                        <asp:TextBox ID="txtPatente" runat="server" Width="98%" />
               
                        <cwc:ResourceLabel ID="lblDepto" runat="server" ResourceName="Entities" VariableName="PARENTI04" />        
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" ParentControls="cbLinea" Width="98%" AddNoneItem="true" OnSelectedIndexChanged="DdlDepartamentoSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                               <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
               
                        <cwc:ResourceLabel ID="lblCentro" runat="server" ResourceName="Entities" VariableName="PARENTI37" />        
                        <asp:UpdatePanel ID="upCentro" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownList ID="ddlCentro" runat="server" ParentControls="cbLinea,cbDepartamento" Width="98%" AddNoneItem="true" OnSelectedIndexChanged="DdlCentroSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblSubCentro" runat="server" ResourceName="Entities" VariableName="PARENTI99" />        
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:SubCentroDeCostosDropDownList ID="ddlSubCentro" runat="server" ParentControls="cbLinea,cbDepartamento,ddlCentro" Width="98%" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlCentro" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
            
                        <cwc:ResourceLabel ID="lblPARENTI07" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" AddAllItem="false" AddNoneItem="true" ParentControls="cbLinea" Width="98%" OnSelectedIndexChanged="DdlTransportistaSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblReferencia" runat="server" ResourceName="Labels" VariableName="REFFERENCE" />
                        <asp:TextBox ID="txtRefference" runat="server" Width="98%" />

                        <cwc:ResourceLabel ID="lblTipoEmpleado" runat="server" ResourceName="Entities" VariableName="PARENTI43" />
                        <asp:UpdatePanel ID="upTipoEmpleado" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoEmpleadoDropDownList ID="cbTipoEmpleado" runat="server" ParentControls="ddlEmpresa, cbLinea" Width="98%" AddAllItem="true" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblChofer" runat="server" ResourceName="Labels" VariableName="RESPONSABLE" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="98%" AddNoneItem="true" AllowOnlyDistrictBinding="true" ParentControls="cbTipoEmpleado,ddlTransportista" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoEmpleado" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
            
                        <cwc:ResourceLabel ID="lblParenti08" runat="server" ResourceName="Entities" VariableName="PARENTI08" />
                        <asp:UpdatePanel ID="upDispositivo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DispositivoDropDownList ID="cbDispositivo" runat="server" Width="98%" OnInitialBinding="CbDispositivoPreBind" ParentControls="cbLinea" AutoPostBack="False" AddNoneItem="True" HideAssigned="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Labels" VariableName="STATE" />
                        <asp:UpdatePanel ID="upEstado" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EstadoChocesDropDownList ID="cbEstados" runat="server" Width="98%" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                       
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:AbmTitledPanel ID="panelDatosVehiculo" runat="server" TitleVariableName="DATOS_VEHICULO" TitleResourceName="Labels"  Height="350px">
                            
                                <cwc:ResourceLabel ID="lblMarca" runat="server" ResourceName="Entities" VariableName="PARENTI06" />        
                                <asp:UpdatePanel ID="upMarca" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <cwc:MarcaDropDownList ID="cbMarca" runat="server" AddAllItem="true" Width="98%" ParentControls="ddlEmpresa, cbLinea" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>                            
                                </asp:UpdatePanel>
                    
                                <cwc:ResourceLabel ID="lblModelo" runat="server" ResourceName="Entities" VariableName="PARENTI61" />
                                <asp:UpdatePanel ID="upModelo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <cwc:ModeloDropDownList ID="cbModelo" runat="server" AddAllItem="false" AddNoneItem="true" Width="98%" ParentControls="ddlEmpresa, cbLinea, cbMarca" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbMarca" EventName="SelectedIndexChanged" />
                                    </Triggers>                            
                                </asp:UpdatePanel>
                        
                                <cwc:ResourceLabel ID="lblAno" runat="server" ResourceName="Labels" VariableName="AÑO" />        
                                <c1:C1NumericInput ID="npAnio" runat="server" Width="100px" Height="17px" NullText="" ShowNullText="true" MaxValue="9999" Increment="1" DecimalPlaces="0" />
                               
                                <cwc:ResourceLabel ID="lblChasis" runat="server" ResourceName="Labels" VariableName="NUMERO_CHASIS" />        
                                <asp:TextBox ID="txtNumeroChasis" runat="server" MaxLength="64" Width="98%" />
                        
                                <cwc:ResourceLabel ID="lblMotor" runat="server" ResourceName="Labels" VariableName="NUMERO_MOTOR" />        
                                <asp:TextBox ID="txtNumeroMotor" runat="server" MaxLength="64" Width="98%" />

                                <cwc:ResourceLabel ID="lblPoliza" runat="server" ResourceName="Labels" VariableName="POLIZA" />
                                <asp:TextBox ID="txtPoliza" runat="server" MaxLength="64" Width="98%" />

                                <cwc:ResourceLabel ID="lblVencimientoPoliza" runat="server" ResourceName="Labels" VariableName="VENCIMIENTO_POLIZA" />
                                <cwc:DateTimePicker ID="dpVencimiento" runat="server" IsValidEmpty="true" Mode="Date" TimeMode="None" />
                            
                                <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="PARENTI18" />
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <cwc:ClienteDropDownCheckList ID="cbCliente" runat="server" ParentControls="cbLinea" Width="98%" AutoPostBack="false" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>

                            </cwc:AbmTitledPanel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbTipoCoche" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <asp:UpdatePanel ID="upReferencias" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:AbmTitledPanel ID="panelValoresReferencia" runat="server" TitleResourceName="Labels" TitleVariableName="VALORES_REFERENCIA" Height="180px">  
                
                                <cwc:ResourceLabel ID="lblVelocidadPromedio" runat="server" ResourceName="Labels" VariableName="VELOCIDAD_PROMEDIO" />                    
                                <c1:C1NumericInput ID="npVelocidadPromedio" runat="server" Width="100px" Height="17px" Value="20" MaxValue="999" Increment="1" DecimalPlaces="0" />
                        
                                <cwc:ResourceLabel ID="lblKilometros" runat="server" ResourceName="Labels" VariableName="KILOMETROS_REFERENCIA" />                    
                                <c1:C1NumericInput ID="npKilometros" runat="server" Width="100px" Height="17px" NullText="" ShowNullText="true" MaxValue="999999" Value="0" Increment="1" DecimalPlaces="0" />
                                        
                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="CAPACIDAD_CARGA" />
                                <c1:C1NumericInput runat="server" ID="txtCapacidad" DecimalPlaces="0" MaxValue="999999" MinValue="0" Width="100px" Height="15px" Value="0" SmartInputMode="true" />
                                        
                                <div></div>
                                <cwc:ResourceCheckBox ID="chkControlaKm" runat="server" ResourceName="Labels" AutoPostBack="true" VariableName="CONTROLA_KM" OnCheckedChanged="ChkChanged"/>
                                <div></div>
                
                                <cwc:ResourceCheckBox ID="chkControlaHs" runat="server" ResourceName="Labels" AutoPostBack="true" VariableName="CONTROLA_HS" OnCheckedChanged="ChkChanged"/>
                                        
                                <div></div>                           
                                <cwc:ResourceCheckBox ID="chkControlaTurnos" runat="server" ResourceName="Labels" Visible="false" VariableName="CONTROLA_TURNOS" />
                                        
                                <div></div>
                                <cwc:ResourceCheckBox ID="chkControlaServicios" runat="server" ResourceName="Labels" Visible="false" VariableName="CONTROLA_SERVICIOS" />
                                
                                <cwc:ResourceLabel ID="lblPorcentaje" runat="server" Visible="false" ResourceName="Labels" VariableName="PORC_PROD" />
                                <c1:C1NumericInput ID="npPorcentaje" runat="server" Width="100px" Visible="false" Height="17px" NullText="" ShowNullText="true" MaxValue="100" Increment="1" DecimalPlaces="0" />
                                
                                <cwc:ResourceLabel ID="lblRendimiento" runat="server" ResourceName="Labels" VariableName="RENDIMIENTO" />
                                <asp:Label ID="txtRendimiento" runat="server" />
                                
                                <cwc:ResourceLabel ID="lblCosto" runat="server" ResourceName="Labels" VariableName="COSTO_KM" />
                                <asp:Label ID="txtCosto" runat="server" />
                                
                                <cwc:ResourceLabel ID="lblInicioActividad" runat="server" ResourceName="Labels" VariableName="INICIO_ACTIVIDAD" />
                                <cwc:DateTimePicker ID="dtInicioActividad" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                        
                            </cwc:AbmTitledPanel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbTipoCoche" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="chkControlaKm" EventName="CheckedChanged" />
                            <asp:AsyncPostBackTrigger ControlID="chkControlaHs" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                
                    <cwc:TitledPanel ID="tpComportamientos" runat="server" TitleResourceName="Labels" TitleVariableName="COMPORTAMIENTOS"  Height="180px">
                        <br />
                        <asp:UpdatePanel ID="updIdentificaChoferes" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkIdentificaChoferes" runat="server" ResourceName="Labels" VariableName="IDENTIFICA_CHOFERES" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <br /><br />
                        <cwc:ResourceCheckBox ID="chkReportaAssistCargo" runat="server" ResourceName="Labels" VariableName="REPORTA_ASSISTCARGO" SecureRefference="ASSISTCARGO" />
                        <br /><br />
                        <asp:UpdatePanel ID="upTelemetria" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkTelemetria" runat="server" ResourceName="Labels" VariableName="UTILIZA_TELEMETRIA" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br /><br />
                        <cwc:ResourceCheckBox ID="chkEsPuerta" runat="server" ResourceName="Labels" VariableName="ES_PUERTA" SecureRefference="VIEW_CONTROL_ACCESO" />
                    </cwc:TitledPanel>
                    
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>

    <cwc:AbmTabPanel ID="abmTabOdometros" runat="server" ResourceName="Labels" VariableName="ODOMETROS" >
        <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="ODOMETROS">
    
            <table cellpadding="5" style="width: 100%">
                <tr>
                    <td>
                        <table style="border-spacing: 5px;">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblOdometroInicial" runat="server" ResourceName="Labels" VariableName="ODOMETRO_INICIAL" Font-Bold="true" />
                                </td>
                                <td>  
                                    <c1:C1NumericInput ID="npOdometroInicial" runat="server" Width="75px" Height="17px" NullText="" ShowNullText="true" MaxValue="999999" Value="0" Increment="1" DecimalPlaces="0" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblOdometroDiario" runat="server" ResourceName="Labels" VariableName="DIARIO" Font-Bold="true" Visible="false" />
                                </td>
                                <td>  
                                    <asp:Label ID="lblDailyOdometer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblOdometroAplicacion" Font-Bold="true" runat="server" ResourceName="Labels" VariableName="ODOMETRO_APLICACION" Visible="false" />
                                </td>
                                <td>  
                                    <asp:Label ID="lblApplicationOdometer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblOdometroParcial" runat="server" ResourceName="Labels" VariableName="ODOMETRO_PARCIAL" Font-Bold="true" Visible="false" />
                                </td>
                                <td>  
                                    <asp:Label ID="lblPartialOdometer" runat="server" />
                                    <cwc:ResourceButton ID="btnReset" runat="server" ResourceName="Controls" VariableName="BUTTON_RESET" Width="75px" 
                                        OnClick="BtnResetClick" Visible="false" Font-Size="X-Small" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblOdometroTotal" runat="server" ResourceName="Labels" VariableName="TOTAL" Font-Bold="true" Visible="false" />
                                </td>
                                <td>  
                                    <asp:Label ID="lblTotalOdometer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblUltimaActualizacion" runat="server" ResourceName="Labels" VariableName="ULTIMA_ACTUALIZACION" Font-Bold="true" Visible="false" />
                                </td>
                                <td>  
                                    <asp:Label ID="lblLastUpdate" runat="server" />
                                </td>
                            </tr>
                        </table>
                        
                        <br />
                        
                        <C1:C1GridView ID="gridOdometros" runat="server" SkinID="SmallGrid" Visible="false" OnRowDataBound="GridOdometrosItemDataBound" OnRowCommand="GridOdometrosItemCommand">
                            <Columns>          
                                <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="DESCRIPCION" DataField="Descripcion" />
                                <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="KILOMETROS" DataField="Kilometros" />
                                <c1h:C1ResourceTemplateColumn ResourceName="Controls" VariableName="AJUSTE_KM">
                                    <ItemTemplate>
                                        <c1:C1NumericInput ID="npAjusteKm" runat="server" Width="75px" Height="17px" NullText="0" ShowNullText="true" MaxValue="999999" MinValue="-999999" Value="0" Increment="1" DecimalPlaces="0" TextAlign="Right" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="DIAS" DataField="Dias" />
                                <c1h:C1ResourceTemplateColumn ResourceName="Controls" VariableName="AJUSTE_DIAS" >
                                    <ItemTemplate>
                                        <c1:C1NumericInput ID="npAjusteDias" runat="server" Width="75px" Height="17px" NullText="0" ShowNullText="true" MaxValue="999999" MinValue="-999999" Value="0" Increment="1" DecimalPlaces="0" TextAlign="Right" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="HS_EN_MARCHA" DataField="Horas" />
                                <c1h:C1ResourceTemplateColumn ResourceName="Controls" VariableName="AJUSTE_HORAS" >
                                    <ItemTemplate>
                                        <c1:C1NumericInput ID="npAjusteHoras" runat="server" Width="75px" Height="17px" NullText="0" ShowNullText="true" MaxValue="999999" MinValue="-999999" Value="0" Increment="1" DecimalPlaces="0" TextAlign="Right" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ULTIMA_ACTUALIZACION" />
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ULTIMO_DISPARO" />
                                <c1h:C1ResourceTemplateColumn>
                                    <ItemTemplate>
                                        <cwc:ResourceLinkButton ID="lnkReset" runat="server" CommandName="Reset" ResourceName="Controls" VariableName="BUTTON_RESET" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                <c1h:C1ResourceTemplateColumn>
                                    <ItemTemplate>
                                        <cwc:ResourceLinkButton ID="lnkActualizar" runat="server" CommandName="Actualizar" ResourceName="Controls" VariableName="BUTTON_REFRESH" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                             </Columns>
                        </C1:C1GridView>                
                    </td>
                </tr>
            </table>
        
        </cwc:TitledPanel>    
    </cwc:AbmTabPanel>

    <cwc:AbmTabPanel ID="abmTabDocumentos" runat="server" ResourceName="Labels" VariableName="DOCUMENTOS_RELACIONADOS">    
        <uc1:DocumentList ID="DocumentList1" runat="server" OnlyForVehicles="true" />  
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="ambTabTickets" runat="server" ResourceName="Labels" VariableName="TICKETS">
        <uc2:TicketsList ID="TicketList" runat="server" />  
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="abmTabTicketsMantenimiento" runat="server" ResourceName="Labels" VariableName="MANTENIMIENTO">
        <uc4:TicketsMantenimientoList ID="TicketsMantenimientoList" runat="server" />  
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="abmTabTurnos" runat="server" ResourceName="Entities" VariableName="PARENTI46" OnSelected="AbmTabTurnosSelected" >
        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <cwc:TitledPanel ID="panelTurnos" runat="server" TitleResourceName="Entities" TitleVariableName="PARENTI46" Visible="false">

                    <C1:C1GridView ID="gridTurnos" runat="server" SkinID="SmallGrid">
                        <Columns>          
                            <c1h:C1ResourceBoundColumn DataField="Codigo" ResourceName="Labels" VariableName="CODE" />
                            <c1h:C1ResourceBoundColumn DataField="Descripcion" ResourceName="Labels" VariableName="DESCRIPCION" />
                            <c1h:C1ResourceBoundColumn DataField="Dias" ResourceName="Labels" VariableName="DIAS" />
                            <c1h:C1ResourceBoundColumn DataField="Desde" ResourceName="Labels" VariableName="DESDE" />
                            <c1h:C1ResourceBoundColumn DataField="Hasta" ResourceName="Labels" VariableName="HASTA" />
                         </Columns>
                    </C1:C1GridView>

                </cwc:TitledPanel> 
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="abmTabConsumos" runat="server" Title="Consumos">        
        <table width="98%">
            <tr>
                <td align="right">
                    <cwc:ResourceLabel ID="lblFechaDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <cwc:DateTimePicker ID="dtFechaDesde" runat="server" Width="105px" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" />
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblFechaHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <cwc:DateTimePicker ID="dtFechaHasta" runat="server" Width="105px" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" />
                    <cwc:ResourceButton ID="btnActualizar" runat="server" OnClick="BtnActualizar_OnClick" ResourceName="Labels" VariableName="ACTUALIZAR" />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:UpdatePanel ID="consumosPanel" runat="server">
                        <ContentTemplate>
                            <uc3:ListaConsumos ID="listConsumos" runat="server" />    
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnActualizar" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>        

    <cwc:AbmTabPanel ID="abmTabCostos" runat="server" Title="Costo" OnSelected="AbmTabCostosSelected">
        <asp:UpdatePanel ID="upCostos" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <table width="100%" border="0">
                    <tr>
                        <td align="center">
                            <C1:C1GridView ID="gridCostos" runat="server" SkinID="ListGridNoGroup" OnRowDataBound="GridCostosItemDataBound" >
                                <Columns>
                                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESCRIPCION"  >
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescripcion" runat="server" />
                                        </ItemTemplate>
                                    </c1h:C1ResourceTemplateColumn>
                                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="MES_CORRIENTE">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCorriente" runat="server" />
                                        </ItemTemplate>
                                    </c1h:C1ResourceTemplateColumn>
                                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ACUMULADO_ANUAL">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcumuladoAnual" runat="server" />
                                        </ItemTemplate>
                                    </c1h:C1ResourceTemplateColumn>
                                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="ACUMULADO_TOTAL">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAcumuladoTotal" runat="server" />
                                        </ItemTemplate>
                                    </c1h:C1ResourceTemplateColumn>
                                </Columns>
                            </C1:C1GridView>
                        </td>
                    </tr>
                </table>            
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>

    
</asp:Content>        