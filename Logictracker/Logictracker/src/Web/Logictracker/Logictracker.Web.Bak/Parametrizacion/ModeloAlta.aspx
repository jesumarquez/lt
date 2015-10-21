<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="ModeloAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ModeloAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="60%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="60%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblMarca" runat="server" ResourceName="Entities" VariableName="PARENTI06" Width="100px" />
                        <asp:UpdatePanel ID="upMarca" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:MarcaDropDownList ID="cbMarca" AddAllItem="false" runat="server" Width="60%" ParentControls="cbEmpresa, cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI61" />
                        <asp:TextBox ID="txtModelo" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="RENDIMIENTO_LTS_KM" />
                        <asp:TextBox ID="txtRendimiento" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="RENDIMIENTO_RALENTI" />
                        <asp:TextBox ID="txtRendimientoRalenti" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="INSUMO_COMBUSTIBLE" Width="100px" />
                        <asp:UpdatePanel ID="updInsumo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:InsumoDropDownList ID="cbInsumo" AddNoneItem="true" runat="server" Width="60%" ParentControls="cbEmpresa, cbLinea" DeCombustible="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="CAPACIDAD" />
                        <asp:TextBox ID="txtCapacidad" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="COSTO_MODELO" />
                        <asp:TextBox ID="txtCostoModelo" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="VIDA_UTIL" />
                        <asp:TextBox ID="txtVidaUtil" runat="server" />
                        
                        
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
