<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.ReporteDesvioConsumo" Codebehind="ReporteDesvioConsumo.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="true" runat="server" Width="200px" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa,cbLinea" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblFechaDesde" runat="server" ResourceName="Labels" VariableName="MES" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtFechaDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Mode="Month" />                
            </td>            
        </tr>
        <tr>
            <td valign="top">
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
            <td valign="top">
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
                                        <asp:Label ID="lblLitrosXKm" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color:#FF5904; width:2%;">
                                        &nbsp;
                                    </td>
                                    <td align="left">
                                        <asp:Label ID="lblLitrosXHora" runat="server" />
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