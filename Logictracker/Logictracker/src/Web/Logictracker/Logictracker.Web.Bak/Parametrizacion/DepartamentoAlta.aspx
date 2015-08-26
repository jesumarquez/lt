<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="DepartamentoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionDepartamentoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <asp:UpdatePanel ID="upDeptos" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="180px">

                            <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="98%" />
                            
                            <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                            <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="98%" ParentControls="cbEmpresa" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <cwc:ResourceLabel ID="lblEmpleado" runat="server" ResourceName="Labels" VariableName="RESPONSABLE_DEPTO" Width="100px" />
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <cwc:EmpleadoDropDownList ID="cbEmpleado" AddNoneItem="true" runat="server" Width="98%" ParentControls="cbEmpresa,cbLinea" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="CODE" />
                            <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="128" />
                            
                            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                            <asp:TextBox runat="server" id="txtDescripcion" Width="98%" MaxLength="64" />

                        </cwc:AbmTitledPanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
            </td>
        </tr>
    </table>
</asp:Content>