<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.DatosOperativos.ReportesEventos" Title="Reporte de Eventos" Codebehind="eventos.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="160px" oninitialbinding="DdlLocacionInitialBinding" AddAllItem="false" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="160px" ParentControls="ddlLocacion" oninitialbinding="DdlPlantaInitialBinding" AddAllItem="true" />
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Width="120" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="160px" AddAllItem="true" ParentControls="ddlLocacion,ddlPlanta" oninitialbinding="DdlTipoVehiculoInitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblTipoMensaje" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI16" />
                <br />
                <asp:UpdatePanel ID="upTipoMensaje" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoMensajeDropDownList ID="ddlTipoMensaje" runat="server" Width="160px" AutoPostBack="True" AddAllItem="true" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" Width="120" />
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkCamiones" runat="server" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbCamiones" ResourceName="Entities" VariableName="PARENTI03" />
                <br />
                <asp:UpdatePanel ID="upCamiones" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="false">
                    <contenttemplate>
                        <cwc:MovilListBox ID="lbCamiones" runat="server" Width="175px" Height="95px" ParentControls="ddlLocacion,ddlPlanta,ddlTipoVehiculo" SelectionMode="Multiple" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkMensaje" runat="server" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbMensajes" ResourceName="Labels" VariableName="MENSAJES" />
                <asp:UpdatePanel ID="upMensajes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <contenttemplate>
                        <cwc:MensajesListBox ID="lbMensajes" runat="server" Width="175px" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTipoMensaje" AutoPostBack="false" />
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoMensaje" EventName="SelectedIndexChanged" />
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkChoferes" runat="server" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbChoferes" ResourceName="Labels" VariableName="Operadores" />
                <asp:UpdatePanel ID="upChoferes" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:EmpleadoListBox runat="server" ID="lbChoferes" Width="175px" Height="95px" AutoPostBack="false" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceCheckBox ID="chkVerEsquinas" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_ESQUINAS" />
                <br/>
                <cwc:ResourceCheckBox ID="chkVerAtenciones" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_ATENCIONES" />
            </td>
        </tr>
    </table>
</asp:Content>
