<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionCentroCostoAlta" Codebehind="CentroCostoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <asp:UpdatePanel ID="upCentros" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:AbmTitledPanel ID="panCentros" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_CENTRO">
                                
                                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="98%" OnInitialBinding="DdlEmpresaPreBind" />
                                
                                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" OnInitialBinding="CbLineaPreBind" ParentControls="ddlEmpresa" AddAllItem="true" />
                                
                                <cwc:ResourceLabel ID="lblParenti04" runat="server" ResourceName="Entities" VariableName="PARENTI04" />
                                <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" Width="98%" OnInitialBinding="CbDeptoPreBind" ParentControls="ddlEmpresa,cbLinea" AddAllItem="true" />
                                
                                <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                                <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
                                
                                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                                
                                <cwc:ResourceLabel ID="lblResponsable" runat="server" ResourceName="Labels" VariableName="RESPONSABLE" />
                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="98%" OnInitialBinding="CbEmpleadoPreBind" ParentControls="ddlEmpresa,cbLinea" AddNoneItem="True" />
                                
                                <cwc:ResourceLabel ID="lblGeneraEntregas" runat="server" ResourceName="Labels" VariableName="GENERA_ENTREGAS" />
                                <asp:CheckBox ID="chkGeneraEntregas" runat="server" />
                                
                                <cwc:ResourceLabel ID="lblHorarioInicio" runat="server" ResourceName="Labels" VariableName="HORARIO_INICIO" />
                                <cwc:DateTimePicker ID="dtHorario" runat="server" Mode="Time" />
                                
                                <cwc:ResourceLabel ID="lblInicioAutomatico" runat="server" ResourceName="Labels" VariableName="INICO_AUTOMATICO" />
                                <asp:CheckBox ID="chkInicioAutomatico" runat="server" />
                                
                        </cwc:AbmTitledPanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
            </td>
        </tr>
    </table>
</asp:Content>
