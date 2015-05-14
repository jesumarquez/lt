<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="CicloLogisticoAlta.aspx.cs" Inherits="Logictracker.CicloLogistico.CicloLogisticoAlta" Title="Untitled Page" %>  
    
<%@ Register Src="~/CicloLogistico/Controls/CicloDetalle.ascx" TagName="CicloDetalle" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Labels" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Panels" Assembly="Logictracker.Web.CustomWebControls" %>   

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">

    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="240px">
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" Width="100%" />
                
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" Width="100%" OnInitialBinding="cbEmpresa_InitialBindig" />
                
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100%" />

                    <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnInitialBinding="cbLinea_InitialBindig" />
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" /></Triggers>
                    </asp:UpdatePanel>
                    
                <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" Width="100%" />
                
                <asp:Textbox ID="txtCodigo" runat="server" Width="100%" MaxLength="32" />

                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                
                <asp:Textbox ID="txtDescripcion" runat="server" Width="100%" MaxLength="128" />
                
                <asp:CheckBox ID="chkCiclo" runat="server" Text="Es Ciclo Logistico" Checked="true" />

                <asp:CheckBox ID="chkEstado" runat="server" Text="Es Estado" />
</cwc:AbmTitledPanel>
</td>
<td style="vertical-align: top; width: 50%;">
        <%--COLUMNA DER--%>
        <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CAMPOS_EXTRA" TitleResourceName="Labels" Height="240px">
                    Campo Personalizado 1
                    <asp:Textbox ID="txtCustom1" runat="server" Width="100%" MaxLength="64" />
                    Campo Personalizado 2
                    <asp:Textbox ID="txtCustom2" runat="server" Width="100%" MaxLength="64" />
                    Campo Personalizado 3
                    <asp:Textbox ID="txtCustom3" runat="server" Width="100%" MaxLength="64" />
       </cwc:AbmTitledPanel>
</td>
</tr>
</table>
</cwc:AbmTabPanel>


<cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="ESTADOS" >
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 100%; vertical-align: top;" colspan="2">
        <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="ESTADOS" TitleResourceName="Labels" Height="240px">         
                <asp:UpdatePanel ID="updEstados" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="panelEstados" runat="server">
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btAgregarEstado" EventName="Click" />
                </Triggers>
                </asp:UpdatePanel>
                
                <%--<div style="background-color: #CCCCCC; border: solid 1px #999999; padding: 5px;margin: 2px;"/>--%>
                <asp:LinkButton ID="btAgregarEstado" runat="server" Text="+ Agregar nuevo estado" OnClick="btAgregarEstado_Click"></asp:LinkButton> 
                               
           </cwc:AbmTitledPanel>
        </td>
    </tr></table>
</cwc:AbmTabPanel>
</asp:Content>
