<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.M2M.ReporteMediciones" Title="Reporte de Mediciones" Codebehind="Mediciones.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="Filtros" Src="../../App_Controls/FiltroDetalles.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <asp:UpdatePanel ID="pnl" runat="server">
        <ContentTemplate>
            <%--FILTROS--%>
            <table width="100%">
                <tr>
                    <td>
                        <table width="100%" border="0">
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                                    <br />
                                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="160px" AddAllItem="false" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblTipoEntidad" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI76" />
                                    <br />
                                    <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <cwc:TipoEntidadDropDownList ID="ddlTipoEntidad" runat="server" Width="160px" AddAllItem="true" ParentControls="ddlPlanta" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel> 
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                                    <br />
                                    <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                                        <contenttemplate>
                                            <cwc:PlantaDropDownList id="ddlPlanta" runat="server" Width="160px" ParentControls="ddlLocacion" AddAllItem="true" />
                                        </contenttemplate>
                                        <triggers>
                                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                        </triggers>
                                    </asp:UpdatePanel>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                                    <br />
                                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" />
                                </td>
                                <td>
                                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                                    <br />
                                    <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" />
                                </td>
                            </tr>                
                        </table>
                    </td>
                    <td valign="top">
                        <cwc:ResourceLinkButton ID="lnkEntidades" runat="server" Font-Bold="true" ListControlTargetID="cbEntidad" ResourceName="Entities" VariableName="PARENTI79" />
                        <br />
                        <asp:UpdatePanel ID="upEntidad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EntidadListBox ID="cbEntidad" runat="server" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion, ddlPlanta, ddlTipoEntidad" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td valign="top">
                        <cwc:ResourceLinkButton ID="lnkSubentidades" runat="server" Font-Bold="true" ListControlTargetID="cbSubEntidad" ResourceName="Entities" VariableName="PARENTI81" />
                        <br />
                        <asp:UpdatePanel ID="upSubEntidad" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:SubEntidadListBox ID="cbSubEntidad" runat="server" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion, ddlPlanta, ddlTipoEntidad, cbEntidad" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td valign="top">
                        <asp:UpdatePanel ID="upFiltros" runat="server" Visible="true">
                            <ContentTemplate>
                                <uc1:Filtros ID="ctrlFiltros" runat="server" />    
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbEntidad" EventName="SelectedIndexChanged" />
                            </Triggers>                
                        </asp:UpdatePanel>
                        <br />
                        <cwc:ResourceCheckBox ID="chkVerColumnasFiltradas" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_COLUMNAS_FILTRADAS" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
