﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="Performance.aspx.cs" Inherits="Logictracker.Reportes.Bolland.Performance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
<table width="100%" style="font-size: x-small">
<tr>
    <td>
        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <br />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" />
        
        <br />
        
        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
        <br />
        <asp:UpdatePanel ID="updLinea" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        
    </td>
    <td>
        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
        <br />
        <asp:UpdatePanel ID="updTipoVehiculo" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" AddAllItem="true" runat="server" ParentControls="cbLinea" Width="200px" AutoPostBack="True"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel> 
        
        <br/>
        
        <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
        <br />
        <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="200px" ParentControls="cbTipoVehiculo"/>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="cbTipovehiculo" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
    <td>
        <table><tr><td colspan="2">
            <cwc:PeriodoDropDownList ID="cbPeriodo" runat="server" Width="260px" ParentControls="cbEmpresa" OnSelectedIndexChanged="cbPeriodo_SelectedIndexChanged" AutoPostBack="True" />
        </td></tr>
        <tr><td>
        <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" RenderMode="Inline">
            <ContentTemplate>
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="Date" />
            </ContentTemplate>
            <Triggers><asp:AsyncPostBackTrigger ControlID="cbPeriodo"/></Triggers>
        </asp:UpdatePanel>
        
        </td><td>
        
        <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
        <br />
        <div style="text-align: right;">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
            <ContentTemplate>
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="Date" />
            </ContentTemplate>
            <Triggers><asp:AsyncPostBackTrigger ControlID="cbPeriodo"/></Triggers>
        </asp:UpdatePanel>
        </div>
        
        </td></tr></table>
    </td>      
</tr>
</table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" Runat="Server">
</asp:Content>

