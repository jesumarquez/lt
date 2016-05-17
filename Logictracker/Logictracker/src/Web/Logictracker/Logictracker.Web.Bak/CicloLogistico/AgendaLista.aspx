<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" CodeFile="AgendaLista.aspx.cs" Inherits="Logictracker.Web.CicloLogistico.AgendaLista" Title="" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="False" OnSelectedIndexChanged="FilterChanged" />
                <br/>
                <br/>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="True"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChanged" />
            </td>
            <td>
                <%--VEHICULO--%>
                <cwc:ResourceLinkButton ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbMovil" />
                <br />
                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="cbMovil" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" OnSelectedIndexChanged="FilterChanged" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
                <br/>
                <br/>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="End" Mode="DateTime" IsValidEmpty="false" AutoPostBack="true" OnDateChanged="FilterChanged" />
            </td>
        </tr>
    </table>
    
</asp:Content>

