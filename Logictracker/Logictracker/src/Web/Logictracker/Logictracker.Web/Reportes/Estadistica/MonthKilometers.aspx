<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ReportesMonthKilometers" Title="Reporte Kilometraje Mensual" Codebehind="MonthKilometers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small; font-weight: bold">
        <tr align="left">
            <td valign="top">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" Width="150px" />
                <br /><br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" AddAllItem="true" runat="server" Width="150px" AutoPostBack="true" ParentControls="ddlDistrito" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="ddlTransportista" AddAllItem="true" runat="server" ParentControls="ddlPlanta" Width="150px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br/>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server" ParentControls="ddlPlanta" Width="150px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblMovil" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="150px" ParentControls="ddlTipoVehiculo,ddlTransportista" UseOptionGroup="true" OptionGroupProperty="Estado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br/>
                <cwc:ResourceCheckBox ID="chkSoloEnRuta" runat="server" ResourceName="Labels" VariableName="FILTRAR_EN_RUTA" Font-Bold="True" />
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="Date" />
                <br /><br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="Date" />
                <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" MaxRange="31" StartControlID="dpDesde" EndControlID="dpHasta" />
                <cwc:DateTimeRangeValidator ID="dpValidator" runat="server" EndControlID="dpHasta" MaxRange="31" StartControlID="dpDesde" />
            </td>
        </tr>
    </table>
</asp:Content>
