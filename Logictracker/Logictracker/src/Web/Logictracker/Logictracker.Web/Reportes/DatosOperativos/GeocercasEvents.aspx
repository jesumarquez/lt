<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.DatosOperativos.EstadisticaGeocercasEvents"
    Title="Reporte de Geocercas" CodeBehind="GeocercasEvents.aspx.cs" %>

<%@ Register Src="~/App_Controls/Pickers/TimePicker.ascx" TagName="TimePicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ContentPlaceHolderID="Filtros" runat="server" ID="cph">
    <%--FILTROS--%>
    <asp:Panel ID="Panel1" runat="server" SkinID="FilterPanel">
        <table width="100%">
            <tr align="left" style="font-weight: bold" valign="top">
                <td>
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="125px" OnSelectedIndexChanged="DdlLocacionOnSelectedIndexChanged" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br>
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="125px" ParentControls="ddlLocacion" OnSelectedIndexChanged="DdlPlantaOnSelectedIndexChanged" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipoVehic" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoDeVehiculo" runat="server" AddAllItem="True" Width="125px" ParentControls="ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lblMobile" runat="server" ListControlTargetID="lbMobile" ResourceName="Labels" VariableName="VEHICULOS" ForeColor="Black" />
                    <br />
                    <asp:UpdatePanel ID="upMobile" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobile" runat="server" Width="125px" Height="95px" SelectionMode="Multiple" ParentControls="ddlTipoDeVehiculo, ddlPlanta" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoDeVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkTipoDomicilio" runat="server" ListControlTargetID="lbTipoDomicilio" ResourceName="Entities" VariableName="PARENTI10" ForeColor="Black" />
                    <br />
                    <asp:UpdatePanel ID="upTipoDomicilio" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:TipoReferenciaGeograficaListBox ID="lbTipoDomicilio" runat="server" Width="200px" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta" ShowAllTypes="true" OnSelectedIndexChanged="LbTipoDomicilioOnSelecteIndexChanged" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkGeocercas" runat="server" ListControlTargetID="lbGeocerca" ResourceName="Entities" VariableName="PARENTI05" ForeColor="Black" />
                    <br />
                    <asp:UpdatePanel ID="upGeocerca" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <asp:ListBox ID="lbGeocerca" runat="server" Width="200px" Height="95px" SelectionMode="Multiple" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lbTipoDomicilio" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInitDate" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpInitDate" runat="server" Width="80" TimeMode="Start" IsValidEmpty="false" Mode="Date" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblEndDate" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpEndDate" runat="server" Width="80" TimeMode="End" IsValidEmpty="false" Mode="Date" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblEnGeocerca" runat="server" ResourceName="Labels" VariableName="EN_GEOCERCA" />
                    <br />
                    <uc1:TimePicker ID="tpEnGeocerca" runat="server" IsValidEmpty="false" Width="80" DefaultTimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblEnMarcha" runat="server" ResourceName="Labels" VariableName="EN_VIAJE" />
                    <br />
                    <uc1:TimePicker ID="tpEnMarcha" runat="server" IsValidEmpty="false" Width="80" DefaultTimeMode="Start" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkCalcularKmRecorridos" runat="server" VariableName="CALCULAR_KM_RECORRIDOS" ResourceName="Labels" />
                    <br />
                    <cwc:ResourceCheckBox ID="chkPaginar" runat="server" Checked="true" VariableName="PAGINARGEOCERCAS" ResourceName="Labels" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <iframe id="ifResumenViaje" runat="server" width="100%" src="GeocercaEvents/RouteDetails.aspx" visible="false" style="border-style: none" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
    <iframe id="Iframe1" runat="server" width="100%" src="GeocercaEvents/RouteDetails.aspx" visible="false" style="border-style: none" />
</asp:Content>
