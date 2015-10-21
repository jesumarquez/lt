<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.ControlDeCombustible.Reportes_ControlDeCombustible_ConsistenciaStockGrafico" Title="Consistencia de Stock" Codebehind="ConsistenciaStockGrafico.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ControlDeCombustible" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td>
                     <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
                    <br />
                     <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" AddAllItem="true" ID="ddlPlanta" Width="150px" ParentControls="ddlLocation" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                   <asp:UpdatePanel ID="upTanque" runat="server">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblTanque" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI36" />
                            <br />
                            <cwc:TanqueDropDownList id="ddlTanque" runat="server" ParentControls="ddlPlanta" AllowBaseBinding="true" AllowEquipmentBinding="true" Width="150px"/>
                        </ContentTemplate>
                        
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTanque" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel runat="server" id="lblInteralo" ResourceName="Labels" Font-Bold="true" VariableName="INTERVALO" />
                    <br />
                    <c1:C1NumericInput ID="npInterval" runat="server" Value="1440" Width="150px" MaxValue="9999" MinValue="0" DecimalPlaces="0" Height="15px" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" Width="100px" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="End" Width="100px" />
                </td>
                <td >
                    <cwc:ResourceCheckBox runat="server" id="chkReal" Checked="true" Font-Bold="true" ResourceName="Labels" VariableName="VOL_REAL" />
                    <br />
                    <br />
                    <cwc:ResourceCheckBox runat="server" id="chkTeorico" Font-Bold="true" Checked="true" ResourceName="Labels" VariableName="VOL_TEORICO" />
                </td>
            </tr>                
        </table>
</asp:Content>
