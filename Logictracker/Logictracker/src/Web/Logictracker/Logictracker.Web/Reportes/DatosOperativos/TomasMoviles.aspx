<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TomasMoviles.aspx.cs" Inherits="Logictracker.Reportes.DatosOperativos.EstadisticaTomasMoviles" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" />

    <script type="text/javascript">
function PrintReport(){
    try{
        
        var oIframe = document.getElementById('ifPrint');
        var oContent = document.getElementById('printContent').innerHTML;
        var oDoc = (oIframe.contentWindow || oIframe.contentDocument);
        if (oDoc.document) oDoc = oDoc.document;
                
		oDoc.write("<html><head><title>"+document.getElementsByTagName('title')[0].innerHTML);
		oDoc.write("</title>");
		var links = document.getElementsByTagName('link');		
		for(l in links) if(links[l].type == 'text/css') oDoc.write('<link href="'+links[l].getAttribute('href')+'" type="text/css" rel="stylesheet" />');
		oDoc.write("</head><body onload='this.focus(); this.print();'>");
		oDoc.write(oContent + "</body></html>");	    
		oDoc.close();
    }
    catch(e){
    alert(e);
	    self.print();
    }
}
    </script>

    <%--FILTROS--%>
    <asp:Panel ID="panel" runat="server" SkinID="FilterPanel">
        <table width="100%">
            <tr>
                <td style="color: #999999; width: 100px;">
                    <span id="btExpandFilters" runat="server" style="cursor: pointer; font-size: 9px;
                        color: Blue; padding-top: 5px;" onclick="$get('trFiltrosAvanzados').style.display = $get('trFiltrosAvanzados').style.display == 'none' ? '' : 'none';">
                        ( + )</span> Filtros<br />
                </td>
                <td align="left" style="width: 275px">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true" />
                </td>
                <td align="left" style="width: 250px">
                    <cwc:ResourceLabel ID="lblLinea" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlLinea" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLabel runat="server" ID="lblTipoCoche" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server" Width="200px" ParentControls="ddlDistrito,ddlLinea" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel runat="server" ID="lblCostCenter" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI37" />
                    <br />
                    <asp:UpdatePanel ID="upCostCenter" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:CentroDeCostosDropDownList ID="ddlCostCenter" AddAllItem="true" runat="server" Width="200px" ParentControls="ddlDistrito,ddlLinea" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel runat="server" ID="ResourceLabel1" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TransportistaDropDownList ID="ddlTransportista" AddAllItem="true" runat="server" Width="200px" ParentControls="ddlDistrito,ddlLinea" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="right">
                    <cwc:ResourceButton ID="btnActualizar" runat="server" ResourceName="Controls" VariableName="BUTTON_REFRESH" OnClick="BtnSearchClick" />
                </td>
            </tr>
        </table>
    
    <div id="trFiltrosAvanzados" style="display: none; border-top: solid 1px #cccccc; padding: 5px; text-align: right;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table style="width: 100%; color: #999999;">
                    <tr>
                        <td align="left">
                            <cwc:ResourceCheckBox ID="chkDispositivosAsignados" runat="server" Checked="true" AutoPostBack="true" ResourceName="Labels" VariableName="SOLO_DISPOSITIVOS" />
                        </td>
                        <td align="left">
                            <cwc:ResourceCheckBox ID="chkVerDirecciones" runat="server" AutoPostBack="true" ResourceName="Labels" VariableName="VER_DIRECCIONES" />
                        </td>
                        <td style="text-align: right;">
                            <asp:Panel ID="panelBuscar" runat="server">
                                Buscar
                                <asp:TextBox ID="txtBuscar" runat="server" Width="200px" AutoPostBack="true"></asp:TextBox>
                                <div style="width: 20px; height: 20px; float: right; padding-top: 5px; text-align: center;
                                    position: relative; right: 24px; cursor: pointer;" onclick="var t = $get('<%= txtBuscar.ClientID %>'); t.value = ''; <%= Page.ClientScript.GetPostBackEventReference(txtBuscar, "") %>;">
                                    X</div>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <cwc:ResourceCheckBox ID="chkOcultarInactivos" runat="server" Checked="true" AutoPostBack="true" ResourceName="Labels" VariableName="OCULTAR_INACTIVOS" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <cwc:ResourceCheckBox ID="chkSoloConGarmin" runat="server" Checked="false" AutoPostBack="true" ResourceName="Labels" VariableName="SOLO_CON_GARMIN" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </asp:Panel>
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
    <asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <c1:C1GridView ID="grid" runat="server" SkinID="ListGrid">
            </c1:C1GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnActualizar" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="updasdf" runat="server">
        <ContentTemplate>
            <asp:Panel ID="PanelModal" runat="server" style="display: none;">
                <div style="background-color: lightgray; border: black; width: auto; padding: 30px;">
                    <table>
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="labelReportName" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="REPORTNAME" />
                            </td>
                            <td align="left">
                                <asp:TextBox ID="textBoxReportName" runat="server" MaxLength="120" TextMode="SingleLine" Width="300px" Height="30px" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="ResourceLabel7" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="PERIODICIDAD" />
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="cbPeriodicidad" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="MAIL" />
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtMail" runat="server" MaxLength="500" TextMode="MultiLine" Width="300px" Height="40px" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:RadioButton ID="rbutExcel" runat="server" Font-Bold="true" Text="Excel Completo" GroupName="FormatList" ValidationGroup="FormatList" Checked="true" />
                                <asp:RadioButton ID="rbutHtml" runat="server" Font-Bold="true" Text="Email Resumido" GroupName="FormatList" ValidationGroup="FormatList" Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceButton ID="btnGuardar" runat="server" ResourceName="Controls" VariableName="BUTTON_SOLICITAR" OnClick="BtScheduleGuardarClick" />
                                <cwc:ResourceButton ID="btnCancelar" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceLabel ID="lblLeyenda" runat="server" ResourceName="Labels" VariableName="CONDICION_REPORTE_PROGRAMADO" ForeColor="#FF0000" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelBobo" runat="server"></asp:Panel>
            <AjaxToolkit:ModalPopupExtender TargetControlId="panelBobo" PopupControlID="PanelModal" ID="mpePanel" CancelControlID="btnCancelar" runat="server" BackgroundCssClass="disabled_back" />
        </ContentTemplate>
     </asp:UpdatePanel>
     
     <asp:UpdatePanel ID="UpdatePanelSendReport" runat="server">
        <ContentTemplate>
            <asp:Panel ID="PanelModalSendReport" runat="server" style="display: none;">
                <div style="background-color: White; border: solid 1px black; width: auto; padding: 30px;">
                    <table>
                        <tr>
                            <td align="right">
                                <cwc:ResourceLabel ID="LabelMailSendReport" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="MAIL" />
                            </td>
                            <td align="left">
                                <asp:TextBox ID="TextBoxEmailSendReport" runat="server" MaxLength="500" TextMode="SingleLine" Width="300px" Height="40px" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <cwc:ResourceButton ID="ButtonOkSendReport" runat="server" ResourceName="Controls" VariableName="BUTTON_SOLICITAR" />
                                <cwc:ResourceButton ID="ButtonCancelSendReport" runat="server" ResourceName="Controls" VariableName="BUTTON_CANCEL" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <asp:Panel ID="hiddenPanelSendReport" runat="server"></asp:Panel>
            <AjaxToolkit:ModalPopupExtender TargetControlId="hiddenPanelSendReport" PopupControlID="PanelModalSendReport" ID="popUpSendReport" CancelControlID="ButtonCancelSendReport" runat="server" BackgroundCssClass="disabled_back" />
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div id="printContent" style="display: none;">
        <asp:UpdatePanel ID="upFiltrosPrinta" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <table style="width: 100%; border-spacing: 5px;">
                    <asp:Repeater ID="FiltrosPrint" runat="server" Visible="true">
                        <HeaderTemplate>
                            <tr>
                                <td style="text-decoration: underline; width: 20%">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Filtros" />
                                    <br />
                                    <br />
                                </td>
                                <td />
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="text-decoration: underline; width: 20%" valign="top">
                                    <%# Eval("key") %>:
                                </td>
                                <td>
                                    <%# Eval("value") %>
                                    <br />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
        <asp:UpdatePanel ID="upPrint" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <c1:C1GridView ID="gridPrint" runat="server" SkinID="PrintGrid">
                </c1:C1GridView>
            </ContentTemplate>
            <Triggers>
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <iframe id="ifPrint" width="0" height="0" style="visibility: hidden;">
        <%--<iframe id="ifPrint" width="100%" height="600" >--%>
    </iframe>
    </form>
</body>
</html>
