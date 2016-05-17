<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
CodeFile="TipoEmpleadoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.Parametrizacion_TipoEmpleadoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">

<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_ASIGNACION" TitleResourceName="Labels" Height="100px">
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="100%" AddAllItem="true"
                    OnInitialBinding="ddlLocacion_InitialBinding" />
                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" Width="100%" AddAllItem="true" ParentControls="ddlLocacion"
                            OnInitialBinding="ddlPlanta_InitialBinding" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
</cwc:AbmTitledPanel>
</td>
<td style="width: 50%; vertical-align: top;">
    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="DATOS_TIPOEMPLEADO" TitleResourceName="Labels" Height="100px">
                <cwc:ResourceLabel ID="ResourceLabel1" Font-Bold="true" runat="server" ResourceName="Labels" VariableName="CODE" />
                <asp:TextBox ID="txtCodigo" runat="server" Width="100%" MaxLength="3" />
                <cwc:ResourceLabel ID="ResourceLabel3" Font-Bold="true" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="50" />
    </cwc:AbmTitledPanel>
</td>
</tr>
</table>
</asp:Content>

