<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="SelectGeoRefference.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_SelectGeoRefference" %>
<asp:Panel ID="panelControl" runat="server" CssClass="direccionsearch">
    <asp:UpdatePanel ID="updControl" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
        <ContentTemplate>
            <AjaxToolkit:TabContainer ID="tab" runat="server">
                <AjaxToolkit:TabPanel ID="tabReferencia" runat="server">
                    <HeaderTemplate>Referencia Geogr&aacute;fica</HeaderTemplate>
                    <ContentTemplate>
                        <asp:Label ID="lblReferencia" runat="server" Text="No se ha seleccionado una Referencia Geográfica"></asp:Label>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabSelect" runat="server">
                    <HeaderTemplate>Seleccionar</HeaderTemplate>
                    <ContentTemplate>
                    <asp:Panel ID="panelSelect" runat="server" DefaultButton="btSelect">
                        <cwc:TipoReferenciaGeograficaDropDownList ID="cbTipoReferenciaGeografica" runat="server" Width="100%" OnSelectedIndexChanged="CbTipoReferenciaGeograficaSelectedIndexChanged" />
                        <asp:ListBox ID="cbReferenciaGeografica" runat="server" Width="100%" DataValueField="Id" DataTextField="Descripcion"  />
                        <div style="text-align: right; padding-top: 5px;">
                            <asp:Button ID="btSelect" runat="server" Text="Seleccionar" OnClick="BtSelectClick"
                                CssClass="direccionsearch_button" />
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabSearch" runat="server">
                    <HeaderTemplate>B&uacute;squeda</HeaderTemplate>
                    <ContentTemplate>
                        <asp:Panel ID="panelSearch" runat="server" DefaultButton="btSearch">
                        <asp:TextBox ID="txtSearch" runat="server" Width="100%" CssClass="LogicTextbox"></asp:TextBox>
                        <AjaxToolkit:TextBoxWatermarkExtender ID="txwSearch" TargetControlID="txtSearch" WatermarkText="Descripción"
                            WatermarkCssClass="LogicWatermark" Enabled="True" runat="server" />
                        <div style="text-align: right; padding-top: 5px;">
                            <asp:Button ID="btSearch" runat="server" Text="Buscar" OnClick="BtSearchClick"
                                CssClass="direccionsearch_button" />
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
                <AjaxToolkit:TabPanel ID="tabResult" runat="server" Enabled="false">
                    <HeaderTemplate>Resultado</HeaderTemplate>
                    <ContentTemplate>
                    <asp:Panel ID="panelResult" runat="server" DefaultButton="btAceptar">
                        <div class="direccionsearch_body">
                            <asp:ListBox ID="cbResults" runat="server" Width="100%"></asp:ListBox>
                            <div style="text-align: right; padding-top: 5px;">
                                <asp:Button ID="btAceptar" runat="server" Text="Aceptar" OnClick="BtAceptarClick"
                                    CssClass="direccionsearch_button" />
                                <asp:Button ID="btCancelar" runat="server" Text="Cancelar" OnClick="BtCancelarClick"
                                    CssClass="direccionsearch_button" />
                            </div>
                        </div>
                        </asp:Panel>
                    </ContentTemplate>
                </AjaxToolkit:TabPanel>
            </AjaxToolkit:TabContainer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
