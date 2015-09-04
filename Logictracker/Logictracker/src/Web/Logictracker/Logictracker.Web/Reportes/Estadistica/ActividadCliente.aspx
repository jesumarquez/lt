<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ReportGridPage.master" Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaActividadCliente" Codebehind="ActividadCliente.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">    
        <table width="100%" style="font-size: x-small">
            <tr valign="top" align="left">
                <td>
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" AddAllItem="true" />
                    <br />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upTipoVehiculo">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities"    
                                VariableName="PARENTI17" />
                            <br />
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" AddAllItem="true" Width="175px" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="btnClientes" runat="server" ResourceName="Entities" VariableName="PARENTI18"
                                Font-Bold="true" ListControlTargetID="lbClientes" />
                        <br />
                    <asp:UpdatePanel ID="upCliente" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:ClienteListBox ID="lbClientes" runat="server" AutoPostBack="false" ParentControls="ddlBase" SelectionMode="Multiple" Width="175px" />    
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />  
                    </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ListControlTargetID="lbVehiculos" Font-Bold="true"
                                ResourceName="Labels" VariableName="VEHICULOS" />
                            <br />
                    <asp:UpdatePanel runat="server" ID="upVehiculo" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>                            
                            <cwc:MovilListBox ID="lbVehiculos" runat="server" SelectionMode="Multiple" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" ParentControls="ddlTipoVehiculo,lbClientes"
                                Width="175px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lbClientes" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" Width="100px" Mode="Date" TimeMode="Start" IsValidEmpty="false" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" Width="100px" Mode="Date" TimeMode="End" IsValidEmpty="false" />
                    <cwc:DateTimeRangeValidator ID="dtValidator" runat="server" MaxRange="31" StartControlID="dpDesde" EndControlID="dpHasta" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblKm" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="KM_SUPERADOS" />
                    <br />
                    <c1:C1NumericInput ID="npKm" runat="server" Value="0" MaxValue="99999" MinValue="0" Width="100px" Height="15px" DecimalPlaces="0" />
                </td>
            </tr>
        </table>
</asp:Content>
    
<asp:Content ID="content" runat="server" ContentPlaceHolderID="DetalleInferior">
    <div style="text-align:center">
        <br />
         <cwc:ResourceLabel ID="TotalVehicles" runat="server" Font-Bold="true" Visible="false" ResourceName="Labels"
                                VariableName="CANTIDAD_VEHICULOS" />
        <asp:Label ID="lblTotalVehicles" runat="server" Visible="false" />
        <cwc:ResourceLabel ID="TotalKilometers" runat="server" Font-Bold="true" Visible="false" ResourceName="Labels"
            VariableName="CANTIDAD_KILOMETROS" />
        <asp:Label ID="lblTotalKilometers" runat="server" Visible="false" />
    </div>   
</asp:Content>

<asp:Content ID="content2" runat="server" ContentPlaceHolderID="DetalleInferiorPrint">
    <div style="text-align:center">
        <br />
         <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" Visible="true" ResourceName="Labels"
                                VariableName="CANTIDAD_VEHICULOS" />
        <asp:Label ID="lblTotalVehiclesPrint" runat="server" Visible="true" />
        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" Visible="true" ResourceName="Labels"
            VariableName="CANTIDAD_KILOMETROS" />
        <asp:Label ID="lblTotalKilometersPrint" runat="server" Visible="true" />
    </div>   
</asp:Content>
