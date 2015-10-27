<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Detalles.ascx.cs" Inherits="Logictracker.App_Controls.Detalles" %>

<asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <cwc:AbmTitledPanel ID="tpDetalles" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">

        </cwc:AbmTitledPanel>
    </ContentTemplate>
</asp:UpdatePanel>
