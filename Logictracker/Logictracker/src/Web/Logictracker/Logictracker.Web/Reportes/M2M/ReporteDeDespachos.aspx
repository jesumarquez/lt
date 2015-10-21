<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ReporteDeDespachos" Codebehind="ReporteDeDespachos.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="true" runat="server" Width="200px" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblFechaDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaDesde" runat="server" IsValidEmpty="false" TimeMode="Start" />                
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea,cbTipoVehiculo" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblFechaHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaHasta" runat="server" IsValidEmpty="false" TimeMode="End" />
            </td>
        </tr>
    </table>
    
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">    
    <asp:UpdatePanel ID="upCharts" runat="server">   
        <ContentTemplate>
            
            <table width="100%">
                <tr align="center">
                    <td>
                        <asp:Panel ID="pnlInferior" runat="server" Visible="false">
                            <table width="90%">
                                <tr>
                                    <td style="background-color:#AFD8F8; width:2%;">
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblLitrosCargados" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td id="tdLitros" runat="server" style="width:2%;">
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblLitros" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            
        </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>