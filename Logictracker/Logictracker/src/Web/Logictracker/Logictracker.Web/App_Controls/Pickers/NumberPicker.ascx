<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="NumberPicker.ascx.cs" Inherits="Logictracker.App_Controls.Pickers.App_Controls_NumberPicker" %>

<asp:TextBox ID="txtInterval" runat="server"  OnTextChanged="TextChanged"/>
<AjaxToolkit:MaskedEditExtender ID="meeInterval" runat="server" MaskType="Number" TargetControlID="txtInterval" />
<AjaxToolkit:MaskedEditValidator ID="mevInterval" runat="server" ControlToValidate="txtInterval" IsValidEmpty="false"
    ControlExtender="meeInterval" EmptyValueMessage="Valor requerido" InvalidValueMessage="Valor invalido" />