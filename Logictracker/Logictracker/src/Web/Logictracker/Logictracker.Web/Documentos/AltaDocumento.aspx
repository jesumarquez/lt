<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Documentos.Documentos_AltaDocumento" Title="Untitled Page" Codebehind="AltaDocumento.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server"> 
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:Panel ID="PanelForm" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

