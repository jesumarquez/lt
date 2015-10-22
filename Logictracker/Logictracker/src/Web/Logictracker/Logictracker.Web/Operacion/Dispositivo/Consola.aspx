<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" Inherits="Logictracker.Operacion.Dispositivo.Consola" Codebehind="Consola.aspx.cs" %>

<%@ Register TagPrefix="cc1" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%--TOOLBAR--%> 
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="OPE_CONSOLA_DISPOSITIVOS" />
    
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />

    <div  class="filterpanel">
    <asp:UpdatePanel ID="updTipoDispositivo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
    <ContentTemplate>
        <table width="100%">
            <tr>
                <td align="left" style="width: 280px">
                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="true" AutoPostBack="true" CssClass="LogicCombo" />
                </td>
                <td align="left" style="width: 250px">
                    <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true" AutoPostBack="true" ParentControls="cbEmpresa" CssClass="LogicCombo" />
                </td>
                <td align="left" style="width: 20px">
                    <asp:Button runat="server" ID="btChangeMode" OnClick="btChangeMode_Click" Text=">"/>
                </td>
                <td align="left" style="width: 350px">
                    <asp:Panel runat="server" ID="panelTipoDispositivo">
                        <cwc:ResourceLabel ID="lblTipoDispositivoF" runat="server" ResourceName="Entities" VariableName="PARENTI32" />
                        <cwc:TipoDispositivoDropDownList ID="cbTipoDispositivo" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" CssClass="LogicCombo" />   
                    </asp:Panel>
                    <asp:Panel runat="server" ID="panelTipoVehiculo" Visible="False">
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                        <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" CssClass="LogicCombo" />   
                    </asp:Panel>
                </td>
                <td align="left" style="width: 280px">
                    <asp:Panel runat="server" ID="panelDispositivo">
                        <cwc:ResourceLabel ID="lblDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI08" />
                        <cwc:DispositivoDropDownList ID="cbDispositivo" runat="server" Width="200px" ParentControls="cbLinea,cbTipoDispositivo" CssClass="LogicCombo" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="panelVehiculo" Visible="False">
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                        <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="200px" ParentControls="cbLinea,cbTipoVehiculo" CssClass="LogicCombo" />
                    </asp:Panel>
                </td>
                <td align="right">
                    <cwc:ResourceButton ID="btnActualizar" runat="server" ResourceName="Controls" VariableName="BUTTON_REFRESH" OnClick="BtnSearchClick" />
                </td>
            </tr>
        </table>
        </ContentTemplate>
                    </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel runat="server" ID="updResult" UpdateMode="Conditional">
        <ContentTemplate>
            
        <asp:Panel runat="server" ID="panelResult" Visible="False">
    <div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 30%;">
                    <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES" Height="200px" Style="padding: 5px;">
                        <table style="width: 100%;">
                            <tr>
                                <td style="font-size: 12px; font-weight: bold; color: #666666;">
                                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI08" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                <asp:Label runat="server" ID="lblTipoDispositivo"></asp:Label>
                                <div style="font-size: 16px;">
                                    <asp:Label runat="server" ID="lblDispositivoCodigo"></asp:Label>
                                </div>
                                </td>
                            </tr>
                            <tr>
                                <td>Id:<asp:Label runat="server" ID="lblDispositivoId"></asp:Label></td>
                            </tr>
                        </table>
                        <br/>
                        <table style="width: 100%;">
                            <tr>
                                <td style="font-size: 12px;font-weight: bold; color: #666666;">
                                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                                </td>
                            </tr>
                            <tr>
                            <td>
                                <asp:Label runat="server" ID="lblTipoVehiculo"></asp:Label>
                                <div style="font-size: 16px;">
                                    <asp:Label runat="server" ID="lblVehiculo"></asp:Label>
                                </div>
                            </td>
                            </tr>
                        </table>
                    </cwc:TitledPanel>
                </td>
                <td style="width: 35%;">
                    <cwc:TitledPanel ID="TitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="ULTIMA_POSICION" Height="200px">
                            <asp:Panel runat="server" ID="panelPosicion">
                                <table style="width: 100%;"><tr>
                                <td style="vertical-align: top; font-size: 12px; padding: 5px;">
                                    <div style="font-weight: bold;"><asp:Label runat="server" ID="lblFecha" /></div>
                                    <br/>
                                    <div><asp:LinkButton runat="server" ID="btMapa" Text="(,)" OnCommand="btMapa_Command" />
                                        <asp:Button runat="server" ID="btDummy" Style="display:none;"/>
                                        <AjaxToolkit:ModalPopupExtender runat="server" ID="modalMapa" PopupControlID="panelMapa" TargetControlID="btDummy" CancelControlID="btCerrarMapa" BackgroundCssClass="disabled_back" />
                                    <br/></div>
                                    <br/>
                                    <div><span style="font-size: 16px"><asp:Label runat="server" ID="lblVelocidad" /></span>km/h</div>                                    
                                </td>
                                </tr></table>
                            </asp:Panel>
                        
                    </cwc:TitledPanel>
                </td>
                <td>
                    <cwc:TitledPanel ID="TitledPanel3" runat="server" TitleResourceName="Labels" TitleVariableName="EVENTOS" Height="200px">
                        <asp:Panel runat="server" ID="panelEventos" Height="160px" ScrollBars="Auto">
                                <asp:Label runat="server" ID="lblEnventos"></asp:Label>
                            </asp:Panel>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td>
                    <cwc:TitledPanel ID="TitledPanel4" runat="server" TitleResourceName="Labels" TitleVariableName="POS_DESCARTADAS" Height="200px">
                        <asp:Panel runat="server" ID="panelPosicionesDescartadas" Height="160px" ScrollBars="Auto">
                            <asp:Label runat="server" ID="lblPosicionesDescartadas"></asp:Label>
                        </asp:Panel>
                    </cwc:TitledPanel>
                </td>
                <td style="width: 30%;">
                    <cwc:TitledPanel ID="TitledPanel5" runat="server" TitleResourceName="Labels" TitleVariableName="EVENTOS_DESCARTADOS" Height="200px">
                        <asp:Panel runat="server" ID="panelEventosDescartados" Height="160px" ScrollBars="Auto">
                            <asp:Label runat="server" ID="lblEventosDescartados"></asp:Label>
                        </asp:Panel>
                    </cwc:TitledPanel>
                </td>
                <td>
                    <cwc:TitledPanel ID="TitledPanel6" runat="server" TitleResourceName="Labels" TitleVariableName="ERRORES" Height="200px">
                        <asp:Panel runat="server" ID="panelErrores" Height="160px" ScrollBars="Auto">
                            <asp:Label runat="server" ID="lblErrores"></asp:Label>
                        </asp:Panel>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <cwc:TitledPanel ID="panelFota" runat="server" TitleResourceName="Labels" TitleVariableName="FOTA" Height="300px">
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
        
        
    </div>
    </asp:Panel>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnActualizar" />
    </Triggers>
    </asp:UpdatePanel>
    <asp:Panel runat="server" ID="panelMapa">
        <div id="divMapa" runat="server" style="visibility: hidden; width: 500px; height: 400px;">
            <div>
                <div style="position: absolute; right: 3px; top: 3px; z-index: 99999999;">
                    <asp:ImageButton runat="server" ID="btCerrarMapa" OnClientClick="return false;" ImageUrl="~/images/close.png"/>
                </div>
                <cc1:Monitor ID="Monitor1" runat="server" Width="500px" Height="400px" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>

