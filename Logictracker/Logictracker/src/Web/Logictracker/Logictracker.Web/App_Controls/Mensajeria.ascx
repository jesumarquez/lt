<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Mensajeria.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_Mensajeria" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<div id="mensajeria">
<asp:UpdatePanel ID="UpdatePanelMensajeMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
    <ContentTemplate>
        <cwc:MovilListBox ID="cbVehiculoMensaje" runat="server" Width="100%" UseOptionGroup="true" OptionGroupProperty="TipoVehiculo"  HideWithNoDevice="True"
            Height="120px" SelectionMode="Multiple"></cwc:MovilListBox>
    </ContentTemplate>
</asp:UpdatePanel>
<div style="height: 5px;">
</div>

<table class="tabtable">
 <tr><td id="btPersonalizado" class="active" onclick="show(0);" title="Personalizado" >
</td><td id="btPredefinido" class="inactive" onclick="show(1);" title="Predefinido">
</td><td id="btEstadosLogisticos" class="inactive" onclick="show(2);" title="Estado Logistico" >
</td><td id="btFoto" class="inactive" onclick="show(3);" title="Foto">
</td><td id="btConfig" class="inactive" onclick="show(4);" title="Configuracion">
</td></tr>
</table>

<div id="divPersonalizado">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <div style="text-align: center; padding: 5px; padding-top:10px;">
                <asp:TextBox ID="txtMensajePer" runat="server" Width="90%" MaxLength="32" CssClass="LogicTextbox"></asp:TextBox>
                <cwc:ResourceTextBoxWatermarkExtender ID="ResourceTextBoxWatermarkExtender1" runat="server"
                    ResourceName="Labels" VariableName="MENSAJE_PERSONALIZADO" TargetControlID="txtMensajePer"
                    WatermarkCssClass="LogicWatermark" />
            </div>
            <div style="text-align: right;">
                <cwc:ResourceButton runat="server" ID="btMensajePer" CssClass="LogicButton_Big"
                    ResourceName="Controls" VariableName="BUTTON_SEND" OnClick="btMensajePer_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<div id="divPredefinido">
    <asp:UpdatePanel ID="UpdatePanelMensajePre" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>
            <div style="padding: 5px; padding-top:10px;">
                <div style="padding: 5px; color: #666;">
                    <cwc:ResourceLabel runat="server" ID="lblSeleccioneMensaje"  ResourceName="Labels" VariableName="SELECCIONAR_MENSAJE" />
                </div>
                <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" Width="99%" AutoPostBack="true" OnSelectedIndexChanged="cbTipoMensaje_SelectedIndexChanged" />
                <cwc:MensajesListBox ID="cbMensajes" runat="server" Width="99%" AutoPostBack="true"  ParentControls="cbTipoMensaje" Height="120px"></cwc:MensajesListBox>
            </div>
            <div style="text-align: right;">
                <cwc:ResourceButton runat="server" ID="btMensajePre" CssClass="LogicButton_Big"
                    ResourceName="Controls" VariableName="BUTTON_SEND" OnClick="btMensajePre_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<div id="divEstadosLogisticos">
    <div style="padding: 5px; padding-top:10px;">
        <div style="padding: 5px; color: #666;">
            <cwc:ResourceLabel runat="server" ID="ResourceLabel3"  ResourceName="Labels" VariableName="CICLO_LOGISTICO" />
        </div>
        <asp:UpdatePanel ID="updEstadoLog" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:DropDownList ID="cbEstadoLog" runat="server" Width="100%" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right;">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate></ContentTemplate>
                <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <cwc:ResourceButton runat="server" ID="Button1" CssClass="LogicButton_Big"
                    ResourceName="Controls" VariableName="BUTTON_SEND" OnClick="btEstadoLog_Click" />
        </div>
    </div>
</div>
<div id="divFoto">
    <div style="text-align: center; padding: 5px; padding-top:10px;">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
            <table style="width: 100%">
            <tr><td>
            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESDE" />
            </td><td>
            <cwc:DateTimePicker ID="dtFotoDesde" runat="server" Mode="DateTime" IsValidEmpty="true" />
            </td></tr>
            <tr><td>
            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="HASTA" />
            </td><td>
            <cwc:DateTimePicker ID="dtFotoHasta" runat="server" Mode="DateTime" IsValidEmpty="true" />
            </td></tr>
            </table>
                <cwc:DateTimeRangeValidator ID="dtFotoValidator" runat="server" StartControlID="dtFotoDesde" EndControlID="dtFotoHasta" MaxRange="00:05:00"/>
            </ContentTemplate>
            <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btAhora" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <div style="text-align: right;">
            <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                <ContentTemplate></ContentTemplate>
                <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btPedirFoto" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <cwc:ResourceButton runat="server" ID="btAhora" CssClass="LogicButton_Big"
                        ResourceName="Labels" VariableName="AHORA" OnClick="btAhora_Click" Style="float: left" />
            <cwc:ResourceButton runat="server" ID="btPedirFoto" CssClass="LogicButton_Big"
                        ResourceName="Controls" VariableName="BUTTON_SEND" OnClick="btPedirFoto_Click" /> 
            </div>
    </div>
