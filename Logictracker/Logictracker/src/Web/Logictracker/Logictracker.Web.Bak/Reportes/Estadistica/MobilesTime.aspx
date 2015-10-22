<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true"
    CodeFile="MobilesTime.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.EstadisticaMobilesTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                    Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" AddAllItem="true" runat="server" Width="200px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02"
                    Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" AddAllItem="true" Width="200px"
                            AutoPostBack="true" ParentControls="ddlLocacion" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server"
                            ParentControls="ddlPlanta" Width="200px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:ResourceLinkButton ID="lnkMovil" runat="server" Font-Bold="true" ListControlTargetID="lbMobiles" ResourceName="Labels" VariableName="VEHICULOS" ForeColor="Black" />
                        <br />
                        <cwc:MovilListBox ID="lbMobiles" runat="server" Width="250px" Height="90px" SelectionMode="Multiple" ParentControls="ddlTipoVehiculo" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbMobiles" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top" align="left">
                <strong>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                </strong>
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="DateTime" />
                <br />
                <br />
                <strong>
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                </strong>
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="DateTime" />
                <cwc:DateTimeRangeValidator ID="dpValidator" runat="server" EndControlID="dpHasta" MaxRange="31" StartControlID="dpDesde" />
            </td>
        </tr>
    </table>
</asp:Content>
