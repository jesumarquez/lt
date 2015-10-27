<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="altaEntidad.ascx.cs" Inherits="Logictracker.App_Controls.AppControlsAltaEntidad" %>

<%@ Register Src="DireccionSearch.ascx" TagName="DireccionSearch" TagPrefix="uc1" %>

<table width="100%" cellpadding="5">
    <tr>
        <td>
            <cwc:ResourceLabel ID="ResourceLabel5" runat="server" CssClass="InputLabel" ResourceName="Labels" VariableName="APELLIDO"></cwc:ResourceLabel>
        </td>
        <td>
            <asp:TextBox ID="txtApellido" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td>
            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" CssClass="InputLabel" ResourceName="Labels" VariableName="NOMBRE"></cwc:ResourceLabel>
        </td>
        <td>
            <asp:TextBox ID="txtNombre" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td>
            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" CssClass="InputLabel" ResourceName="Labels" VariableName="DOCUMENT_NUMBER"></cwc:ResourceLabel>
        </td>
        <td>
            <asp:TextBox ID="txtDocumento" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td>
            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" CssClass="InputLabel" ResourceName="Labels" VariableName="DOCUMENT_TYPE"></cwc:ResourceLabel>
        </td>
        <td>
            <asp:TextBox ID="txtTipo" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td>
            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" CssClass="InputLabel" ResourceName="Labels" VariableName="CUIL"></cwc:ResourceLabel>
        </td>
        <td>
            <asp:TextBox ID="txtCuil" runat="server" Width="100%" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <uc1:DireccionSearch ID="DireccionSearch1" runat="server" EnableViewState="true" Height="160px"/>
        </td>
    </tr>
</table>
