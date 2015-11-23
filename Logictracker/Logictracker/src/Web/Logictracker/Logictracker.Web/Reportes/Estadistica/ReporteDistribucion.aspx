<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.Estadistica.ReporteDistribucion" Title="Reporte de Distribución" Codebehind="ReporteDistribucion.aspx.cs" %>

<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="150px" AddAllItem="false" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="150px" ParentControls="ddlLocacion" AddAllItem="true" />
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkTransportistas" runat="server" ListControlTargetID="ddlTransportista" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" />
                <br />
                <asp:UpdatePanel ID="upTransportista" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistasListBox ID="ddlTransportista" runat="server" AddNoneItem="true" SelectionMode="Multiple" Width="150px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkEstados" runat="server" ListControlTargetID="ddlEstados" ResourceName="Labels" VariableName="ESTADOS" Font-Bold="true" ForeColor="Black" />
                <br />
                <cwc:EstadoEntregaDistribucionListBox ID="ddlEstados" runat="server" SelectionMode="Multiple" Width="150px" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ListControlTargetID="ddlVehiculo" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black"/>
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="ddlVehiculo" runat="server" Width="150px" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTransportista" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            
            <td valign="top">
                <cwc:ResourceLabel ID="lnkRutas" runat="server" ResourceName="Labels" VariableName="RUTAS" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upRutas" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:AutoCompleteTextBox ID="ddlRuta" runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetRutas" Width="150px" ParentControls="ddlLocacion,ddlPlanta,ddlTransportista,ddlVehiculo" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceCheckBox ID="chkVerOrdenManual" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_ORDEN_MANUAL" />
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPuntoEntrega" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:AutoCompleteTextBox ID="ddlPuntoEntrega" runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetPuntosEntrega" Width="150px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br/>
                <cwc:ResourceCheckBox runat="server" ID="chkVerConfirmacion" ResourceName="Labels" VariableName="VER_CONFIRMACION" Font-Bold="True" />
                <br/>
                <cwc:ResourceCheckBox runat="server" ID="chkInteraccionGarmin" ResourceName="Labels" VariableName="VER_INTERACCION_GARMIN" Font-Bold="True" />
                <br/>
                <cwc:ResourceCheckBox runat="server" ID="chkVerDescripcion" ResourceName="Labels" VariableName="VER_DESCRIPCION" Font-Bold="True" />
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Width="110" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" Width="110" />
                <%--<cwc:DateTimeRangeValidator runat="server" ID="dtvalidator" StartControlID="dpDesde" EndControlID="dpHasta" MaxRange="23:59" />--%>
            </td>
        </tr>
    </table>
</asp:Content>
