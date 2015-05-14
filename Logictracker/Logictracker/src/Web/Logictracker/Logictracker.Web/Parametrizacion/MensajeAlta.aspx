<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="MensajeAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionMensajeAlta" Title="Untitled Page" %>

<%@ Register TagPrefix="uc" TagName="IconPicker" Src="~/App_Controls/IconPicker.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES" Height="205px">
                    
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" OnInitialBinding="cbEmpresa_PreBind" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" ParentControls="cbEmpresa"
                                AddAllItem="true" OnInitialBinding="cbLinea_PreBind" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                                
                    <cwc:ResourceLabel ID="lblTipoMensaje" runat="server" ResourceName="Entities" VariableName="PARENTI16" />
                    <asp:UpdatePanel ID="upTipoMensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoMensajeDropDownList ID="cbTipoMensaje" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" OnInitialBinding="cbTipoMensaje_PreBind"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" MaxLength="10" Width="200px" />
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="64" Width="200px" />
        
                    <cwc:ResourceLabel ID="lblMensaje" runat="server" ResourceName="Entities" VariableName="PAREVEN01" />
                    <asp:TextBox ID="txtMensaje" runat="server" MaxLength="32" Width="200px" />
                </cwc:AbmTitledPanel>
                </td>
                <td>
                <cwc:AbmTitledPanel ID="TitledPanel1" runat="server" Title="Comportamiento" Height="205px">
                    <cwc:ResourceLabel ID="lblOrigen" runat="server" ResourceName="Labels" VariableName="ORIGEN" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajeOrigenDropDownList ID="cbOrigen" runat="server" Width="200px" OnInitialBinding="cbOrigen_PreBind" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblDestino" runat="server" ResourceName="Labels" VariableName="DESTINO" />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajeOrigenDropDownList ID="cbDestino" runat="server" Width="200px" OnInitialBinding="cbDestino_PreBind" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblEsAlarma" runat="server" ResourceName="Labels" VariableName="ES_ALARMA" />
                    <asp:CheckBox ID="chkAlarma" runat="server" />
                    <cwc:ResourceLabel ID="lblNivelAcceso" runat="server" ResourceName="Labels" VariableName="NIVEL_ACCESO" />
                    <asp:UpdatePanel ID="upNivelAcceso" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoUsuarioDropDownList ID="cbAcceso" runat="server" Width="200px" OnInitialBinding="cbAcceso_PreBind" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblExpiracion" runat="server" ResourceName="Labels" VariableName="EXPIRACION" />
                    <c1:C1NumericInput ID="npExpiracion" runat="server" ShowNullText="false" MaxValue="999" Value="8" DecimalPlaces="0" Width="200" Height="17px"/>
                    <cwc:ResourceLabel ID="lblIcono" runat="server" ResourceName="Labels" VariableName="ICON" />
                     <uc:IconPicker ID="selectIcon1" runat="server" ParentControls="cbEmpresa,cbLinea" />
                </cwc:AbmTitledPanel>
                </td></tr>
    </table>
    <asp:UpdatePanel ID="upHack" runat="server" UpdateMode="Conditional">
        <ContentTemplate />
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbDestino" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cbOrigen" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cbAcceso" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
