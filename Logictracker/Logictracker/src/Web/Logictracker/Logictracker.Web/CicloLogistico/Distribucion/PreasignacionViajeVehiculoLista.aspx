<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Distribucion.PreasignacionViajeVehiculoLista" Title="" Codebehind="PreasignacionViajeVehiculoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="False" OnSelectedIndexChanged="FilterChangedHandler" />
                <br/>
                <br/>
                <%--LINEA--%>
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="True"/>
                <br />
                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" AddAllItem="true" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChangedHandler" />
            </td>
            <td>
                <%--TRANSPORTISTA--%>
                <cwc:ResourceLinkButton ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbTransportista" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TransportistasListBox ID="cbTransportista" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea" SelectionMode="Multiple" OnSelectedIndexChanged="FilterChangedHandler" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--VEHICULO--%>
                <cwc:ResourceLinkButton ID="lblParenti03" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="True" ForeColor="Black" ListControlTargetID="cbMovil" />
                <br />
                <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="cbMovil" runat="server" Width="100%" ParentControls="cbEmpresa,cbLinea,cbTransportista"  SelectionMode="Multiple" OnSelectedIndexChanged="FilterChangedHandler" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

