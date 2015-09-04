<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.M2M.AnalizadorMediciones" Title="Untitled Page" Codebehind="AnalizadorMediciones.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ContentPlaceHolderID="Filtros" runat="server" ID="cph">
        <table width="100%">
            <tr align="left" style="font-weight: bold" valign="top">
                <td>
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" />
                    <br />
                    <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoEntidadDropDownList ID="ddlTipoEntidad" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion,ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI79" />
                    <br />
                    <asp:UpdatePanel ID="upEntidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:EntidadDropDownList ID="ddlEntidad" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion,ddlPlanta,ddlTipoEntidad" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br/>
                    <cwc:ResourceLabel ID="lblMedidores" runat="server" ResourceName="Entities" VariableName="PARENTI81" />
                    <br />
                    <asp:UpdatePanel ID="upMedidores" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:SubEntidadDropDownList ID="ddlSubentidad" runat="server" Width="200px" ParentControls="ddlLocacion,ddlPlanta,ddlTipoEntidad,ddlEntidad" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlEntidad" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lbTipoMensaje" runat="server" ResourceName="Entities" VariableName="PARENTI16" />
                    <br />
                    <asp:UpdatePanel ID="upTipoMensaje" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoMensajeDropDownList ID="ddlTipoMensaje" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion,ddlPlanta" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lbMensajes" runat="server" ResourceName="Labels" VariableName="EVENTO_INICIO" />
                    <br />
                    <asp:UpdatePanel ID="upMensajeOrigen" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajesDropDownList ID="ddlMensajeOrigen" runat="server" Width="200px" ParentControls="ddlTipoMensaje" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lbMensajeFin" runat="server" ResourceName="Labels" VariableName="EVENTO_FIN" />
                    <br />
                    <asp:UpdatePanel ID="upMensajeFin" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MensajesDropDownList ID="ddlMenasjeFin" runat="server" Width="200px" ParentControls="ddlTipoMensaje" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoMensaje" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lblSubentidad" runat="server" ResourceName="Entities" VariableName="PARENTI81" ListControlTargetID="lbSubentidad" ForeColor="Black" Font-Bold="true"  />
                    <br />
                    <asp:UpdatePanel ID="upSubentidad" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:SubEntidadListBox ID="lbSubentidad" runat="server" Width="200px" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTipoEntidad,ddlEntidad" TipoMedicion="NU" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoEntidad" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlEntidad" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInitDate" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpInitDate" runat="server" Width="80" TimeMode="Start" IsValidEmpty="false" Mode="Date" />
                    <br />
                    <cwc:ResourceLabel ID="lblEndDate" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpEndDate" runat="server" Width="80" TimeMode="End" IsValidEmpty="false" Mode="Date" />
                </td>
            </tr>
        </table>
</asp:Content>