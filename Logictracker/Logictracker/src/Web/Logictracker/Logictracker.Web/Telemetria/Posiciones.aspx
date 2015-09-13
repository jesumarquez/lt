<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" Inherits="Logictracker.Telemetria.Posiciones" Codebehind="Posiciones.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="REP_POSICIONES" /> 
    
    <iframe height='500px' width='100%' name='reportEmbed-RI_POSICIONES_VIEW_ACTIVAS' frameborder='0' scrolling='auto' allowTransparency ='true' src='https://creator.zoho.com/zoho_isphu/sri/view-embed/RI_POSICIONES_VIEW_ACTIVAS/zc_Paging=false&zc_Search=false&zc_Summary=false&zc_Header=false'></iframe>
    
</asp:Content>