<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.ControlDeAccesos.AccessControl" Title="Untitled Page" Codebehind="AccessControl.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%">
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI01" />
                <br />
                <asp:UpdatePanel ID="upLocacion" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="160px" AutoPostBack="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="160px" ParentControls="ddlLocacion"
                            AddAllItem="true" AutoPostBack="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="rbEmpleados" runat="server" ResourceName="Entities" VariableName="PARENTI09"
                    ListControlTargetID="lbEmpleado" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:EmpleadoListBox ID="lbEmpleado" runat="server" Width="160px" ParentControls="ddlLocacion,ddlPlanta"
                            SelectionMode="Multiple" AutoPostBack="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="rbEmpleados" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start"
                    Width="120" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="120"
                    TimeMode="Now" />
            </td>
        </tr>
    </table>
</asp:Content>
