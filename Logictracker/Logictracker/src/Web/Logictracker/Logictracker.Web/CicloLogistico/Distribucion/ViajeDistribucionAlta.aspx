<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="ViajeDistribucionAlta.aspx.cs" Inherits="Logictracker.CicloLogistico.Distribucion.ViajeDistribucionAlta" %>
<%@ Import Namespace="System.IO" %>

<%@ Import Namespace="Logictracker.CicloLogistico.Distribucion" %>
<%@ Import Namespace="Logictracker.Security" %>
<%@ Register Src="~/App_Controls/EditLine.ascx" TagName="EditLine" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    
    <style type="text/css">
        li{list-style: none; padding: 0px; margin: 0px;}
        ul{padding: 0px;margin: 0px;}
    </style>
    
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="320px">
                        <div id="Div1" runat="server">
                            <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Labels" VariableName="TYPE" />
                            <cwc:BaloonTip ID="hTipo" runat="server" VariableName="TIPOS_DISTRIBUCION" HorizontalAlign="Left" />
                        </div>
                        <div runat="server">
                            <asp:UpdatePanel runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:ResourceButtonList ID="radTipo" runat="server" RepeatDirection="Horizontal" CellSpacing="0" CellPadding="0" AutoPostBack="True" OnSelectedIndexChanged="RadTipoElectedIndexChanged" >
                                        <asp:ListItem VariableName="TIPODISTRI_NORMAL" Value="0" />
                                        <asp:ListItem VariableName="TIPODISTRI_DESORDENADA" Value="1" Selected="true" />
                                        <asp:ListItem VariableName="TIPODISTRI_RECORRIDO_FIJO" Value="2" />
                                    </cwc:ResourceButtonList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AutoPostBack="True" OnSelectedIndexChanged="CbLineaSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblCentroDeCosto" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownList ID="cbCentroDeCosto" AddAllItem="true" runat="server" Width="100%" TabIndex="14" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="CbCentroDeCostoSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblSubCentroDeCosto" runat="server" ResourceName="Entities" VariableName="PARENTI99" />
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:SubCentroDeCostosDropDownList ID="cbSubCentroDeCosto" AddAllItem="true" runat="server" Width="100%" TabIndex="15" ParentControls="cbEmpresa,cbLinea,cbCentroDeCosto" OnSelectedIndexChanged="CbSubCentroDeCostoSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCosto" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownList ID="cbTransportista" AddAllItem="true" runat="server" Width="100%" TabIndex="16" ParentControls="cbEmpresa,cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                        <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbTransportista,cbCentroDeCosto,cbSubCentroDeCosto" OnSelectedIndexChanged="CbVehiculoSelectedIndexChanged" TabIndex="20" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCosto" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbSubCentroDeCosto" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblChofer" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                        <asp:UpdatePanel ID="updChofer" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="cbChofer" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbEmpresa,cbLinea,cbCentroDeCosto,cbTransportista" TabIndex="25" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCosto" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblTipoCiclo" runat="server" ResourceName="Entities" VariableName="PARTICK09" />
                        <asp:UpdatePanel ID="updTipoCiclo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoCicloLogisticoDropDownList ID="cbTipoCicloLogistico" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbEmpresa" TabIndex="26" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE_DISTRIBUCION" />
                        <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
                        
                        <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="DATE" />     
                        <asp:UpdatePanel ID="updFecha" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DateTimePicker ID="dtFecha" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" Visible="True" OnDateChanged="DtFechaDateChanged" AutoPostBack="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbCentroDeCosto" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbSubCentroDeCosto" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblUmbral" runat="server" ResourceName="Labels" VariableName="UMBRAL_SALIDA" />
                        <asp:Panel runat="server">
                            <asp:TextBox ID="txtUmbral" runat="server" Width="10%" MaxLength="3" Text="30" />
                            <cwc:ResourceLabel ID="lblMinutos" runat="server" ResourceName="Labels" VariableName="MINUTOS" />
                        </asp:Panel>
                            
                        <cwc:ResourceLabel ID="lblComentario" runat="server" ResourceName="Labels" VariableName="COMENTARIO" />
                        <asp:TextBox ID="txtComentario" runat="server" Width="98%" MaxLength="128" TextMode="MultiLine" />
                                               
                        <div runat="server"></div>
                        <asp:UpdatePanel runat="server" ChildrenAsTriggers="True" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox runat="server" ID="chkRegresaABase" ResourceName="Labels" VariableName="REGRESA_A_BASE" AutoPostBack="True" OnCheckedChanged="ChkRegresaABaseCheckedChanged"/>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <div runat="server"></div>
                        <cwc:ResourceCheckBox runat="server" ID="chkProgramacionDinamica" ResourceName="Labels" VariableName="PROGRAMACION_DINAMICA" />
                        
                        <div runat="server"></div>
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" ChildrenAsTriggers="True" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:ResourceButton runat="server" ID="btnAnular" ResourceName="Labels" VariableName="ANULAR" AutoPostBack="True" OnClick="BtnAnularClick" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnAnular" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblMotivo" runat="server" ResourceName="Labels" VariableName="MOTIVO_ANULACION" />
                        <asp:TextBox ID="txtMotivo" runat="server" Width="98%" MaxLength="128" TextMode="MultiLine" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Labels" VariableName="FECHA_ALTA" />
                        <asp:Label ID="lblFechaAlta" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel17" runat="server" ResourceName="Labels" VariableName="FECHA_INICIO" />
                        <asp:Label ID="lblInicioReal" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblFechaFin" runat="server" ResourceName="Labels" VariableName="FECHA_FIN" />
                        <cwc:DateTimePicker ID="dtFechaFin" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" AutoPostBack="True" />
                        
                    </cwc:AbmTitledPanel>
                    <br/>
                    <asp:UpdatePanel runat="server" ID="updBtInvertir" ChildrenAsTriggers="true" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TitledPanel ID="panelOpcionesRecorrido" runat="server" TitleVariableName="RECORRIDO" TitleResourceName="Labels" Height="205px" Visible="False">
                                <div class="filterpanel">
                                    <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="USAR_RECORRIDO_PREDEFINIDO" ></cwc:ResourceLabel>
                                    
                                    <cwc:RecorridoDropDownList runat="server" ID="cbRecorrido" ParentControls="cbLinea" Width="200px" />
                                    <cwc:ResourceButton runat="server" ID="btCopiarRecorrido" ResourceName="Labels" VariableName="COPIAR" OnClick="BtCopiarRecorridoClick" CssClass="LogicButton LogicButton_RecoCopy" />
                                    
                                </div>
                                <div class="filterpanel">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="DESVIO" />
                                                <asp:UpdatePanel runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:TextBox id="txtDesvio" runat="server" Width="60px" MaxLength="6" Text="100" />
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="btCopiarRecorrido"/>
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                                <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="METROS" />
                                            </td>
                                            <td style="text-align: right;">
                                                <asp:Button runat="server" ID="btLimpiar" Text="Limpiar" OnClick="BtLimpiarClick" CssClass="LogicButton LogicButton_RecoClear" />
                                                <asp:Button runat="server" ID="btInvertir" Text="Invertir Sentido" OnClick="BtInvertirClick" CssClass="LogicButton LogicButton_RecoInvert" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </cwc:TitledPanel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                            <asp:AsyncPostBackTrigger ControlID="cbLinea"/>
                            <asp:AsyncPostBackTrigger ControlID="radTipo"/>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td style="vertical-align: top; width: 600px;">
                    <div style="position: relative;">
                        <div style="position: absolute; top: 4px; right: 5px; z-index: 999999;" class="LogicButton_Background">
                            <asp:UpdatePanel runat="server" ID="updFiltroEntregas" UpdateMode="Conditional" ChildrenAsTriggers="True">
                                <ContentTemplate>
                                    <cwc:DropDownCheckList runat="server" ID="cbFiltroEntregas" AutoPostBack="true" Width="200px" OnSelectedIndexChanged="CbFiltroEntregasSelectedIndexChanged"></cwc:DropDownCheckList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        
                        <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="RECORRIDO" TitleResourceName="Labels" Height="320px">
                            <uc1:EditLine ID="EditLine1" runat="server" Width="600px" Height="500px" OnMapLoad="EditLine1OnMapLoad" />
                            <asp:UpdatePanel runat="server" ID="updMap" UpdateMode="Conditional">
                                <ContentTemplate>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="EditLine1"  />
                                    <asp:AsyncPostBackTrigger ControlID="cbFiltroEntregas"  />
                                </Triggers>
                            </asp:UpdatePanel>
                        </cwc:AbmTitledPanel>
                    </div>
                </td>
            </tr>
        </table>
        
        <asp:UpdatePanel ID="updasdf" runat="server" >
            <ContentTemplate>
                <asp:Panel ID="PanelModal" runat="server" style="display: none;">
                    <asp:Panel ID="panelReprocesarTicket" runat="server">
                        <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                            <div style="margin-bottom: 10px;text-align: center;">
                                <cwc:ResourceLabel ID="ResourceLabel18" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="RUTA" />:
                                <asp:Label ID="lblRuta" runat="server" />
                            </div>
                            <div style="margin-bottom: 30px;text-align: center;">
                                <cwc:ResourceLabel ID="ResourceLabel19" runat="server" Font-Bold="True" ResourceName="Entities" VariableName="PARENTI03" />:
                                <asp:Label ID="lblVehiculoReprocesar" runat="server" />
                            </div>
                            <div style="margin-bottom: 30px; text-align: left;">
                                <cwc:ResourceLabel ID="ResourceLabel20" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="Inicio" />: 
                                <cwc:DateTimePicker ID="dtRegeneraDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Visible="True" />
                                <br />
                                <cwc:ResourceLabel ID="ResourceLabel21" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="Hasta" />: 
                                <cwc:DateTimePicker ID="dtRegeneraHasta" runat="server" IsValidEmpty="false" TimeMode="End" Mode="DateTime" Visible="True" />
                                <br/>
                                <cwc:ResourceCheckBox ID="chkNoCerrar" runat="server" ResourceName="Labels" VariableName="RECALCULAR_SIN_CERRAR" Font-Bold="true" />
                            </div>
                            <div style="text-align: right;">            
                                <cwc:ResourceButton ID="btRegeneraAceptar" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtRegeneraAceptarClick"  />
                                <cwc:ResourceButton ID="btRegeneraCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                            </div>
                        </div>
                    </asp:Panel>
                </asp:Panel>
                
                <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
        
                <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" CancelControlID="btCancelar" ID="mpePanel" runat="server" BackgroundCssClass="disabled_back" >
                </AjaxToolkit:ModalPopupExtender>
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="ENTREGAS">
        <asp:UpdatePanel ID="pnlExistente" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:Panel runat="server" ID="panelAbrirEntregaExistente">
                    <cwc:ResourceButton CssClass="Grid_Header" BorderStyle="None" runat="server" ID="btEntregaExistente" ResourceName="Controls" VariableName="BUTTON_AGREGAR_PUNTO_EXISTENTE" OnClick="BtEntregaExistenteClick"  />
                </asp:Panel>
                <asp:Panel runat="server" ID="panelEntregaExistente" Visible="False">
                    <div class="PopupPanel">
                        <table style="width: 100%;">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Entities" VariableName="PARENTI18" />
                                </td>
                                <td>
                                    <cwc:ClienteDropDownList runat="server" ID="cbCliente" ParentControls="cbLinea" Width="100%"></cwc:ClienteDropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Entities" VariableName="PARENTI44" />
                                </td>
                                <td>
                                    <asp:UpdatePanel ID="upPuntoEntrega" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cwc:AutoCompleteTextBox ID="cbPuntoEntrega" runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetPuntosEntrega" Width="100%" ParentControls="cbEmpresa,cbLinea,cbCliente" OnSelectedIndexChanged="CbPuntoEntregaSelectedIndexChanged" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtDescripcion" Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <br/>
                        <div style="text-align: right;">
                            <cwc:ResourceButton runat="server" ID="btAcceptEntregaExistente" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtAcceptEntregaExistenteClick" CssClass="LogicButton_Big" />
                            <cwc:ResourceButton runat="server" ID="btCancelEntregaExistente" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelEntregaExistenteClick" CssClass="LogicButton_Big" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btEntregaExistente"/>
                <asp:AsyncPostBackTrigger ControlID="btNuevaEntrega"/>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                <asp:AsyncPostBackTrigger ControlID="cbLinea"/>
            </Triggers>
        </asp:UpdatePanel>
        
        <asp:UpdatePanel ID="pnlNuevaEntrega" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:Panel runat="server" ID="panelAbrirNuevaEntrega">
                    <cwc:ResourceButton CssClass="Grid_Header" BorderStyle="None" runat="server" ID="btNuevaEntrega" ResourceName="Controls" VariableName="BUTTON_AGREGAR_NUEVO_PUNTO" OnClick="BtNuevaEntregaClick"  />
                </asp:Panel>
                <asp:Panel runat="server" ID="panelNuevaEntrega" Visible="False">
                    <div class="PopupPanel">
                        <table width="100%">
                            <tr valign="top">
                                <td width="50%">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="PARENTI18" />
                                            </td>
                                            <td>
                                                <cwc:ClienteDropDownList runat="server" ID="cbClienteNuevo" ParentControls="cbLinea" Width="100%" AutoPostBack="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDescripcionNuevaEntrega" runat="server" Width="100%" MaxLength="128" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblDir" runat="server" ResourceName="Labels" VariableName="DIRECCION" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDireccion" runat="server" Width="100%" MaxLength="128" />
                                            </td>
                                        </tr>
                                         <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblLatitud" runat="server" ResourceName="Labels" VariableName="Latitud" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLatitud" runat="server" Width="50%" MaxLength="128" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <cwc:ResourceLabel ID="lblLongitud" runat="server" ResourceName="Labels" VariableName="Longitud" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtLongitud" runat="server" Width="50%" MaxLength="128" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" align="right">
                                                <cwc:ResourceButton ID="btnBuscarDireccion" runat="server" ResourceName="Labels" VariableName="BUSCAR" OnClick="BtnBuscarDireccionClick" CssClass="LogicButton_Big" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td width="50%">
                                    <table width="100%">
                                        <tr>
                                            <td>
                                                <asp:Panel ID="panelResult" runat="server" DefaultButton="btAceptar">
                                                    <div>
                                                        <asp:Label ID="lblDireccion" runat="server" />
                                                        <asp:ListBox ID="cbResults" runat="server" Width="100%" Height="110px" />
                                                        <div style="text-align: right; padding-top: 5px;">
                                                            <cwc:ResourceButton ID="btAceptar" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="BtAceptarClick" CssClass="LogicButton_Big" />
                                                            <cwc:ResourceButton ID="btCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" OnClick="BtCancelarClick" CssClass="LogicButton_Big" />
                                                        </div>
                                                    </div>
                                                </asp:Panel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btNuevaEntrega"/>
                <asp:AsyncPostBackTrigger ControlID="btEntregaExistente"/>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                <asp:AsyncPostBackTrigger ControlID="cbLinea"/>
            </Triggers>
        </asp:UpdatePanel>
        
        <div style="height: 10px;"></div>
        <div>
            <table class="Grid_Header" style="height: 36px; border-spacing: 0px;">
                <tr>
                    <td style="width: 80px;">&nbsp;</td>
                    <td style="width: 120px;">
                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    </td>
                    <td style="width: 100px;">
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Entities" VariableName="PARENTI44" />
                        (<cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Entities" VariableName="PARENTI18" />)
                    </td>
                    <td style="width: 80px;">
                        <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="DEMORA" />
                    </td>
                    <td style="width: 180px;">
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="PROGRAMADO_ENTRE" />
                    </td>
                    <td style="width: 80px;">
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="ENTRADA" />
                    </td>
                    <td style="width: 80px;">
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="TIME_MANUAL" />
                    </td>
                    <td style="width: 80px;">
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="SALIDA" />
                    </td>
                    <td style="width: 80px;">
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="STATE" />
                    </td>
                    <td style="width: 30px;">&nbsp;</td>
                </tr>
            </table>

            <asp:UpdatePanel ID="updEntregas" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <AjaxToolkit:ReorderList ID="ReorderList1" runat="server" AllowReorder="true" DragHandleAlignment="Left" SortOrderField="Orden" OnItemReorder="ReorderList1ItemReorder" OnItemDataBound="ReorderList1ItemDataBound" PostBackOnReorder="True">
                        <DragHandleTemplate>
                            <div runat="server" id="drag_handle" class="reorder_handle" style="height: 36px;">&nbsp;</div>
                        </DragHandleTemplate>
                        <ItemTemplate>
                            <table class="reorder_item" style="height: 36px;<%# (Container.DataItem as Entrega).Nomenclado ? "" : "background-color: LightCoral" %>">
                                <tr>                                                   
                                    <td style="width: 15px;">
                                        <%# (Container.DataItem as Entrega).Orden %>
                                    </td>
                                    <td style="width: 36px;">
                                        <img src="<%# ResolveUrl(Path.Combine(IconDir, (Container.DataItem as Entrega).Icono)) %>"/>
                                    </td>
                                    <td style="width: 120px;">
                                        <asp:HiddenField ID="hidOrden" runat="server" Value="<%# (Container.DataItem as Entrega).Orden %>" />
                                        <asp:HiddenField ID="hidId" runat="server" Value="<%# (Container.DataItem as Entrega).Id %>" />
                                        <asp:HiddenField ID="hidPuntoEntrega" runat="server" Value="<%# (Container.DataItem as Entrega).PuntoEntrega %>" />
                                        <b><%# (Container.DataItem as Entrega).Descripcion%></b>
                                    </td>
                                    <td style="width: 100px;">
                                        <%# (Container.DataItem as Entrega).Nomenclado ? "" : ("<a href='"+ResolveUrl("~/Parametrizacion/PtoEntregaAlta.aspx?id="+(Container.DataItem as Entrega).PuntoEntrega) + "' target='_blank'>") %>
                                        <%# (Container.DataItem as Entrega).PuntoEntregaDescripcion%> 
                                        (<%# (Container.DataItem as Entrega).ClienteDescripcion%>)
                                        <%# (Container.DataItem as Entrega).Nomenclado ? "" : "</a>" %>
                                    </td>
                                    <td style="width: 80px;">
                                        <cwc:TipoServicioCicloDropDownList ID="cbTipoServicio" runat="server" ParentControls="cbLinea"  AutoPostBack="true" OnSelectedIndexChanged="CbTipoServicioSelectedIndexChanged"></cwc:TipoServicioCicloDropDownList>
                                    </td>
                                    <td style="width: 120px;">
                                        <asp:UpdatePanel runat="server" ID="updDt" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="True">
                                            <ContentTemplate>
                                                <cwc:DateTimePicker ID="dtHoraEstado" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" Visible="True" AutoPostBack="True" OnDateChanged="DtHoraEstadoDateChanged" HideCalendarButton="True" />  -
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td style="width: 60px;">
                                        <asp:UpdatePanel runat="server" ID="updHasta" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="True">
                                            <ContentTemplate>
                                                <cwc:DateTimePicker ID="dtHasta" runat="server" IsValidEmpty="false" Mode="Time" TimeMode="Now" Visible="True" AutoPostBack="True" OnDateChanged="DtHastaDateChanged" HideCalendarButton="True" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td style="width: 80px;">
                                        <%# (Container.DataItem as Entrega).Entrada.HasValue ? (Container.DataItem as Entrega).Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%>
                                    </td>
                                    <td style="width: 80px;">
                                        <%# (Container.DataItem as Entrega).Manual.HasValue ? (Container.DataItem as Entrega).Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%>
                                    </td>
                                    <td style="width: 80px;">
                                        <%# (Container.DataItem as Entrega).Salida.HasValue ? (Container.DataItem as Entrega).Salida.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty%>
                                    </td>
                                    <td style="width: 80px;">
                                        <asp:UpdatePanel runat="server" ID="updEstado" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="True">
                                            <ContentTemplate>
                                                <cwc:EstadoEntregaDistribucionDropDownList ID="cbEstadoEntrega" runat="server" OnSelectedIndexChanged="CbEstadoEntregaOnSelectedIndexChanged" Enabled="True" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                    <td style="width: 30px;">
                                        <asp:ImageButton runat="server" ID="btDelete" ImageUrl="~/images/delete.png" CommandArgument="<%# (Container.DataItem as Entrega).Orden.ToString() %>" OnCommand="BtDeleteCommand"/>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </AjaxToolkit:ReorderList> 
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btNuevaEntrega"/>
                    <asp:AsyncPostBackTrigger ControlID="cbLinea"/>
                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </cwc:AbmTabPanel>
</asp:Content>
