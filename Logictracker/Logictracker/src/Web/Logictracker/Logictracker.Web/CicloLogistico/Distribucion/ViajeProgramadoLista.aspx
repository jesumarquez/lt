<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Distribucion.ViajeProgramadoLista" Title="" Codebehind="ViajeProgramadoLista.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" Runat="Server">
    <table>
        <tr>
            <td>
                <%--EMPRESA--%>
                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True"/>
                <br />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" AddAllItem="False" OnSelectedIndexChanged="FilterChanged" />                
            </td>
            <td>
                <%--TRANSPORTISTA--%>
                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="True" ForeColor="Black" />
                <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="100%" OnSelectedIndexChanged="FilterChanged" ParentControls="cbEmpresa" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />                        
                    </Triggers>   
                </asp:UpdatePanel> 
            </td>
            <td>
                <%--TIPO VEHICULO--%>
                <cwc:ResourceLabel ID="lblParenti17" runat="server" ResourceName="Entities" VariableName="PARENTI17" Font-Bold="True" ForeColor="Black" />
                <br />
                <asp:UpdatePanel ID="updTipoMovil" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:TipoVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="100%" ParentControls="cbEmpresa" OnSelectedIndexChanged="FilterChanged" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>    
    
</asp:Content>

