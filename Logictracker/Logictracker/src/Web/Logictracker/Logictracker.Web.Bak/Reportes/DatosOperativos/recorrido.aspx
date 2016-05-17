<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="recorrido.aspx.cs" Inherits="Logictracker.Reportes.DatosOperativos.ReportesRecorrido" Title="Reporte de recorrido" %>

<%@ Register Src="~/App_Controls/Pickers/NumberPicker.ascx" TagName="NumberPicker" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <%--FILTROS--%>
        <table width="100%" style="font-size: x-small">
            <tr align="left">
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" AddAllItem="true" Width="175px" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESDE"
                        Font-Bold="true" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="140" TimeMode="Start" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" AddAllItem="true" runat="server" Width="175px"
                                AutoPostBack="true" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="HASTA"
                        Font-Bold="true" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="140" TimeMode="End" />
                    <cwc:DateTimeRangeValidator runat="server" StartControlID="dpDesde" EndControlID="dpHasta" MaxRange="31" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server"
                                ParentControls="ddlPlanta" Width="175px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <cwc:ResourceLabel ID="lblStoppedDistance" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="DISTANCIA" />
                    <br />
                    <uc3:NumberPicker ID="npDistance" runat="server" Mask="9999" MaximumValue="9999"
                        Number="100" Width="175" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblMovil" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI03" />
                    <br />
                    <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="175px" ParentControls="ddlTipoVehiculo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <table style="width: 175px;">
                    <tr><td>
                    <cwc:ResourceLabel ID="lblStoppedTime" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="DETENCION" />
                    <br />
                    <uc3:NumberPicker ID="npStopped" runat="server" Mask="999" MaximumValue="999" Number="1"
                        Width="60" />
                    </td><td style="text-align: right;">
                        <cwc:ResourceLabel ID="lblNoReport" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="SIN_REPORTAR" />
                    <br />
                    <uc3:NumberPicker ID="numNoReport" runat="server" Mask="999" MaximumValue="999" Number="15"
                        Width="60" />
                    </td></tr></table>
                </td>
            </tr>
        </table>        
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <%--    <asp:UpdatePanel ID="updInf" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>--%>
    <table width="100%">
        <tr align="center" style="font-size: x-small">
            <td>
                <cwc:ResourceLabel ID="lblMovimiento" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="EN_MOVIMIENTO" Visible="False" />
                <asp:Label ID="lblMovement" runat="server" Visible="False" />
                <cwc:ResourceLabel ID="lblDetenido" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="DETENIDO" Visible="False" />
                <asp:Label ID="lblStopped" runat="server" Visible="False" />
                <cwc:ResourceLabel ID="lblDistancia" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="DISTANCIA" Visible="False" />
                <asp:Label ID="lblDistance" runat="server" Visible="False" />
                <cwc:ResourceLabel ID="lblVelocidadPromedio" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="VELOCIDAD_PROMEDIO" Visible="False" />
                <asp:Label ID="lblAverageSpeed" runat="server" Visible="False" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
    <%--    <asp:UpdatePanel ID="updInf" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>--%>
    <table width="100%">
        <tr align="center" style="font-size: x-small">
            <td>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="EN_MOVIMIENTO" Visible="true" />
                <asp:Label ID="lblMovementPrint" runat="server" Visible="true" />
                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="DETENIDO" Visible="true" />
                <asp:Label ID="lblStoppedPrint" runat="server" Visible="true" />
                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="DISTANCIA" Visible="true" />
                <asp:Label ID="lblDistancePrint" runat="server" Visible="true" />
                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" Font-Bold="true" ResourceName="Labels"
                    VariableName="VELOCIDAD_PROMEDIO" Visible="true" />
                <asp:Label ID="lblAverageSpeedPrint" runat="server" Visible="true" />
            </td>
        </tr>
    </table>
    
    
</asp:Content>