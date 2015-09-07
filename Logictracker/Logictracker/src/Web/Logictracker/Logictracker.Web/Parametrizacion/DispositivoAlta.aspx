<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Parametrizacion.ParametrizacionDispositivoAlta" Title="Dispositivos" Codebehind="DispositivoAlta.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownCheckLists" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_ASIGNACION" TitleResourceName="Labels" Height="115px">
                        
                        <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI32" />
                        <asp:UpdatePanel ID="upTipoDispositivo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="200px" OnSelectedIndexChanged="DdlTipoDispositivoSelectedIndexChanged" AutoPostBack="True" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" AddAllItem="true"  />
                        
                        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" ParentControls="ddlLocacion" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblMobile" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                        <asp:TextBox ID="tbMovil" runat="server" MaxLength="64" Width="196px" Enabled="false"/>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosDispositivo" runat="server" TitleVariableName="DATOS_DISPOSITIVO" TitleResourceName="Labels" Height="115px">
                        
                        <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" MaxLength="64" Width="196px" />
                        
                        <cwc:ResourceLabel ID="lblIMEI" runat="server" ResourceName="Labels" VariableName="IMEI" />
                        <asp:TextBox ID="txtIMEI" runat="server" MaxLength="64" Width="196px" />
                        
                        <cwc:ResourceLabel ID="lblEstado" runat="server" ResourceName="Labels" VariableName="STATE" />
                        <asp:UpdatePanel ID="upEstado" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EstadoDispositivosDropDownList ID="ddlEstado" runat="server" Width="200px"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblId" runat="server" ResourceName="Labels" VariableName="ID" />
                        <asp:TextBox ID="txtId" runat="server" MaxLength="10" Width="196px" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosReporte" runat="server" TitleVariableName="DATOS_REPORTE" TitleResourceName="Labels" Height="200px">
                        
                        <cwc:ResourceLabel ID="lblPrecinto" runat="server" ResourceName="Entities" VariableName="PARENTI78" />
                        <cwc:PrecintoDropDownList ID="cbPrecinto" runat="server" Width="200px" ParentControls="" AddAllItem="false" AddNoneItem="true" />
                                            
                        <cwc:ResourceLabel ID="lblPuerto" runat="server" ResourceName="Labels" VariableName="PUERTO" />
                        <asp:TextBox ID="txtPuerto" runat="server" Text="0" Width="200px" MaxLength="10" Enabled="false" />
                        
                        <cwc:ResourceLabel ID="lblPollInterval" runat="server" ResourceName="Labels" VariableName="POLL_INTERVAL" />
                        <c1:C1NumericInput ID="npPollInterval" runat="server" ShowNullText="false" Number="60" MaximumValue="99" Width="200px" DecimalPlaces="0" Height="17px" />
                        
                        <cwc:ResourceLabel ID="lblClave" runat="server" ResourceName="Labels" VariableName="CLAVE" />
                        <asp:TextBox ID="txtClave" runat="server" Width="200px" MaxLength="10" />
                        
                        <cwc:ResourceLabel ID="lblTablas" runat="server" ResourceName="Labels" VariableName="TABLAS" />
                        <c1:C1NumericInput ID="npTablas" runat="server" ShowNullText="false" Number="0" MaximumValue="99" Width="200px" DecimalPlaces="0" Height="17px" />

                        <cwc:ResourceLabel ID="lblEmpresaTelefonica" runat="server" ResourceName="Labels" VariableName="EMPRESA" />
                        <cwc:EmpresaTelefonicaDropDownList ID="cbEmpresaTelefonica" runat="server" Width="200px" AddAllItem="true" />
                        
                        <cwc:ResourceLinkButton ID="lblLineaTelefonica" runat="server" ResourceName="Entities" VariableName="PARENTI74" OnClientClick="window.open('LineaTelefonicaLista.aspx','lineaTelefonica')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upLineaTelefonica" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:LineaTelefonicaDropDownList ID="cbLineaTelefonica" runat="server" Width="200px" ParentControls="cbEmpresaTelefonica" AddAllItem="false" AddNoneItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresaTelefonica" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLabel ID="lblTelefono" runat="server" ResourceName="Labels" VariableName="MDN" />
                        <asp:TextBox ID="txtTelefono" runat="server" Width="200px" MaxLength="10" />
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="lblDatosFirmware" runat="server" TitleVariableName="FIRMWARE" TitleResourceName="Labels" Height="200px">
                        
                        <cwc:ResourceLabel ID="lblFirmware" runat="server" ResourceName="Labels" VariableName="FIRMWARE" />
                        <asp:UpdatePanel ID="upFirmware" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:FirmwareDropDownList ID="ddlFirmware" runat="server" Width="200px"  />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="lblConfiguracion" runat="server" ResourceName="Labels" VariableName="CONFIGURACION" />
                        <cwc:ConfiguracionDispositivoDropDownCheckList ID="ddlConfiguracion" runat="server" Width="200px" AutoPostBack="false" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="abmTabDetalles" runat="server" Title="Detalles" >
        <asp:UpdatePanel ID="updDetalles" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <C1:C1GridView ID="grid" runat="server" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id,IdParametro" OnRowDataBound="GridRowDataBound">
                    <Columns>
                        <c1h:C1ResourceBoundColumn DataField="Parametro" ResourceName="Labels" VariableName="PARAMETRO" SortExpression="Parametro"  AllowGroup="false" AllowMove="false" AllowSizing="false" />
                        <c1h:C1ResourceBoundColumn DataField="Descripcion" ResourceName="Labels" VariableName="DESCRIPCION" SortExpression="Descripcion"  AllowGroup="false" AllowMove="false" AllowSizing="false" />
                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR" SortExpression="Valor" AllowGroup="false" AllowMove="false" AllowSizing="false">
                            <ItemTemplate>
                                <asp:TextBox ID="txtValor" runat="server" Width="100%" />
                            </ItemTemplate>
                            </c1h:C1ResourceTemplateColumn>
                        <c1h:C1ResourceBoundColumn DataField="Tipo" ResourceName="Labels" VariableName="TYPE" SortExpression="Tipo" AllowGroup="false" AllowMove="false" AllowSizing="false">
                            <ItemStyle Width="50px" />
                        </c1h:C1ResourceBoundColumn>                
                    </Columns>
                </C1:C1GridView> 
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlTipoDispositivo" />
            </Triggers>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>
</asp:Content>
