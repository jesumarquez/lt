<%@Page Language="C#"   MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaMobileExtraHoursGraph" Codebehind="mobileExtraHoursGraph.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="200px" ParentControls="ddlDistrito" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel id="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="200px" ParentControls="ddlBase" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton id="lblMoviles" runat="server" ListControlTargetID="lbMovil" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upMovil" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMovil" runat="server" Width="250px" Height="100px" SelectionMode="Multiple" ParentControls="ddlTipoVehiculo" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />   
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblFechaDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dtpDesde" runat="server" Width="90px" IsValidEmpty="false" TimeMode="Start" Mode="Date" />
                    <br />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFechaHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dtpHasta" runat="server" Width="90px" IsValidEmpty="false" TimeMode="Start" Mode="Date" />
                    <cwc:DateTimeRangeValidator ID="dprng" runat="server" MinRange="0" MaxRange="31" EndControlID="dtpHasta" StartControlID="dtpDesde" />
                </td>
            </tr>
        </table>
</asp:Content>
