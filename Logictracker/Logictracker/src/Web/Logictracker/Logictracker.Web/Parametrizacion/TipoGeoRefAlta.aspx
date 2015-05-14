<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TipoGeoRefAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionTipoGeoRefAlta" %>

<%@ Register Src="../App_Controls/IconPicker.ascx" TagName="SelectIcon" TagPrefix="uc1" %>  
<%@ Register Src="../App_Controls/ColorPicker.ascx" TagName="ColorPicker" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">

    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="150px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="100%" AutoPostBack="true" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" AutoPostBack="true" OnSelectedIndexChanged="OwnerSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
            
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="6" />
            
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                    
                    </cwc:AbmTitledPanel>
                </td>
                <td style="vertical-align: top;width:50%;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="COMPORTAMIENTO" TitleResourceName="Labels" Height="150px">
                        
                        <br /><cwc:ResourceCheckBox ID="chkControlaVelocidad" runat="server" AutoPostBack="true" Width="100%" ResourceName="Labels" VariableName="CONTROLA_VELOCIDAD" OnCheckedChanged="ChkControlaVelocidadCheckedChanged" />
                        <br /><cwc:ResourceCheckBox ID="chkControlaES" runat="server" ResourceName="Labels" VariableName="CONTROLA_ES" Width="100%"/>
                        <br /><cwc:ResourceCheckBox ID="chkZonaRiesgo" runat="server" ResourceName="Labels" VariableName="ES_ZONA_RIESGO" Width="100%"/>
                        <br /><cwc:ResourceCheckBox ID="chkInhibeAlarma" runat="server" ResourceName="Labels" VariableName="INHIBE_ALARMA" Width="100%"/>
                        <br /><cwc:ResourceCheckBox ID="chkExcluyeMonitor" runat="server" ResourceName="Labels" VariableName="EXCLUIR_MONITOR" Width="100%"/>
                        <br /><cwc:ResourceCheckBox ID="chkEsTaller" runat="server" ResourceName="Labels" VariableName="ES_TALLER" Width="100%"/>
                        
                        <br />
                        <asp:UpdatePanel ID="updChkControlaPermanencia" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkControlaPermanencia" runat="server" OnCheckedChanged="ChkControlaPermanenciaOnCheckedChanged" AutoPostBack="true" ResourceName="Labels" VariableName="CONTROLA_PERMANENCIA" />    
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br/>
                        <asp:UpdatePanel ID="updMaximaPermanencia" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblMaximaPermanencia" runat="server" ResourceName="Labels" VariableName="MAXIMA_PERMANENCIA" />
                                <asp:TextBox ID="txtMaximaPermanencia" runat="server" MaxLength="4" Width="35px" Enabled="False" Text="0" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                        <asp:UpdatePanel ID="updChkControlaPermanenciaEntrega" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceCheckBox ID="chkControlaPermanenciaEntrega" runat="server" OnCheckedChanged="ChkControlaPermanenciaEntregaOnCheckedChanged" AutoPostBack="true" ResourceName="Labels" VariableName="CONTROLA_PERMANENCIA_ENTREGA" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br/>
                        <asp:UpdatePanel ID="updMaximaPermanenciaEntrega" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblMaximaPermanenciaEntrega" runat="server" ResourceName="Labels" VariableName="MAXIMA_PERMANENCIA_ENTREGA" />
                                <asp:TextBox ID="txtMaximaPermanenciaEntrega" runat="server" MaxLength="4" Width="35px" Enabled="False" Text="0" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel3" runat="server" TitleVariableName="ESTILO" TitleResourceName="Labels" Height="120px">
                
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="ICON"></cwc:ResourceLabel>
                        <uc1:SelectIcon ID="SelectIcon2" runat="server" ParentControls="cbEmpresa,cbLinea"/>
                
                        <cwc:ResourceLabel ID="lblColor" runat="server" ResourceName="Labels" VariableName="COLOR" Width="100%" />
                        <uc1:ColorPicker id="ColorPicker" runat="server" />
                    
                    </cwc:AbmTitledPanel>
                </td>
                <td style="width: 50%; vertical-align: top;">
                    <cwc:AbmTitledPanel ID="AbmTitledPanel4" runat="server" TitleVariableName="TIME_TRACKING" TitleResourceName="Labels" Height="120px">                
                
                        <br /><cwc:ResourceCheckBox ID="chkInicio" runat="server" ResourceName="Labels" VariableName="ES_INICIO" Width="100%"/>
                        <br /><cwc:ResourceCheckBox ID="chkIntermedio" runat="server" ResourceName="Labels" VariableName="ES_INTERMEDIO" Width="100%"/> 
                        <br /><cwc:ResourceCheckBox ID="chkFin" runat="server" ResourceName="Labels" VariableName="ES_FIN" Width="100%"/>
        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
                        
    <cwc:AbmTabPanel ID="AbmTabPanel1" runat="server" ResourceName="Labels" VariableName="VELOCIDADES" >
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td style="width: 50%; vertical-align: top;">    
                    <asp:UpdatePanel ID="updGridVelocidades" runat="server">
                        <ContentTemplate>
                            <cwc:AbmTitledPanel ID="AbmTitledPanel2" runat="server">
                                
                                <C1:C1GridView ID="gridVelocidades" runat="server" SkinID="SmallGrid" DataKeyNames="Id" OnRowDataBound="GridVelocidadesItemDataBound">
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
                                </C1:C1GridView>                 
                
                            </cwc:AbmTitledPanel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="chkControlaVelocidad" EventName="CheckedChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>        
            </tr>
        </table>
    </cwc:AbmTabPanel>   
</asp:Content>