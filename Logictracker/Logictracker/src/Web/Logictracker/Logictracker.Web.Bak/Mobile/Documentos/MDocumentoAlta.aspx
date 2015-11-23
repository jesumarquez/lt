 <%@ Page Language="C#" AutoEventWireup="true" CodeFile="MDocumentoAlta.aspx.cs" Inherits="Logictracker.Mobile.Documentos.MDocumentos_Mobile_DocumentoAlta" Theme="" %>    

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"> <title></title>
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
    <form id="form1" runat="server">
    <div>
            <%--TOOLBAR--%>
            <div id="divToolbar">
                <asp:Button runat="server" ID="btnGuardar" Text="Guardar" OnClick="btnSave_Click"/>
                <asp:Button runat="server" ID="btnEliminar" Text="Eliminar" OnClick="btnDelete_Click" />
                <asp:Button runat="server" ID="btnListado" Text="Listado" OnClick="btnList_Click"/>
            </div>
            <asp:Label ID="lblError" runat="server" Font-Size="X-Small" ForeColor="#EE0000" />      
           
            <asp:Panel ID="PanelForm" runat="server"></asp:Panel>
    </div>
    </form>
</body>
</html>
