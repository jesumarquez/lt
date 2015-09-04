<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Reportes.Estadistica.KilometrosDiarios" Codebehind="KilometrosDiarios.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%">
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" />
                <br />
                <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="150px" ParentControls="ddlLocation" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                
                <cwc:ResourceLabel ID="lblTransportista" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList runat="server" ID="ddlTransportista" Width="150px" ParentControls="ddlLocation,ddlPlanta" AutoPostBack="false" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lblMovil" runat="server" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbMovil" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:MovilListBox runat="server" SelectionMode="Multiple" ID="lbMovil" Width="250px" Height="100px" ParentControls="ddlLocation,ddlPlanta,ddlTransportista" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br/>
                <cwc:DateTimePicker ID="dtDesde" runat="server" Width="75px" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                <br/>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br/>
                <cwc:DateTimePicker ID="dtHasta" runat="server" Width="75px" IsValidEmpty="false" Mode="Date" TimeMode="End" />
            </td>
        </tr>
    </table>
</asp:Content>
