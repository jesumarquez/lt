 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="MDocumentoLista.aspx.cs" Inherits="Logictracker.Mobile.Documentos.Documentos_Mobile_MDocumentoLista" Theme="" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    body, table, form {
    font-family: Verdana, Arial; 
    font-size: 11px;    
    margin: 0px;
}
body {background-color: #E7E7E7;}
    </style>
</head>
<body>
    <form runat="server" id="form1">
    <div>
        <table style="margin:2px; background-color:White ; border: solid 1px black; width: 100%;">
                  <tr><td colspan="2">
                  <asp:Button runat="server" ID="btnNuevo" Text="Nuevo" OnClick="btnNuevo_Click" />
                  <cwc:TipoDocumentoDropDownList ID="cbTipoDocumento" runat="server"
                        AutoPostBack="true" OnSelectedIndexChanged="cbTipoDocumento_SelectedIndexChanged" />
                        </td>    
            </tr>
            
        
        <asp:Repeater ID="grid" runat="server" Visible="true" OnItemDataBound="Repeater_ItemDataBound">
            <HeaderTemplate>
                <tr>
                    <td style=" background-color:#FFA500; padding: 2px;" colspan="2">
                        <asp:Label ID="Label1" runat="server" Width="500px" Font-Bold="true" Text="Documentos" />
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style='background-color: <%# Container.ItemIndex % 2== 0 ? "#FFFFFA":"#f7f7f7" %> '>
                        <asp:LinkButton runat="server" ID="btDocumento" Text="" OnCommand="btDocumento_Command" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        </table>
    </div>
    </form>
</body>
</html>
