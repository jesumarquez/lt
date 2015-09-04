<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CombustibleEnPozos.ReportesCombustibleEnPozosEventosCombustible" Codebehind="EventosCombustible.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">    
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="150px" ParentControls="ddlLocation" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <cwc:ResourceLabel ID="lblEquipo" runat="server" ResourceName="Entities" VariableName="PARENTI19" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upEquipo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:EquipoDropDownList ID="ddlEquipo" AddAllItem="true" runat="server" ParentControls="ddlPlanta" Width="150px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="btnMotores" runat="server" ResourceName="Entities" VariableName="PARENTI39" Font-Bold="true" ListControlTargetID="lbMotores" />
                    <br />
                    <asp:UpdatePanel ID="upMotor" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:CaudalimetroListBox ID="lbMotores" runat="server" ShowDeIngreso="true" SelectionMode="Multiple" Width="175px" 
                            Height="100px"  ParentControls="ddlLocation,ddlPlanta,ddlEquipo" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td >
                    <cwc:ResourceLinkButton ID="btnTanques" runat="server" ResourceName="Entities" VariableName="PARENTI36" Font-Bold="true" ListControlTargetID="lbTanques"/>
                    <br />
                    <asp:UpdatePanel ID="upTanque" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:TanqueListBox ID="lbTanques" runat="server" SelectionMode="Multiple" Width="175px" Height="100px" AutoPostBack="false"
                                ParentControls="ddlLocation,ddlPlanta,ddlEquipo" AllowEquipmentBinding="true" AllowBaseBinding="true"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <cwc:ResourceLinkButton ID="lnkMensaje" runat="server" Font-Bold="true" ListControlTargetID="lbMensajes" ResourceName="Labels"
                        VariableName="MENSAJES" />
                    <asp:UpdatePanel id="upMensajes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <contenttemplate>
                            <cwc:MensajesListBox id="lbMensajes" runat="server" Width="175px" Height="100px" SelectionMode="Multiple"
                                ParentControls="ddlLocation,ddlPlanta" SoloCombustible="true" AutoPostBack="false" />
                        </contenttemplate>
                        <triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </triggers>
                    </asp:UpdatePanel>
                </td>
                <td >
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125px" Mode="DateTime" TimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125px" Mode="DateTime" TimeMode="End" />
                    <cwc:DateTimeRangeValidator ID="dpRng" runat="server" StartControlID="dpDesde" EndControlID="dpHasta" MinRange="0" MaxRange="31" />
                </td>     
            </tr>                
        </table>
</asp:Content>
