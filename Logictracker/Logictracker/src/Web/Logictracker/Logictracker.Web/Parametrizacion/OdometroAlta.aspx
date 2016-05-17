<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.OdometroAlta" Codebehind="OdometroAlta.aspx.cs" %>

<%@ Register Src="../App_Controls/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_ODOMETRO" Height="300px">
                    
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" AddAllItem="true" />
                    
                    <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" ParentControls="ddlDistrito" Width="100%" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="350px" MaxLength="64" />
                    
                    <cwc:ResourceLabel ID="lblAsignarTipos" runat="server" ResourceName="Labels" VariableName="ASIGNACION_TIPO_VEHICULO" />
                    <asp:UpdatePanel ID="upVehiculos" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownCheckList ID="ddclTipos" runat="server" ParentControls="ddlBase" Width="350px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddclTipos" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>                    
                    
                    <cwc:ResourceLabel ID="lblPorKm" runat="server" ResourceName="Labels" VariableName="POR_KM" />
                    <asp:CheckBox ID="chkPorKm" runat="server" OnCheckedChanged="ChkPorKmCheckedChanged" AutoPostBack="true" />
                    
                    <cwc:ResourceLabel ID="lblKilometros" runat="server" ResourceName="Labels" VariableName="KILOMETROS" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npKilometrosReferencia" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorKm" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblPorTiempo" runat="server" ResourceName="Labels" VariableName="POR_TIEMPO" />
                    <asp:CheckBox ID="chkPorTiempo" runat="server" OnCheckedChanged="ChkPorTiempoCheckedChanged" AutoPostBack="true" />
                    
                    <cwc:ResourceLabel ID="lblDias" runat="server" ResourceName="Labels" VariableName="DIAS" />
                    <asp:UpdatePanel ID="upDiasAlarmas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npDiasReferencia" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorTiempo" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblPorHorasDeMarcha" runat="server" ResourceName="Labels" VariableName="POR_HORAS_EN_MARCHA" />
                    <asp:CheckBox ID="chkPorHoras" runat="server" OnCheckedChanged="ChkPorHorasCheckedChanged" AutoPostBack="true" />
                    
                    <cwc:ResourceLabel ID="lblHorasDeMarcha" runat="server" ResourceName="Labels" VariableName="HS_EN_MARCHA" />
                    <asp:UpdatePanel ID="upHorasMarcha" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npHorasReferencia" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorHoras" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblEsIterativo" runat="server" ResourceName="Labels" VariableName="ES_ITERATIVO" />
                    <asp:CheckBox ID="chkEsIterativo" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblEsReseteable" runat="server" ResourceName="Labels" VariableName="ES_RESETEABLE" />
                    <asp:CheckBox ID="chkReseteable" runat="server" />
                    
                    <cwc:ResourceLabel ID="lblInsumo" runat="server" ResourceName="Entities" VariableName="PARENTI58" />
                    <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:InsumoDropDownList ID="cbInsumos" runat="server" ParentControls="ddlDistrito,ddlBase" AddNoneItem="True" Width="350px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                </cwc:AbmTitledPanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
                <cwc:TitledPanel ID="AbmTitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="VEHICULOS" Height="300px">
                    <table border="0" width="100%">
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="lblMarca" runat="server" ResourceName="Entities" VariableName="PARENTI06" />
                            </td>
                            <td align="left">
                                <asp:UpdatePanel ID="upMarca" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:MarcaDropDownList ID="cbMarca" runat="server" ParentControls="ddlDistrito,ddlBase" Width="100%" AddAllItem="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="lblModelo" runat="server" ResourceName="Entities" VariableName="PARENTI61" />
                            </td>
                            <td align="left">
                                <asp:UpdatePanel ID="upModelo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ModeloDropDownList ID="cbModelo" runat="server" ParentControls="ddlDistrito,ddlBase,cbMarca" Width="100%" AddAllItem="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbMarca" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td align="right">
                                <cwc:ResourceLabel ID="lblVehiculos" runat="server" ResourceName="Labels" VariableName="ASIGNACION_VEHICULOS" />
                            </td>
                            <td align="left">
                                <asp:UpdatePanel ID="updVehiculos" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:MovilListBox ID="VehiculoListBox" runat="server" ParentControls="ddlDistrito,ddlBase,ddclTipos,cbMarca,cbModelo" Width="300px" Height="260px" SelectionMode="Multiple" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddclTipos" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbMarca" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="cbModelo" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>        
                            </td>
                        </tr>
                    </table>
                </cwc:TitledPanel>
            </td>
        </tr>
        <tr>
            <td>
                <cwc:AbmTitledPanel ID="AbmPrimerAviso" runat="server" TitleResourceName="Labels" TitleVariableName="PRIMER_AVISO" Height="140px">
                    <cwc:ResourceLabel ID="lblKilometrosAlarma1" runat="server" ResourceName="Labels" VariableName="KILOMETROS" />
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npKmAlarma1" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorKm" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblDiasAlarma1" runat="server" ResourceName="Labels" VariableName="DIAS" />
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npDiasAlarma1" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorTiempo" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblHorasAlarma1" runat="server" ResourceName="Labels" VariableName="HS_EN_MARCHA" />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npHorasAlarma1" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorHoras" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblColorAlarma1" runat="server" ResourceName="Labels" VariableName="COLOR" />
                    <uc:ColorPicker ID="txtColorAlarma1" runat="server" />
                </cwc:AbmTitledPanel>
            </td>
            <td>
                <cwc:AbmTitledPanel ID="AbmSegundoAviso" runat="server" TitleResourceName="Labels" TitleVariableName="SEGUNDO_AVISO" Height="140px">
                    <cwc:ResourceLabel ID="lblKilometrosAlarma2" runat="server" ResourceName="Labels" VariableName="KILOMETROS" />
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npKmAlarma2" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorKm" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblDiasAlarma2" runat="server" ResourceName="Labels" VariableName="DIAS" />
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npDiasAlarma2" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorTiempo" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblHorasAlarma2" runat="server" ResourceName="Labels" VariableName="HS_EN_MARCHA" />
                    <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <c1:C1NumericInput ID="npHorasAlarma2" runat="server" NullText="0" ShowNullText="true"
                                MaxValue="999" Value="0" DecimalPlaces="0" Width="100%" Height="17px" Enabled="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="chkPorHoras" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblColorAlarma2" runat="server" ResourceName="Labels" VariableName="COLOR" />
                    <uc:ColorPicker ID="txtColorAlarma2" runat="server" />                        
                </cwc:AbmTitledPanel>
            </td>
        </tr>
        <%--    <cwc:AbmTitledPanel ID="AbmTipoVehiculo" runat="server" TitleResourceName="Labels"
                TitleVariableName="ASIGNACION_TIPO_VEHICULO" Height="205px">
                <cwc:ResourceLabel ID="lblDisponibles" runat="server" ResourceName="Labels" VariableName="DISPONIBLES" />
                <asp:UpdatePanel ID="upVehiculosDisponibles" runat="server" UpdateMode="Conditional"
                    RenderMode="Inline" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox ID="lbVehiculosDisponibles" runat="server" Width="100%"
                            ParentControls="ddlBase" SelectionMode="Multiple" Height="90px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnToAssigned" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnToAvailable" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <asp:Button ID="btnToAssigned" runat="server" Width="50px" Text=">>" OnClick="btnToAssigned_Click" />
                <asp:Button ID="btnToAvailable" runat="server" Width="50px" Text="<<" OnClick="btnToAvailable_Click" />
                <cwc:ResourceLabel ID="lblAsignados" runat="server" ResourceName="Labels" VariableName="ASIGNADOS" />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline"
                    ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoListBox ID="lbVehiculosAsignados" runat="server" Width="250px" AutoBind="false"
                            SelectionMode="Multiple" Height="90px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnToAssigned" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnToAvailable" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </cwc:AbmTitledPanel>--%>
    </table>
</asp:Content>
