<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true"
    CodeFile="ResumenVehicular.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.AccidentologiaResumenVehicular" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities"
                    VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" Width="125px" />
            </td>
            <td align="left">
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities"
                            VariableName="PARENTI02" />
                        <br />
                        <cwc:PlantaDropDownList ID="ddlBase" AddAllItem="true" runat="server" ParentControls="ddlDistrito"
                            Width="125px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17"
                            Font-Bold="true" />
                        <br />
                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server"
                            Width="125px" ParentControls="ddlBase" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                            VariableName="PARENTI03" />
                        <br />
                        <cwc:MovilDropDownList ID="ddlVehiculo" runat="server" UseOptionGroup="true" OptionGroupProperty="Estado"
                            Width="125px" ParentControls="ddlBase, ddlTipoVehiculo" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="75" TimeMode="Start"
                    Mode="Date" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA"
                    Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="75" TimeMode="End"
                    Mode="Date" />
                <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" StartControlID="dpDesde"
                    EndControlID="dpHasta" MaxRange="30" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="Server">
    <asp:UpdatePanel ID="upDetalleVehiculo" runat="server" ChildrenAsTriggers="false"
        UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifDetalleVehiculo" visible="false" runat="server" width="100%" height="130px"
                src="ResumenVehicular/DetalleVehiculo.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <asp:UpdatePanel ID="upConductores" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifConductores" visible="false" runat="server" width="100%" height="750px"
                src="ResumenVehicular/ListaConductores.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="DetalleSuperiorPrint" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true"
        UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifDetalleVehiculoPrint" visible="false" runat="server" width="100%" height="130px"
                src="ResumenVehicular/DetalleVehiculo.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
 </asp:Content>
    
    <asp:Content ID="Content5" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifConductoresPrint" visible="false" runat="server" width="100%" height="750px"
                src="ResumenVehicular/ListaConductores.aspx?IsPrinting=true" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    

</asp:Content>

