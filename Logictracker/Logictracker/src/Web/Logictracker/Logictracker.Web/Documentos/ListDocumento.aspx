<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true"
    CodeFile="ListDocumento.aspx.cs" Inherits="Logictracker.Documentos.Documentos_ListDocumento" Title="Untitled Page" %>

<%@ Register Src="../App_Controls/Pickers/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc1" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists" Assembly="Logictracker.Web.CustomWebControls" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" /><br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true"
                    OnSelectedIndexChanged="FilterChangedHandler" AutoPostBack="true" />
            <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" /><br />
                <cwc:PlantaDropDownList ID="ddlPlanta" Width="200px" runat="server" ParentControls="ddlDistrito"
                    AutoPostBack="true" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" /><br />
                <cwc:TransportistaDropDownList ID="cbTransportistas" runat="server" Width="200px"
                    OnSelectedIndexChanged="FilterChangedHandler" />
                <br />
                <cwc:ResourceLabel ID="lblMobil" runat="server" VariableName="PARENTI03" ResourceName="Entities"></cwc:ResourceLabel>
                <br />
                <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="200px" AddAllItem="true"
                    ParentControls="ddlPlanta,cbTransportistas">
                </cwc:MovilDropDownList>
            </td>
            <td valign="top">
                <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI19" /><br />
                <cwc:EquipoDropDownList ID="ddlEquipo" Width="200px" runat="server" AddAllItem="true"
                    ParentControls="ddlPlanta" />
            </td>
            <td>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="DESDE" /><br />
                <cwc:DateTimePicker ID="dtInicio" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="false"/>
            <br />
                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="HASTA" /><br />
                <cwc:DateTimePicker ID="dtFin" runat="server" Mode="Date" TimeMode="End" IsValidEmpty="false"/>
            </td>
            <td valign="bottom">
                <cwc:ResourceButton ID="btnBuscar" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH"
                    OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" runat="Server">

<cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="ESTADO" /><br />
    <asp:DropDownList ID="cbEstado" runat="server">
        <asp:ListItem Text="Todos" Value="-1"></asp:ListItem>
        <asp:ListItem Text="Sin Controlar" Value="0"></asp:ListItem>
        <asp:ListItem Text="Controlado" Value="1"></asp:ListItem>
        <asp:ListItem Text="Verificado" Value="2"></asp:ListItem>
    </asp:DropDownList>
</asp:Content>
