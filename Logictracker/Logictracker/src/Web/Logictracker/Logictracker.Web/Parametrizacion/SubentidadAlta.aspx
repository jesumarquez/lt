<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="SubentidadAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.SubentidadAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top" width="50%">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="250px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="60%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="60%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <cwc:ResourceLinkButton ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76" OnClientClick="window.open('../Parametrizacion/TipoEntidadLista.aspx', 'tipoEntidad')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upTipoEntidad" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" Width="60%" AddAllItem="true" ParentControls="cbEmpresa, cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLinkButton ID="lblEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI79" OnClientClick="window.open('../Parametrizacion/EntidadLista.aspx', 'entidad')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upEntidad" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:EntidadDropDownList ID="cbEntidad" runat="server" Width="60%" ParentControls="cbEmpresa, cbLinea, cbTipoEntidad" OnSelectedIndexChanged="CbEntidadOnSelectedIndexChanged" AutoPostBack="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoEntidad" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLinkButton ID="lblTipoMedicion" runat="server" ResourceName="Entities" VariableName="PARENTI77" OnClientClick="window.open('../Parametrizacion/TipoMedicionLista.aspx', 'tipoMedicion')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upTipoMedicion" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoMedicionDropDownList ID="cbTipoMedicion" runat="server" Width="60%" AddAllItem="true" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLinkButton ID="lblSensor" runat="server" ResourceName="Entities" VariableName="PARENTI80" OnClientClick="window.open('../Parametrizacion/SensorLista.aspx', 'sensor')" ForeColor="Black" />
                        <asp:UpdatePanel ID="upSensor" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:SensorDropDownList ID="cbSensor" runat="server" Width="60%" AddNoneItem="true" ParentControls="cbEmpresa, cbLinea, cbTipoMedicion" OnSelectedIndexChanged="CbSensor_OnSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoMedicion" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                                                
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" />
                        
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>                        
                                <cwc:ResourceLabel ID="lblControlaMaximo" runat="server" ResourceName="Labels" VariableName="CONTROLA_MAXIMO" Visible="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="updChkMax" runat="server">
                            <ContentTemplate>
                                <asp:CheckBox ID="chkControlaMaximo" runat="server" Visible="false" OnCheckedChanged="ChkControlaMaximo_OnCheckedChanged" AutoPostBack="true" />    
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblMaximo" runat="server" ResourceName="Labels" VariableName="MAXIMO" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="updChkMin" runat="server">
                            <ContentTemplate>
                                <asp:TextBox ID="txtMaximo" runat="server" MaxLength="10" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblControlaMinimo" runat="server" ResourceName="Labels" VariableName="CONTROLA_MINIMO" Visible="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <asp:CheckBox ID="chkControlaMinimo" runat="server" Visible="false" OnCheckedChanged="ChkControlaMinimo_OnCheckedChanged" AutoPostBack="true" />
                            </ContentTemplate>
                        </asp:UpdatePanel> 
                        
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                            <ContentTemplate>
                                <cwc:ResourceLabel ID="lblMinimo" runat="server" ResourceName="Labels" VariableName="MINIMO" />
                            </ContentTemplate>
                        </asp:UpdatePanel> 
                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                            <ContentTemplate>
                                <asp:TextBox ID="txtMinimo" runat="server" MaxLength="10" />
                            </ContentTemplate>
                        </asp:UpdatePanel> 
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td valign="top" width="50%">
                    <cwc:TitledPanel ID="panelRight" runat="server" TitleVariableName="PLANO" TitleResourceName="Labels" Height="250px" Style="position: relative;">
                        <asp:UpdatePanel ID="pnlImg" runat="server">
                            <ContentTemplate>                                
                                <div style="position: relative; width: auto; margin: auto;">
                                     <asp:ImageButton ID="imgAnchor" runat="server" style="border: solid 1px #000000; cursor: crosshair;" OnClick="ImgAnchor_Click" />
                                     <asp:Image ID="imgAnchorPointer" runat="server" Visible="false" ImageUrl="~/images/anchor.gif" Width="10px" Height="10px" />
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:TitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
