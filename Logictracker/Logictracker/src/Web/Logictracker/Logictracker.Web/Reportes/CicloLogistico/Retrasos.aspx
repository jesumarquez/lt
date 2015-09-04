<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CicloLogistico.Retrasos" Codebehind="Retrasos.aspx.cs" %>

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
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblCentroCosto" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI37" ListControlTargetID="lbCentroCosto" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:CentroDeCostosListBox runat="server" SelectionMode="Multiple" ID="lbCentroCosto" Width="150px" ParentControls="ddlLocation,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblTipoMovil" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" ListControlTargetID="lbTipoMovil" />
                <br />
                <asp:UpdatePanel ID="updTipoMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox runat="server" SelectionMode="Multiple" ID="lbTipoMovil" Width="150px" ParentControls="ddlLocation,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblMovil" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbMovil" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:MovilListBox runat="server" SelectionMode="Multiple" ID="lbMovil" Width="150px" ParentControls="ddlLocation,ddlPlanta,lbTipoMovil,lbCentroCosto" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbTipoMovil" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbCentroCosto" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblEnCiclo" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="EN_CICLO" />
                <br />
                <c1:C1NumericInput ID="npEnCiclo" runat="server" MaxValue="1000" MinValue="0" Value="60" Width="50px" DecimalPlaces="0" Height="15px" />
                <br />
                <cwc:ResourceLabel ID="lblEnGeocerca" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="EN_GEOCERCA" />
                <br />
                <c1:C1NumericInput ID="npEnGeocerca" runat="server" MaxValue="1000" MinValue="0" Value="60" Width="50px" DecimalPlaces="0" Height="15px" />
            </td>
        </tr>
    </table>
</asp:Content>
