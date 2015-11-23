<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.Medidores" Codebehind="Medidores.aspx.cs" %>

<%@ Register TagPrefix="uc2" TagName="Filtros" Src="../../App_Controls/FiltroDetalles.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="true" runat="server" Width="200px" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa, cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="PERIODO" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFecha" runat="server" IsValidEmpty="false" Mode="Month" TimeMode="Start" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI79" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upEntidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:EntidadListBox ID="cbEntidad" runat="server" Width="200px" SelectionMode="Multiple" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea,cbTipoEntidad" OnSelectedIndexChanged="CbEntidadSelectedIndexChanged" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblSubEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI81" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upSubEntidad" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:SubEntidadListBox ID="cbSubEntidad" runat="server" Width="200px" SelectionMode="Multiple" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea,cbTipoEntidad,cbEntidad" TipoMedicion="NU" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td colspan="2" align="center" valign="top">
                <asp:UpdatePanel ID="upFiltros" runat="server" Visible="true">
                    <ContentTemplate>
                        <uc2:Filtros ID="ctrlFiltros" runat="server" />    
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                    </Triggers>                
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    
    <asp:HiddenField ID="hidMes" runat="server" Value="-1" />
    <asp:HiddenField ID="hidDia" runat="server" Value="-1" />
    <asp:Button ID="btnMes" runat="server" OnClick="ClickMes" style="display: none;" />
    <asp:Button ID="btnDia" runat="server" OnClick="ClickDia" style="display: none;" />
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">   
    <asp:UpdatePanel ID="upCharts" runat="server">   
        <ContentTemplate>
            <table width="100%">
                <tr align="center">
                    <td>
                        <div id="divChartDias" runat="server" />
                    </td>
                </tr>
            </table>
            
            <table width="100%">
                <tr align="center">
                    <td>
                        <div id="divChartHoras" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>