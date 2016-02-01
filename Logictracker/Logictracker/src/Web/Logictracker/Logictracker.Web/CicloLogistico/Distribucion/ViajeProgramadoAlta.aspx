<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Distribucion.ViajeProgramadoAlta" Codebehind="ViajeProgramadoAlta.aspx.cs" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="250px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="80%" AutoPostBack="True" />

                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                    <asp:UpdatePanel ID="upDepto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="80%" ParentControls="cbEmpresa" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="80%" ParentControls="cbEmpresa" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                                     
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE_DISTRIBUCION" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="80%" MaxLength="32" />

                </cwc:AbmTitledPanel> 
            </td>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopRight" runat="server" TitleVariableName="FILTROS" TitleResourceName="Labels" Height="250px">
                
                </cwc:AbmTitledPanel>        
            </td>
        </tr>
    </table>    
</asp:Content>
