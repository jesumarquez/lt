<%@ Page Title="Conciliaciones" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="Conciliaciones.aspx.cs" Inherits="Logictracker.Reportes.CombustibleEnPozos.Reportes_CombustibleEnPozos_Conciliaciones" %>  

<%@ Register Src="~/App_Controls/Pickers/DateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">  
<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
<cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="200px">
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnInitialBinding="cbEmpresa_PreBind" />
                    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true"
                            OnInitialBinding="cbLinea_PreBind"/>
                        </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblParenti19" runat="server" ResourceName="Entities" VariableName="PARENTI19" />
                    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:EquipoDropDownList ID="cbEquipo" runat="server" Width="100%" ParentControls="cbEmpresa, cbLinea" AddAllItem="true"
                             OnInitialBinding="cbEquipo_PreBind"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                     </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblParenti36" runat="server" ResourceName="Entities" VariableName="PARENTI36" />
                    <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TanqueDropDownList ID="cbTanque" runat="server" Width="100%" AllowEquipmentBinding="true"
                            OnInitialBinding="cbTanque_PreBind" ParentControls="cbEmpresa,cbLinea,cbEquipo" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
</cwc:AbmTitledPanel>
</td>
<td style="width: 50%; vertical-align: top;">
<cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="DETALLES_CONCILIACION" TitleResourceName="Labels" Height="200px">
    
                <cwc:ResourceLabel ID="lblTipoConciliacion" runat="server" ResourceName="Entities" VariableName="OPECOMB01" />
               <asp:UpdatePanel runat="server" ID="UpdatePanel4" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TipoMovimientoDropDownList ID="ddlTipoMov" runat="server" Width="100%"
                         OnInitialBinding="ddlTipoMov_PreBind" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoMov" EventName="SelectedIndexChanged" />
                    </Triggers>
               </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblObservacion" runat="server" ResourceName="Labels" VariableName="REM_OBS" />  
                <asp:TextBox ID="txtObservacion" runat="server" Width="100%" MaxLength="20" />
                <cwc:ResourceLabel ID="lblMotivo" runat="server" ResourceName="Labels" VariableName="MOTIVO_CONCILIACION" />
                <asp:UpdatePanel runat="server" ID="upMotivo" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MotivoConciliacionDropDownList ID="ddlMotivoConciliacion" runat="server" Width="100%"
                        OnInitialBinding="ddlMotivoConciliacion_PreBind" OnSelectedIndexChanged="ddlMotivoConciliacion_SelectedIndexChanged" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlMotivoConciliacion" EventName="SelectedIndexChanged" />
                    </Triggers>
               </asp:UpdatePanel>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                <asp:UpdatePanel runat="server" ID="UpdatePanel5" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="100%" ParentControls= "cbEmpresa, cbLinea" 
                         OnInitialBinding="ddlMovil_PreBind"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlMotivoConciliacion" EventName="SelectedIndexChanged" />
                    </Triggers>
               </asp:UpdatePanel>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="FECHA" />
                <cwc:DateTimePicker ID="dpFecha" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Now" Width="120px" />
                <cwc:ResourceLabel ID="lblVolumen" runat="server" ResourceName="Labels" VariableName="VOLUMEN" />
                <c1:C1NumericInput ID="npVolumen" runat="server" Width="120px" MinValue="0" DecimalPlaces="2" Value="0" Height="15px" />
</cwc:AbmTitledPanel>
</td>
</tr>
</table>
</asp:Content>

