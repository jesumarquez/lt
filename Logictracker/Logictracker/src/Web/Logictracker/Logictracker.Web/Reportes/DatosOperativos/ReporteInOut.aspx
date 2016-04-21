<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" Inherits="Logictracker.Reportes.DatosOperativos.ReporteInOut" Title="Reporte In / Out" Codebehind="ReporteInOut.aspx.cs" %>

<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td valign="top">
                <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="150px" AddAllItem="false" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                    <contenttemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="150px" ParentControls="ddlLocacion" AddAllItem="true" />
                    </contenttemplate>
                    <triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </triggers>
                </asp:UpdatePanel>
            </td>
            <td valign="top">
                <cwc:ResourceLinkButton ID="lnkTipoGeocerca" runat="server" ListControlTargetID="ddlTipoGeocerca" ResourceName="Entities" VariableName="PARENTI10" Font-Bold="true" ForeColor="Black" />
                <br />
                <asp:UpdatePanel ID="upTipoGeocerca" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TipoReferenciaGeograficaListBox ID="ddlTipoGeocerca" runat="server" AddNoneItem="true" SelectionMode="Multiple" Width="150px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>            
            <td valign="top">
                <cwc:ResourceLabel ID="lblFecha" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="FECHA" />
                <br />
                <cwc:DateTimePicker ID="dpFecha" runat="server" IsValidEmpty="false" TimeMode="Start" Width="110" />
            </td>
        </tr>
    </table>
</asp:Content>
