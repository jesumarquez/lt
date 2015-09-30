<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="EstadoLogisticoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.CicloLogistico.EstadoLogisticoAlta" %>  

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="SelectIcon" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="220px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="60%" AutoPostBack="True" OnInitialBinding="cbEmpresa_PreBind" />
                    
                    <cwc:ResourceLabel ID="lblAsociarMensaje" runat="server" ResourceName="Labels" VariableName="ASOCIAR_MENSAJE" />
                    <asp:UpdatePanel ID="upMensaje" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:MensajesDropDownList ID="cbMensaje" runat="server" Width="60%" ParentControls="cbEmpresa" OnInitialBinding="cbMensaje_PreBind" BindIds="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="60%" MaxLength="50" />                       
                                           
                    <cwc:ResourceLabel ID="lblDemora" runat="server" ResourceName="Labels" VariableName="DEMORA" />
                    <c1:C1NumericInput ID="npDemora" runat="server" MaxValue="999" MinValue="0" Value="0" Width="60px" Height="15px" DecimalPlaces="0" />

                    <cwc:ResourceLabel ID="lblIcono" runat="server" ResourceName="Labels" VariableName="ICON" />
                    <uc1:SelectIcon ID="cbIcono" runat="server" ParentControls="cbEmpresa,cbLinea" />

               </cwc:AbmTitledPanel> 
           </td>
       </tr>
    </table>    
</asp:Content>
