<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FiltroDetalles.ascx.cs" Inherits="Logictracker.App_Controls.FiltroDetalles" %>

<asp:UpdatePanel ID="updGrid" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Panel ID="pnlDetalles" runat="server">
            <table>
                <tr>
                    <td align="center">
                        <C1:C1GridView ID="grdFiltros" runat="server" AutoGenerateColumns="false" OnRowDataBound="GrdFiltrosItemDataBound">
                            <RowStyle VerticalAlign="Top" />
                            <Columns>
                                <C1:C1TemplateField HeaderText="U">
                                    <ItemTemplate>
                                        <cwc:UnionDropDownList ID="cbUnion" runat="server" />
                                    </ItemTemplate>
                                </C1:C1TemplateField>
                                
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DETALLE">
                                    <ItemTemplate>
                                        <cwc:DetalleDropDownList ID="cbDetalle" runat="server" OnSelectedIndexChanged="CbDetalleOnSelectedIndexChanged" BindPadres="false" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="OPERADOR">
                                    <ItemTemplate>
                                        <cwc:OperadorDropDownList ID="cbOperador" runat="server" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                
                                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtValor" runat="server" Visible="false" />
                                        <asp:DropDownList ID="cbValor" runat="server" Visible="false" />
                                        <asp:ListBox ID="lstValor" runat="server" SelectionMode="Multiple" Visible="false" />
                                    </ItemTemplate>
                                </c1h:C1ResourceTemplateColumn>
                                
                                <C1:C1TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnEliminar" runat="server" OnClick="BtnEliminarOnClick" ImageUrl="~/images/delete.png" />
                                    </ItemTemplate>
                                </C1:C1TemplateField>
                            </Columns>
                        </C1:C1GridView>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <cwc:ResourceButton ID="btnAgregar" runat="server" ResourceName="Labels" VariableName="AGREGAR_FILTRO" OnClick="BtnAgregarOnClick" />
                    </td>
                </tr>
            </table>
        </asp:Panel>        
    </ContentTemplate>
</asp:UpdatePanel>
