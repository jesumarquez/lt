<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Organizacion.AltaUsuario" Title="Usuarios" MaintainScrollPositionOnPostback="true" Codebehind="UsuarioAlta.aspx.cs" %>

<%@ Register Src="../App_Controls/IpRangeEditor.ascx" TagName="IpRangeEditor" TagPrefix="uc2" %>
<%@ Register Src="../App_Controls/altaEntidad.ascx" TagName="altaEntidad" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <style type="text/css">
        .strength_border { border: solid 1px #CCCCCC; background-color: White; width: 100px; }
        .strength_bar { background-color: Blue; }
        .strength_bar_red { background-color: Red; }
        .strength_bar_green { background-color: Green; }
        .separator {border-top: solid 1px #cccccc; margin: 5px;}
    </style>
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table id="tbTipoPunto" style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
            <tr>
                <td style="vertical-align: top; width: 50%;">
                    <%--COLUMNA IZQ--%>
                    <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="320px">
                    
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="TYPE" />
                        <asp:UpdatePanel ID="updTipoUsuario" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoUsuarioDropDownList ID="cbTipoUsuario" runat="server" Width="100%" OnInitialBinding="cbTipoUsuario_PreBind" OnSelectedIndexChanged="cbTipoUsuario_SelectedIndexChanged" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="USUARIO" />
                        <asp:TextBox ID="txtUsuario" runat="server" Width="100%" autocomplete="off" />
                        
                        <div></div>
                        <cwc:ResourceCheckBox ID="chkInhabilitar" runat="server" ResourceName="Labels" VariableName="INHABILITAR" AutoPostBack="true" OnCheckedChanged="chkInhabilitar_changed" />
                        
                        <div runat="server"></div>
                        <div class="separator"></div>
                        
                        <cwc:ResourceLabel ID="lblPassword" runat="server" ResourceName="Labels" VariableName="PASSWORD" />
                        <div runat="server">
                            <asp:TextBox ID="txtClave" runat="server" Width="100%" TextMode="Password" Text="" autocomplete="off" />
                            <AjaxToolkit:PasswordStrength ID="passStrength" runat="server" TargetControlID="txtClave"
                                StrengthIndicatorType="BarIndicator" BarBorderCssClass="strength_border" BarIndicatorCssClass="strength_bar"
                                DisplayPosition="BelowLeft" MinimumSymbolCharacters="1" MinimumNumericCharacters="1"
                                PreferredPasswordLength="6" MinimumLowerCaseCharacters="1" MinimumUpperCaseCharacters="0"
                                StrengthStyles="strength_bar_red;strength_bar;strength_bar_green" >
                            </AjaxToolkit:PasswordStrength>
                        </div>
                        
                        <cwc:ResourceLabel ID="lblConfirmacion" runat="server" ResourceName="Labels" VariableName="CONFIRM_PASSWORD" />
                        <asp:TextBox ID="txtConfirmacion" runat="server" TextMode="Password" Width="100%" />  
                                
                        <div id="Div1" runat="server"></div>
                        <div class="separator"></div>
                                              
                        <div id="Div4" runat="server"></div>
                        <asp:UpdatePanel ID="upChkNoCambioPass" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkNoCambioPass" runat="server" ResourceName="Labels" VariableName="INHBILITAR_CAMBIAR_PASS" AutoPostBack="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoUsuario" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <div runat="server"></div>
                        <asp:UpdatePanel ID="upChkNoCambioUso" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkNoCambioUso" runat="server" ResourceName="Labels" VariableName="INHBILITAR_CAMBIAR_USO" AutoPostBack="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoUsuario" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <div id="Div5" runat="server"></div>
                        <div class="separator"></div>
                        
                        <div id="Div2" runat="server"></div>
                        <div id="Div3" runat="server">
                            <asp:UpdatePanel ID="upComportamiento" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                <ContentTemplate>
                                    <cwc:ResourceCheckBox ID="chkExpira" runat="server" ResourceName="Labels" VariableName="EXPIRA" AutoPostBack="true" OnCheckedChanged="chkExpira_changed" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="chkInhabilitar" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <cwc:DateTimePicker ID="dtpExpira" runat="server" Enabled="false" IsValidEmpty="true" Mode="Date" TimeMode="Start" />
                        </div>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td>
                    <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server" TitleVariableName="DATOS_PERSONALES" TitleResourceName="Labels" Height="320px">
                        <uc1:altaEntidad ID="AltaEntidad" runat="server" ShowDir="false" />
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="CONFIGURACION">
        <table id="Table1" style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
            <tr>
                <td style="vertical-align: top; width: 50%;">
                    <%--COLUMNA IZQ--%>
                    <cwc:AbmTitledPanel ID="TitledPanel4" runat="server" TitleVariableName="CONFIG_REGIONAL" TitleResourceName="Labels" Height="140px">
                        
                        <cwc:ResourceLabel ID="lblUsoHorario" runat="server" ResourceName="Labels" VariableName="USO_HORARIO" />
                        <cwc:TimeZoneDropDownList ID="ddlUsoHorario" runat="server" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblGrupoRecursos" runat="server" ResourceName="Labels" VariableName="GRUPO_RECURSOS" />
                        <cwc:ResourceGroupDropDownList ID="ddlGrupoRecursos" runat="server" Width="100%" />
                        
                        <cwc:ResourceLabel ID="lblPais" runat="server" ResourceName="Labels" VariableName="CULTURA" />
                        <asp:UpdatePanel ID="upCulture" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:CultureSelector ID="cultureSelector" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td>
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CONFIG_VISUAL" TitleResourceName="Labels" Height="140px">
                        
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="THEME" />
                        <asp:DropDownList ID="cbTheme" runat="server" Width="100%" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="LOGO" />
                        <asp:DropDownList ID="cbLogo" runat="server" Width="100%" />
                        
                        <div>
                        </div>
                        
                        <div>
                        </div>
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
            <tr>
                <td>
                    <cwc:AbmTitledPanel ID="AbmTitledPanel3" runat="server" TitleVariableName="CONFIG_PERFILES" TitleResourceName="Labels" Height="200px">
                        
                        <cwc:ResourceLabel ID="ResourceLabel15" runat="server" ResourceName="Entities" VariableName="ROLE" />
                        <cwc:PerfilDropDownCheckList ID="cbPerfil" runat="server" Width="200px" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel16" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownCheckList ID="cbEmpresa" runat="server" Width="200px" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel17" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="updLinea" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownCheckList ID="cbLinea" runat="server" Width="200px" ParentControls="cbEmpresa" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel18" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                        <asp:UpdatePanel ID="updtransportista" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TransportistaDropDownCheckList ID="cbTransportista" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" AddNoneItem="true" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                        <asp:UpdatePanel ID="upCentros" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:CentroDeCostosDropDownCheckList ID="cbCentroCostos" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel19" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                        <asp:UpdatePanel ID="updVehiculo" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:MovilDropDownCheckList ID="cbVehiculo" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea,cbTransportista,cbCentroCostos" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" />
                                <asp:AsyncPostBackTrigger ControlID="cbTransportista" />
                                <asp:AsyncPostBackTrigger ControlID="cbCentroCostos" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Entities" VariableName="PARENTI16" />
                        <asp:UpdatePanel ID="updTipoMensaje" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoMensajeDropDownCheckList ID="cbTipoMensaje" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" Todos="(Todos los actuales)" Ninguno="(Todos)" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblEmpleado" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                        <asp:UpdatePanel ID="updEmpleado" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="cbEmpleado" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="upd13" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbPerfil" />
                                <asp:AsyncPostBackTrigger ControlID="cbVehiculo" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMensaje" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td>
                    <cwc:AbmTitledPanel ID="AbmTitledPanel4" runat="server" Title="Direcciones IP de acceso" Height="200px">
                        <div style="margin-top: 10px;">
                            <uc2:IpRangeEditor ID="ipRanges" runat="server"></uc2:IpRangeEditor>
                        </div>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="tabParametros" runat="server" Title="Parametros">
        <asp:UpdatePanel ID="updParametros" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <C1:C1GridView id="gridParametros" runat="server" Width="100%" cssclass="SmallGrid" autogeneratecolumns="False" OnRowDataBound="gridParametros_RowDataBound">
                    <Columns>
                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="NOMBRE" SortDirection="Ascending" SortExpression="Nombre">
                            <ItemStyle Width="50%" />
                            <ItemTemplate>
                                <asp:TextBox ID="txtNombre" runat="server" Width="100%" />
                            </ItemTemplate>
                        </c1h:C1ResourceTemplateColumn>
                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR">
                            <ItemStyle Width="50%" />
                            <ItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Width="100%" />
                            </ItemTemplate>
                        </c1h:C1ResourceTemplateColumn>
                    </Columns>
                </C1:C1GridView>
                <div style="text-align: right;">
                    <asp:Button  ID="btAddParameter" runat="server" Text="Agregar Parametro" CssClass="LogicButton" OnClick="btAddParameter_Click" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>
</asp:Content>
