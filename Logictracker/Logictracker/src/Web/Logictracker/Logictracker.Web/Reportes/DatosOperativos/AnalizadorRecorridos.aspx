<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.DatosOperativos.Reportes_DatosOperativos_AnalizadorRecorridos" Title="Untitled Page" Codebehind="AnalizadorRecorridos.aspx.cs" %>

<%@ Register Src="~/App_Controls/Pickers/TimePicker.ascx" TagName="TimePicker" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="Filtros" runat="server" ID="cph">
        <table width="100%">
            <tr align="left" style="font-weight: bold" valign="top">
                <td>
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" ParentControls="ddlLocacion" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipoVehic" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoDeVehiculo" runat="server" Width="200px"
                                ParentControls="ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lblMobile" runat="server" ListControlTargetID="lbMobile" ResourceName="Labels"
                        VariableName="VEHICULOS" />
                    <br />
                    <asp:UpdatePanel ID="upMobile" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobile" runat="server" Width="200px" Height="95px" SelectionMode="Multiple"
                                ParentControls="ddlTipoDeVehiculo, ddlPlanta" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoDeVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lbTipoMensaje" runat="server" ResourceName="Entities" VariableName="PARENTI16" />
                    <br />
                    <asp:UpdatePanel ID="upTipoMensaje" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoMensajeDropDownList ID="ddlTipoMensaje" runat="server" Width="200px" ParentControls="ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lbMobile" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lbMensajes" runat="server" ResourceName="Labels" VariableName="EVENTO_INICIO" />
                    <br />
                    <asp:UpdatePanel ID="upMensajeOrigen" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajesDropDownList ID="ddlMensajeOrigen" runat="server" Width="200px" ParentControls="ddlTipoMensaje" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lbMensajeFin" runat="server" ResourceName="Labels" VariableName="EVENTO_FIN" />
                    <br />
                    <asp:UpdatePanel ID="upMensajeFin" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajesDropDownList ID="ddlMenasjeFin" runat="server" Width="200px" ParentControls="ddlTipoMensaje" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInitDate" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpInitDate" runat="server" Width="80" TimeMode="Start" IsValidEmpty="false"
                        Mode="Date" />
                    <br />
                    <cwc:ResourceLabel ID="lblEndDate" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpEndDate" runat="server" Width="80" TimeMode="End" IsValidEmpty="false"
                        Mode="Date" />
                    <br />
                    <cwc:ResourceLabel ID="lblEnGeocerca" runat="server" ResourceName="Labels" VariableName="DURACION" />
                    <br />
                    <uc1:TimePicker ID="tpDuracion" runat="server" IsValidEmpty="false" Width="80" DefaultTimeMode="Start" />
                </td>
            </tr>
        </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <asp:UpdatePanel ID="updInf" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <iframe id="ifResumenViaje" runat="server" width="100%" src="GeocercaEvents/RouteDetails.aspx"
                visible="false" style="border-style: none" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
