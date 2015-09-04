<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.DatosOperativos.VerificadorEmpleados" Codebehind="VerificadorEmpleados.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

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
            <tr valign="top">
                <td style="color: #999999; width: 100px;">
                    <span id="btExpandFilters" runat="server" style="cursor: pointer; font-size: 9px;
                        color: Blue; padding-top: 5px;" onclick="$get('trFiltrosAvanzados').style.display = $get('trFiltrosAvanzados').style.display == 'none' ? '' : 'none';">
                        ( + )</span> Filtros<br />
                </td>
                <td align="left" style="width: 275px">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" />
                    <br/>
                    <cwc:ResourceLabel ID="lblLinea" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true" ParentControls="cbEmpresa" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLinkButton runat="server" ID="lblCategoriaAcceso" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI15" ListControlTargetID="lstCategoriaAcceso" />
                    <br />
                    <asp:UpdatePanel ID="upCategoriaAcceso" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:CategoriaAccesoListBox ID="lstCategoriaAcceso" AddAllItem="true" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLinkButton runat="server" ID="lblTipoZonaAcceso" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI91" ListControlTargetID="lstTipoZonaAcceso" />
                    <br />
                    <asp:UpdatePanel ID="upTipoZonaAcceso" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoZonaAccesoListBox ID="lstTipoZonaAcceso" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLinkButton runat="server" ID="lblZonaAcceso" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI92" ListControlTargetID="lstZonaAcceso" />
                    <br />
                    <asp:UpdatePanel ID="upZonaAcceso" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:ZonaAccesoListBox ID="lstZonaAcceso" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea,lstTipoZonaAcceso" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lstTipoZonaAcceso" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr valign="top">
                <td></td>                
                <td>
                    <cwc:ResourceLinkButton runat="server" ID="lblCostCenter" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI37" ListControlTargetID="lstCentroCosto" />
                    <br />
                    <asp:UpdatePanel ID="upCostCenter" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:CentroDeCostosListBox ID="lstCentroCosto" AddAllItem="true" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton runat="server" ID="lblDepto" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI04" ListControlTargetID="lstDepartamento" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:DepartamentoListBox ID="lstDepartamento" AddAllItem="true" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLinkButton runat="server" ID="lblPuertas" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI55" ListControlTargetID="lstPuerta" />
                    <br />
                    <asp:UpdatePanel ID="upPuertas" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PuertaListBox ID="lstPuerta" runat="server" Width="200px" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 325px">
                    <cwc:ResourceLabel runat="server" ID="lblLegajo" Font-Bold="true" ResourceName="Labels" VariableName="LEGAJO" />
                    <br/>
                    <asp:TextBox runat="server" ID="txtLegajo" />            
                    <br/>
                    <cwc:ResourceLabel runat="server" ID="lblHorasFichada" Font-Bold="true" ResourceName="Labels" VariableName="HORAS_FICHADA" />
                    <br/>
                    <c1:C1NumericInput runat="server" ID="npHorasFichada" DecimalPlaces="0" Increment="1" Value="12" Width="50px" Height="14px"/>
                </td>
                <td align="left" valign="bottom">
                    <cwc:ResourceButton ID="btnActualizar" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtnSearchClick" />
                </td>
            </tr>
        </table>
    
    <div id="trFiltrosAvanzados" style="display: none; border-top: solid 1px #cccccc;
        padding: 5px; text-align: right;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table style="width: 100%; color: #999999;">
                    <tr><td style="text-align: right;">
                            <asp:Panel ID="panelBuscar" runat="server">
                                Buscar
                                <asp:TextBox ID="txtBuscar" runat="server" Width="200px" AutoPostBack="true" />
                                <div style="width: 20px; height: 20px; float: right; padding-top: 5px; text-align: center;
                                    position: relative; right: 24px; cursor: pointer;" onclick="var t = $get('<%= txtBuscar.ClientID %>'); t.value = ''; <%= Page.ClientScript.GetPostBackEventReference(txtBuscar, "") %>;">
                                    X</div>
                            </asp:Panel>
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
