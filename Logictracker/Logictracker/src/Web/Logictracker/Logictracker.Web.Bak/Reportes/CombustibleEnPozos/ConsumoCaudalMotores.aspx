<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" CodeFile="ConsumoCaudalMotores.aspx.cs" Inherits="Logictracker.Reportes.CombustibleEnPozos.Reportes_CombustibleEnPozos_ConsumoCaudalMotores" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
<table width="100%" cellpadding="5">
    <tr>
        <td align="left" >
            <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01"/>
            <br />
            <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="150px" AddAllItem="true" />
            <br />
            <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
            <br />
            <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                <ContentTemplate>
                    <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" Width="150px" AddAllItem="true" ParentControls="ddlLocation" AutoPostBack="true" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td >  
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
            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="INTERVALO" />
            <br />
            <c1:C1NumericInput ID="npIntervalo" runat="server" MinValue="0" MaxValue="1440" Value="60" DecimalPlaces="0" Width="150px" Height="15px" /> 
        </td>
        <td valign="middle">
            <cwc:ResourceLinkButton ID="lblMotor" runat="server" Font-Bold="true" ResourceName="Entities"
                VariableName="PARENTI39" ListControlTargetID="lbMotor" />
            <br />
            <asp:UpdatePanel ID="upMotor" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                <ContentTemplate>
                    <cwc:CaudalimetroListBox runat="server" SelectionMode="Multiple" ID="lbMotor" Width="200px" 
                                Height="80px" ParentControls="ddlEquipo" AutoPostBack="false" />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlEquipo" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>                                 
         <td align="justify">                 
            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
            <br />
            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125px" TimeMode="Start" Mode="DateTime" />
            <br />
            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
            <br />
            <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125px" TimeMode="End" Mode="DateTime" />
            <cwc:DateTimeRangeValidator ID="rngDate" runat="server" MaxRange="31" StartControlID="dpDesde" EndControlID="dpHasta" />
        </td>
        <td >
            <cwc:ResourceCheckBox ID="chkConsumo" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="CONSUMO" Checked="true" />
            <br />
            <br />
            <cwc:ResourceCheckBox ID="chkCaudal" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="CAUDAL" Checked="true" />
        </td>
    </tr>                
</table>
</asp:Content>

