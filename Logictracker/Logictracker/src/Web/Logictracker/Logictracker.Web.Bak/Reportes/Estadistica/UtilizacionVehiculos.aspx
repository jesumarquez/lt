<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" CodeFile="UtilizacionVehiculos.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.UtilizacionVehiculos" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                    Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:LocacionDropDownCheckList ID="lbDistrito" runat="server" Width="150px" AutoPostBack="true"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02"
                    Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:PlantaDropDownCheckList ID="lbPlanta" runat="server" AutoPostBack="true" Width="150px"
                            ParentControls="lbDistrito" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownCheckList runat="server" ID="ddcTipoVehiculo" Width="150px"
                            ParentControls="lbDistrito,lbPlanta" AutoPostBack="true"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlMovil" runat="server" UseOptionGroup="true" OptionGroupProperty="Estado"
                            Width="150px" ParentControls="lbPlanta,lbDistrito,ddcTipoVehiculo" AutoPostBack="true"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="lbDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddcTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlMovil" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="Start" Mode="Date" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="End" Mode="Date" />
                <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" MaxRange="31" StartControlID="dpDesde"
                    EndControlID="dpHasta" />
            </td>
        </tr>
    </table>
</asp:Content>
