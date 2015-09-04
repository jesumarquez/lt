<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ReportGraphPage.master"
    Inherits="Logictracker.Reportes.Estadistica.Reportes_MontlyTimes" Codebehind="MonthlyTimes.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
 
    <%--FILTROS--%>
    <table width="100%" style="font-size: x-small; font-weight: bold">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" Width="150px" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" AddAllItem="true" runat="server" Width="150px"
                            AutoPostBack="true" ParentControls="ddlDistrito" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server"
                            ParentControls="ddlPlanta" Width="150px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblMovil" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="150px" ParentControls="ddlDistrito,dllPlanta,ddlTipoVehiculo"
                            UseOptionGroup="true" OptionGroupProperty="Estado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <strong>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" /></strong>
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="Start" Mode="Date" />
            </td>
            <td>
                <strong>
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" /></strong>
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100"
                    TimeMode="End" Mode="Date" />
                <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" MaxRange="31" StartControlID="dpDesde"
                    EndControlID="dpHasta" />
            </td>
        </tr>
    </table>
</asp:Content>
