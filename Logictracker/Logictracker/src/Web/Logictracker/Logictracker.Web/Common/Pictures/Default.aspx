<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Common.Pictures.ShowPictures" Codebehind="Default.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fotos</title>
    <link rel="stylesheet" type="text/css" href="style.css" />    
</head>
<body>
    <form id="form1" runat="server">
        
        <asp:Panel ID="panelGallery" runat="server">
            <div id="gallery">
              <div id="time"></div>
              <div id="imagearea">
                <div id="image">
                  <a href="javascript:slideShow.nav(-1)" class="imgnav " id="previmg"></a>
                  <a href="javascript:slideShow.nav(1)" class="imgnav " id="nextimg"></a>
                </div>
              </div>
              <div id="thumbwrapper">
                <div id="thumbarea">
                  <ul id="thumbs" runat="server">
                  </ul>
                </div>
              </div>
            </div>

            <script type="text/javascript">
            var imgid = 'image';
            var imgdir = '<asp:Literal id="litDir" runat="server" />';
            var thumbid = 'thumbs';
            var timeid = 'time';
            var auto = true;
            var autodelay = 5;
            </script>
            <script type="text/javascript" src="slide.js"></script>
        </asp:Panel>
        <asp:Panel ID="panelNotFound" runat="server" Visible="false" style="background-color: #FFFFFF; text-align: center; padding: 50px; padding-top: 80px;">
            
            <h1>
            <cwc:ResourceLabel ID="lblNotFoundMessage" runat="server" Font-Bold="true" ResourceName="SystemMessages" VariableName="PICTURES_NOT_FOUND" />
            </h1>            
            <p>
                <cwc:ResourceLabel ID="lblNotFoundText" runat="server" ResourceName="SystemMessages" VariableName="PICTURES_NOT_FOUND_TEXT"  />
            </p>
            <p>
                <cwc:ResourceLinkButton ID="lblNotFoundRefreshLink" runat="server" ResourceName="Controls" VariableName="BUTTON_REFRESH" OnClick="lblNotFoundRefreshLink_Click"  />
            </p>
            <script type="text/javascript">
                document.body.style.backgroundColor = '#FFFFFF'; //pone el fondo blanco cuando no hay fotos
            </script>
        </asp:Panel>
    </form>
</body>
</html>
