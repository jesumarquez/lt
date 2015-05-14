<%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoEventM2M.aspx.cs" Inherits="Logictracker.Monitor.MonitorDeEntidades.InfoEventM2M" %>

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
    <asp:Literal ID="litSonido" runat="server" />
    <div>
        <asp:Panel ID="panelAlarma" runat="server" Height="300px" Width="238px">
            <asp:Panel ID="panel2" runat="server" Style="background-color: #cccccc; padding: 2px;
                font-weight: bold; font-size: 10px; text-align: center;">
                <asp:Label ID="lblHora" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelTitle" runat="server" Style="padding: 5px; font-weight: bold;">
                <asp:Image ID="imgAccion" runat="server" Style="float: left; margin-right: 5px;" />
                <asp:LinkButton ID="lbMensaje" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelEntidad" runat="server" Style="padding: 5px;">
                <b><cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI79" />:</b>
                <asp:Label ID="lblEntidad" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelSubEntidad" runat="server" Style="padding: 5px;">
                <b><cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI81" />:</b>
                <asp:Label ID="lblSubEntidad" runat="server" Text="" />
            </asp:Panel>
            <asp:Panel ID="panelDetalles" runat="server" Style="padding: 5px;">
                
            </asp:Panel>
            <asp:Panel ID="panelDireccion" runat="server" Style="padding: 5px;">
                <b>Lugar: </b>
                <asp:Label ID="lblDireccion" runat="server" Text="" Style="color: #999999" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="panelBotones" runat="server" Height="30px" Width="228px" Style="text-align: right;
            padding-right: 10px;">
            <asp:ImageButton ID="btAceptar" runat="server" ImageUrl="~/Operacion/btAceptar.gif" OnClick="BtAceptarClick" />
            <asp:ImageButton ID="btIgnorar" runat="server" ImageUrl="~/Operacion/btCancelar.gif" OnClick="BtIgnorarClick" />
        </asp:Panel>
        <div style="width: 238px; height: 150px; border-top: solid 1px black;">
            <div>
                <cc1:Monitor ID="Monitor1" runat="server" Width="238px" Height="200px" />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
