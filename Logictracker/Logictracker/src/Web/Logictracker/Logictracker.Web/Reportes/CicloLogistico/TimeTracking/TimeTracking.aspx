<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="TimeTracking.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.TimeTracking.TimeTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <style type="text/css">
        #tablafiltros td 
        {
            padding-right: 20px;
            vertical-align: top;
        }        
    </style>
     <table id="tablafiltros">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" AutoPostBack="true" ParentControls="cbEmpresa" />
                    
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                    <br />
                    <asp:UpdatePanel runat="server" ID="updVehiculos" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilDropDownCheckList ID="cbVehiculo" runat="server" ParentControls="cbEmpresa,cbLinea"  Width="200px" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Entities" VariableName="PARENTI09" />
                        <asp:UpdatePanel runat="server" ID="UpdatePanel7" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownCheckList ID="cbEmpleado" runat="server" ParentControls="cbEmpresa,cbLinea,cbTipoEmpleado,cbDepartamento,cbCentroDeCostos" Width="200px"/>
                            </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpleado" EventName="SelectedIndexChanged" />
                                </Triggers>
                        </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dtDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dtHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" />
                </td>       
            </tr>
        </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" Runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" Runat="Server">
</asp:Content>

