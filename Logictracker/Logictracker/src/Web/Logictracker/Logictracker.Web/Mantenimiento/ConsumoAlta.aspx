<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Mantenimiento.ConsumoAlta" Codebehind="ConsumoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="200px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="100%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownList ID="cbTransportista" AddAllItem="true" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                        <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                        <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbEmpresa,cbLinea,cbTransportista,cbTipoVehiculo" TabIndex="20" OnSelectedIndexChanged="CbVehiculoOnSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMovimiento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblResponsable" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="100%" AddAllItem="false" AddNoneItem="true" ParentControls="cbEmpresa,cbLinea,cbTransportista" TabIndex="20" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTitCentroCosto" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Label ID="lblCentroCostos" runat="server" />
                            </ContentTemplate>
                            <Triggers>                                
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="vertical-align: top; width: 50%;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="DATOS_FACTURA" TitleResourceName="Labels" Height="200px">
                        
                        <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="DATE" />
                        <cwc:DateTimePicker ID="dtFecha" runat="server" Width="105px" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" />
                        
                        <cwc:ResourceLabel ID="lblKm" runat="server" ResourceName="Labels" VariableName="KM_DECLARADOS" />
                        <asp:TextBox ID="txtKilometros" runat="server" Width="100px" />         
                        
                        <cwc:ResourceLabel ID="lblTipoMovimiento" runat="server" ResourceName="Labels" VariableName="TIPO_MOVIMIENTO" />
                        <asp:UpdatePanel ID="updTipoMovimiento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TiposMovimientoDropDownList ID="cbTipoMovimiento" runat="server" OnSelectedIndexChanged="CbTipoMovimiento_OnSelectedIndexChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblDeposito" runat="server" ResourceName="Entities" VariableName="PARENTI87" />
                        <asp:UpdatePanel ID="updDeposito" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DepositoDropDownList ID="cbDeposito" runat="server" ParentControls="cbEmpresa,cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMovimiento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblTipoProveedor" runat="server" ResourceName="Entities" VariableName="PARENTI86" />
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoProveedorDropDownList ID="cbTipoProveedor" runat="server" ParentControls="cbEmpresa,cbLinea" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMovimiento" EventName="SelectedIndexChanged" />
                            </Triggers>                        
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblProveedor" runat="server" ResourceName="Entities" VariableName="PARENTI59" />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:ProveedorDropDownList ID="cbProveedor" runat="server" ParentControls="cbEmpresa,cbLinea,cbTipoProveedor" AddNoneItem="true" />
                            </ContentTemplate>    
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoProveedor" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMovimiento" EventName="SelectedIndexChanged" />
                            </Triggers>                        
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblDepositoDestino" runat="server" ResourceName="Labels" VariableName="DEPOSITO_DESTINO" />
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DepositoDropDownList ID="cbDepositoDestino" runat="server" ParentControls="cbEmpresa,cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMovimiento" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblFactura" runat="server" ResourceName="Labels" VariableName="NRO_FACTURA" Width="100px" />
                        <asp:TextBox ID="txtFactura" runat="server" Width="100px" MaxLength="50" />
                        
                        <cwc:ResourceLabel ID="lblImporte" runat="server" ResourceName="Labels" VariableName="IMPORTE_TOTAL" />
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:TextBox ID="txtImporte" runat="server" Width="100px" Enabled="false" Text="0.00" />
                            </ContentTemplate>                            
                        </asp:UpdatePanel>
                                                
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <cwc:TitledPanel ID="pnlConsumos" runat="server" TitleVariableName="MAN_INSUMOS" TitleResourceName="Menu">
                        <asp:UpdatePanel ID="updConsumos" runat="server" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <C1:C1GridView ID="gridConsumos" runat="server" UseEmbeddedVisualStyles="false" SkinId="SmallGrid"
                                    DataKeyNames="Id" AllowColMoving="false" AllowGrouping="false" AllowSorting="false"
                                    OnRowDataBound="GridConsumosItemDataBound" OnRowCommand="GridConsumosItemCommand">
                                    <Columns>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI60">
                                            <ItemTemplate>
                                                <cwc:TipoInsumoDropDownList ID="cbTipoInsumo" runat="server" ParentControls="cbEmpresa,cbLinea" AddAllItem="true" OnSelectedIndexChanged="CbTipoInsumo_OnSelectedIndexChanged" AutoPostBack="true" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI58">
                                            <ItemTemplate>
                                                <cwc:InsumoDropDownList ID="cbInsumo" runat="server" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="CbInsumo_OnSelectedIndexChanged" AutoPostBack="true" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI85">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUnidadMedida" runat="server" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CANTIDAD">
                                            <ItemTemplate>                                                
                                                <asp:TextBox ID="txtCantidad" runat="server" Width="150px" OnTextChanged="TxtCantidad_OnTextChanged" AutoPostBack="true" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="IMPORTE_UNITARIO">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtImporteUnitario" runat="server" Width="150px" OnTextChanged="TxtImporteUnitario_OnTextChanged" AutoPostBack="true" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="IMPORTE_TOTAL">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtImporteTotal" runat="server" Width="150px" Enabled="false" />
                                            </ItemTemplate>
                                        </c1h:C1ResourceTemplateColumn>
                                        <C1:C1TemplateField>
                                            <ItemTemplate>
                                                <cwc:ResourceLinkButton id="btEliminarParam" runat="server" ResourceName="Controls" VariableName="BUTTON_DELETE" CommandName="Eliminar" OnClientClick="return ConfirmDeleteParameterTipoDoc();" />
                                            </ItemTemplate>
                                            <ItemStyle Width="150px" />
                                        </C1:C1TemplateField>
                                    </Columns>    
                                </C1:C1GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btNewParam" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <div>
                            <cwc:ResourceLinkButton ID="btNewParam" runat="server" OnClick="BtNewParamClick" ResourceName="Controls" VariableName="BUTTON_ADD_INSUMO" />
                        </div>
                        
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
