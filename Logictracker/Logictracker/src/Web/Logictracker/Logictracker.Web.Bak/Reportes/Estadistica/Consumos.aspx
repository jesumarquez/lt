<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="Consumos.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.Consumos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%">
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="False" />
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
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblCentroDeCostos" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI37" ListControlTargetID="lbCentroDeCostos" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:CentroDeCostosListBox runat="server" SelectionMode="Multiple" ID="lbCentroDeCostos" Width="150px" ParentControls="ddlLocation,ddlPlanta" AutoPostBack="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17"  ListControlTargetID="lbTipo" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox runat="server" SelectionMode="Multiple" ID="lbTipo" Width="150px" ParentControls="ddlPlanta" AutoPostBack="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblMovil" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbMovil" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:MovilListBox runat="server" SelectionMode="Multiple" ID="lbMovil" Width="150px" ParentControls="ddlLocation,ddlPlanta,lbTipo" UseOptionGroup="true" OptionGroupProperty="Estado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbTipo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
