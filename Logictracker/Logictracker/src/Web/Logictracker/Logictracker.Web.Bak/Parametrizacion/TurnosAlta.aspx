<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TurnosAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionTurnosAlta" %>  

<%@ Register Src="~/App_Controls/Pickers/TimePicker.ascx" TagName="TimePicker" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    
    <table style="width: 100%; border-spacing: 10px">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panTurnos" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_TURNO" Height="200px">
                    
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Width="100%" />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" AddAllItem="true" OnInitialBinding="DdlDistritoInitialBinding" Width="100%" />
                    
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100%"/>
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" AddAllItem="true" ParentControls="ddlDistrito" OnInitialBinding="DdlBaseInitialBinding" Width="100%" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" Width="100%" />
                    <asp:TextBox ID="txtCodigo" runat="server" MaxLength="16" Width="98%" />
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" Width="100%"/>
                    <asp:TextBox ID="txtDescripcion" runat="server" MaxLength="128" Width="98%" />
                    
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Width="100%"/>
                    <uc1:TimePicker ID="tpDesde" runat="server" IsValidEmpty="false" Width="75" SelectedTime="00:00:00" />
                    
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Width="100%"/>
                    <uc1:TimePicker ID="tpHasta" runat="server" IsValidEmpty="false" Width="75" SelectedTime="23:59:59" />
                
                </cwc:AbmTitledPanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="TURNO_ASIGNACION" Height="200px">
                
                    <cwc:ResourceLabel ID="lblDepto" runat="server" ResourceName="Entities" VariableName="PARENTI04" />
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <cwc:DepartamentoDropDownList ID="ddlDepartamento" Width="200px" runat="server" ParentControls="ddlDistrito,ddlBase" AddAllItem="True" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblCentro" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <cwc:CentroDeCostosDropDownList id="ddlCentroDeCostos" runat="server" Width="200px" ParentControls="ddlDistrito,ddlBase,ddlDepartamento" AddAllItem="True" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblSubCentro" runat="server" ResourceName="Entities" VariableName="PARENTI99" />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <cwc:SubCentroDeCostosDropDownList id="ddlSubCentroDeCostos" runat="server" Width="200px" ParentControls="ddlDistrito,ddlBase,ddlDepartamento,ddlCentroDeCostos" AddAllItem="True" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlCentroDeCostos" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblVehiculos" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <cwc:MovilDropDownCheckList id="ddlVehiculo" runat="server" Width="200px" ParentControls="ddlDistrito,ddlBase,ddlDepartamento,ddlCentroDeCostos,ddlSubCentroDeCostos" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlDepartamento" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlCentroDeCostos" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlSubCentroDeCostos" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
       <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 100%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panDias" runat="server" TitleResourceName="Labels" TitleVariableName="DIAS">
                    
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <table style="width: 100%; text-align: center; border: solid 1px #999999;" >
                                <tr class="Grid_Header" style="text-align: center;" >
                                    <td colspan="5">
                                        <cwc:ResourceLinkButton ID="btSemana" runat="server" ResourceName="Labels" AutoPostBack="true" VariableName="SEMANA" Font-Bold="true" OnClick="btSemana_Click" ForeColor="White"/>
                                    </td>
                                    <td colspan="2">
                                        <cwc:ResourceLinkButton ID="btFinDeSemana" runat="server" ResourceName="Labels" AutoPostBack="true" VariableName="FIN_DE_SEMANA" Font-Bold="true" OnClick="btFinDeSemana_Click" ForeColor="White" /> 
                                    </td>
                                </tr>
                                <tr class="Grid_Alternating_Item" style="text-align: center;">
                                    <td>
                                        <cwc:ResourceLabel ID="lblLunes" runat="server" ResourceName="Labels" VariableName="LUNES" Font-Bold="true" /> 
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblMartes" runat="server" ResourceName="Labels" VariableName="MARTES" Font-Bold="true" />
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblMiercoles" runat="server" ResourceName="Labels" VariableName="MIERCOLES" Font-Bold="true" />
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblJueves" runat="server" ResourceName="Labels" VariableName="JUEVES" Font-Bold="true" /> 
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblViernes" runat="server" ResourceName="Labels" VariableName="VIERNES" Font-Bold="true" /> 
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblSabado" runat="server" ResourceName="Labels" VariableName="SABADO" Font-Bold="true" /> 
                                    </td>
                                    <td>
                                        <cwc:ResourceLabel ID="lblDomingo" runat="server" ResourceName="Labels" VariableName="DOMINGO" Font-Bold="true" /> 
                                    </td>
                                </tr>
                                <tr class="Grid_Item" style="text-align: center;">
                                    <td>
                                        <asp:CheckBox ID="chkLunes" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkMartes" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkMiercoles" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkJueves" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkViernes" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkSabado" runat="server" /> 
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="chkDomingo" runat="server" /> 
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="7" style="text-align: left;"  class="Grid_Alternating_Item">
                                        <cwc:ResourceCheckBox ID="chkAplicaFeriados" runat="server" ResourceName="Labels" VariableName="APLICA_FERIADOS" />
                                    </td>
                                </tr>
                                <%--<tr>
                                    <td colspan="7" style="text-align: left;">
                                        <asp:Label id="Label1" runat="server" Text="Francos"  Font-Bold="true" Width="100px"></asp:Label>
                                        <asp:DropDownList id="cbFrancos" runat="server" Width="200px" DataTextField="Descripcion" DataValueField="Id" />
                                    </td>
                                </tr>--%>
                            </table>
                
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>
    
</asp:Content>