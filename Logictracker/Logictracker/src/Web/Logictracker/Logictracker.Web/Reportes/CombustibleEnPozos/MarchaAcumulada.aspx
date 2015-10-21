<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CombustibleEnPozos.Reportes_CombustibleEnPozos_MarchaAcumulada" Codebehind="MarchaAcumulada.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
        <table width="100%" >
            <tr>
                <td >
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" ParentControls="ddlLocacion" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblEquipo" runat="server" ResourceName="Entities"  VariableName="PARENTI19" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upEquipos" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <cwc:EquipoDropDownList ID="ddlEquipo" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td >
                    <cwc:ResourceLinkButton ID="btnMotores" runat="server" ResourceName="Entities"  VariableName="PARENTI39" Font-Bold="true" ListControlTargetID="lbMotores" />
                    <br />
                    <asp:UpdatePanel ID="upMotor" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:CaudalimetroListBox ID="lbMotores" runat="server" SelectionMode="Multiple" Width="250px" Height="100px" 
                                                    ParentControls="ddlEquipo" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td >
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="120px" Mode="DateTime" TimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="120px" Mode="DateTime" TimeMode="End" />
                </td>
            </tr>                
        </table>
</asp:Content>

