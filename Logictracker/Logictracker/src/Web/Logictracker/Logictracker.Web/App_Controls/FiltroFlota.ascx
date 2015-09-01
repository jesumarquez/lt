<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="FiltroFlota.ascx.cs" Inherits="Logictracker.App_Controls.App_Controls_FiltroFlota" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownCheckLists" Assembly="Logictracker.Web.CustomWebControls" %>
<asp:UpdatePanel ID="upFiltroFlota" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table>
        <tr>
        <td style="width: 90px">
            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
        </td>
        <td>        
            <cwc:LocacionDropDownCheckList ID="ddclEmpresa" runat="server" width="200px" AutoPostBack="true">
                <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <PanelStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <SelectAllStyle BackColor="LightGray" />               
            </cwc:LocacionDropDownCheckList>
        </td>
        <td style="width: 90px">
            <cwc:ResourceLabel ID="ResourceLabel3" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
        </td>
        <td>      
        <asp:UpdatePanel ID="upTipoDeVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>  
                <cwc:TipoDeVehiculoDropDownCheckList ID="ddclTipoDeVehiculo" runat="server" width="200px" AutoPostBack="true">
                    <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                    <PanelStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                    <SelectAllStyle BackColor="LightGray" />               
                </cwc:TipoDeVehiculoDropDownCheckList>
                </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddclLinea" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel>
        </td>
        <td style="width: 90px">
            <cwc:ResourceLabel ID="ResourceLabel4" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI32" />
        </td>
        <td>      
        <asp:UpdatePanel ID="upTipoDeDispositivo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>  
                <cwc:TipoDispositivoDropDownCheckList ID="ddclTipoDeDispositivo" runat="server" width="200px" AutoPostBack="true">
                    <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                    <PanelStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                    <SelectAllStyle BackColor="LightGray" />               
                </cwc:TipoDispositivoDropDownCheckList>
                </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddclVehiculo" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel>
        </td>
        </tr>
        
        <tr>
        <td style="width: 90px">
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
        </td>
        <td>        
        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
            <cwc:PlantaDropDownCheckList ID="ddclLinea" runat="server" width="200px"
                        ParentControls="ddclEmpresa">
                <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <PanelStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <SelectAllStyle BackColor="LightGray" />               
            </cwc:PlantaDropDownCheckList>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddclEmpresa" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel>
        </td>
        
        
        <td style="width: 90px">
                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
        </td>
        <td>        
        <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
            <cwc:MovilDropDownCheckList ID="ddclVehiculo" runat="server" width="200px" ParentControls="ddclLinea,ddclTipoDeVehiculo">
                <Style BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <PanelStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" />
                <SelectAllStyle BackColor="LightGray" />               
            </cwc:MovilDropDownCheckList>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddclTipoDeVehiculo" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel>
        </td>
        
        
        
        <td style="width: 90px">
                        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI08" />
        </td>
        <td>        
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
            <ContentTemplate>
            <cwc:DispositivoDropDownCheckList ID="ddclDispositivo" runat="server" width="200px" ParentControls="ddclLinea,ddclTipoDeDispositivo">
            </cwc:DispositivoDropDownCheckList>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddclTipoDeDispositivo" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel>
        </td>
        </tr>
        </table>        
    </ContentTemplate>
</asp:UpdatePanel>