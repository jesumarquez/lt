<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Transportistas.ReportesTransportistasTransportMobileKilometers" Codebehind="TransportMobileKilometers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="150px" AddAllItem="true" />
                </td>
                <td>
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                            <br />
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="150px" AddAllItem="true" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblTransportista" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                    <br />
                    <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" Width="150px" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="80px" Mode="Date" TimeMode="Start" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="80px" Mode="Date" TimeMode="End" />
                    <cwc:DateTimeRangeValidator ID="dtrange" runat="server" MaxRange="31" MinRange="0" StartControlID="dpDesde" EndControlID="dpHasta" />
                </td>
            </tr>
        </table>
</asp:Content>