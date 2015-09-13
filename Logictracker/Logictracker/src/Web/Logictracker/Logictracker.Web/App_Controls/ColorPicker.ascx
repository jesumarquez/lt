<%@ Control Language="C#" AutoEventWireup="true" Inherits="Logictracker.App_Controls.App_Controls_ColorPicker" CodeBehind="ColorPicker.ascx.cs" %>
<%@ Register Assembly="CDT.ColorPickerExtender" Namespace="CDT" TagPrefix="cdt" %>
<script type="text/javascript">
var <%= JsSelectedColorVariable %> = '';

function <%=JsChangeFunction %>()
{
   <%=JsSelectedColorVariable %> =  $get('<%=tbColor.ClientID %>').value;
    <%=Page.ClientScript.GetPostBackEventReference(tbColor, String.Empty) %>;
}
</script>
<asp:TextBox ID="tbColor" runat="server" Columns="7" MaxLength="7" Width="60px" OnTextChanged="tbColor_TextChanged" />
<asp:ImageButton ID="imgColor" runat="server" ImageUrl="~/images/icon_colorpicker.png" Style="position: relative; right: 6px; top: 6px;" />
<cdt:ColorPickerExtender ID="cpe" runat="server" TargetControlID="tbColor" SampleControlID="imgColor" PopupButtonID="imgColor" OnClientShown="function (bh){ bh._popupDiv.style.zIndex = 999999999;}"  />
