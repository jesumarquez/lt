<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CombustibleEnPozos.ReportesControlDeCombustiblePozosDespachosPozo" Codebehind="consumosPozos.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">        
<table width="100%" >
<tr>
    <td >
        <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
        <br />
        <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" AddAllItem="true" Width="200px" />
        <br />
        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
        <br />
        <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" ParentControls="ddlLocacion" AddAllItem="true" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <cwc:ResourceLabel ID="lblEquipo" runat="server" ResourceName="Entities"  VariableName="PARENTI19" Font-Bold="true" />
        <br />
        <asp:UpdatePanel ID="upEquipos" runat="server" UpdateMode="Conditional" >
            <ContentTemplate>
                <cwc:EquipoDropDownList ID="ddlEquipo" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion, ddlPlanta" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
    <td >
        <cwc:ResourceLinkButton ID="btnMotores" runat="server" ResourceName="Entities"  VariableName="PARENTI39" Font-Bold="true" OnClick="btnMotores_Click"
                    ListControlTargetID="lbMotores" />
        <br />
        <asp:UpdatePanel ID="upMotor" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
                <cwc:CaudalimetroListBox ID="lbMotores" runat="server" AutoPostBack="false" SelectionMode="Multiple" Width="250px" Height="100px" 
                                        ParentControls="ddlEquipo,ddlPlanta,ddlLocacion" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </td>
    <td >
        <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
        <br />
        <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="120px" Mode="DateTime" TimeMode="Start" />
        <br />
        <br />
        <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
        <br />
        <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="120px" Mode="DateTime" TimeMode="End" />
</tr>                
</table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="Server">  
 <div style="text-align: center">
        <br />                               
        <cwc:ResourceLabel ID="lblTitle" runat="server" Font-Bold="true" Font-Size="10" ResourceName="Entities" VariableName="PARENTI37" Visible="false" />
        <br />
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <table  id="tblTotal"  runat="server" width="100%" style="font-size: x-small; 
                        border-color: Black; border-style: solid; border-width: 1px" visible="false" >
        <tr>
            <td>
                <asp:Label  ID="lblTotal" runat="server" Font-Size="8"  Font-Bold="true" />
            </td>
        </tr>
    </table>
    <br />
    <div style="text-align: center">        
                <cwc:ResourceLabel ID="lblMobiles" runat="server" Font-Bold="true" Font-Size="10" Visible="false"
                                      ResourceName="Labels" VariableName="CONSUMOS_DETALLE" />
                <br />
                <iframe id="ifMobiles" runat="server" width="100%" src="Consumos/ConsumosPorMotor.aspx?IsPrinting=false"
                    visible="false" style="border-style: none" height="600px" />
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="DetalleInferiorPrint" runat="server">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="text-align: center; width: auto;">
                <br />
                <br />   
                <asp:Label  ID="lblTotalPrint" runat="server" Font-Size="8" Font-Bold="true" />
                <br />
            </div>
            <br />
            <div style="text-align: center; width: auto; page-break-before:always;">
                <cwc:ResourceLabel ID="lblDetallePrint" runat="server" Font-Bold="true" Font-Size="10" Visible="false"
                                          ResourceName="Labels" VariableName="CONSUMOS_DETALLE" />
                <br />
                <iframe id="ifMobilesPrint" visible="false" runat="server" width="100%" height="100%"
                src="Consumos/ConsumosPorMotor.aspx?IsPrinting=true" frameborder="0" scrolling="no" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
