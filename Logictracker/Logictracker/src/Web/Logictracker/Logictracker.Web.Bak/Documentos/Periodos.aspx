<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="Periodos.aspx.cs" Inherits="Logictracker.Documentos.Documentos_Periodos" Title="Untitled Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table>
<tr>
<td>
<asp:Panel ID="panelEmpresa" runat="server">
<cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
<cwc:LocacionDropDownList ID="cbEmpresa" runat="server" OnSelectedIndexChanged="FilterChangedHandler" />
</asp:Panel>    
</td>
<td>
    A&ntilde;o: 
        <asp:LinkButton ID="btAnioDown" runat="server" CommandName="Down" OnCommand="ChangeAnio">&lt;&lt;</asp:LinkButton>
        <asp:Label ID="lblAnio" runat="server"></asp:Label>
        <asp:LinkButton ID="btAnioUp" runat="server" CommandName="Up" OnCommand="ChangeAnio">&gt;&gt;</asp:LinkButton>                
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btGenerar" runat="server" Text="Generar" OnClick="btGenerar_Click" />
</td><td>
    
</td></tr></table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>
