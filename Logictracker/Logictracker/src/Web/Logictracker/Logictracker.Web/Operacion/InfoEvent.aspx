<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoEvent.aspx.cs" Inherits="Logictracker.Operacion.Operacion_InfoEvent" %>

<%@ Register TagPrefix="cc1" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Alarma Logictracker</title>
    <style type="text/css">
    a { color: inherit; text-decoration: inherit; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
    <asp:Literal ID="litSonido" runat="server"></asp:Literal>
    <div>
        <asp:Panel ID="panelAlarma" runat="server" Height="310px" Width="238px">
            <asp:Panel ID="panel2" runat="server" Style="background-color: #cccccc; padding: 2px; font-size: 10px; text-align: center;">
                <asp:Label ID="lblHora" runat="server" Font-Bold="True" />
                <asp:Label ID="lblRecepcion" runat="server" />
            </asp:Panel>
            <asp:Panel ID="panelTitle" runat="server" Style="padding: 5px; font-weight: bold;">
                <asp:Image ID="imgAccion" runat="server" Style="float: left; margin-right: 5px;" />
                <asp:UpdatePanel runat="server" ID="updDummy" UpdateMode="Conditional" ChildrenAsTriggers="True" RenderMode="Inline">
                    <ContentTemplate>
                        <asp:LinkButton ID="lbMensaje" runat="server" Text="" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <asp:Panel ID="panelMovil" runat="server" Style="padding: 5px; font-weight: bold;">
                <asp:Image ID="imgMovil" runat="server" Style="float: left; margin-right: 5px;" />
                <asp:Label ID="lblMovil" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelChofer" runat="server" Style="padding: 5px;">
                <b><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI09" />:</b>
                <asp:Label ID="lblChofer" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelDireccion" runat="server" Style="padding: 5px;">
                <b>Lugar: </b>
                <asp:Label ID="lblDireccion" runat="server" Text="" Style="color: #999999" />
            </asp:Panel>
            <asp:Panel ID="panel1" runat="server" Style="padding: 5px;">
                <b>Latitud: </b>
                <asp:Label ID="lblLatitud" runat="server" Text="" Style="color: #999999" />
            </asp:Panel>
            <asp:Panel ID="panel3" runat="server" Style="padding: 5px;">
                <b>Longitud: </b>
                <asp:Label ID="lblLongitud" runat="server" Text="" Style="color: #999999" />
            </asp:Panel>
            <asp:Panel ID="panel7" runat="server" Style="padding: 5px;">
                <b>Mensaje: </b>
                <cwc:MensajesDropDownList ID="cbMensaje" runat="server" Width="85%" AutoPostBack="False" />
            </asp:Panel>
            <asp:Panel ID="panel5" runat="server" Style="padding: 5px;">
                <b>Observación: </b>
                <asp:TextBox ID="txtObservacion" runat="server" MaxLength="255" TextMode="MultiLine" Rows="2" />
            </asp:Panel>
            <asp:Panel ID="panelUsuario" runat="server" Style="padding: 5px;" Visible="False">
                <b>Usuario: </b>
                <asp:Label ID="lblUsuario" runat="server" Style="color: #999999" />
            </asp:Panel>
            <asp:Panel ID="panelFecha" runat="server" Style="padding: 5px;" Visible="False">
                <b>Fecha: </b>
                <asp:Label ID="lblFecha" runat="server" Style="color: #999999" />
            </asp:Panel>
            <asp:Panel ID="panelBotones" runat="server" Height="30px" Width="228px" Style="text-align: right; padding-right: 10px;">
                <asp:ImageButton ID="btAceptar" runat="server" ImageUrl="~/Operacion/btAceptar.gif" OnClick="BtAceptarClick" />
                <asp:ImageButton ID="btIgnorar" runat="server" ImageUrl="~/Operacion/btCancelar.gif" OnClick="BtIgnorarClick" />
            </asp:Panel>
            <asp:Panel ID="panelMonitor" runat="server" Style="padding: 5px;">
                <div style="width: 238px; height: 150px; border-top: solid 1px black;">
                    <div>
                        <cc1:Monitor ID="Monitor1" runat="server" Width="238px" Height="200px" />
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
        
        
    </div>
    </form>
</body>
</html>
