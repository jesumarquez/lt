﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Logictracker.Operacion.Camaras.Operacion_Camaras_Default" Theme="" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body { margin: 0px; font-family: Verdana;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <script type="text/javascript">
var BaseURL = '<asp:Literal ID="litBaseUrl" runat="server" />';
var DisplayWidth = <asp:Literal ID="litWidth" runat="server" />;
var DisplayHeight = <asp:Literal ID="litHeight" runat="server" />;
var File = '<asp:Literal ID="litUrl" runat="server" />';

var output = "";
if ((navigator.appName == "Microsoft Internet Explorer")
&& (navigator.platform != "MacPPC") && (navigator.platform != "Mac68k")) {
// If Internet Explorer under Windows then use ActiveX
output = '<OBJECT ID="Player" width='
output += DisplayWidth;
output += ' height=';
output += DisplayHeight;
output += ' CLASSID="CLSID:DE625294-70E6-45ED-B895-CFFA13AEB044" ';
output += 'CODEBASE="';
output += BaseURL;
output += 'activex/AMC.cab">';
output += '<PARAM NAME="MediaURL" VALUE="';
output += BaseURL + File + '">';
output += '<param name="MediaType" value="mjpeg-unicast">';
output += '<param name="ShowStatusBar" value="0">';
output += '<param name="ShowToolbar" value="0">';
output += '<param name="AutoStart" value="1">';
output += '<param name="StretchToFit" value="1">';
output += '<BR><B>Axis Media Control</B><BR>';
output += 'The AXIS Media Control, which enables you ';
output += 'to view live image streams in Microsoft Internet';
output += ' Explorer, could not be registered on your computer.';
output += '<BR></OBJECT>';
} else {
// If not IE for Windows use the browser itself to display
theDate = new Date();
output = '<IMG SRC="';
output += BaseURL;
output += File;
output += '&dummy=' + theDate.getTime().toString(10);
output += '" HEIGHT="';
output += DisplayHeight;
output += '" WIDTH="';
output += DisplayWidth;
output += '" ALT="">';
}
document.write(output);
</script>
    </div>
    </form>
</body>
</html>
