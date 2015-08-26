<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="AccionesAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionAccionAlta" Title="Acciones" %>

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="IconPicker" TagPrefix="uc" %>  
<%@ Register Src="../App_Controls/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" Title="Datos Generales"> 
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server"  Title="Datos Generales" Height="220px">
                
                        <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" />
                
                        <cwc:ResourceLabel ID="lblMensajeBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100%" />
                        <asp:UpdatePanel ID="upMensajeBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="100%" ParentControls="ddlDistrito" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                
                        <cwc:ResourceLabel ID="lblMensaje" runat="server" ResourceName="Entities" VariableName="PAREVEN01" Width="100%" />
                        <asp:UpdatePanel ID="upMensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:MensajesDropDownList ID="ddlMensaje" runat="server" Width="100%" ParentControls="ddlDistrito,ddlBase" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
        
                        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" Width="100%" />
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="64" />

                        <cwc:ResourceLabel ID="lblColor" runat="server" ResourceName="Labels" VariableName="COLOR" Width="100%" />               
                        <asp:UpdatePanel ID="updColor" runat="server" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <uc:ColorPicker ID="cpColor" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                
                        <cwc:ResourceLabel ID="lblAlpha" runat="server" ResourceName="Labels" VariableName="ALPHA" Width="100%" />
                        <c1:C1PercentInput ID="npAlpha" runat="server" DecimalPlaces="0" MaxValue="100" MinValue="0" Value="0" Height="15px" Width="80px" />
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panFiltros" runat="server" TitleResourceName="Labels" TitleVariableName="FILTROS" Height="220px">
                
                        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                        <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="100%" AddAllItem="true" ParentControls="ddlDistrito,ddlBase" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" AddAllItem="true" Width="100%" ParentControls="ddlDistrito,ddlBase" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblDepartamento" runat="server" ResourceName="Entities" VariableName="PARENTI04" />
                        <asp:UpdatePanel ID="upDepartamento" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:DepartamentoDropDownList ID="ddlDepartamento" runat="server" AddAllItem="true" Width="100%" ParentControls="ddlDistrito,ddlBase" OnSelectedIndexChanged="DdlDepartamentoSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="upReportaDepto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblReportaDepartamento" runat="server" ResourceName="Labels" VariableName="REPORTA_RESPONSABLE_PARENTI04" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upChkDepto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:CheckBox ID="chkReportaDepartamento" runat="server"/>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblCentroDeCosto" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                        <asp:UpdatePanel ID="upCentroDeCosto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownList ID="ddlCentroDeCosto" runat="server" AddAllItem="true" Width="100%" ParentControls="ddlDistrito,ddlBase,ddlDepartamento" OnSelectedIndexChanged="DdlCentroDeCostoSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="upReportaCC" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblReportaCentroDeCosto" runat="server" ResourceName="Labels" VariableName="REPORTA_RESPONSABLE_PARENTI37" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlCentroDeCosto" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="upChkCC" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:CheckBox ID="chkReportaCentroDeCosto" runat="server"/>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlCentroDeCosto" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblPOIS" runat="server" ResourceName="Entities" VariableName="PARENTI10" />                    
                        <asp:UpdatePanel ID="upPOI" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoPOI" runat="server" Width="100%" AddAllItem="true" ShowAllTypes="true" RememberSessionValues="false" ParentControls="ddlDistrito,ddlBase" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    
                        <div id="div1" runat="server"></div>
                        <asp:Panel ID="panelGeocerca" runat="server" GroupingText="Geocerca">
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <cwc:ResourceCheckBox ID="chkEvaluaGeocerca" runat="server" ResourceName="Labels" VariableName="ACTION_EVALUATES_GEOFENCE" />
                                    </td>
                                    <td style="text-align: right;">
                                        <cwc:ResourceRadioButton ID="radDentro" runat="server" GroupName="InOut" Checked="true" ResourceName="Labels" VariableName="ACTION_GEOFENCEMODE_INSIDE" />
                                        <cwc:ResourceRadioButton ID="radFuera" runat="server" GroupName="InOut" Checked="false"  ResourceName="Labels" VariableName="ACTION_GEOFENCEMODE_OUTSIDE" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                            <ContentTemplate>
                                                <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoGeocerca" runat="server" Width="100%" AddAllItem="false" ParentControls="ddlDistrito,ddlBase" />
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                    
                            <%--<cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="ACTION_GEOFENCEMODE" />  --%>

                        </asp:Panel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
            <tr>
            <td style="width: 50%; vertical-align: top;">
                <asp:UpdatePanel ID="upComportamiento" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:AbmTitledPanel ID="panComportamiento" runat="server" TitleResourceName="Labels" TitleVariableName="COMPORTAMIENTO" Align="left" Height="285px" >
                
                            <cwc:ResourceCheckBox ID="chkGrabaEnBase" runat="server" Checked="true" ResourceName="Labels" VariableName="GRABA_EN_BASE" />
                            <div></div>
                            
                            <cwc:ResourceCheckBox ID="chkCambiaMensaje" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="CAMBIA_MENSAJE" />                        
                            <asp:Panel ID="panelCambiaMensaje" runat="server" Visible="false">                                                        
                                <asp:TextBox ID="txtMensajeACambiar" runat="server" Width="90%" CssClass="LogicTextbox" />
                                <cwc:ResourceWatermarkExtender ID="ResourceWatermarkExtender1" runat="server" TargetControlID="txtMensajeACambiar" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="MENSAJE_A_CAMBIAR" />
                            </asp:Panel>
                        
                            <cwc:ResourceCheckBox ID="chkPopUp" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="ES_POPUP" />                        
                            <asp:Panel ID="panelPopup" runat="server" Visible="false">
                                <table style="width: 100%">
                                    <tr>
                                        <td style="width: 40px;">
                                            <uc:IconPicker ID="selectIcon" runat="server" ParentControls="ddlDistrito,ddlBase" />
                                        </td>
                                        <td>                                
                                            <asp:TextBox ID="txtTituloPopUp" runat="server" Width="90%" CssClass="LogicTextbox" />
                                            <cwc:ResourceWatermarkExtender ID="wmTituloPopup" runat="server" TargetControlID="txtTituloPopUp" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="POPUP_TITLE" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <cwc:ResourceCheckBox ID="chkRequiereAtencion" runat="server" ResourceName="Labels" VariableName="REQUIERE_ATENCION" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>&nbsp;</td>
                                        <td>
                                            <cwc:ResourceLabel ID="lblPerfil" runat="server" ResourceName="Entities" VariableName="SOCUSUA02" />
                                            <cwc:PerfilDropDownList ID="cbPerfil" runat="server" AddAllItem="true" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                            <cwc:ResourceCheckBox ID="chkAlarmaSonora" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="ES_ALARMA_SONORA" />                       
                            <asp:Panel ID="panelSonido" runat="server" Visible="false">
                                <cwc:ResourceLabel ID="lblSonido" runat="server" ResourceName="Labels" VariableName="SOUND" />
                                <cwc:SonidoDropDownList ID="cbSonido" runat="server" Width="50%" CssClass="LogicCombo" />
                                
                                <img style="vertical-align: middle" alt="Escuchar" src="../images/play.gif" />
                                <cwc:ResourceButton Style="border-right: medium none; border-top: medium none; vertical-align: middle; border-left: medium none; border-bottom: medium none; background-color: transparent"
                                                ID="lnkSonido" OnClick="LnkSonidoClick" runat="server" ResourceName="Controls" VariableName="BUTTON_ESCUCHAR" />
                            </asp:Panel>
                    
                            <cwc:ResourceCheckBox ID="chkEnviaMails" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="ENVIA_MAILS" />
                            <asp:Panel ID="panelMail" runat="server" Visible="false">
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtAsuntoMail" runat="server" Width="90%" CssClass="LogicTextbox" />
                                            <cwc:ResourceWatermarkExtender ID="wmAsuntoMail" runat="server" TargetControlID="txtAsuntoMail" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="SUBJECT" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDestinatariosMails" runat="server" Width="90%" CssClass="LogicTextbox" />
                                            <cwc:ResourceWatermarkExtender ID="wmDestinatariosMails" runat="server" TargetControlID="txtDestinatariosMails" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="DESTINATARIOS" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        
                            <cwc:ResourceCheckBox ID="chkEnviaSMS" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="ENVIA_SMS" />
                            <asp:Panel ID="panelSms" runat="server" Visible="false">
                                <asp:TextBox ID="txtDestinatariosSMS" runat="server" Width="90%" CssClass="LogicTextbox" />
                                <cwc:ResourceWatermarkExtender ID="wmDestinatariosSMS" runat="server" TargetControlID="txtDestinatariosSMS" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="DESTINATARIOS" />
                            </asp:Panel>

                            <cwc:ResourceCheckBox ID="chkHabilita" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="HABILITA_USUARIO"/>
                            <asp:Panel ID="panelHabilita" runat="server" Visible="false">
                                <table style="width: 100%">
                                    <tr>
                                        <td>
                                            <cwc:ResourceLabel ID="lblHorasHabilitado" runat="server" ResourceName="Labels" VariableName="HORAS_HABILITADO" />
                                            <c1:C1NumericInput ID="npHorasHabilitado" runat="server" NullText="" ShowNullText="true" MaxValue="200" Value="200" DecimalPlaces="0" Width="80px" Height="17px" CssClass="LogicTextbox"/>
                                        </td>
                                        <td>
                                            <cwc:ResourceLabel ID="lbUsuarioHabilitado" runat="server" ResourceName="Entities" VariableName="USUARIO" />
                                            <cwc:UsuarioDropDownList ID="ddlUsuarioHabilitado" runat="server" Width="90%" CssClass="LogicCombo" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>                    

                            <cwc:ResourceCheckBox ID="chkInhabilita" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="INHABILITA_USUARIO" />
                            <asp:Panel ID="panelInhabilita" runat="server" Visible="false">
                                <cwc:ResourceLabel ID="lblUsuarioInhabilitado" runat="server" ResourceName="Entities" VariableName="USUARIO" />
                                <cwc:UsuarioDropDownList ID="ddlUsuarioInhabilitado" runat="server" Width="90%" CssClass="LogicCombo" />
                            </asp:Panel>
                        
                            <cwc:ResourceCheckBox ID="chkChangeIcon" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="CHANGE_ICON" />
                            <asp:Panel ID="panelChangeIcon" runat="server" Visible="false">
                                <cwc:ResourceLabel ID="lblNewIcon" runat="server" ResourceName="Labels" VariableName="ICON" />
                                <uc:IconPicker ID="isChangeIcon" runat="server" ParentControls="ddlDistrito,ddlBase" />
                            </asp:Panel>
                        
                            <cwc:ResourceCheckBox ID="chkPideFoto" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="PIDE_FOTO" />                        
                            <asp:Panel ID="panelPideFoto" runat="server" Visible="false">                                                        
                                <asp:TextBox ID="txtSegundosFoto" runat="server" Width="90%" MaxLength="3" CssClass="LogicTextbox" />
                                <cwc:ResourceWatermarkExtender ID="ResourceWatermarkExtender2" runat="server" TargetControlID="txtSegundosFoto" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="SEGUNDOS_FOTO" />
                            </asp:Panel> 
                        
                            <cwc:ResourceCheckBox ID="chkAssistCargo" runat="server" AutoPostBack="true" OnCheckedChanged="ChkComportamientoCheckedChanged" ResourceName="Labels" VariableName="REPORTA_ASSISTCARGO" SecureRefference="ASSISTCARGO" />
                            <cwc:SecuredPanel ID="panelAssistCargo" runat="server" Visible="false" SecureRefference="ASSISTCARGO">                                                        
                                <asp:TextBox ID="txtCodigoAssistCargo" runat="server" Width="90%" MaxLength="3" CssClass="LogicTextbox" />
                                <cwc:ResourceWatermarkExtender ID="wexCodigoAssistCargo" runat="server" TargetControlID="txtCodigoAssistCargo" WatermarkCssClass="LogicWatermark" ResourceName="Labels" VariableName="CODIGO_ASSISTCARGO" />
                            </cwc:SecuredPanel> 
                                         
                        </cwc:AbmTitledPanel>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPopUp" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkEnviaSMS" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkEnviaMails" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkAlarmaSonora" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkHabilita" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkInhabilita" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkChangeIcon" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkCambiaMensaje" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkPideFoto" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:TitledPanel ID="panCondicion" runat="server" TitleResourceName="Labels" TitleVariableName="CONDICION" Height="285px">
                        <table style="width: 100%">
                            <tr>
                                <td colspan="2">
                                    <asp:UpdatePanel ID="upCondition" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:TextBox ID="txtCondicion" runat="server" TextMode="MultiLine" Width="99%" Rows="12" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="btnExceso" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnDuracion" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnAnd" EventName="Click" />
                                            <asp:AsyncPostBackTrigger ControlID="btnOr" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 50%" align="left">
                                    <cwc:ResourceButton ID="btnExceso" runat="server" OnClick="BtnExcesoClick" ResourceName="Labels" VariableName="EXCESO" />
                                    <cwc:ResourceButton ID="btnDuracion" runat="server" OnClick="BtnDuracionClick" ResourceName="Labels" VariableName="DURACION" />
                                </td>
                                <td style="width: 50%" align="right">
                                    <cwc:ResourceButton ID="btnAnd" runat="server" OnClick="BtnAndClick" ResourceName="Labels" VariableName="AND" />
                                    <cwc:ResourceButton ID="btnOr" runat="server" OnClick="BtnOrClick" ResourceName="Labels" VariableName="OR" />
                                </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>