</div>
<div id="divConfig">
    <asp:Panel runat="server" ID="AccPaneConfig" Style="text-align: center;">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <table style="width: 100%;"><tr><td>
                    <div class="config_container">
                        <div class="config_title">CORTE DE COMBUSTIBLE</div>
                        <div>
                            <table><tr><td>
                            <asp:Button ID="btCorte" runat="server" CssClass="config_button_red" OnClick="btCorte_Click" OnClientClick="return confirm('¿Esta seguro?');"/>
                            <br/>
                            Corte
                            </td><td>
                            <asp:Button ID="btCorteYa" runat="server" CssClass="config_button_red" OnClick="btCorteYa_Click" OnClientClick="return confirm('¿Esta seguro?');"/>
                            <br/>
                            Inmediato
                            </td><td>
                            <asp:Button ID="btHabilitar" runat="server" CssClass="config_button_green" OnClick="btHabilitar_Click" OnClientClick="return confirm('¿Esta seguro?');"/>
                            <br/>
                            Rehabilitar
                            </td></tr></table>
                    </div>
                    </div>
                </td></tr></table>
                <table style="width: 100%;"><tr><td>
                    <div class="config_container">
                        <div class="config_title">QTREE</div>
                        <div>
                            <table><tr><td>
                            <asp:Button ID="btSendFullQTree" runat="server" CssClass="config_button_fullqtree" OnClick="btSendFullQTree_Click" OnClientClick="return confirm('¿Esta seguro?');"/>
                            <br/>
                            Completo
                            </td><td>
                            <asp:Button ID="btSendQTree" runat="server" CssClass="config_button_difqtree" OnClick="btSendQTree_Click" OnClientClick="return confirm('¿Esta seguro?');"/>
                            <br/>
                            Diferencial
                            </td></tr></table>
                        </div>
                    </div>
                </td></tr></table>
                <table style="width: 100%;"><tr><td style="width: 50%;">
                    <div class="config_container">
                        <div class="config_title">EQUIPO</div>
                        <div>
                            <table><tr><td>
                            <asp:Button ID="btMensajeReboot" runat="server" CssClass="config_button_blue" OnClick="btMensajeReboot_Click"/>
                            <br/>
                            <cwc:ResourceLabel runat="server" ResourceName="Labels" VariableName="ENVIAR_REBOOT" ></cwc:ResourceLabel>
                            </td></tr></table>
                        </div>
                    </div>
                </td><td>
                    <div class="config_container">
                        <div class="config_title">FIRMWARE</div>
                        <div>
                            <table><tr><td>
                            <asp:Button ID="btMensajeFirm" runat="server" CssClass="config_button_firm" OnClick="btMensajeFirm_Click"/>
                            <br/>
                            Actualizar
                            </td></tr></table>
                        </div>
                    </div>
                </td></tr></table>
                <table style="width: 100%;"><tr><td>
                    <div class="config_container">
                        <div class="config_title">PURGAR</div>
                        <div>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btPurgarMensajes" runat="server" CssClass="config_button_message" OnClick="btPurgarMensajes_Click"/>
                                        <br/>
                                        Mensajes
                                    </td>
                                    <td>
                                        <asp:Button ID="btnPurgarConfiguracion" runat="server" CssClass="config_button_config" OnClick="btnPurgarConfiguracion_Click"/>
                                        <br/>
                                        Config
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnClearQueues" runat="server" CssClass="config_button_queue" OnClick="btnClearQueues_Click"/>
                                        <br/>
                                        Colas
                                    </td>
                                    <td>
                                        <asp:Button ID="btnClearFota" runat="server" CssClass="config_button_fota" OnClick="btnClearFota_Click" OnClientClick="return confirm('¿Confirma que desea resetear los archivos FOTA?');"/>
                                        <br/>
                                        FOTA
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td></tr></table>
                <table style="width: 100%;"><tr><td>
                    <div class="config_container">
                        <div class="config_title">GARMIN</div>
                        <div>
                            <table><tr><td>
                            <asp:Button ID="btnResetFMIOnGarmin" runat="server" CssClass="garmin_button_resetfmi" OnClick="btnResetFMIOnGarmin_Click"/>
                            <br/>
                            FMI Reset
                            </td></tr></table>
                        </div>
                    </div>
                </td></tr></table>                
        </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
</div>


<script type="text/javascript">
    var mensajeria_paneles = ['divPersonalizado', 'divPredefinido', 'divEstadosLogisticos', 'divFoto', 'divConfig'];
    var mensajeria_botones = ['btPersonalizado', 'btPredefinido', 'btEstadosLogisticos', 'btFoto', 'btConfig'];
    function show(index) {
        for (var i = 0; i < mensajeria_paneles.length; i++) {
            var el = $get(mensajeria_paneles[i]);
            var bt = $get(mensajeria_botones[i]);
            if (index == i) {
                el.style.display = '';
                bt.className = 'active';
            } else {
                el.style.display = 'none';
                bt.className = 'inactive';
            }
        }
    }
    show(0);
</script>
</div>