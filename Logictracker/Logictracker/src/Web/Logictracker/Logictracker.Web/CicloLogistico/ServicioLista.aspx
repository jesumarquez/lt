<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.ListaServicio" Title="Untitled Page" Codebehind="ServicioLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
<table><tr><td>
        <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="125px" AddAllItem="true" />
        </td>
        <td>
        <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="125px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FilterChangedHandler" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        </td>
        </tr>
        </table>
</asp:Content>

