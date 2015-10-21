<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="True" CodeBehind="Error.aspx.cs" Inherits="Logictracker.Error" Title="Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div style="text-align: center">
        <asp:Image id="imgError" runat="server" Visible="true" />
        <div>
            <cwc:ResourceLabel ID="lblError" runat="server"  ResourceName="Errors" Font-Names="lucida console" VariableName="ERROR_PAGE" Font-Size="Small" Font-Bold="true" />
            <br />
            <br />
            <br />
            <br />
            <br />
            <asp:Label ID="lblMailOK" runat="server" Font-Size="Small" />
            <br />
            <asp:Label ID="lblDisplayError" runat="server" Font-Size="Small" />  
        </div>
    </div>
</asp:Content>

