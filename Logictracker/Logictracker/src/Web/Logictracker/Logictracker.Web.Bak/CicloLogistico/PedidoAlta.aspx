<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="PedidoAlta.aspx.cs"  Inherits="Logictracker.CicloLogistico.PedidoAlta" %>
      
<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >  
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="320px">
        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" />
        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
        
                        <cwc:ResourceLabel ID="lblBocaDeCarga" runat="server" ResourceName="Entities" VariableName="BOCADECARGA" Width="100px" />
                        <asp:UpdatePanel ID="upBocaDeCarga" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:BocaDeCargaDropDownList ID="cbBocaDeCarga" runat="server" Width="100%" ParentControls="cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
        
                        <cwc:ResourceLinkButton ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" Width="100px" OnClientClick="window.open('../Parametrizacion/ClienteAlta.aspx','cliente')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upCliente" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ClienteDropDownList ID="cbCliente" runat="server" Width="100%" ParentControls="cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
        
                        <cwc:ResourceLinkButton ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" Width="100px" OnClientClick="window.open('../Parametrizacion/PtoEntregaAlta.aspx','ptoEntrega')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upPuntoEntrega" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PuntoDeEntregaDropDownList ID="cbPuntoEntrega" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea,cbCliente" OnSelectedIndexChanged="cbPuntoEntrega_SelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbCliente" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:UpdatePanel ID="upCodigo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <asp:TextBox id="txtCodigo" runat="server" Width="99%" MaxLength="64" Enabled="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
              
                    </cwc:AbmTitledPanel>
                </td>
                <td style="vertical-align: top; width: 50%;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CARGA" TitleResourceName="Labels" Height="320px">
            
                        <cwc:ResourceLabel ID="lblFechaEnObra" runat="server" ResourceName="Labels" VariableName="FECHA_EN_OBRA" />
                        <cwc:DateTimePicker ID="dtFechaEnObra" runat="server" Mode="DateTime" IsValidEmpty="true" AutoPostBack="true" OnDateChanged="dtFechaEnObra_DateChanged" />
                        
                        <cwc:ResourceLinkButton ID="lblProducto" runat="server" ResourceName="Entities" VariableName="PARENTI63" OnClientClick="window.open('../Parametrizacion/ProductoAlta.aspx','producto')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upProducto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ProductoDropDownList ID="cbProducto" runat="server" Width="100%" ParentControls="cbLinea,cbBocaDeCarga" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbBocaDeCarga" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                               
                        <cwc:ResourceLabel ID="lblCantidad" runat="server" ResourceName="Labels" VariableName="CANTIDAD_PEDIDO" />
                        <asp:TextBox id="txtCantidad" runat="server" Width="100px" MaxLength="16" />
                        
                        <cwc:ResourceLabel ID="lblAjuste" runat="server" ResourceName="Labels" VariableName="CANTIDAD_AJUSTES" />
                        <asp:TextBox id="txtAjuste" runat="server" Width="100px" MaxLength="16" />
                                
                        <cwc:ResourceLabel ID="lblHoraCarga" runat="server" ResourceName="Labels" VariableName="HORA_CARGA" />
                        <asp:UpdatePanel ID="updHoraCarga" runat="server" >
                            <ContentTemplate>
                                <cwc:DateTimePicker ID="dtHoraCarga" runat="server" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="dtFechaEnObra" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblBomba" runat="server" ResourceName="Labels" VariableName="NUMERO_BOMBA" />
                        <asp:TextBox id="txtBomba" runat="server" Width="100px" MaxLength="16" />
                        
                        <cwc:ResourceLabel ID="lblEsMinimixer" runat="server" ResourceName="Labels" VariableName="MULTIPLES_REMITOS" />
                        <asp:CheckBox ID="chkEsMinimixer" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblTiempoCiclo" runat="server" ResourceName="Labels" VariableName="PEDIDO_TIEMPO_CICLO" />
                        <asp:TextBox id="txtTiempoCiclo" runat="server" Width="100px" MaxLength="16" />
                        
                        <cwc:ResourceLabel ID="lblFrecuencia" runat="server" ResourceName="Labels" VariableName="PEDIDO_FRECUENCIA_ENTREGA" />
                        <asp:TextBox id="txtFrecuencia" runat="server" Width="100px" MaxLength="16" />
            
                        <cwc:ResourceLabel ID="lblCargaViaje" runat="server" ResourceName="Labels" VariableName="PEDIDO_CARGA_POR_VIAJE" />
                        <div runat="server">
                            <asp:UpdatePanel id="updCargaViaje" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <asp:TextBox id="txtCargaViaje" runat="server" Width="100px" MaxLength="16" Value="10" Enabled="false" />    
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="chkCargaViaje" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <cwc:ResourceCheckBox ID="chkCargaViaje" runat="server" ResourceName="Labels" VariableName="PEDIDO_CARGA_POR_VIAJE_MAXIMA" Checked="true" AutoPostBack="true" OnCheckedChanged="ChkCargaViaje_CheckedChanged" />
                        </div>
                                        
                        <cwc:ResourceLabel ID="lblContacto" runat="server" ResourceName="Labels" VariableName="CONTACTO" />
                        <asp:UpdatePanel ID="updContacto" runat="server">
                            <ContentTemplate>
                                <asp:TextBox id="txtContacto" runat="server" Width="100%" MaxLength="255" Rows="5" TextMode="MultiLine" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbPuntoEntrega" />
                            </Triggers>                  
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblObservacion" runat="server" ResourceName="Labels" VariableName="OBSERVACION" />
                        <asp:UpdatePanel ID="updObservaciones" runat="server">
                            <ContentTemplate>
                                <asp:TextBox id="txtObservacion" runat="server" Width="100%" MaxLength="255" Rows="5" TextMode="MultiLine" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbPuntoEntrega" />
                            </Triggers>  
                        </asp:UpdatePanel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>
