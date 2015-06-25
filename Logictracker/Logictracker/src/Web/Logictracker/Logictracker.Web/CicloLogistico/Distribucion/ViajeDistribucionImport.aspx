<%@ Page Language="C#" MasterPageFile="~/MasterPages/ImportPage.master" AutoEventWireup="true" CodeFile="ViajeDistribucionImport.aspx.cs" Inherits="Logictracker.Web.CicloLogistico.Distribucion.ViajeDistribucionImport" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentImport" Runat="Server">
        
    <table style="width: 100%">                
        <tr>
            <td><cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" /></td>
            <td><cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" /></td>
        </tr>                    
        <tr>
            <td><cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" /></td>
            <td>
                <asp:UpdatePanel runat="server" ID="upLinea" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddAllItem="True" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>            
        </tr>
    </table>
            
    <asp:UpdatePanel runat="server" ID="updPanelAxiodis" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="panelAxiodis">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblClienteEntregas" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                            Entregas
                        </td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="updClienteEntregas" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:ClienteDropDownList ID="cbClienteEntregas" runat="server" Width="175px" ParentControls="cbLinea" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblClienteSucursales" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                            Sucursales
                        </td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="updClienteSucursales" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:ClienteDropDownList ID="cbClienteSucursales" runat="server" Width="175px" ParentControls="cbLinea" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblVigencia" runat="server" ResourceName="Labels" VariableName="VIGENCIA_HORAS" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtVigencia" runat="server" Width="60px" Text="12" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cwc:ResourceCheckBox runat="server" ID="chkRegresaABase" ResourceName="Labels" VariableName="REGRESA_A_BASE" Checked="False"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <cwc:ResourceCheckBox runat="server" ID="chkCalcularHoras" ResourceName="Labels" VariableName="CALCULAR_HORAS" Checked="true"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="updPanelRoadshow" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="panelRoadshow">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                        </td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:ClienteDropDownList ID="cbClienteRoadshow" runat="server" Width="175px" ParentControls="cbLinea" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="VIGENCIA_HORAS" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtVigenciaRoadshow" runat="server" Width="60px" Text="24" MaxLength="3" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="SOBREESCRIBIR_PUNTO_ENTREGA" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkSobreescribir" runat="server" Checked="True" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="updPanelRoadnet" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="panelRoadnet">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblVigenciaRoadnet" runat="server" ResourceName="Labels" VariableName="VIGENCIA_HORAS" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtVigenciaRoadnet" runat="server" Width="60px" Text="24" MaxLength="3" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="updPanelExcel" UpdateMode="Conditional" ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="panelExcel">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                        </td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:ClienteDropDownList ID="cbCliente" runat="server" Width="175px" ParentControls="cbLinea" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblInicio" runat="server" ResourceName="Labels" VariableName="INICIO" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtInicio" runat="server" Width="60px" Text="08:00" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="VIGENCIA_HORAS" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtVigenciaExcelTemplate" runat="server" Width="60px" Text="24" MaxLength="3" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

