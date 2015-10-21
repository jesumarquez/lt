<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="GeoRefLista.aspx.cs" Inherits="Logictracker.Parametrizacion.GeoRefLista" Title="Domicilios" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <cwc:ResourceLabel runat="server" ID="lblEmpresa" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList runat="server" ID="cbEmpresa" Width="200px" AutoPostBack="True" AddAllItem="false" />
            </td>
            <td>
                <cwc:ResourceLabel runat="server" ID="lblLinea" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <cwc:PlantaDropDownList runat="server" ID="cbLinea" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" AutoPostBack="True" />
            </td>
            <td>
                <cwc:ResourceLabel runat="server" ID="ResourceLabel1" ResourceName="Entities" VariableName="PARENTI10" />
                <br />
                <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="FilterChangedHandler" ParentControls="cbEmpresa, cbLinea" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFiltrosAvanzados" Runat="Server">
</asp:Content>

    <%--
    <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
        <ContentTemplate>
            <C1WebGrid:C1WebGrid ID="gridDomicilios" DataKeyField="Id" runat="server" OnItemDataBound="grid_ItemDataBound"
                OnSelectedIndexChanging="grid_SelectedIndexChanging" OnSortingCommand="grid_SortingCommand">
                <Columns>
                    <C1WebGrid:C1TemplateColumn>
                        <ItemStyle Width="1px" />
                        <ItemTemplate><asp:CheckBox ID="chkSelected" runat="server" /></ItemTemplate>
                        <HeaderTemplate><asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" OnCheckedChanged="chkSelectAll_CheckedChanged" /></HeaderTemplate>
                    </C1WebGrid:C1TemplateColumn>
                    <C1WebGrid:C1TemplateColumn>
                        <ItemStyle Width="1px" />
                    </C1WebGrid:C1TemplateColumn>
                    <c1h:C1ResourceBoundColumn DataField="Codigo" SortDirection="Descending" SortExpression="Codigo" ResourceName="Labels"
                        VariableName="CODE" >
                        <ItemStyle Width="200px" />
                        </c1h:C1ResourceBoundColumn>
                    <c1h:C1ResourceBoundColumn DataField="Descripcion" SortDirection="Descending" SortExpression="Descripcion" ResourceName="Labels"
                        VariableName="DESCRIPCION" >
                        <ItemStyle Width="1000px" />
                        </c1h:C1ResourceBoundColumn>
                    <C1WebGrid:C1TemplateColumn>
                        <ItemStyle Width="30px" />
                        <ItemTemplate><asp:Image ID="imgPoint" runat="server" ImageUrl="~/images/point.png" /></ItemTemplate>
                    </C1WebGrid:C1TemplateColumn>
                    <C1WebGrid:C1TemplateColumn>
                        <ItemStyle Width="30px" />
                        <ItemTemplate><asp:Image ID="imgPolygon" runat="server" ImageUrl="~/images/polygon.png" /></ItemTemplate>
                    </C1WebGrid:C1TemplateColumn>
                </Columns>
            </C1WebGrid:C1WebGrid>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="cbTipoReferenciaGeografica" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>--%>

