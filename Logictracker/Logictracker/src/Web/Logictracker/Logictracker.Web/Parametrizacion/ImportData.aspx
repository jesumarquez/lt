<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ImportData.aspx.cs" MasterPageFile="~/MasterPages/MasterPage.master" Inherits="Logictracker.Parametrizacion.ParametrizacionImportData" Title="Importador Masivo" %>

<%@ Register src="~/App_Controls/Pickers/NumberPicker.ascx" tagname="NumberPicker" tagprefix="uc1" %> 
<%@ Register src="~/App_Controls/DireccionImport.ascx" tagname="DireccionImport" tagprefix="uc1" %>
<%@ Register src="~/App_Controls/TicketImport.ascx" tagname="TicketImport" tagprefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <%--TOOLBAR--%>  
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="PAR_IMPORTADOR" />

    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
 
    <table id="tbTipoPunto" style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblArchivo" runat="server" ResourceName="Labels" VariableName="ARCHIVO" />
                <asp:FileUpload ID="fuImportData" runat="server" />
            </td>
        </tr>
    <tr>
            <td>
    <AjaxToolkit:Accordion ID="acdImport" runat="server">
        <Panes>
            <AjaxToolkit:AccordionPane runat="server" ID="acpPanel">
                <Header>
                    <cwc:ResourceLabel ID="lblImportChoferes" runat="server" ResourceName="Labels" VariableName="IMPORT_CHOFERES" Font-Bold="true" />
                </Header>
                <Content>
                    <table style="width: 100%; font-weight: bold">
                         <tr>
                            <td align="left" style="width: 250px">
                                <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <br />
                                <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <br />
                                <asp:UpdatePanel runat="server" ID="upPlanta" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlPlanta" AddAllItem="true" runat="server" Width="175px" ParentControls="ddlLocation" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                              <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblTipoE" runat="server" ResourceName="Entities" VariableName="PARENTI43" />
                                <br />
                                <asp:UpdatePanel runat="server" ID="upTipoE" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TipoEmpleadoDropDownList ID="ddlTipoEmpleado" runat="server" Width="175px" ParentControls="ddlLocation,ddlPlanta" AddNoneItem="true" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="right" style="width: 75px">
                                <cwc:ResourceLinkButton ID="btnTemplateEmpleado" runat="server" OnClick="btnTemplateEmpleado_Click"
                                    ResourceName="Labels" VariableName="PLANTILLA" />       
                            </td>
                            <td align="right" style="width: 75px">
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ResourceLinkButton ID="btnHelpEmpleado" runat="server" OnClick="btnHelpEmpleado_Click"
                                            ResourceName="Controls" VariableName="BUTTON_HELP" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnHelpEmpleado" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <tr>
                            <td align="right">
                                <cwc:ResourceButton ID="btnImportarChoferes" runat="server" Width="85px" OnClick="btnImportarChoferes_Click"
                                    ResourceName="Controls" VariableName="BUTTON_IMPORT" />
                                    <cwc:ResourceCheckBox ID="chkExportarTxt" runat="server" ResourceName="Controls" VariableName="Exportar inexistentes" AutoPostBack="false" />
                            </td>
                            </tr>
                        </tr>
                    </table>
                </Content>
            </AjaxToolkit:AccordionPane>
            <AjaxToolkit:AccordionPane ID="AcordVehiculos" runat="server" HeaderCssClass="accordionHeader" ContentCssClass="accordionContent">
                <Header>
                    <cwc:ResourceLabel ID="lblImportVehiculos" runat="server" ResourceName="Labels" VariableName="IMPORT_VEHICULOS" Font-Bold="true" />
                </Header>
                <Content>
                    <table width="100%" style="font-weight: bold">
                        <tr>
                            <td align="left" style="width: 210px">
                                <cwc:ResourceLabel ID="lblLocacionVehiculos" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlLocacionVehiculos" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 210px">
                                <cwc:ResourceLabel ID="lblPlantaVehiculos" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlPlantaVehiculos" runat="server" Width="175px"
                                            ParentControls="ddlLocacionVehiculos" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlLocacionVehiculos" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="left" style="width: 210px">
                                <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Labels" VariableName="TYPE" />
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" AddSinAsignar="true" AutoPostBack="false" Width="175px"
                                            ParentControls="ddlPlantaVehiculos" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlLocacionVehiculos" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlPlantaVehiculos" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="left" style="width: 75px">
                                <cwc:ResourceLinkButton ID="btnTemplateVehiculo" runat="server" OnClick="btnTemplateVehiculo_Click"
                                    ResourceName="Labels" VariableName="PLANTILLA" />
                            </td>
                            <td align="left" style="width: 75px"> 
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" >
                                    <ContentTemplate>
                                        <cwc:ResourceLinkButton ID="btnHelpVehiculo" runat="server" OnClick="btnHelpVehiculo_Click"
                                            ResourceName="Controls" VariableName="BUTTON_HELP" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnHelpVehiculo" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5" align="right" style="width: 100%">
                                <cwc:ResourceButton ID="btnImportarVehiculos" runat="server" Width="85px" OnClick="btnImportarVehiculos_Click"
                                    ResourceName="Controls" VariableName="BUTTON_IMPORT" />
                            </td>
                        </tr>
                    </table>                       
                </Content>
            </AjaxToolkit:AccordionPane>
            
            <AjaxToolkit:AccordionPane runat="server" ID="AccordionPane3">
                <Header>
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="IMPORT_POIS" Font-Bold="true" /> (Nomenclados)
                </Header>
                <Content>
                    <uc1:DireccionImport ID="DireccionImport1" runat="server" />
                </Content>
            </AjaxToolkit:AccordionPane> 
            
            <AjaxToolkit:AccordionPane runat="server" ID="AccordionPane1">
                <Header>
                    <cwc:ResourceLabel ID="lblImportPOIS" runat="server" ResourceName="Labels" VariableName="IMPORT_POIS" Font-Bold="true" />
                </Header>
                <Content>
                    <table width="100%" style="font-weight: bold">
                        <tr>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" ParentControls="ddlDistrito" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="left" style="width: 220px">
                                <cwc:ResourceLabel ID="lblTipoPOI" runat="server" ResourceName="Labels" VariableName="TYPE" />
                                <asp:UpdatePanel ID="upTipoDomicilio" runat="server" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoDomicilio" runat="server" AutoPostBack="false" Width="175px"
                                            ParentControls="ddlBase, ddlDistrito" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="left" style="width: 125px">
                                <cwc:ResourceLabel ID="lblGeocerca" runat="server" ResourceName="Labels" VariableName="SON_GEOCERCA" />
                                <asp:CheckBox ID="cbGeocercas" runat="server" AutoPostBack="true" OnCheckedChanged="cbGeocercas_CheckedChanged" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="upRadio" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <cwc:ResourceLabel ID="lblRadio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="RADIO" />
                                        <uc1:NumberPicker ID="npRadio" runat="server" Mask="999" Width="100" MaximumValue="999" Number="100"
                                            Enabled="false" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbGeocercas" EventName="CheckedChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="5" align="right" style="width: 100%">
                                <br />
                                <cwc:ResourceButton ID="btnImportarPuntosDeInteres" runat="server" Width="85px" ResourceName="Controls"
                                    OnClick="btnImportarPuntosDeInteres_Click" VariableName="BUTTON_IMPORT" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </AjaxToolkit:AccordionPane>                
            <AjaxToolkit:AccordionPane runat="server" ID="AccordionPane2">
                <Header>
                    <cwc:ResourceLabel ID="lblImportGeocercas" runat="server" ResourceName="Labels" VariableName="IMPORT_GEOCERCAS" Font-Bold="true" />
                </Header>
                <Content>
                    <table width="100%" style="font-weight: bold">
                        <tr>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblDistritoGeocerca" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlDistritoGeocerca" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblBaseGeocerca" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <asp:UpdatePanel runat="server" ID="upBaseGeocercas" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlBaseGeocerca" runat="server" Width="175px"
                                            ParentControls="ddlDistritoGeocerca" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistritoGeocerca" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="left" style="width: 220px">
                                <cwc:ResourceLabel ID="lblTipoGeocerca" runat="server" ResourceName="Labels" VariableName="TYPE" />
                                <asp:UpdatePanel ID="upTipoGeocerca" runat="server" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoGeocerca" runat="server" Width="175px"
                                            ParentControls="ddlDistritoGeocerca, ddlBaseGeocerca" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistritoGeocerca" EventName="SelectedIndexChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="ddlBaseGeocerca" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                         </tr>
                        <tr>
                            <td colspan="3" align="right" style="width: 100%">
                                <br />
                                <cwc:ResourceButton ID="btnImportarGeocercas" runat="server" Width="85px" ResourceName="Controls"
                                    OnClick="btnImportarGeocercas_Click" VariableName="BUTTON_IMPORT" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </AjaxToolkit:AccordionPane>  
                        <AjaxToolkit:AccordionPane runat="server" ID="acpTarjetas">
                <Header>
                    <cwc:ResourceLabel ID="lbTarjetas" runat="server" ResourceName="Labels" VariableName="IMP_TARJETA" Font-Bold="true" />
                </Header>
                <Content>
                    <table width="100%" style="font-weight: bold">
                        <tr>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblDistritoTarjeta" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlDistritoTarjeta" AddAllItem="true" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblBaseTarjeta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <asp:UpdatePanel runat="server" ID="upBaseTarjeta" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlBaseTarjeta" AddAllItem="true" runat="server" Width="175px" ParentControls="ddlDistritoTarjeta" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistritoTarjeta" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                             
                            <td align="left" style="width: 75px">
                                <cwc:ResourceLinkButton ID="btnPlantillaTarjeta" runat="server" OnClick="btnTemplateTarjeta_Click"
                                    ResourceName="Labels" VariableName="PLANTILLA" />
                            </td>
                            <td align="left" style="width: 75px"> 
                                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" >
                                    <ContentTemplate>
                                        <cwc:ResourceLinkButton ID="btnHelpTarjeta" runat="server" OnClick="btnHelpTarjeta_Click"
                                            ResourceName="Controls" VariableName="BUTTON_HELP" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnHelpTarjeta" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>                          
                        </tr>
                        <tr>
                            <td colspan="5" align="right" style="width: 100%">
                                <br />
                                <cwc:ResourceButton ID="rbTarjetas" runat="server" Width="85px" ResourceName="Controls"
                                    OnClick="btnImportarTarjetas_Click" VariableName="BUTTON_IMPORT" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </AjaxToolkit:AccordionPane>
           
            <%--DISPOSITIVOS--%>
           
            <AjaxToolkit:AccordionPane runat="server" ID="apDispositivos">
                <Header>
                    <cwc:ResourceLabel ID="lblImpDispositivo" runat="server" ResourceName="Labels" VariableName="IMP_DISPOSITIVO" Font-Bold="true" />
                </Header>
                <Content>
                    <table width="100%" style="font-weight: bold">
                        <tr>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblDistritoDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlDistritoDispositivo" AddAllItem="true" runat="server" Width="175px" />
                            </td>
                            <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblBaseDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <asp:UpdatePanel runat="server" ID="upBaseDispositivo" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:PlantaDropDownList ID="ddlBaseDispositivo" AddAllItem="true" runat="server" Width="175px" ParentControls="ddlDistritoDispositivo" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlDistritoDispositivo" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                             <td align="left" style="width: 225px">
                                <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" ResourceName="Entities" VariableName="PARENTI32" />
                                <asp:UpdatePanel runat="server" ID="UpdatePanel6" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" Width="175px" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlTipoDispositivo" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            
                            <td align="left" style="width: 75px">
                                <cwc:ResourceLinkButton ID="btnTemplateDispositivo" runat="server" OnClick="btnTemplateDispositivo_Click"
                                    ResourceName="Labels" VariableName="PLANTILLA" />
                            </td>
                            <td align="left" style="width: 75px"> 
                                <asp:UpdatePanel ID="upHelpDispositivo" runat="server" UpdateMode="Conditional" >
                                    <ContentTemplate>
                                        <cwc:ResourceLinkButton ID="btnHelpDispositivo" runat="server" OnClick="btnHelpDispositivo_Click"
                                            ResourceName="Controls" VariableName="BUTTON_HELP" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnHelpDispositivo" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>                    
                        </tr>
                        <tr>
                            <td colspan="5" align="right" style="width: 100%">
                                <br />
                                <cwc:ResourceButton ID="btnImportarDispositivo" runat="server" Width="85px" ResourceName="Controls"
                                    OnClick="btnImportarDispositivo_Click" VariableName="BUTTON_IMPORT" />
                            </td>
                        </tr>
                    </table>
                </Content>
            </AjaxToolkit:AccordionPane>
            
            <%--Tickets--%>
            
            <AjaxToolkit:AccordionPane runat="server" ID="apTickets">
                <Header>
                    <cwc:ResourceLabel ID="lblImportadorTickets" runat="server" ResourceName="Labels" VariableName="IMPORT_TICKETS" Font-Bold="true" />
                </Header>
                <Content>
                    <uc1:TicketImport ID="TicketImport1" runat="server" />
                </Content>
            </AjaxToolkit:AccordionPane>
            
        </Panes>
    </AjaxToolkit:Accordion>
    </td>
        </tr>
    </table>
</asp:Content>
