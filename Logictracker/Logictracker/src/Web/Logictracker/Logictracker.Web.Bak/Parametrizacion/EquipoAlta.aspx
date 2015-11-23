<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    CodeFile="EquipoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.Parametrizacion_EquipoAlta" Title="Untitled Page" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference"
    TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef"
    TagPrefix="uc1" %>
<%@ Register Src="../App_Controls/DocumentList.ascx" TagName="DocumentList" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>
<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_EMPLEADO">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_GENERALES"
                        TitleResourceName="Labels" Height="225px">
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                            AutoPostBack="True" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" Width="100%"
                            OnInitialBinding="cbEmpresa_InitialBinding" />
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa"
                                    OnInitialBinding="cbLinea_InitialBinding" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" />
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ClienteDropDownList ID="cbCliente" runat="server" AutoPostBack="false" Width="100%"
                                    ParentControls="cbEmpresa,cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" Width="100%" MaxLength="32" />
                        <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="64" />
                        <cwc:ResourceLabel ID="lblResponsable" runat="server" ResourceName="Labels" VariableName="RESPONSABLE" />
                        <asp:UpdatePanel ID="updEmpleado" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" ParentControls="cbEmpresa,cbLinea"
                                    Width="100%" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <cwc:ResourceLabel ID="lblTarjeta" runat="server" ResourceName="Entities" VariableName="TARJETA"
                            ParentControls="cbEmpresa,cbLinea" />
                        <cwc:TarjetaDropDownList ID="cbTarjeta" runat="server" AddAllItem="true" Width="100%" />
                    </cwc:AbmTitledPanel>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosReporte" runat="server" TitleVariableName="REFERENCIA_GEOGRAFICA"
                        TitleResourceName="Labels" Height="200px">
                        <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                            <asp:CheckBox ID="chkExistente" runat="server" />
                            Seleccionar una Referencia Geografica existente.
                        </asp:Panel>
                        <asp:Panel ID="panSelectGeoRef" runat="server" Style="display: none;">
                            <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea"
                                Height="200px" />
                        </asp:Panel>
                        <asp:Panel ID="panNewGeoRef" runat="server">
                            <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="cbEmpresa,cbLinea" />
                        </asp:Panel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="abmTabOdometros" runat="server" ResourceName="Labels" VariableName="DOCUMENTOS_RELACIONADOS">
        <uc1:DocumentList ID="DocumentList1" runat="server" OnlyForEquipment="true" />
    </cwc:AbmTabPanel>
    <%--Hack anti-postback--%>
    <asp:UpdatePanel ID="upHack" runat="server" RenderMode="Inline" UpdateMode="Conditional">
        <ContentTemplate>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbTarjeta" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
