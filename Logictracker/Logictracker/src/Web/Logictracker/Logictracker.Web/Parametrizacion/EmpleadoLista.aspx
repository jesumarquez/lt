<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.EmpleadoLista" Title="Choferes" Codebehind="EmpleadoLista.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" AddAllItem=true />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbEmpresa" AddAllItem="true" />
            </td>
            <td>    
                <cwc:ResourceLabel ID="lblparenti43" runat="server" ResourceName="Entities" VariableName="PARENTI43" />
                <br />
                <cwc:TipoEmpleadoDropDownList ID="cbTipoEmpleado" runat="server" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" AddNoneItem="true" Width="175px" ParentControls="cbEmpresa,cbLinea" AutoPostBack="true"/>
            </td>
            <td>    
                <cwc:ResourceLabel ID="lblCategoriaAcceso" runat="server" ResourceName="Entities" VariableName="PARENTI15" />
                <br />
                <cwc:CategoriaAccesoDropDownList ID="cbCategoriaAcceso" runat="server" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" AddNoneItem="true" Width="175px" ParentControls="cbEmpresa,cbLinea" AutoPostBack="true"/>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPARENTI07" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" AddNoneItem="true" OnSelectedIndexChanged="FilterChangedHandler" AddAllItem="true" Width="175px" ParentControls="cbEmpresa,cbLinea" AutoPostBack="true" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>
