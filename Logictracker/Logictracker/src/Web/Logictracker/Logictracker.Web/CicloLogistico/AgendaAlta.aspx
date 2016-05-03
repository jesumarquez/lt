<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.CicloLogistico.AgendaAlta" Codebehind="AgendaAlta.aspx.cs" %>

<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.Controls" Assembly="Logictracker.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="server">
    <table width="100%">
        <tr>
            <td width="50%" align="center">
                <cwc:TitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="200px" Width="50%">
                    <table width="100%">
                        <tr>
                            <td width="40%" align="right">
                                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                            </td>
                            <td width="40%" align="left">
                                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="98%" OnSelectedIndexChanged="ConditionChanged" />
                            </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="40%" align="right">
                                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                            </td>
                            <td width="40%" align="left">
                                <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" ParentControls="cbEmpresa" OnSelectedIndexChanged="ConditionChanged" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="40%" align="right">
                                <cwc:ResourceLabel ID="lblDepto" runat="server" ResourceName="Entities" VariableName="PARENTI04" />
                            </td>
                            <td width="40%" align="left">
                                <asp:UpdatePanel ID="upDepto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" Width="98%" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="ConditionChanged" FiltraPorUsuario="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="40%" align="right">
                                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                            </td>
                            <td width="40%" align="left">
                                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="ConditionChanged" />
                                <asp:Label ID="lblDtDesde" runat="server" />
                            </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td width="40%" align="right">
                                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                            </td>
                            <td width="40%" align="left">
                                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="ConditionChanged" />
                                <asp:Label ID="lblDtHasta" runat="server" />
                            </td>
                            <td width="20%">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3" align="center">
                                <cwc:ResourceButton ID="btnConsultar" runat="server" ResourceName="Labels" VariableName="CONSULTAR" OnClick="BtnConsultarOnClick" />
                            </td>
                        </tr>
                    </table>
                </cwc:TitledPanel>
            </td>
            <td width="50%" align="center">
                <cwc:TitledPanel ID="panelTopRight" runat="server" TitleVariableName="ASIGNACION" TitleResourceName="Labels" Height="200px" Width="50%">
                    
                    <table width="100%">
                        <tr>
                            <td width="50%" align="right">
                                <cwc:ResourceLabel ID="lblVehiculos" runat="server" ResourceName="Labels" VariableName="VEHICULOS_DISPONIBLES" />
                            </td>
                            <td width="50%" align="left">
                                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="cbVehiculo" runat="server" Width="60%" Enabled="false" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="dtpDesde" EventName="DateChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="dtpHasta" EventName="DateChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="btnConsultar" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td width="50%" align="right">
                                <cwc:ResourceLabel ID="lblEmpleados" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                            </td>
                            <td width="50%" align="left">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="60%" ParentControls="cbEmpresa,cbLinea,cbDepartamento" />
                                        <cwc:AutoCompleteTextBox ID="auEmpleado" runat="server" ServicePath="~\App_Services\AutoComplete.asmx" ServiceMethod="GetEmpleados" Width="60%" ParentControls="cbEmpresa,cbLinea,,,,cbDepartamento" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbDepartamento" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td width="50%" align="right">
                                <cwc:ResourceLabel ID="lblDestino" runat="server" ResourceName="Labels" VariableName="DESTINO" />
                            </td>
                            <td width="50%" align="left">
                                <asp:TextBox ID="txtDestino" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td width="50%" align="right">
                                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="LIMITADO_AL_TURNO" />
                            </td>
                            <td width="50%" align="left">
                                 <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TurnoDropDownList ID="cbTurno" runat="server" Width="60%" ParentControls="cbEmpresa,cbLinea" AddNoneItem="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="center">
                                <cwc:ResourceButton ID="btnReservar" runat="server" ResourceName="Labels" VariableName="RESERVAR" OnClick="BtnReservarOnClick" />
                                <cwc:ResourceButton ID="btnCancelar" runat="server" ResourceName="Labels" VariableName="CANCELAR_RESERVA" OnClick="BtnCancelarOnClick" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>
