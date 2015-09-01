<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="EditGeoRef.ascx.cs" Inherits="Logictracker.App_Controls.EditGeoRef" %>
<%@ Register src="DireccionSearch.ascx" tagname="DireccionSearch" tagprefix="uc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<table style="width: 100%;margin: auto; height: 300px;">
    <tr>
        <td style="width: 350px; vertical-align: top;">
        <uc1:DireccionSearch ID="DireccionSearch1" runat="server" Height="300px" OnDireccionSelected="DireccionSearch1DireccionSelected" />
        </td>
        <td id="tdmonitor" style=" vertical-align: top;">
        <asp:Panel ID="panMonitor" runat="server">
            <div><mon:Monitor runat="server" ID="monitor" /></div>
        </asp:Panel>
        </td>
    </tr>
</table>
<script type="text/javascript">
 var add = 35;
function _resizemap()
{
var td = document.getElementById('tdmonitor');
var w = td.offsetWidth;
var h = td.offsetHeight;
var pan = document.getElementById('<% Response.Write(panMonitor.ClientID); %>');
pan.style.width = (w-4) +'px';
pan.style.height = (h+add) +'px';
add = -2;
}
window.onresize = _resizemap;
_resizemap();
</script>
