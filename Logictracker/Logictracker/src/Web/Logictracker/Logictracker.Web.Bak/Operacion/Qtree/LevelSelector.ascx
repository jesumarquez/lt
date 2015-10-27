 <%@ Control Language="C#" AutoEventWireup="true" CodeFile="LevelSelector.ascx.cs" Inherits="Logictracker.Operacion.Qtree.Operacion_Qtree_LevelSelector" %>

<asp:UpdatePanel ID="updLevelSelector" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" RenderMode="Inline">
<ContentTemplate>

    <asp:Panel ID="panNivel" runat="server" Width="60px">
        <asp:Button ID="btSelected" runat="server" Text="0" CommandArgument="-1" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" OnClientClick="return false;" />
    </asp:Panel>
    
    <AjaxToolkit:DropDownExtender ID="dd" runat="server" TargetControlID="panNivel" DropDownControlID="panDropDown"></AjaxToolkit:DropDownExtender>

    <asp:Panel ID="panDropDown" runat="server" Width="180px" style="background-color: #FFFFFF; border: solid 1px black; padding: 3px; z-index: 9999999999;">
    
        <asp:Button ID="btNivel0" runat="server" Text="0" CommandArgument="0" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel1" runat="server" Text="1" CommandArgument="1" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel2" runat="server" Text="2" CommandArgument="2" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel3" runat="server" Text="3" CommandArgument="3" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />

        <asp:Button ID="btNivel4" runat="server" Text="4" CommandArgument="4" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel5" runat="server" Text="5" CommandArgument="5" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel6" runat="server" Text="6" CommandArgument="6" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel7" runat="server" Text="7" CommandArgument="7" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />

        <asp:Button ID="btNivel8" runat="server" Text="8" CommandArgument="8" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel9" runat="server" Text="9" CommandArgument="9" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel10" runat="server" Text="10" CommandArgument="10" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel11" runat="server" Text="11" CommandArgument="11" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />

        <asp:Button ID="btNivel12" runat="server" Text="12" CommandArgument="12" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel13" runat="server" Text="13" CommandArgument="13" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel14" runat="server" Text="14" CommandArgument="14" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />
        <asp:Button ID="btNivel15" runat="server" Text="15" CommandArgument="15" OnCommand="btNivel_Command" Width="40px" Height="40px" BorderWidth="1" BorderColor="Black" Style="margin: 2px 0px 0px 2px;" />

    </asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>