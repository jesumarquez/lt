<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IpRangeEditor.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_IpRangeEditor" %>

<table>
<tr>
<td>Desde</td>
<td><asp:TextBox ID="txtIpFrom" runat="server" MaxLength="15" Width="100px" AutoPostBack="true" OnTextChanged="txtIpFrom_TextChanged"></asp:TextBox></td>
<td>Hasta</td>
<td>
    <asp:UpdatePanel ID="updIpTo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false" >
        <ContentTemplate>
            <asp:TextBox ID="txtIpTo" runat="server" MaxLength="15" Width="100px"></asp:TextBox>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txtIpFrom" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>
</td>
<td>
<asp:Button ID="btAddIpRange" runat="server" onclick="btAddIpRange_Click" Text="Agregar" />
</td>
</tr>
<tr>
<td colspan="5">
    
</td>
</tr>
</table>
<asp:UpdatePanel ID="updIpRangeList" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false" >
        <ContentTemplate>
            <asp:ListBox ID="cbIpRanges" runat="server"  Width="100%"></asp:ListBox>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btAddIpRange" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btDelIpRange" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:Button ID="btDelIpRange" runat="server" onclick="btDelIpRange_Click" Text="Eliminar" />


