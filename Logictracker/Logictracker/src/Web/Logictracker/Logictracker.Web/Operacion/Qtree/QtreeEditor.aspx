<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.Qtree.Parametrizacion_QtreeEditor" Codebehind="QtreeEditor.aspx.cs" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register src="LevelSelector.ascx" tagname="LevelSelector" tagprefix="uc" %>
<%@ Register src="AutoGenConfig.ascx" tagname="AutoGenConfig" tagprefix="uc" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
    <style type="text/css">
        body
        {
            overflow: hidden;
            margin: 0px;
            padding: 0px;
            border: 0px none;
            font-family: Verdana, Arial, Sans-Serif;
            font-size: 11px;
        }
        html, body
        {
            height: 100%;
        }
        form
        {
            width: 100%;
            height: 100%;
        }
    </style>
    <script type="text/javascript">
        function getDimensions() {
            var winWidth, winHeight;
            var d = document;
            if (typeof window.innerWidth != 'undefined') 
            {
                winWidth = window.innerWidth;
                winHeight = window.innerHeight;
            } 
            else 
            {
                if (d.documentElement && typeof d.documentElement.clientWidth != 'undefined' && d.documentElement.clientWidth != 0) 
                {
                    winWidth = d.documentElement.clientWidth;
                    winHeight = d.documentElement.clientHeight;
                } 
                else 
                {
                    if (d.body && typeof d.body.clientWidth != 'undefined') 
                    {
                        winWidth = d.body.clientWidth;
                        winHeight = d.body.clientHeight;
                    }
                }
            }
            return { width: winWidth, height: winHeight };
        }
        function Resize() {
            var size = getDimensions();
            var div = $get('progress');
            div.style.width = size.width + "px";
            div.style.height = size.height + "px";
            var msg = $get('progress_message');
            msg.style.top = (size.height/2 - 40) + "px";
            msg.style.left = (size.width/2 - 100) + "px";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" AsyncPostBackTimeout="2400" />
        
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<img src='logoqtree.png' />" tabPosition="top"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="250" split="false" minSize="250" maxSize="250" collapsible="false" title="Edicion"></cc1:LayoutRegion>
        <cc1:LayoutRegion ID="rgEast" TargetControlID="EastPanel" runat="server" initialSize="250" split="false" minSize="250" maxSize="250" collapsible="false" title="Opciones"></cc1:LayoutRegion>
        
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" East="rgEast" West="rgWest" runat="server"></cc1:LayoutManager>
        
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
    
    
        <asp:Panel ID="CenterPanel" runat="server">
            <mon:Monitor ID="Monitor" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
    
        <asp:Panel ID="WestPanel" runat="server">
            
            <div class="toolbar" style="padding-left: 3px; ">
            <table style="width: 100%" cellspacing="0"><tr><td>
                <asp:UpdatePanel ID="updStates" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true"><ContentTemplate>
                    <asp:ImageButton ID="btInfo" runat="server" ImageUrl="information.png" CommandArgument="0" Width="16px" Height="16px" style="padding: 5px;" OnCommand="btState_Click" />
                    <asp:ImageButton ID="btEdit" runat="server" ImageUrl="pencil.png" CommandArgument="1" Width="16px" Height="16px" style="padding: 5px;" OnCommand="btState_Click" />
                    <asp:ImageButton ID="btPickColor" runat="server" ImageUrl="colorpicker.png" CommandArgument="2" Width="16px" Height="16px" style="padding: 5px;" OnCommand="btState_Click" />
                </ContentTemplate></asp:UpdatePanel>
            </td></tr></table>
            </div>
    
            <div class="toolbar" style="padding-top: 3px; padding-bottom: 3px;">
                <asp:UpdatePanel ID="updSize" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                    
                        <table style="width: 98%;"><tr>
                        <td style="font-size: 9px;">Tama&ntilde;o</td>
                        <td><asp:TextBox ID="Slider1" runat="server" Width="100%"></asp:TextBox> </td>
                        <td style="width: 30px;"><asp:TextBox ID="lblSlider" runat="server" Width="100%"></asp:TextBox></td>
                        </tr></table>
                        
                        <ajaxToolkit:SliderExtender ID="SliderExtender1" runat="server" TargetControlID="Slider1" Minimum="1" Maximum="6" BoundControlID="lblSlider" Steps="6" />
                        
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            
            <div class="filterpanel">
                <table style="width: 100%"><tr><td style="font-size: 9px;">Nivel&nbsp;
                </td><td style="width: 60px;">
                    <uc:LevelSelector ID="lvlSel" runat="server" />
                </td></tr></table>
            </div>
            <div class="filterpanel">
            <asp:CheckBox ID="chkLock" runat="server" Text="Bloquear celdas" Checked="false" />
            </div>
               
            <asp:UpdatePanel ID="updInfo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                <asp:Panel ID="panelInfo" runat="server" BorderColor="Black" BorderWidth="1px" style="margin-top: 50px;">
                    <table>
                    <tr><td style="font-weight: bold;">Archivo</td><td><asp:Label Id="lblArchivo" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Sector</td><td><asp:Label Id="lblSector" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Byte</td><td><asp:Label Id="lblByte" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Nivel</td><td><asp:Label Id="lblInfoNivel" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Estado</td><td><asp:Label Id="lblLock" runat="server"></asp:Label></td></tr>
                    </table>
                </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>

    </asp:Panel>
    <asp:Panel ID="EastPanel" runat="server">
        <cc1:TabContainer ID="tabOpciones" runat="server" ActiveTabIndex="0" Height="600px" Width="250px">
        
            <cc1:TabPanel ID="tabArchivo" runat="server" HeaderText="Archivo">
                <ContentTemplate>
                <asp:UpdatePanel ID="updArchivo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <table class="Grid_Header"><tr><td>
                        Formato de Qtree
                    </td></tr></table>
                    <table style="width: 100%"><tr>
                        <td>
                            <asp:DropDownList ID="cbQtreeFormat" runat="server" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cbQtreeFormat_SelectedIndexChanged"></asp:DropDownList>
                        </td>
                    </tr></table>
                
                    <table class="Grid_Header"><tr><td>
                        Abrir Qtree existente
                    </td></tr></table>
                    
                    <table style="width: 100%"><tr>
                        <td>
                            <asp:UpdatePanel ID="updQtrees" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="cbQtree" runat="server" Width="100%"></asp:DropDownList>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbQtreeFormat" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td style="width: 66px;"><asp:Button ID="btNuevo" runat="server" Text="Abrir" OnClick="btNuevo_Click" Width="60px" /></td>
                    </tr></table>
                    
                    <table class="Grid_Header"><tr><td>
                        Informaci&oacute;n del Qtree Actual
                    </td></tr></table>
                    
                    <table style="width: 100%" cellspacing="5">
                    <tr><td style="font-weight: bold;">Nombre</td><td style="font-size: 12px;font-weight: bold;"><asp:Label ID="lblQtreeName" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Latitud</td><td><asp:Label ID="lblPosicionLat" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Longitud</td><td><asp:Label ID="lblPosicionLon" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Tamaño</td><td><asp:Label ID="lblGridSize" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Ancho celda</td><td><asp:Label ID="lblCellWidth" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Alto celda</td><td><asp:Label ID="lblCellHeight" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;">Sectores</td><td><asp:Label ID="lblSectorCount" runat="server"></asp:Label></td></tr>
                    <tr><td style="font-weight: bold;"></td><td></td></tr>
                    </tr></table>
                    <hr />
                    
                    <table class="Grid_Header"><tr><td>
                        <asp:LinkBUtton ID="btCrear" runat="server" Text="Crear un nuevo Qtree" OnClick="btCrear_Click" style="text-decoration: none; color: inherit;" />
                    </td></tr></table>
                    <asp:Panel ID="panelCrearQtree" runat="server" Visible="false" style="padding: 10px;">
                    <b>1</b>. Ingrese el nombre del nuevo Qtree
                    <asp:TextBox ID="txtNewQtreeName" runat="server" Width="100%"></asp:TextBox>
                    <asp:Panel ID="panelCrearGte" runat="server">
                    <br />
                    <b>2</b>. Ingrese el tamaño de las celdas (grados / 10.000.000)
                    <br />
                    <br />
                    <table>
                    <tr><td>Ancho</td><td><asp:TextBox ID="txtCellWidth" runat="server" Width="50px"></asp:TextBox></td></tr>
                    <tr><td>Alto</td><td><asp:TextBox ID="txtCellHeight" runat="server" Width="50px"></asp:TextBox></td></tr>
                    </table>
                    <br />
                    <b>3</b>. Seleccione el area del Qtree con la herramienta <i>Dibujar Cuadrado</i> del mapa (<img src="draw_square.gif" alt="Dibujar Cuadrado" style="vertical-align: middle" />)
                    <br />
                    <br />
                    </asp:Panel>
                    <table style="width: 100%; text-align:center;"><tr><td style="width: 50%">
                        <asp:Button ID="btCancelarNuevo" runat="server" Text="Cancelar" OnClick="btCancelarNuevo_Click" />
                    </td><td>
                        <asp:Button ID="btAceptarNuevo" runat="server" Text="Aceptar" OnClick="btAceptarNuevo_Click" />
                    </td></tr></table>                   
                    
                    </asp:Panel>
                </ContentTemplate>
                </asp:UpdatePanel>
                </ContentTemplate>
            </cc1:TabPanel>
            
            <cc1:TabPanel ID="tabAutoGen" runat="server" HeaderText="AutoGen">
                <ContentTemplate>                
                    <uc:AutoGenConfig ID="autoGenConfig" runat="server"></uc:AutoGenConfig>  
                    
                    <asp:UpdatePanel ID="updAutoGen" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <div style="text-align: right;">
                            <asp:Button ID="btAutoGen" runat="server" OnClick="btAutoGen_Click" Text="Auto-Generar Qtree" Width="100%" style="padding: 5px;" />
                        </div>
                    </ContentTemplate>
                    </asp:UpdatePanel>                
                </ContentTemplate>
            </cc1:TabPanel>
                       
        </cc1:TabContainer>
        
       
        
        
    </asp:Panel>
    
    <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div id="progress" class="disabled_back" style="position: absolute; z-index: 9999999999;">
                <div id="progress_message" style="position: absolute; border: solid 5px #AA0000; color: #AA0000; background-color: White; text-align: center; padding: 10px; width: 200px; font-weight: bold;">
                Espere por favor...
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <script type="text/javascript">
        $addHandler(window, 'resize', Resize);
        Resize();
    </script>
    </form>
</body>
</html>
