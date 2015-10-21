<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ReportGridPage.master"  CodeFile="VolumenTanque.aspx.cs" Inherits="Logictracker.Reportes.CombustibleEnPozos.Reportes_CombustibleEnPozos_VolumenTanque" Title="Volumen del Tanque" %>

<%@ Register Src="~/App_Controls/Pickers/DateTimePicker.ascx" TagName="DateTimePicker" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/Pickers/NumberPicker.ascx" TagName="NumberPicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td align="left">
                     <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="150px" ParentControls="ddlLocation" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
               <td>
                     <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI19" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:EquipoDropDownList runat="server" ID="ddlEquipo" AddAllItem="true" Width="150px" ParentControls="ddlLocation,ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TanqueDropDownList runat="server" AddAllItem="true" AllowEquipmentBinding="true" AllowBaseBinding="false"
                                 ID="ddlTanque" Width="150px" ParentControls="ddlEquipo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100px" Mode="DateTime" TimeMode="Start" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100px" Mode="DateTime" TimeMode="End" />
                </td>
                <td align="left">
                    <cwc:ResourceLabel runat="server" ID="lblInterval" Font-Bold="true" ResourceName="Labels" VariableName="INTERVALO" />
                    <br />
                    <c1:C1NumericInput runat="server" ID="npInterval" Value="30" Width="90px" MaxValue="1440" MinValue="0" DecimalPlaces="0" Height="15px" />
                </td>
            </tr>             
        </table>
</asp:Content>
