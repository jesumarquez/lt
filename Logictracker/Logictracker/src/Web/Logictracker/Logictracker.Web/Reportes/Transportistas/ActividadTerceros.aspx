<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.Transportistas.EstadisticaActividadTerceros" MasterPageFile="~/MasterPages/ReportGridPage.master" Codebehind="ActividadTerceros.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">   
        <table width="100%" style="font-size: x-small">
            <tr valign="top" align="left">
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="180px" AutoPostBack="True" AddAllItem="true" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="180px" ParentControls="ddlDistrito" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lblTransport" runat="server" ListControlTargetID="lbTransport" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" ForeColor="Black" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upTercero" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:TransportistasListBox ID="lbTransport" runat="server" AutoPostBack="false" Width="180px" ParentControls="ddlBase" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" Width="100px" Mode="DateTime" TimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" Width="100px" Mode="DateTime" TimeMode="End" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblKm" runat="server" Font-Bold="true" VariableName="KM_SUPERADOS" ResourceName="Labels" />
                    <br />
                    <c1:C1NumericInput ID="npKm" runat="server" Value="0" MaxValue="999" MinValue="0" Width="90px" DecimalPlaces="0" Height="15px" />
                </td>
            </tr>
        </table>
    <br />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <br />
    <div style="text-align: center">
        <cwc:ResourceLabel ID="TotalVehicles" runat="server" Font-Bold="true" Visible="false" ResourceName="Labels" VariableName="CANTIDAD_VEHICULOS" />
        <asp:Label ID="lblTotalVehicles" runat="server" Visible="false" />
        <cwc:ResourceLabel ID="TotalKilometers" runat="server" Font-Bold="true" Visible="false" ResourceName="Labels" VariableName="CANTIDAD_KILOMETROS" />
        <asp:Label ID="lblTotalKilometers" runat="server" Visible="false" />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
    <br />
    <div style="text-align: center">
        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" Visible="true" ResourceName="Labels" VariableName="CANTIDAD_VEHICULOS" />
        <asp:Label ID="lblTotalVehiclesPrint" runat="server" Visible="true" />
        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" Visible="true" ResourceName="Labels" VariableName="CANTIDAD_KILOMETROS" />
        <asp:Label ID="lblTotalKilometersPrint" runat="server" Visible="true" />
    </div>
</asp:Content>
