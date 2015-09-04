<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.Estadistica.KmHsProductivos" Title="Km y Horas Productivas" Codebehind="KmHsProductivos.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td>
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                            <br />
                            <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="160px" AddAllItem="false" />
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                            <br />
                            <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:TipoVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="160px" AddAllItem="true" ParentControls="ddlLocacion,ddlPlanta" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel> 
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                            <br />
                            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Width="120" />
                        </td>
                        <td valign="top" rowspan="2">
                            <cwc:ResourceRadioButton ID="rbutSinDetalle" runat="server" ResourceName="Labels" VariableName="SIN_DETALLE" GroupName="Detalle" Font-Bold="true" />
                            <br />
                            <cwc:ResourceRadioButton ID="rbutVerTurnos" runat="server" ResourceName="Labels" VariableName="VER_DETALLE_TURNOS" GroupName="Detalle" Font-Bold="true" />
                            <br />
                            <cwc:ResourceRadioButton ID="rbutVerViajes" runat="server" ResourceName="Labels" VariableName="VER_DETALLE_VIAJES" GroupName="Detalle" Font-Bold="true" />
                            <br />
                            <cwc:ResourceRadioButton ID="rbutVerTimeTracking" runat="server" ResourceName="Labels" VariableName="VER_DETALLE_TIME_TRACKING" GroupName="Detalle" Font-Bold="true" />
                            <br />
                            <cwc:ResourceRadioButton ID="rbutVerTickets" runat="server" ResourceName="Labels" VariableName="VER_DETALLE_TICKETS" GroupName="Detalle" Font-Bold="true" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                            <br />
                            <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
                                    <cwc:PlantaDropDownList id="ddlPlanta" runat="server" Width="160px" ParentControls="ddlLocacion" AddAllItem="true" />
                                </contenttemplate>
                                <triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                </triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td valign="top">
                            <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ListControlTargetID="ddlVehiculo" Font-Bold="true" ForeColor="Black" ResourceName="Labels" VariableName="VEHICULOS" />
                            <br />
                            <asp:UpdatePanel runat="server" ID="upVehiculo" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <cwc:MovilListBox ID="ddlVehiculo" runat="server" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTipoVehiculo" Width="175px" AutoPostBack="false" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td valign="top">
                            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                            <br />
                            <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" Width="120" />
                        </td>
                    </tr>                
                </table>
            </td>           
        </tr>        
    </table>
</asp:Content>
