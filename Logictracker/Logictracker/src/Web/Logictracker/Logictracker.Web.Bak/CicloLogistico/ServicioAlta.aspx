<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="ServicioAlta.aspx.cs" Inherits="Logictracker.CicloLogistico.AltaServicio" Title="Untitled Page" %>

<%@ Register Src="~/CicloLogistico/Controls/ServicioDetalle.ascx" TagName="ServicioDetalle" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.Tickets" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">   

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >    
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">

    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="240px">
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" Width="100%" />
                
                <asp:UpdatePanel ID="updEmpresa" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" Width="100%" OnInitialBinding="cbEmpresa_InitialBindig"  />
                    </ContentTemplate>
                </asp:UpdatePanel>
                
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100%" />
                
                    <asp:UpdatePanel ID="updLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnInitialBinding="cbLinea_InitialBindig" />
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" /></Triggers>
                    </asp:UpdatePanel>            
                    
                    <cwc:ResourceLabel ID="lblciclo" runat="server" ResourceName="Labels" VariableName="CICLO_LOGISTICO" Width="100%" />
                    
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:CicloLogisticoDropDownList ID="cbCicloLogistico" runat="server" Width="100%" ParentControls="cbLinea" AutoPostBack="true" OnSelectedIndexChanged="cbCicloLogistico_SelectedIndexChanged" OnInitialBinding="cbCicloLogistico_InitialBinding" />
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" /></Triggers>
                    </asp:UpdatePanel>    
                                
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" Width="100%" />
                    <asp:Textbox ID="txtCodigo" runat="server" Width="100%" MaxLength="32" />

                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" Width="100%" />
                <asp:Textbox ID="txtDescripcion" runat="server" Width="100%" MaxLength="128" />
                
                <cwc:ResourceLabel ID="lblVehicle" runat="server" ResourceName="Entities" VariableName="PARENTI03" Width="100%" />
                
                <asp:UpdatePanel ID="updVehiculo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="100%" AddAllItem="true" ParentControls="cbCicloLogistico" AutoPostBack="true" OnSelectedIndexChanged="cbVehiculo_SelectedIndexChanged" OnInitialBinding="cbVehiculo_InitialBinding" />
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="cbCicloLogistico" EventName="SelectedIndexChanged" /></Triggers>
                    </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblChofer" runat="server" ResourceName="Labels" VariableName="CHOFER" />               
                    <asp:UpdatePanel ID="updChofer" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:EmpleadoDropDownList ID="cbChofer" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbVehiculo" AutoPostBack="false" OnInitialBinding="cbChofer_InitialBinding" />
                        </ContentTemplate>
                        <Triggers><asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" /></Triggers>
                    </asp:UpdatePanel>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="FECHA_SALIDA" Width="100%" />                 
                    <cwc:DateTimePicker ID="dtInicio" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="false" PopupPosition="BottomRight" />
    </cwc:AbmTitledPanel>
</td>
<td style="width: 50%; vertical-align: top;">
        <%--COLUMNA DER--%>
       <cwc:AbmTitledPanel ID="panDer" runat="server" TitleVariableName="CAMPOS_EXTRA" TitleResourceName="Labels" Height="240px">
                        <asp:UpdatePanel ID="updCustom1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate><asp:Label ID="lblCustom1" runat="server"></asp:Label></ContentTemplate>
                            <Triggers><asp:AsyncPostBackTrigger ControlID="cbCicloLogistico" EventName="SelectedIndexChanged" /></Triggers>
                        </asp:UpdatePanel>    
            <asp:Textbox ID="txtCustom1" runat="server" Width="100%" MaxLength="64" />
                        <asp:UpdatePanel ID="updCustom2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate><asp:Label ID="lblCustom2" runat="server"></asp:Label></ContentTemplate>
                            <Triggers><asp:AsyncPostBackTrigger ControlID="cbCicloLogistico" EventName="SelectedIndexChanged" /></Triggers>
                        </asp:UpdatePanel>                    
            <asp:Textbox ID="txtCustom2" runat="server" Width="100%" MaxLength="64" />
                        <asp:UpdatePanel ID="updCustom3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate><asp:Label ID="lblCustom3" runat="server"></asp:Label></ContentTemplate>
                            <Triggers><asp:AsyncPostBackTrigger ControlID="cbCicloLogistico" EventName="SelectedIndexChanged" /></Triggers>
                        </asp:UpdatePanel> 
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
        <%--COLUMNA ABAJO--%>
        <cwc:AbmTitledPanel ID="panEstados" runat="server" TitleVariableName="ESTADOS" TitleResourceName="Labels">
                <asp:UpdatePanel ID="updEstados" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <asp:Panel ID="panelEstados" runat="server">
                        <uc1:ServicioDetalle ID="listaDetalles" runat="server" ParentControls="cbLinea" OnDateChanged="listaDetalles_DateChange"></uc1:ServicioDetalle>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="cbCicloLogistico" EventName="SelectedIndexChanged" />
                </Triggers>
                </asp:UpdatePanel>                
          </cwc:AbmTitledPanel>
        </td>
    </tr></table>
    </cwc:AbmTabPanel>
    
</asp:Content>

