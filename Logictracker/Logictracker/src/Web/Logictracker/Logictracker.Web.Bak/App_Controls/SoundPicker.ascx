<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SoundPicker.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_SoundPicker" %>

<asp:Panel ID="panelSonido" runat="server">
<table style="width: 100%; border-spacing: 0px; padding: 0px;">
<tr>
<td>
<cwc:SonidoDropDownList ID="cbSonido" runat="server" Width="100%" OnInitialBinding="cbSonido_InitialBind" />
<asp:UpdatePanel ID="updSonido" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false"><ContentTemplate>
<asp:Literal ID="litSonido" runat="server" />
</ContentTemplate>
<Triggers><asp:AsyncPostBackTrigger ControlID="lnkSonido" EventName="Click" /></Triggers>
</asp:UpdatePanel>
</td>
<td style="width: 22px;">
<%--<img style="vertical-align: middle" alt="Escuchar" src="../images/play.gif" />--%>
<asp:ImageButton Style="position: relative; right: 4px; border-right: medium none; border-top: medium none; vertical-align: middle;
    border-left: medium none; border-bottom: medium none; background-color: transparent" ID="lnkSonido" 
    OnClick="lnkSonido_Click" runat="server" ImageUrl="../images/txt_play.png"  />
</td>
</tr>
</table>
</asp:Panel>
