<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPages/ReportGridPage.master" Inherits="Logictracker.Reportes.Estadistica.ReportesActividadVehicular" Title="Actividad Vehícular" Codebehind="ActividadVehicular.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">  
        <table width="100%" style="font-size: x-small">
            <tr valign="top" align="left">
                <td>
                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" Font-Bold="true" VariableName="PARENTI01" ResourceName="Entities" />
                    <br />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AddAllItem="false" />
                    <br/>
                    <br/>
                    <cwc:ResourceLabel ID="lblLinea" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                    <br />
                    <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" AddAllItem="true" ParentControls="cbEmpresa" />
                </td>
                <td>
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
                    <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ListControlTargetID="lbVehiculos" Font-Bold="true" ResourceName="Labels" VariableName="VEHICULOS" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upVehiculo" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbVehiculos" runat="server" SelectionMode="Multiple" ParentControls="ddlTipoVehiculo"
                                Width="175px" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" Width="100px" Mode="DateTime" TimeMode="Start" IsValidEmpty="false" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" Width="100px" Mode="DateTime" TimeMode="End" IsValidEmpty="false" />
                    <cwc:DateTimeRangeValidator ID="dtValidator" runat="server" MaxRange="31" StartControlID="dpDesde" EndControlID="dpHasta" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblKm" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="KM_SUPERADOS" />
                    <br />
                    <c1:C1NumericInput ID="npKm" runat="server" Value="0" MaxValue="99999" MinValue="0" Width="100px" Height="15px" DecimalPlaces="0" />
                    <br />
                    <br />
                    <cwc:ResourceCheckBox ID="chkDetalleInfracciones" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_DETALLE_INFRACCIONES" />
                </td>
            </tr>
        </table>
 </asp:Content>

<asp:Content ID="c2" runat="server" ContentPlaceHolderID="DetalleInferior">
<div style="text-align:center">
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