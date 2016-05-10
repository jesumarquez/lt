<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ReportesOdometrosReporteOdometros" Codebehind="ReporteOdometros.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%">
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                <br />
                <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="150px" ParentControls="ddlLocation" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblDepto" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI04" />
                <br />
                <asp:UpdatePanel ID="upDepto" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:DepartamentoDropDownList runat="server" ID="ddlDepto" AddAllItem="true" Width="150px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblTipo" runat="server" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI17"  ListControlTargetID="lbTipo" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox runat="server" SelectionMode="Multiple" ID="lbTipo" Width="150px" Height="90px" ParentControls="ddlPlanta" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblMovil" runat="server" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbMovil" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:MovilListBox runat="server" SelectionMode="Multiple" ID="lbMovil" Width="150px" Height="90px" ParentControls="ddlLocacion,ddlPlanta,ddlDepto,lbTipo" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDepto" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbTipo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblOdometro" runat="server" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI40" ListControlTargetID="lbOdometro"/>
                <br />
                <asp:UpdatePanel ID="upOdometro" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:OdometroListBox SelectionMode="Multiple" runat="server" ID="lbOdometro" Width="150px" Height="90px" AddGeneral="true" ParentControls="ddlPlanta, lbTipo" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbTipo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceCheckBox ID="chkVencimiento" runat="server" ResourceName="Labels" VariableName="POR_VENCER" />
                <cwc:BaloonTip ID="bPorVencer" runat="server" ResourceName="Labels" VariableName="BALOON_POR_VENCER" />
                <br />
                <cwc:ResourceCheckBox ID="chkConDispositivo" runat="server" ResourceName="Labels" VariableName="SOLO_DISPOSITIVOS" Checked="True" />
            </td>
        </tr>
    </table>
</asp:Content>
