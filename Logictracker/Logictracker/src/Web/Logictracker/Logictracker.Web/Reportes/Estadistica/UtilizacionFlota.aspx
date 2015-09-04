<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.Reportes_Estadistica_UtilizacionVehiculos"
    Title="Untitled Page" Codebehind="UtilizacionFlota.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownCheckLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                    Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:LocacionDropDownCheckList ID="lbDistrito" runat="server" Width="150px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02"
                    Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:PlantaDropDownCheckList ID="lbPlanta" runat="server" AutoPostBack="true" Width="150px"
                            ParentControls="lbDistrito" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownCheckList runat="server" ID="ddcTipoVehiculo" Width="150px"
                            ParentControls="lbDistrito,lbPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddcTipoVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblCentro" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI37" />
                <br />
                <asp:UpdatePanel ID="upCentro" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:CentroDeCostosDropDownCheckList ID="ddlCentro" runat="server" Width="150px"
                            ParentControls="lbDistrito,lbPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlCentro" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="Start" Mode="Date" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="End" Mode="Date" />
                <cwc:DateTimeRangeValidator ID="dtrVal" runat="server" StartControlID="dpDesde" EndControlID="dpHasta"
                    MaxRange="31" />
            </td>
            <td align="left">
                <cwc:ResourceCheckBox ID="cbSoloImproductivos" runat="server" ResourceName="Labels"
                    VariableName="SOLO_IMPROD" Font-Bold="true" />
            </td>
        </tr>
    </table>
</asp:Content>
