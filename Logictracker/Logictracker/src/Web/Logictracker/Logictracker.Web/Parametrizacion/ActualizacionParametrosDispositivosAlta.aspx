<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionActualizacionParametrosDispositivosAlta" Codebehind="ActualizacionParametrosDispositivosAlta.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">      

<table style="width: 100%; border-spacing: 10px;">
<tr>
<td style="width: 50%; vertical-align: top;">

    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_TIPO_PARAMETRO_DISPOSITIVO" TitleResourceName="Labels" Height="250px">
                <cwc:ResourceLabel ID="lnlParametro" runat="server" ResourceName="Labels" VariableName="NAME"/>
                <asp:Label ID="lblParametro" runat="server" Width="300px" />
                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="TIPO_DATO"/>
                <asp:Label ID="lblTipoDato" runat="server"  Width="300px"/>
                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="VALOR_INICIAL" />
                <asp:Label ID="lblValor" runat="server" Width="300px" />
   </cwc:AbmTitledPanel>
    </td>
    <td style="width: 50%; vertical-align: top;">

<cwc:AbmTitledPanel ID="panelTopRight" runat="server" TitleVariableName="FILTROS" TitleResourceName="Labels" Height="250px">
                <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" VariableName="PARENTI32" ResourceName="Entities" />
                <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="filters_SelectedIndexChanged" />
                <cwc:ResourceLabel ID="lblDistrito" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" AutoPostBack="true" AddAllItem="true" OnSelectedIndexChanged="filters_SelectedIndexChanged" />
                <cwc:ResourceLabel ID="lblBase" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="100%" AutoPostBack="true" AddAllItem="true" ParentControls="ddlDistrito"
                            OnSelectedIndexChanged="filters_SelectedIndexChanged" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLinkButton ID="lnkDispositivosActualizar" runat="server" VariableName="DISPOSITIVOS_A_ACTUALIZAR" ResourceName="Labels"
                    OnClick="lnkDispositivosActualizar_Click" />
                <asp:UpdatePanel ID="upDispositivos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:DispositivoListBox ID="lbDispositivos" runat="server" Width="100%" Height="100px" SelectionMode="Multiple" AutoPostBack="true"
                            ParentControls="ddlDistrito, ddlBase, ddlTipoDispositivo" OnSelectedIndexChanged="lbDispositivos_SelectedIndexChanged" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoDispositivo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lnkDispositivosActualizar" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>              
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="VALOR_A_ENVIAR" ResourceName="Labels" />
                <asp:TextBox ID="txtValorEnviar" runat="server" Width="100%" MaxLength="255" />
                </cwc:AbmTitledPanel>
    </td>
    </tr>
    <tr>
    <td  colspan="2">
        <asp:UpdatePanel ID="up" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cwc:AbmTitledPanel ID="tblPreview" runat="server" TitleVariableName="VALORES_ACTUALES" TitleResourceName="Labels">
                        <C1:C1GridView id="gridParametros" runat="server" Width="100%" cssclass="SmallGrid" autogeneratecolumns="False"
                            OnRowDataBound="gridParametros_RowDataBound">
                            <Columns>
                                <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI08" SortDirection="Ascending" SortExpression="Dispositivo">
                                    <ItemStyle Width="50%" />
                                </c1h:C1ResourceTemplateColumn>
                                <c1h:C1ResourceBoundColumn ResourceName="Labels" VariableName="VALOR" DataField="Valor">
                                    <ItemStyle Width="50%" />
                                </c1h:C1ResourceBoundColumn>
                            </Columns>
                        </C1:C1GridView>
            </cwc:AbmTitledPanel>
        </ContentTemplate>              
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlTipoDispositivo" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="lbDispositivos" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="lnkDispositivosActualizar" EventName="Click" />
        </Triggers>
</asp:UpdatePanel>
</td>
</tr>
</table>
    
</asp:Content>