<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.EntidadAlta" Codebehind="EntidadAlta.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="SelectGeoRefference" Src="~/App_Controls/SelectGeoRefference.ascx" %>
<%@ Register TagPrefix="uc1" TagName="EditEntityGeoRef" Src="~/App_Controls/EditEntityGeoRef.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Detalles" Src="~/App_Controls/Detalles.ascx" %>

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

                        <cwc:ResourceLinkButton ID="lblDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI08" OnClientClick="window.open('../Parametrizacion/DispositivoLista.aspx', 'dispositivo')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upDispositivo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:DispositivoDropDownList ID="cbDispositivo" runat="server" Width="60%" ParentControls="cbEmpresa, cbLinea" Padre="Entidad" AddNoneItem="True" HideAssigned="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLinkButton ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" OnClientClick="window.open('../Parametrizacion/TipoEntidadLista.aspx', 'tipoEntidad')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" Width="60%" ParentControls="cbEmpresa, cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                                                
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server"></asp:TextBox>
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="10" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="IMAGEN" />
                        <asp:FileUpload ID="flImagen" runat="server" />
                        
                        <asp:Label ID="ResourceLabel4" runat="server" Text="" />
                        <cwc:ResourceButton ID="btnImagen" runat="server" ResourceName="Controls" VariableName="VER_IMAGEN_ACTUAL" Enabled="false" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleResourceName="Labels"
                        TitleVariableName="REFERENCIA_GEOGRAFICA">
                        <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                            <asp:CheckBox ID="chkExistente" runat="server" />
                            Seleccionar una Referencia Geográfica existente.
                        </asp:Panel>
                        <asp:Panel ID="panSelectGeoRef" runat="server" Style="display: none;">
                            <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea"
                                Height="200px" />
                        </asp:Panel>
                        <asp:Panel ID="panNewGeoRef" runat="server">
                            <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea" Height="200px"/>
                        </asp:Panel>
                    </cwc:AbmTitledPanel>
                </td>
                
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="DETALLES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <uc1:Detalles ID="ctrlDetalles" runat="server" />
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
