<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.EstadisticaMobileMaintenance" Codebehind="MobileMaintenance.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    
        <table width="100%" style="font-size: x-small; font-weight: bold">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" />
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" ParentControls="ddlDistrito" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                            <br />
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="175px" AddAllItem="true" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />  
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDate" runat="server" ResourceName="Labels" VariableName="DIA" />
                    <br />
                    <cwc:DateTimePicker ID="dpDia" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                </td>
            </tr>
        </table>
</asp:Content>