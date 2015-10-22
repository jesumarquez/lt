<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CombustibleEnPozos.ReportesCombustibleEnPozosVariacionDeNivel" Codebehind="VariacionDeNivel.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
        <table width="100%" cellpadding="5">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01"/>
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" AddAllItem="true" ID="ddlPlanta" Width="150px" ParentControls="ddlLocation" AutoPostBack="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblEquipo" runat="server" ResourceName="Entities" VariableName="PARENTI19" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upEquipo" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <cwc:EquipoDropDownList ID="ddlEquipo" runat="server" ParentControls="ddlPlanta" AddAllItem="true" Width="150px" AutoPostBack="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
                    <br />
                    <asp:UpdatePanel ID="upTanque" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TanqueDropDownList id="ddlTanque" runat="server" ParentControls="ddlEquipo" AllowBaseBinding="false" AllowEquipmentBinding="true" Width="150px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125px" TimeMode="Start" Mode="DateTime" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125px" TimeMode="End" Mode="DateTime" />
                    <cwc:DateTimeRangeValidator ID="dtRange" runat="server" MaxRange="31" MinRange="0" StartControlID="dpDesde" EndControlID="dpHasta" />
                </td>
                <td valign="top">
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="INTERVALO" />
                    <br />
                    <c1:C1NumericInput ID="npIntervalo" runat="server" MaxValue="9999" MinValue="0" Value="1440" Width="100px" Height="15px" DecimalPlaces="0" /> 
                </td>
            </tr>                
        </table>
</asp:Content>
