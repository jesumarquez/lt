<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Admin.LogiclinkAlta" Codebehind="LogiclinkAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td width="50%">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="300px" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="300px" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblEstrategia" runat="server" ResourceName="Labels" VariableName="STRATEGY" Width="100px" />
                        <asp:UpdatePanel ID="upEstrategia" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:EstrategiaDropDownList ID="cbEstrategia" runat="server" Width="300px" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>                        

                        <cwc:ResourceLabel ID="lblArchivo" runat="server" ResourceName="Labels" VariableName="ARCHIVO" />
                        <asp:FileUpload ID="fileUpload" runat="server" />

                        <div></div>
                        <cwc:ResourceButton runat="server" ID="btnGuardar" CssClass="LogicButton_Big" VariableName="GUARDAR" ResourceName="Labels" OnClick="BtnGuardarOnClick" />

                    </cwc:AbmTitledPanel>
                </td>
                <td width="50%">
                    <cwc:AbmTitledPanel ID="panelTopRight" runat="server" TitleVariableName="DETALLES" TitleResourceName="Labels" Height="150px" Visible="false">
                        
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="NOMBRE" />
                        <asp:Label ID="lblNombre" runat="server" />

                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="FILE_SOURCE" />
                        <asp:Label ID="lblOrigen" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DATE_ADDED" />
                        <asp:Label ID="lblFechaAlta" runat="server" />

                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DATE_PROCESSED" />
                        <asp:Label ID="lblFechaProcesamiento" runat="server" />

                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="STATUS" />
                        <asp:Label ID="lblEstado" runat="server" />

                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="RESULT" />
                        <asp:Label ID="lblResultado" runat="server" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>

