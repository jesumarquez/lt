<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ReporteConsumos" Codebehind="ReporteConsumos.aspx.cs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true"  />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" AddAllItem="true" runat="server" Width="150px" oninitialbinding="DdlDistritoInitialBinding" />
            </td>
            
            <td align="left">
                <cwc:ResourceLabel ID="lblModelo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI61" />
                <br />
                <asp:UpdatePanel ID="upModelo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ModeloDropDownList runat="server" AddAllItem="true" ID="ddlModelo" Width="150px" ParentControls="ddlDistrito, ddlBase" oninitialbinding="DdlModeloInitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>            
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels"  VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlBase" AddAllItem="true" runat="server" Width="150px" ParentControls="ddlDistrito" oninitialbinding="DdlBaseInitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilDropDownList ID="ddlVehiculo" runat="server" Width="150px" oninitialbinding="DdlVehiculoInitialBinding" 
                            ParentControls="ddlDistrito, ddlBase, ddlModelo" UseOptionGroup="true" OptionGroupProperty="Estado" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlModelo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" />
            </td>
        </tr>
    </table>
</asp:Content>

