<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="InsumoAlta.aspx.cs" Inherits="Logictracker.Mantenimiento.InsumoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="vertical-align: top; width:50%;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
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
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="vertical-align: top; width:50%;">
                    <cwc:AbmTitledPanel ID="panelTopRight" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" />
                        
                        <cwc:ResourceLabel ID="lblTipoInsumo" runat="server" ResourceName="Entities" VariableName="PARENTI60"/>
                        <asp:UpdatePanel ID="upTipoInsumo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoInsumoDropDownList ID="cbTipoInsumo" AddAllItem="false" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea"/>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblUnidadMedida" runat="server" ResourceName="Entities" VariableName="PARENTI85"/>
                        <cwc:UnidadMedidaDropDownList ID="cbUnidadMedida" runat="server" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="50" />
                        
                        <cwc:ResourceLabel ID="lblValorReferencia" runat="server" ResourceName="Labels" VariableName="VALOR_REFERENCIA" />
                        <asp:TextBox ID="txtValorReferencia" runat="server" MaxLength="10" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
