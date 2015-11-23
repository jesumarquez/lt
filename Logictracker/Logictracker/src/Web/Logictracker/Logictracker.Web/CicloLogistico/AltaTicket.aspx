<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.AltaTicket" Codebehind="AltaTicket.aspx.cs" %> 
       
<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
        <asp:UpdatePanel ID="updRefreshTabGeneral" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>   
                <table style="width: 100%; border-spacing: 10px;">
                    <tr>
                        <td style="width: 50%; vertical-align: top;">
                            <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="320px">
                                
                                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" OnSelectedIndexChanged="owner_SelectedIndexChanged" />
                                
                                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                                <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" OnSelectedIndexChanged="owner_SelectedIndexChanged" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                
                                <cwc:ResourceLabel ID="lblBaseDestino" runat="server" ResourceName="Labels" VariableName="BASE_DESTINO" Width="100px" />
                                <asp:UpdatePanel ID="upBaseDestino" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="cbBaseDestino" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                
                                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                                <asp:UpdatePanel ID="upCliente" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ClienteDropDownList ID="cbCliente" runat="server" Width="100%" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" TabIndex="10" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
        
                                <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" />
                                <asp:UpdatePanel ID="upPuntoDeEntrega" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PuntoDeEntregaDropDownList ID="cbPuntoEntrega" runat="server" ParentControls="cbEmpresa,cbLinea,cbCliente" Width="100%" TabIndex="10"  OnSelectedIndexChanged="cbPuntoEntrega_SelectedIndexChanged" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
        
                                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:TransportistaDropDownList ID="cbTransportista" AddAllItem="true" runat="server" Width="100%" TabIndex="15" ParentControls="cbEmpresa,cbLinea" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
        
                                <cwc:ResourceLabel ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:MovilDropDownList ID="cbMovil" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbTransportista"  OnSelectedIndexChanged="cbMovil_SelectedIndexChanged" TabIndex="20" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                
                                <cwc:ResourceLabel ID="lblParenti09" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                                <asp:UpdatePanel ID="updChofer" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:EmpleadoDropDownList ID="cbChofer" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbEmpresa,cbLinea" TabIndex="25"  />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbMovil" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
        
                                <cwc:ResourceLabel ID="ResourceLabel11" runat="server" ResourceName="Labels" VariableName="CODE" />
                                <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
        
                                <cwc:ResourceLabel ID="ResourceLabel13" runat="server" ResourceName="Labels" VariableName="DATE" />
                                <cwc:DateTimePicker ID="dtFecha" runat="server" Width="80px" IsValidEmpty="false" Mode="Date" TimeMode="Now" />
                                
                                <cwc:ResourceLabel ID="lblOrdenDiarioText" runat="server" ResourceName="Labels" VariableName="ORDEN_DIARIO" />
                                <asp:Label ID="lblOrdenDiario" runat="server" />
                            </cwc:AbmTitledPanel>
                        </td>
                        <td style="vertical-align: top; width: 50%;">
                            <%--COLUMNA DER--%>
                            <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CARGA" TitleResourceName="Labels" Height="320px">
        
                                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="PRODUCT_CODE" />
                                <asp:TextBox ID="txtCodigoProducto" runat="server" Width="98%" TabIndex="40" MaxLength="50" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="PRODUCT_DESCRIPCION" />
                                <asp:TextBox ID="txtDescripcionProducto" runat="server" Width="98%" TabIndex="45" MaxLength="50" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="CANTIDAD_PEDIDO" />
                                <asp:TextBox ID="txtCantidadPedido" runat="server" Width="98%" TabIndex="50" MaxLength="12" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="CANTIDAD_CARGA" />
                                <asp:TextBox ID="txtCantidadCarga" runat="server" Width="98%" TabIndex="55" MaxLength="12" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Labels" VariableName="CANTIDAD_CARGA_REAL" />
                                <asp:TextBox ID="txtCantidadCargaReal" runat="server" Width="98%" TabIndex="56" MaxLength="12" Enabled="false" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="CANTIDAD_ACUMULADA" />
                                <asp:TextBox ID="txtCantidadAcumulada" runat="server" Width="98%" TabIndex="60" MaxLength="12" />
                                
                                <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Labels" VariableName="UNIT" />
                                <asp:TextBox ID="txtUnidad" runat="server" Width="98%" TabIndex="65" MaxLength="5"/>
                                
                                <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="COMMENT_1" />
                                <asp:TextBox ID="txtComentario1" runat="server" Width="98%" TabIndex="70" MaxLength="50"/>
                                
                                <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="COMMENT_2" />
                                <asp:TextBox ID="txtComentario2" runat="server" Width="98%" TabIndex="75" MaxLength="50"/>
                                
                                <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="COMMENT_3" />
                                <asp:TextBox ID="txtComentario3" runat="server" Width="98%" TabIndex="80" MaxLength="50" Enabled="false" />

                            </cwc:AbmTitledPanel>        
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>

    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Menu" VariableName="SYS_CICLO_LOGISTICO" >
        <asp:UpdatePanel ID="updRefreshTabDetalles" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <table style="width: 100%; border-spacing: 10px;">
                    <tr>
                        <td style="width: 100%; vertical-align: top;" colspan="2">
                            <cwc:TitledPanel ID="TitledPanel1" runat="server" TitleResourceName="Menu" TitleVariableName="SYS_CICLO_LOGISTICO">
                                <asp:UpdatePanel ID="updGridEstados" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <C1:C1GridView ID="gridEstados" runat="server" Width="100%" CssClass="Grid" AllowColMoving="false" AllowGrouping="false"
                                            GridLines="Horizontal" CellPadding="10" AutoGenerateColumns="False" AllowSorting="false" DataKeyNames="Id">
                                            <Columns>
                                                <C1:C1TemplateField>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkIncluirEstado" runat="server" Checked="true" />
                                                    </ItemTemplate>
                                                </C1:C1TemplateField>
                                                <c1h:C1ResourceBoundColumn DataField="Descripcion" ResourceName="Labels" VariableName="STATE" />
                                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIME_PROGRAMMED">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblDiaProgramado"></asp:Label>
                                                        <asp:TextBox ID="txtHoraEstado" runat="server" Width="60px" OnTextChanged="txtHoraEstado_Click" AutoPostBack="true" />
                                                            <AjaxToolkit:MaskedEditExtender ID="maskHoraEstado" runat="server" TargetControlID="txtHoraEstado" Mask="99:99" 
                                                                MaskType="Time" UserTimeFormat="TwentyFourHour" CultureName="es-AR" AutoComplete="true" AutoCompleteValue="0">
                                                            </AjaxToolkit:MaskedEditExtender>
                                                    </ItemTemplate>
                                                </c1h:C1ResourceTemplateColumn>
                                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIME_MANUAL" />
                                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIME_AUTOMATIC" />
                                            </Columns>
                                        </C1:C1GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </cwc:TitledPanel>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>

    <div class="notab">
        <asp:UpdatePanel ID="updAnular" runat="server" UpdateMode="Conditional">
            <ContentTemplate>            
                <%--POPUP DE MOTIVO DE ANULACION--%>
                <asp:Panel ID="PanelModal" runat="server" style="display: none;">                
                    <asp:Panel ID="panelStartTicket" runat="server">
                        <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                            <div style="margin-bottom: 10px;text-align: center;">
                                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ResourceName="SystemMessages" VariableName="TICKET_CANCEL" />
                                <br />
                                <br />
                                <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Labels" VariableName="TICKET_CANCEL_REASON" />                    
                            </div>                
                            <div style="margin-bottom: 30px;text-align: center;">
                                <asp:TextBox ID="txtMotivo" runat="server" MaxLength="256" TextMode="MultiLine"></asp:TextBox>
                            </div>
                            <div style="text-align: right;">            
                                <cwc:ResourceButton ID="btCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                                &nbsp;&nbsp;&nbsp;
                                <cwc:ResourceButton ID="btAnular" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnClick="btAnular_Click" />
                            </div>
                        </div>
                    </asp:Panel>             
                </asp:Panel>
                <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
                <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" CancelControlID="btCancelar" ID="ModalPopupExtender1" runat="server" BackgroundCssClass="disabled_back" ></AjaxToolkit:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel> 
        
        <asp:UpdatePanel ID="updDocumentos" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <%--POPUP DE DOCUMENTOS VENCIDOS--%>
                <asp:Panel ID="panelModalDocumentos" runat="server" style="display: none;">                
                    <asp:Panel ID="panelDocumentos" runat="server">
                        <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                            <div style="margin-bottom: 10px;text-align: center;">
                                <cwc:ResourceLabel ID="ResourceLabel17" runat="server" Font-Bold="true" ResourceName="SystemMessages" VariableName="TICKET_DOCUMENTOS_VENCIDOS" />
                                <br />
                                <br />
                            </div>                
                            <div style="margin-bottom: 30px;">
                                <asp:BulletedList ID="cbDocumentosVencidos" runat="server" />
                            </div>
                            <div style="text-align: right;">            
                                <cwc:ResourceButton ID="btCancelarDocumentos" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                                &nbsp;&nbsp;&nbsp;
                                <cwc:ResourceButton ID="btAceptarDocumentos" runat="server" ResourceName="Controls" VariableName="BUTTON_SAVE" OnClick="btAceptarDocumentos_Click" />
                            </div>
                        </div>
                    </asp:Panel>             
                </asp:Panel>
                <asp:Panel ID="panelBobo2" runat="server"></asp:Panel>
                <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo2" PopupControlID="panelModalDocumentos" CancelControlID="btCancelarDocumentos" ID="ModalPopupExtenderDocumentos" runat="server" BackgroundCssClass="disabled_back" ></AjaxToolkit:ModalPopupExtender>
            </ContentTemplate>
        </asp:UpdatePanel>       
    </div>
</asp:Content>
