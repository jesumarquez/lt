<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.Parametrizacion_ClienteAlta" Title="Clientes" Codebehind="ClienteAlta.aspx.cs" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panAlarmas" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES">
                    
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%" />
                    
                    <cwc:ResourceLabel ID="lblMensajeBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                    <asp:UpdatePanel ID="upMensajeBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="100%" Font-Size="Small" MaxLength="32" />
                    
                    <cwc:ResourceLabel ID="lblDescCorta" runat="server" ResourceName="Labels" VariableName="DESCRIPCION_CORTA" />
                    <asp:TextBox ID="txtDescripcionCorta" runat="server" Width="100%" MaxLength="17" />
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" Font-Size="Small" MaxLength="40" />
                    
                    <cwc:ResourceLabel ID="lblTelefono" runat="server" ResourceName="Labels" VariableName="TELEFONO" />
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="32" Width="100%" />
                    
                    <div></div>
                    <asp:Label ID="lblDireccion" runat="server"></asp:Label>
                </cwc:AbmTitledPanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
            <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_ADICIONALES">
                <cwc:ResourceLabel ID="ResourceLabel10" runat="server" ResourceName="Labels" VariableName="COMMENT_1" />
                <asp:TextBox ID="txtComentario1" runat="server" Width="100%" TabIndex="70" MaxLength="50"/>
                <cwc:ResourceLabel ID="ResourceLabel12" runat="server" ResourceName="Labels" VariableName="COMMENT_2" />
                <asp:TextBox ID="txtComentario2" runat="server" Width="100%" TabIndex="75" MaxLength="50"/>
                <cwc:ResourceLabel ID="ResourceLabel14" runat="server" ResourceName="Labels" VariableName="COMMENT_3" />
                <asp:TextBox ID="txtComentario3" runat="server" Width="100%" TabIndex="80" MaxLength="50"/>
            </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 100%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleResourceName="Labels"
                    TitleVariableName="REFERENCIA_GEOGRAFICA">
                    <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                        <asp:CheckBox ID="chkExistente" runat="server" />
                        Seleccionar una Referencia Geografica existente.
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
</asp:Content>
