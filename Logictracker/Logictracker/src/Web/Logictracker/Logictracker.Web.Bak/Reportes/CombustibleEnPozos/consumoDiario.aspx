<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" CodeFile="consumoDiario.aspx.cs" Inherits="Logictracker.Reportes.CombustibleEnPozos.Reportes_CombustibleEnPozos_consumoDiario" Title="Consumo Diario"%>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">  
<table width="100%">
    <tr>
        <td align="left">
             <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
            <br />
            <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="200px" AddAllItem="true" />
            <br />
             <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
            <br />
            <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" Width="200px" ParentControls="ddlLocation" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td>
            <asp:UpdatePanel ID="upEquipo" runat="server" UpdateMode="Conditional" >
                <ContentTemplate>
                    <cwc:ResourceLabel ID="lblEquipo" runat="server" ResourceName="Entities" VariableName="PARENTI19" Font-Bold="true" />
                    <br />
                    <cwc:EquipoDropDownList ID="ddlEquipo" runat="server" ParentControls="ddlPlanta" AddAllItem="true" Width="200px" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
           <asp:UpdatePanel ID="upTanque" runat="server">
                <ContentTemplate>
                    <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
                    <br />
                    <cwc:TanqueDropDownList id="ddlTanque" runat="server" ParentControls="ddlEquipo" AllowBaseBinding="false" AllowEquipmentBinding="true" Width="200px" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlTanque" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td align="left">
            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
            <br />
            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125px" TimeMode="Start" Mode="DateTime" />
            <br />
            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
            <br />
            <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125px" TimeMode="End" Mode="DateTime" />
            <cwc:DateTimeRangeValidator ID="rngDate" runat="server" MaxRange="31" StartControlID="dpDesde" EndControlID="dpHasta" />
        </td>
    </tr>                
</table>
</asp:Content>

