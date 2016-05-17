﻿<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="SubentidadLista.aspx.cs" Inherits="Logictracker.Parametrizacion.SubentidadLista" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
        <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--TIPO ENTIDAD--%>
                <cwc:ResourceLabel ID="lblTipoEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI76"/>
                <br />
                <cwc:TipoEntidadDropDownList ID="cbTipoEntidad" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--ENTIDAD--%>
                <cwc:ResourceLabel ID="lblEntidad" runat="server" ResourceName="Entities" VariableName="PARENTI79"/>
                <br />
                <cwc:EntidadDropDownList ID="cbEntidad" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa,cbLinea,cbTipoEntidad" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
        </tr>
    </table>
</asp:Content>
