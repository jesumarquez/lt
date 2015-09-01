<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="IconPicker.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_IconPicker" %>

<style type="text/css">
.icon
{
    width: 32px; 
    height: 32px; 
    background-color: White; 
    border: solid 1px Black;
}
</style>

<asp:UpdatePanel ID="updIconos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
<ContentTemplate>
<asp:Panel ID="imgSelectedPanel" runat="server" Width="52px">
    <asp:Panel ID="imgSelected" runat="server"  CssClass="icon" /></asp:Panel>
    <AjaxToolkit:DropDownExtender ID="dde" runat="server" DropDownControlID="panelIconos" TargetControlID="imgSelectedPanel"  />
    
    <asp:Panel ID="panelIconos" runat="server" Width="420px" Height="300px" ScrollBars="Vertical" Style="background-color: White; border: solid 1px black; visibility: hidden;z-index: 999999999;">
        <asp:Repeater ID="gridIconos" runat="server" OnItemDataBound="gridIconos_ItemDataBound" OnItemCommand="gridIconos_ItemCommand">
            <ItemTemplate>
                <div style="float: left; margin-top: 5px; margin-left: 3px; width: 64px; height: 64px;">
                        <asp:Literal ID="litAconym" runat="server"></asp:Literal>
                            <asp:Button ID="Button1" runat="server" Text="" Width="64px" Height="64px" Style="background-color: White; border: solid 1px Black; " CommandName="Select" CommandArgument="0" />
                            <br />
                            <div  style="background-color: #f0f0f0; font-size: 9px; position: relative; left: 2px; bottom: 12px; width: 60px; overflow: hidden; height: 10px;">
                                <asp:Label ID="lblDescri" runat="server" Text=""></asp:Label>
                            </div>
                        </acronym>
                    </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
