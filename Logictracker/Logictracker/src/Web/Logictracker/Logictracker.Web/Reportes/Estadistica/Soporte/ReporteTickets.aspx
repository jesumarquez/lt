<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.Soporte.ReporteTickets" Title="Reporte de Tickets" Codebehind="ReporteTickets.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="Server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td>
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <cwc:ResourceLinkButton ID="lblLocacion" runat="server" ForeColor="Black" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" ListControlTargetID="cbEmpresa"/>  
                            <br />
                            <cwc:LocacionListBox ID="cbEmpresa" runat="server" Width="160px" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
                        </td>
                        <td>
                            <cwc:ResourceLinkButton ID="lblCategoria" runat="server" ForeColor="Black" Font-Bold="true" ResourceName="Entities" VariableName="AUDSUP03" ListControlTargetID="cbCategoria"/>  
                            <br />
                            <asp:UpdatePanel ID="upCategoria" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:CategoriaListBox ID="cbCategoria" runat="server" ParentControls="cbEmpresa" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <cwc:ResourceLinkButton ID="lblSubcategoria" runat="server" ForeColor="Black" Font-Bold="true" ResourceName="Entities" VariableName="AUDSUP04" ListControlTargetID="cbSubcategoria"/>  
                            <br />
                            <asp:UpdatePanel ID="upSubcategoria" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:SubcategoriaListBox ID="cbSubcategoria" runat="server" Width="160px" ParentControls="cbEmpresa,cbCategoria" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbCategoria" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <cwc:ResourceLinkButton ID="lblNivel" runat="server" ForeColor="Black" Font-Bold="true" ResourceName="Entities" VariableName="AUDSUP05" ListControlTargetID="cbNivel"/>  
                            <br />
                            <asp:UpdatePanel ID="upNivel" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:NivelListBox ID="cbNivel" runat="server" Width="160px" ParentControls="cbEmpresa" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td>
                            <cwc:ResourceLinkButton ID="lblEstados" runat="server" ForeColor="Black" Font-Bold="true" ResourceName="Labels" VariableName="STATE" ListControlTargetID="cbEstados"/>
                            <br />                                   
                            <cwc:EstadoTicketSoporteListBox ID="cbEstados" runat="server" SelectionMode="Multiple" AutoPostBack="true" OnSelectedIndexChanged="FilterChangedHandler" />
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td>
                            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                            <br />
                            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
                        </td>
                        <td>&nbsp;</td>
                        <td>
                            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                            <br />
                            <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChangedHandler" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>                
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
