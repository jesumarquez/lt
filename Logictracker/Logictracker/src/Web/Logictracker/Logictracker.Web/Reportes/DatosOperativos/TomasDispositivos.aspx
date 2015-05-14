<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TomasDispositivos.aspx.cs" Inherits="Logictracker.Reportes.DatosOperativos.ReportesTomasGps" %>

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
                <td align="left" style="width: 350px">
                    <cwc:ResourceLabel ID="lblTipoDispositivo" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI32" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoDispositivoDropDownList ID="ddlTipoDispositivo" runat="server" AddAllItem="true"
                                Width="200px" AutoPostBack="true" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
                <td align="left" style="width: 280px">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01"
                        Font-Bold="true" />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true"
                        AutoPostBack="true" />
                </td>
                <td align="left" style="width: 250px">
                    <cwc:ResourceLabel ID="lblLinea" runat="server" Font-Bold="true" ResourceName="Entities"
                        VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AddAllItem="true"
                                AutoPostBack="true" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="right">
                    <cwc:ResourceButton ID="btnActualizar" runat="server" ResourceName="Controls" VariableName="BUTTON_REFRESH"
                        OnClick="BtnSearchClick" />
                </td>
            </tr>
        </table>
        <div id="trFiltrosAvanzados" style="display: none; border-top: solid 1px #cccccc;
            padding: 5px; text-align: right;">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <table style="width: 100%; color: #999999;">
                        <tr>
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
                    </table>
                </ContentTemplate>
                <Triggers>
                    
                </Triggers>
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
