<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TomasDetalle.aspx.cs" EnableEventValidation="false"
    Inherits="Logictracker.Reportes.DatosOperativos.EstadisticaTomasDetalle" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/App_Controls/Pickers/NumberPicker.ascx" TagPrefix="uc" TagName="NumberPicker" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title ></title>
</head>
<body>

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

    <form id="form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" />
    <%--FILTROS--%>
    <asp:Panel ID="panel" runat="server" SkinID="FilterPanel">
        <table width="100%">
            <tr>
                <td align="left">
                    <uc:NumberPicker ID="npCount" runat="server" Mask="999" MaximumValue="100" Number="10"
                        Width="50" />
                    <cwc:ResourceLabel ID="lblUltimas" runat="server" Font-Bold="true" ResourceName="Labels"
                        VariableName="ULTIMAS_POSICIONES" />
                </td>
                <td align="right">
                    <cwc:ResourceButton ID="butonSearch" Width="75px" runat="server" OnClick="BtnSearchClick"
                        ResourceName="Controls" VariableName="BUTTON_REFRESH" />
                </td>
            </tr>
        </table>
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
    <iframe id="ifPrint" width="0" height="0" style="visibility: hidden;"></iframe>
    </form>
</body>
</html>
