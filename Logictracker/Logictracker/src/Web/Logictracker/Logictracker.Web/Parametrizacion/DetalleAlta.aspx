<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.DetalleAlta" Codebehind="DetalleAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="40%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="40%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLinkButton ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" OnClientClick="window.open('../Parametrizacion/TipoEntidadLista.aspx', 'tipoEntidad')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" Width="40%" ParentControls="cbEmpresa, cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                                                
                        <cwc:ResourceLabel ID="lblNombre" runat="server" ResourceName="Labels" VariableName="NAME" />
                        <asp:TextBox ID="txtNombre" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblRepresentacion" runat="server" ResourceName="Labels" VariableName="REPRESENTACION" />
                        <cwc:RepresentacionDropDownList ID="cbRepresentacion" runat="server" Width="40%" OnSelectedIndexChanged="CbRepresentacion_OnSelectedIndexChanged" />
                                                
                        <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Labels" VariableName="TIPO_DATO" />
                        <cwc:TipoDetalleDropDownList ID="cbTipoDetalle" runat="server" Width="40%" />
                        
                        <cwc:ResourceLabel ID="lblMascara" runat="server" ResourceName="Labels" VariableName="MASCARA" Visible="false" />
                        <asp:TextBox ID="txtMascara" runat="server" Visible="false" />

                        <cwc:ResourceLabel ID="lblOrden" runat="server" ResourceName="Labels" VariableName="ORDEN" />
                        <asp:TextBox ID="txtOrden" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblFiltro" runat="server" ResourceName="Labels" VariableName="ES_FILTRO" />
                        <asp:CheckBox ID="chkFiltro" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblObligatorio" runat="server" ResourceName="Labels" VariableName="OBLIGATORIO" />
                        <asp:CheckBox ID="chkObligatorio" runat="server" />
                        
                        <cwc:ResourceLinkButton ID="lblDetalle" runat="server" ResourceName="Labels" VariableName="DETALLE_PADRE" OnClientClick="window.open('../Parametrizacion/DetalleLista.aspx', 'detalle')" ForeColor="Black" Visible="false" />
                        <asp:UpdatePanel ID="upDetalle" runat="server" UpdateMode="Conditional" RenderMode="Inline" Visible="false">
                            <ContentTemplate>
                                <cwc:DetalleDropDownList ID="cbDetalle" runat="server" Width="40%" AddNoneItem="true" ParentControls="cbEmpresa, cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblOpciones" runat="server" ResourceName="Labels" VariableName="OPCIONES" />
                        <asp:TextBox ID="txtOpciones" runat="server" TextMode="MultiLine" />
                        
                        <div />
                        <cwc:ResourceLabel ID="lblFormato" runat="server" ForeColor="Red" ResourceName="Labels" VariableName="FORMATO_OPCIONES" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
