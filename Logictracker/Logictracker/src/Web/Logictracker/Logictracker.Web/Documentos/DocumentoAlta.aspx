<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Documentos.Documentos_DocumentoAlta" Title="Untitled Page" Codebehind="DocumentoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>        
            <asp:Panel ID="PanelForm" runat="server">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

