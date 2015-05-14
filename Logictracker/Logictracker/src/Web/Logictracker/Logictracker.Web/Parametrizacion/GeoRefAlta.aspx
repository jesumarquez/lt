<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="GeoRefAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionGeoRefAlta" Title="" %>

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="IconPicker" TagPrefix="uc1" %>
<%@ Register Src="../App_Controls/EditGeoRef.ascx" TagName="EditGeoRef" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/Pickers/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register Src="../App_Controls/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_REF_GEO">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panAlarmas" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES" Height="350px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%" AutoPostBack="true" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" AutoPostBack="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI10"></cwc:ResourceLabel>
                        <asp:UpdatePanel ID="updTipoReferenciaGeografica" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="cbTipoReferenciaGeografica_SelectedIndexChanged" FilterMode="false" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="CODE"></cwc:ResourceLabel>
                        <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESCRIPCION"></cwc:ResourceLabel>
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="128" />
                        
                        <cwc:ResourceLabel ID="lblObservaciones" runat="server" Width="100%" ResourceName="Labels" VariableName="OBSERVACIONES" />
                        <asp:TextBox ID="txtObservaciones" runat="server" Width="98%" MaxLength="256" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Width="100%" ResourceName="Labels" VariableName="VIGENCIA_DESDE" />
                        <cwc:DateTimePicker ID="txtFechaDesde" runat="server" Mode="DateTime" TimeMode="Start" PopupPosition="TopLeft" IsValidEmpty="true" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Width="100%" ResourceName="Labels" VariableName="VIGENCIA_HASTA" />
                        <cwc:DateTimePicker ID="txtFechaHasta" runat="server" Mode="DateTime" TimeMode="End" PopupPosition="TopLeft" IsValidEmpty="true" />
                        
                        <asp:Label ID="lblEstilo" runat="server" Font-Bold="true" Text="Estilo" />
                        <div>
                        </div>
                        
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="ICON"></cwc:ResourceLabel>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>  
                                <uc1:IconPicker ID="SelectIcon2" runat="server" ParentControls="cbEmpresa,cbLinea" OnSelectedIconChange="SelectIcon2_SelectedIconChange" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoReferenciaGeografica"/>
                            </Triggers>
                        </asp:UpdatePanel> 
                                                
                        <cwc:ResourceLabel ID="lblColor" runat="server" ResourceName="Labels" VariableName="COLOR" Width="100px" />
                        <asp:UpdatePanel ID="updColor" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>  
                                <uc:ColorPicker ID="cpColor" runat="server" AutoPostback="true" OnColorChanged="cpColor_ColorChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbTipoReferenciaGeografica"/>
                            </Triggers>
                        </asp:UpdatePanel>
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:TitledPanel ID="AbmTitledPanel1" runat="server" TitleResourceName="Labels" TitleVariableName="COMPORTAMIENTO" Height="350px">
                        <br />
                        <asp:UpdatePanel ID="updComportamiento" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="vertical-align: top;">
                                            <cwc:ResourceCheckBox ID="chkControlaVelocidad" runat="server" Enabled="false" ResourceName="Labels" VariableName="CONTROLA_VELOCIDAD" />
                                            <br />
                                            <br />
                                            <cwc:ResourceCheckBox ID="chkControlaES" runat="server" Enabled="false" ResourceName="Labels" VariableName="CONTROLA_ES" />
                                            <br />
                                            <br />
                                            <cwc:ResourceCheckBox ID="chkIgnoraLogiclink" runat="server" Enabled="true" ResourceName="Labels" VariableName="IGNORA_LOGICLINK" />
                                        </td>
                                        <td style="vertical-align: top;">
                                            <cwc:ResourceCheckBox ID="chkZonaRiesgo" runat="server" Enabled="false" ResourceName="Labels" VariableName="ES_ZONA_RIESGO" />
                                            <br />
                                            <br />
                                            <cwc:ResourceCheckBox ID="chkInhibeAlarma" runat="server" ResourceName="Labels" VariableName="INHIBE_ALARMA" />
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <br />
                                <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Time Tracking" />
                                <br />
                                <br />
                                <cwc:ResourceCheckBox ID="chkInicio" runat="server" ResourceName="Labels" VariableName="ES_INICIO" />
                                <br />
                                <br />
                                <cwc:ResourceCheckBox ID="chkIntermedio" runat="server" ResourceName="Labels" VariableName="ES_INTERMEDIO" />
                                <br />
                                <br />
                                <cwc:ResourceCheckBox ID="chkFin" runat="server" ResourceName="Labels" VariableName="ES_FIN" />
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td style="width: 100%; vertical-align: top;" colspan="2">
                    <cwc:TitledPanel ID="AbmTitledPanel2" runat="server" TitleResourceName="Entities" TitleVariableName="PARENTI05">
                        <div style="text-align: right;">
                            <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="APLICAR_DESDE" />
                            <cwc:DateTimePicker ID="txtVigenciaDesde" runat="server" Mode="DateTime" PopupPosition="TopLeft" IsValidEmpty="true" />
                            <asp:Button ID="btBorrarPunto" runat="server" Text="Borrar Direccion" OnClick="btBorrarPunto_Click" />
                            <asp:Button ID="btBorrarPoly" runat="server" Text="Borrar Geocerca" OnClick="btBorrarPoly_Click" />
                        </div>
                        <uc1:EditGeoRef ID="EditGeoRef1" runat="server" />
                    </cwc:TitledPanel>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btBorrarPunto" />
                            <asp:AsyncPostBackTrigger ControlID="btBorrarPoly" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="VELOCIDADES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                <asp:Label ID="lblVelocidades" runat="server" ForeColor="Red" Text="No se encontraron datos" Visible="false" />
                    <cwc:AbmTitledPanel ID="panelVelocidades" runat="server" TitleResourceName="Entities" TitleVariableName="Velocidades" Visible="false">
                        <asp:UpdatePanel ID="updGridVelocidades" runat="server">
                            <ContentTemplate>
                                <c1:C1GridView ID="gridVelocidades" runat="server" SkinID="SmallGrid" DataKeyNames="Id" OnRowDataBound="gridVelocidades_ItemDataBound">
                                    <Columns>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Entities" VariableName="PARENTI17">
                                            <ItemStyle Width="25%" HorizontalAlign="Left" />
                                        </c1h:C1ResourceTemplateColumn>
                                        <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VELOCIDAD_MAXIMA">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtVelocidadMaxima" runat="server" Width="75px" Text="0" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </c1h:C1ResourceTemplateColumn>
                                    </Columns>
                                </c1:C1GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoReferenciaGeografica" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
</asp:Content>
