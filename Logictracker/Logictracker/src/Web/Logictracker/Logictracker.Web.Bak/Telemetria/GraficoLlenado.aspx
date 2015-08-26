<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="GraficoLlenado.aspx.cs" Inherits="Logictracker.Telemetria.GraficoLlenado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="REP_GRAFICO_LLENADO" /> 
    
    <iframe height='500px' width='100%' name='pageEmbed-Grafico_Contenedor' frameborder='0' scrolling='auto' allowTransparency ='true' src='https://creator.zoho.com/zoho_isphu/sri/view-embed/Grafico_Contenedor/'></iframe>
    
</asp:Content>