<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="altaMarca.ascx.cs" Inherits="Logictracker.App_Controls.AppControlsAltaMarca" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>  

<asp:Panel ID="panelMarcas" runat="server">
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    
        <asp:MultiView ID="multiMarca" runat="server" ActiveViewIndex="0">
        
        <asp:View ID="viewSelect" runat="server">
            <table style="width: 100%; border-spacing: 0px; padding: 0px;">
            <tr>
                <td><cwc:MarcaDropDownList ID="cbMarca" runat="server" Width="100%" OnSelectedIndexChanged="cbMarca_SelectedIndexChanged" OnInitialBinding="cbMarca_PreBind" /></td>
                <td style="width: 22px; height: 22px;"><asp:ImageButton ID="btEdit" runat="server" ToolTip="Editar" ImageUrl="~/images/txt_edit.gif" OnClick="btEdit_Click" style="position: relative; right: 4px;" /></td>
                <td style="width: 22px; height: 22px;"><asp:ImageButton ID="btAdd" runat="server" ToolTip="Nuevo" ImageUrl="~/images/txt_add.gif" OnClick="btAdd_Click" style="position: relative; right: 6px;" /></td>
            </tr>
            </table>
        </asp:View>
        
        <asp:View ID="viewEdit" runat="server">
            <table style="width: 100%; border-spacing: 0px; padding: 0px;">
            <tr>
                <td><asp:TextBox ID="txtMarca" runat="server" Width="100%" /></td>
                <td style="width: 22px; height: 22px;"><asp:ImageButton ID="btAceptar" runat="server" ToolTip="Aceptar" ImageUrl="~/images/txt_aceptar.gif" OnClick="btAceptar_Click" style="position: relative; right: 4px;" /></td>
                <td style="width: 22px; height: 22px;"><asp:ImageButton ID="btCerrar" runat="server" ToolTip="Cancelar" ImageUrl="~/images/txt_cancelar.gif" OnClick="btCerrar_Click" style="position: relative; right: 6px;" /></td>
            </tr>
            </table>
        </asp:View>
        
        </asp:MultiView>

    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>