<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true"
    CodeFile="StoppedHours.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaStoppedHours" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <asp:Panel ID="panel1" runat="server" SkinID="FilterPanel">
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                        Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02"
                        ResourceName="Entities" />
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="200px" ParentControls="ddlDistrito"
                                AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipo" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="200px"
                                ParentControls="ddlBase" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblMoviles" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI03" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upMovil" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="200px" UseOptionGroup="true" OptionGroupProperty="Estado" ParentControls="ddlTipoVehiculo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <br />
                    <cwc:ResourceCheckBox ID="chkUndefined" Font-Bold="true" runat="server" Checked="false"
                        ResourceName="Labels" VariableName="INCLUYE_INDEFINIDO" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblFechaDesde" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dtpDesde" runat="server" Width="120" TimeMode="Start" IsValidEmpty="false" />
                    <br />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFechaHasta" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dtpHasta" runat="server" Width="120" TimeMode="End" IsValidEmpty="false" />
                    <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" MaxRange="1" StartControlID="dtpDesde"
                        EndControlID="dtpHasta" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    
</asp:Content>
