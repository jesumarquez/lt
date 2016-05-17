<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.ControlAcceso.Fichada" Codebehind="Fichada.aspx.cs" %>

<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <asp:UpdatePanel runat="server" ID="updFiltros" UpdateMode="Conditional">
        <ContentTemplate>
<style type="text/css">
#table td
{
    vertical-align: top;
}
</style>
<table id="table" style="width: 100%;">
    <tr>
        <td>
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                        <br />
                        <cwc:LocacionDropDownList ID="cbEmpresa" AddAllItem="false" runat="server" Width="200px" OnSelectedIndexChanged="cbEmpresa_SelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" />
                                <br />
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="200px" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </td>
        
        <td style="border-left: solid 1px #CCCCCC;">
            <table>
                <tr>
                    <td colspan="2">
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:PeriodoDropDownList ID="cbPeriodo" runat="server" Width="220px" ParentControls="cbEmpresa" AutoPostBack="true" OnSelectedIndexChanged="cbPeriodo_SelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="lblFecha" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                        <br />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                            <cwc:DateTimePicker ID="dtDesde" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="false" />
                        </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbPeriodo" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                        <br />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                        <cwc:DateTimePicker ID="dtHasta" runat="server" Mode="Date" TimeMode="End" IsValidEmpty="false" />
                        </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPeriodo" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>        
        </td>
        <td style="border-left: solid 1px #CCCCCC;">
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI37" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel4" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownCheckList ID="cbCentroDeCostos" runat="server" ParentControls="cbLinea" Width="200px" />
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>            
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI04" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel5" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:DepartamentoDropDownCheckList ID="cbDepartamento" runat="server" ParentControls="cbLinea" Width="200px" />
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table> 
        </td>
        <td style="border-left: solid 1px #CCCCCC;">
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI43" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel6" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:TipoEmpleadoDropDownCheckList ID="cbTipoEmpleado" runat="server" ParentControls="cbLinea" Width="200px" AddNoneItem="True" />
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>            
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI09" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel7" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <%--<cwc:EmpleadoDropDownCheckList ID="cbEmpleado" runat="server" ParentControls="cbEmpresa,cbLinea,cbTipoEmpleado,cbDepartamento,cbCentroDeCostos" Width="200px" AutoPostBack="True" />--%>
                                <cwc:AutoCompleteTextBox runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetEmpleados" ID="txtEmpleado" Width="200px" ParentControls="cbEmpresa,cbLinea,cbTipoEmpleado,,cbCentroDeCostos,cbDepartamento" />
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbTipoEmpleado" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCentroDeCostos" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table> 
        </td>
        <td style="border-left: solid 1px #CCCCCC;">
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI55" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel8" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:PuertaDropDownCheckList ID="cbPuerta" runat="server" ParentControls="cbLinea" Width="200px" />
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbPuerta" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="lblLegajo" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="LEGAJO" />
                        <asp:TextBox ID="txtLegajo" runat="server" Width="200px" />
                    </td>
                </tr>
            </table> 
        </td>
    </tr>
</table>

        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel runat="server" ID="updEdit" UpdateMode="Conditional">
    <ContentTemplate>
        
    <cwc:PopupPanel ID="popupEdit" runat="server" Width="500px" CssClass="PopupPanel" CancelControlID="btCancelar">
        <table style="border-spacing: 10px; width: 100%;">
        <tr>
        <td><cwc:ResourceLabel ID="lblEditEmpleado" runat="server" ResourceName="Entities" VariableName="PARENTI09" /></td>
        <td>
            <cwc:AutoCompleteTextBox runat="server" Width="100%" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetEmpleados" ID="txtEditEmpleados" />
        </td>
        <td></td>
        </tr>
        <tr>
        <td><cwc:ResourceLabel ID="ResourceLabel8" runat="server" ResourceName="Labels" VariableName="ENTRO_A" /></td>
        <td><cwc:PuertaDropDownList ID="cbEditPuertaEntrada" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbLinea" AutoPostBack="false" /></td>
        <td><cwc:DateTimePicker ID="dtEditHoraEntrada" runat="server" Mode="DateTime" IsValidEmpty="true" /></td>
        </tr>
        <tr>
        <td><cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="SALIO_DE" /></td>
        <td><cwc:PuertaDropDownList ID="cbEditPuertaSalida" runat="server" Width="100%" AddNoneItem="true" ParentControls="cbLinea" AutoPostBack="false" /></td>
        <td><cwc:DateTimePicker ID="dtEditHoraSalida" runat="server" Mode="DateTime" IsValidEmpty="true" /></td>
        </tr>
        </table>
        <br />
        <div style="text-align: right;">
        <cwc:ResourceButton ID="btAceptar" runat="server" ResourceName="Controls" VariableName="BUTTON_ACCEPT" OnCommand="btAceptar_Command" />
        &nbsp;&nbsp;&nbsp;
        <cwc:ResourceButton ID="btCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
        </div>
    </cwc:PopupPanel>

</ContentTemplate>
</asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" Runat="Server">

    <table>
    <tr><td>
        <cwc:ResourceCheckBox ID="chkHuerfanos" runat="server" ResourceName="Labels" VariableName="SOLO_HUERFANOS" />
        <br />
        <cwc:ResourceCheckBox ID="chkEliminados" runat="server" ResourceName="Labels" VariableName="VER_ELIMINADOS" />
        
    </td>
    <td style="padding-left: 30px;">
        <cwc:ResourceLabel ID="lblDuracion" runat="server" ResourceName="Labels" VariableName="DURACION" />
        <br />
        <asp:TextBox ID="txtDuracion" runat="server" Width="50px" Text="1" />
        <cwc:BaloonTip ID="hDuracion" runat="server" ResourceName="Labels" VariableName="BALOON_FICHADA" />       
    </td>
    </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" Runat="Server">
<div style="position:relative; height: 0px;">
<div style="position: relative; top: 30px; text-align: right; padding-right: 10px;">
<cwc:ResourceLinkButton ID="btNuevo" runat="server" ResourceName="Controls" VariableName="BUTTON_NEW" Visible="false" OnClick="btNuevo_Click" CausesValidation="False" />
</div>
</div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" Runat="Server">
</asp:Content>

