<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TipoMensajeAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionTipoMensajeAlta" Title="Untitled Page" %>

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="SelectIcon" TagPrefix="uc1" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_TIPO_MENSAJE" TitleResourceName="Labels" Height="200px">
            
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" AddAllItem="true" AutoPostBack="true" OnInitialBinding="DdlDistritoInitialBinding" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbGenerico" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
            
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="100%" AddAllItem="true" AutoPostBack="true" ParentControls="ddlDistrito" OnInitialBinding="DdlBaseInitialBinding" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbGenerico" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="6" />
                    
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                    
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="ICON" />
                    <uc1:SelectIcon ID="ucIcon" runat="server" ParentControls="ddlDistrito,ddlBase" />
                    
                </cwc:AbmTitledPanel>
            </td>
            <td>
                <cwc:AbmTitledPanel ID="AbmPanelRight" runat="server" TitleVariableName="COMPORTAMIENTO" TitleResourceName="Labels" Height="200px">
                    
                    <cwc:ResourceLabel ID="lblGenerico" runat="server" ResourceName="Labels" VariableName="ES_GENERICO" />
                    <asp:CheckBox ID="cbGenerico" runat="server" AutoPostBack="true" OnCheckedChanged="ChkGenericoChanged" />
                    
                    <cwc:ResourceLabel ID="lblEstadoLogistico" runat="server" ResourceName="Labels" VariableName="ES_ESTADO_LOGISTICO"/>
                    <asp:CheckBox ID="chkEstadoLogistico" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblUsuario" runat="server" ResourceName="Labels" VariableName="ES_USUARIO" />
                    <asp:CheckBox ID="chkUsuario" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblMantenimiento" runat="server" ResourceName="Labels" VariableName="ES_MANTENIMIENTO" />
                    <asp:CheckBox ID="chkMantenimiento" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblCombustible" runat="server" ResourceName="Labels" VariableName="ES_COMBUSTIBLE" />
                    <asp:CheckBox ID="chkCombustible" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblDeConfirmacion" runat="server" ResourceName="Labels" VariableName="DE_CONFIRMACION" />
                    <asp:UpdatePanel runat="server" ID="updConfirmacion">
                        <ContentTemplate>
                            <asp:CheckBox ID="chkConfirmacion" runat="server" OnCheckedChanged="ChkConfirmacionCheckedChanged" AutoPostBack="True" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblDeRechazo" runat="server" ResourceName="Labels" VariableName="DE_RECHAZO" />
                    <asp:UpdatePanel runat="server" ID="updRechazo">
                        <ContentTemplate>
                            <asp:CheckBox ID="chkRechazo" runat="server" OnCheckedChanged="ChkRechazoCheckedChanged" AutoPostBack="True" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblDeAtencion" runat="server" ResourceName="Labels" VariableName="DE_ATENCION" />
                    <asp:CheckBox ID="chkAtencion" runat="server" />
                    
                </cwc:AbmTitledPanel>           
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td width="50%" valign="top">
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="AGRUPADOR" TitleResourceName="Labels" Height="200px">
            
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="MENSAJES" />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajesListBox ID="cbMensaje" runat="server" Width="100%" Height="160px" SelectionMode="Multiple" ParentControls="ddlDistrito,ddlBase" BindIds="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>