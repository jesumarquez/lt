<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"  
    Inherits="Logictracker.Parametrizacion.ParametrizacionEstadoLogAlta" Codebehind="EstadoLogAlta.aspx.cs" %>  

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="SelectIcon" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="220px">
                        <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AutoPostBack="True" OnInitialBinding="cbEmpresa_PreBind" />
                        <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AutoPostBack="True" ParentControls="cbEmpresa"
                                    OnInitialBinding="cbLinea_PreBind" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <cwc:ResourceLabel ID="lblAsociarMensaje" runat="server" ResourceName="Labels" VariableName="ASOCIAR_MENSAJE" />
                        <asp:UpdatePanel ID="upMensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:MensajeLogisticoDropDownList ID="cbMensaje" runat="server" Width="100%" ParentControls="cbLinea"
                                    OnInitialBinding="cbMensaje_PreBind" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="50" />
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <c1:C1NumericInput ID="npCodigo" runat="server" MaxValue="99" MinValue="0" Value="0" Width="125px" Height="15px" DecimalPlaces="0" />
               </cwc:AbmTitledPanel> 
           </td>
                
           <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA DER--%>
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CONFIGURACION" TitleResourceName="Labels" Height="220px">
                        <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Labels" VariableName="TYPE" />
                        <cwc:TipoLogisticoDropDownList ID="cbTipo" runat="server" Width="100%" OnInitialBinding="cbTipo_PreBind" />
                        <cwc:ResourceRadioButton ID="radManual" runat="server" Checked="True" GroupName="grpModo" ResourceName="Labels"
                            VariableName="MANUAL" />
                        <cwc:ResourceRadioButton ID="radAutomatico" runat="server" GroupName="grpModo" ResourceName="Labels" VariableName="AUTOMATICO" />
                        <cwc:ResourceLabel ID="lblInformarCMD" runat="server" ResourceName="Labels" VariableName="INFORMAR_CMD" />
                        <asp:CheckBox ID="chkInformar" runat="server" />
                        <cwc:ResourceLabel ID="lblIcono" runat="server" ResourceName="Labels" VariableName="ICON" />
                        <uc1:SelectIcon ID="SelectIcon1" runat="server" ParentControls="cbEmpresa,cbLinea" />
                        <cwc:ResourceLabel ID="lblDeltaTime" runat="server" ResourceName="Labels" VariableName="DELTA_TIME" />
                        <c1:C1NumericInput ID="npDeltaTime" runat="server" MaxValue="999" MinValue="0" Value="0" Width="125px" Height="15px" DecimalPlaces="0" />
                        <cwc:ResourceLabel ID="lblOrden" runat="server" ResourceName="Labels" VariableName="ORDER" />
                        <c1:C1NumericInput ID="npOrden" runat="server" MaxValue="999" MinValue="0" Value="0" Width="125px" Height="15px" DecimalPlaces="0" />
          </cwc:AbmTitledPanel>
       </td>
       </tr>
    </table>
    
    <%--HACK TIPO ASYNC--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate />
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbTipo" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
