<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" Inherits="Logictracker.Telemetria.Mediciones" Codebehind="Mediciones.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="REP_MEDICIONES" /> 
    
    <table width="100%">
        <tr>
            <td colspan="3"><br/></td>
        </tr>    
        <tr>
            <td width="20%">&nbsp;</td>
            <td width="60%" align="center">
                <iframe height='500px' width='100%' name='reportEmbed-RI_TELEMETRIA_VIEW_HISTORICO' frameborder='0' scrolling='auto' allowTransparency ='true' src='https://creator.zoho.com/zoho_isphu/sri/view-embed/RI_TELEMETRIA_VIEW_HISTORICO/&zc_Search=false&zc_Summary=false&zc_Header=false'></iframe>
            </td>
            <td width="20%">&nbsp;</td>
        </tr>
    </table>
    
</asp:Content>