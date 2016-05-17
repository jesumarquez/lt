<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true"
    CodeFile="ResumenOperador.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaResumenOperador" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <%--FILTROS--%>
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
                        <cwc:PlantaDropDownList ID="ddlBase" runat="server" AddAllItem="true" ParentControls="ddlDistrito"
                            Width="125px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <asp:UpdatePanel ID="upTipoEmpleado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblTipoEmpleado" runat="server" ResourceName="Entities" VariableName="PARENTI43"
                            Font-Bold="true" />
                        <br />
                        <cwc:TipoEmpleadoDropDownList ID="ddlTipoEmpleado" runat="server" Width="125px" ParentControls="ddlBase"
                            AddNoneItem="true" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <asp:UpdatePanel ID="upEmpleado" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblEmpleado" runat="server" ResourceName="Entities" VariableName="PARENTI09"
                            Font-Bold="true" />
                        <br />
                        <cwc:EmpleadoDropDownList ID="ddlEmpleado" runat="server" Width="125px" ParentControls="ddlTipoEmpleado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoEmpleado" EventName="SelectedIndexChanged" />
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
                    EndControlID="dpHasta" MaxRange="31" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifDetalleVehiculo" visible="false" runat="server" width="100%" height="130px"
                src="ResumenOperador/DetalleOperador.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <asp:UpdatePanel ID="upConductores" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifMoviles" visible="false" runat="server" width="100%" height="750px"
                src="ResumenOperador/ListaMobiles.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleSuperiorPrint" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifDetalleVehiculoPrint" visible="false" runat="server" width="100%" height="130px"
                src="ResumenOperador/DetalleOperador.aspx" frameborder="0" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifMovilesPrint" visible="false" runat="server" width="100%" height="750px"
                src="ResumenOperador/ListaMobiles.aspx?IsPrinting=true" frameborder="0" scrolling="no" />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
