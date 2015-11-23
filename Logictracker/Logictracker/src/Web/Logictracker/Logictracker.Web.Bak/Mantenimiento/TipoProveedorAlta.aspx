<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TipoProveedorAlta.aspx.cs" Inherits="Logictracker.Mantenimiento.TipoProveedorAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="50%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="50%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="50" />
                        
                        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="50" />
                                
                     </cwc:AbmTitledPanel>           
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
