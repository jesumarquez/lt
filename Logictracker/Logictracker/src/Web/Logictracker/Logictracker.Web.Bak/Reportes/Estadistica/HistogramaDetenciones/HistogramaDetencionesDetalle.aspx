<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HistogramaDetencionesDetalle.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.HistogramaDetenciones.ReportesEstadisticaHistogramaDetencionesHistogramaDetencionesDetalle" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title />
</head>
<body>
    <form id="form1" runat="server">
    
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

    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" />
    
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
    
     <%--GRID--%>
        <asp:UpdatePanel ID="upGrid" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <C1:C1GridView ID="gridDetenciones" runat="server" SkinID="ListGrid">
                </C1:C1GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <div id="printContent" style="display: none;">
        
        <asp:UpdatePanel ID="upFiltrosPrinta" runat="server" UpdateMode="Always">
        <ContentTemplate>
        <table style="width: 100%; border-spacing: 5px;">
        <asp:Repeater ID="FiltrosPrint" runat="server" Visible="true">
            <HeaderTemplate>
                <tr>
                    <td style="text-decoration:underline; width:20%">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Filtros" />
                        <br/> 
                        <br/> 
                    </td> 
                    <td  />
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="text-decoration:underline; width:20%" valign="top" >
                        <%# Eval("key") %>:
                    </td>
                    <td >
                        <%# Eval("value") %> <br/> 
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
        </ContentTemplate>
        </asp:UpdatePanel>
        <br/> 
        
        <asp:UpdatePanel ID="upPrint" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            
            <C1:C1GridView ID="gridPrint" runat="server" SkinID="PrintGrid">
            </C1:C1GridView>
           
        </ContentTemplate>
        <Triggers>
        </Triggers>
        </asp:UpdatePanel>  
        
    </div>    
    <iframe id="ifPrint" width="0" height="0" style="visibility: hidden;" >
    <%--<iframe id="ifPrint" width="100%" height="600" >--%>
    </iframe>
    
    
    </form>
</body>
</html>