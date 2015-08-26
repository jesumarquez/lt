<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="SinAcceso.aspx.cs" Inherits="Logictracker.SinAcceso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div style="text-align: center; width: 400px; margin: auto; margin-top: 100px;">
        <div>
        <asp:Image runat="server" ID="imgSinAcceso" ImageUrl="~/images/sinacceso.png"/>
        </div>
        <div style="font-size: 14px;">
        <cwc:ResourceLabel ID="lblMensaje" runat="server" ResourceName="Errors" VariableName="NO_MODULE_ACCESS"></cwc:ResourceLabel>
        </div>
        <div style="font-size: 14px; font-weight: bold;">
        <asp:Label ID="lblModulo" runat="server"></asp:Label>
        </div>
        <div style="font-size: 12px; margin-top: 20px;">
        <cwc:ResourceLabel ID="lblDetalle" runat="server" ResourceName="Errors" VariableName="NO_MODULE_ACCESS_DETAIL"></cwc:ResourceLabel>
        </div>
    </div>
</asp:Content>

