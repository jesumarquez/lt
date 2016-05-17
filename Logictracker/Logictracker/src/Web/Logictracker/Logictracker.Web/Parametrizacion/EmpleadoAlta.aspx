<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="True"
    Inherits="Logictracker.Parametrizacion.ParametrizacionEmpleadoAlta" Title="Choferes" Codebehind="EmpleadoAlta.aspx.cs" %>

<%@ Register Src="../App_Controls/altaEntidad.ascx" TagName="altaEntidad" TagPrefix="uc1" %>
<%@ Register Src="../App_Controls/DocumentList.ascx" TagName="DocumentList" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_EMPLEADO">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="220px">
                        
                        <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AddAllItem="false" />
                        
                        <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI04"/>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" Width="200px" ParentControls="cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblparenti43" runat="server" ResourceName="Entities" VariableName="PARENTI43"/>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoEmpleadoDropDownList ID="ddlTipoEmpleado" runat="server" Width="200px" ParentControls="cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblPARENTI07" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" Width="200px" AddNoneItem="true" ParentControls="cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI37"/>
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownList ID="cbCentroDeCosto" runat="server" Width="200px" ParentControls="cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                                               
                        <cwc:ResourceLabel ID="lblLegajo" runat="server" ResourceName="Labels" VariableName="LEGAJO" />
                        <asp:TextBox ID="txtLegajo" runat="server" Width="200px" MaxLength="10" />
                        
                        <cwc:ResourceLabel ID="lblAntiguedad" runat="server" ResourceName="Labels" VariableName="ANTIGUEDAD" />
                        <c1:C1NumericInput ID="npAntiguedad" runat="server" Width="200" NullText="" ShowNullText="true" DecimalPlaces="0" MaxValue="100" Number="0" Height="17px" />
                        
                        <cwc:ResourceLabel ID="lblART" runat="server" ResourceName="Labels" VariableName="ART" />
                        <asp:TextBox ID="txtART" runat="server" Width="200px" MaxLength="20" />
                        
                        <cwc:ResourceLabel ID="lblLicencia" runat="server" ResourceName="Labels" VariableName="LICENCIA" />
                        <asp:TextBox ID="txtLicencia" runat="server" MaxLength="32" Width="200px" />
                        
                        <cwc:ResourceLabel ID="lblTelefono" runat="server" ResourceName="Labels" VariableName="TELEFONO" />
                        <asp:TextBox ID="txtTelefono" runat="server" MaxLength="32" Width="200px" />
                        
                        <cwc:ResourceLabel ID="lblMail" runat="server" ResourceName="Labels" VariableName="MAIL" />
                        <asp:TextBox ID="txtMail" runat="server" Width="200px" />
                        
                        <cwc:ResourceLabel ID="lblDevice" runat="server" ResourceName="Entities" VariableName="PARENTI08" />
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:DispositivoDropDownList ID="cbDispositivo" runat="server" Width="200px" ParentControls="cbEmpresa, cbLinea" AddNoneItem="true" HideAssigned="True" OnInitialBinding="CbDispositivoPreBind" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosDispositivo" runat="server" TitleVariableName="RESPONSABLE" TitleResourceName="Labels" Height="200px">
                        
                        <div></div>
                        <cwc:ResourceCheckBox ID="chbResponsable" runat="server" ResourceName="Labels" VariableName="ES_RESPONSABLE" />
                        
                        <cwc:ResourceLabel ID="lblReporta1" runat="server" ResourceName="Labels" VariableName="REPORTA1" />                        
                        <asp:UpdatePanel ID="upReporta1" runat="server">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="ddlReporta1" runat="server" Width="200px" SoloResponsables="true" ParentControls="cbEmpresa" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta2" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta3" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="REPORTA2" />
                        <asp:UpdatePanel ID="upReporta2" runat="server">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="ddlReporta2" runat="server" Width="200px" SoloResponsables="true" ParentControls="cbEmpresa" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta1" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta3" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="upReporta3" runat="server" ResourceName="Labels" VariableName="REPORTA3" />
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="ddlReporta3" runat="server" Width="200px" SoloResponsables="true" ParentControls="cbEmpresa" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta1" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlReporta2" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                    <br />
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="Control de Acceso" TitleResourceName="Labels" Height="135px">
                        
                        <cwc:ResourceLabel ID="lblTarjeta" runat="server" ResourceName="Entities" VariableName="TARJETA" />
                        <asp:UpdatePanel ID="upTarjeta" runat="server">
                            <ContentTemplate>
                                <cwc:TarjetaDropDownList ID="ddlTarjeta" runat="server" Width="200" ParentControls="cbLinea" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI15" />
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <cwc:CategoriaAccesoDropDownList ID="cbCategoria" runat="server" Width="200" ParentControls="cbLinea" AddNoneItem="True" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="DATOS_PERSONALES" TitleResourceName="Labels" Height="400px">
                        <uc1:altaEntidad ID="AltaEntidad" runat="server" />
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="abmTabOdometros" runat="server" ResourceName="Labels" VariableName="DOCUMENTOS_RELACIONADOS">
        <uc1:DocumentList ID="DocumentList1" runat="server" OnlyForEmployees="true" />
    </cwc:AbmTabPanel>
</asp:Content>
