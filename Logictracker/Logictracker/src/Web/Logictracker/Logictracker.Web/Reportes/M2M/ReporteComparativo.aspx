<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ReporteComparativo" Codebehind="ReporteComparativo.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="False" runat="server" Width="200px" />
                <br />
                <br />
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
                <cwc:ResourceLabel ID="lblTransportista" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="200px" AddAllItem="True" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br/>
                <br/>
                <cwc:ResourceLabel ID="lblMovil" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea,cbTransportista" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>       
            <td>
                <cwc:ResourceLabel ID="lblFechaDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaDesde" runat="server" IsValidEmpty="false" TimeMode="Start" />                
                <br/>
                <cwc:ResourceLabel ID="lblFechaHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaHasta" runat="server" IsValidEmpty="false" TimeMode="End" />
            </td>
        </tr>
    </table>
    
    <asp:HiddenField ID="hidMes" runat="server" Value="-1" />
    <asp:Button ID="btnMes" runat="server" OnClick="ClickMes" style="display: none;" />
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">    
    <asp:UpdatePanel ID="upCharts" runat="server">   
        <ContentTemplate>
            
            <table width="100%">
                <tr align="center">
                    <td>
                        <div id="divChartDias" runat="server" />
                    </td>
                </tr>
            </table>
            
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
                                    <td style="background-color:#FF5904; width:2%;">
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblLitrosConsumidos" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color:#FAB802; width:2%;">
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblLitrosCalculados" runat="server" />
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