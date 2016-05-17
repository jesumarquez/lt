<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ReporteTolva.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.Hormigon.ReporteTolva" Title="Untitled Page" %>

<asp:Content ContentPlaceHolderID="Filtros" runat="server" ID="cph">
        <table>
            <tr>
                <td style="width: 300px; vertical-align: top; padding-right: 20px;">
                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" CssClass="LogicCombo" />
                    <br />
                </td>
                <td style="width: 300px; vertical-align: top; padding-right: 20px;">
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="100%" ParentControls="cbLinea" AddAllItem="True" CssClass="LogicCombo" />
                </td>
                <td rowspan="4" style="width: 300px; vertical-align: bottom;">
                    <cwc:ResourceCheckBox runat="server" ID="chkHuerfanos" ResourceName="Labels" VariableName="OCULTAR_HUERFANOS"/>
                </td>
            </tr>
            <tr>
                <td style="width: 300px; vertical-align: top; padding-right: 20px;">    
                    <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="True" CssClass="LogicCombo" />
                </td>
                <td rowspan="3" style="width: 300px; vertical-align: top; padding-right: 20px;">
                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                    <cwc:SelectAllExtender ID="SelectAllExtender1" runat="server" ListControlId="cbVehiculo" TargetControlId="lblVehiculo"/>
                    <br />
                    <cwc:MovilListBox ID="cbVehiculo" runat="server" Width="100%" Height="110px" SelectionMode="Multiple"
                                ParentControls="cbTipoVehiculo" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" />
                </td>
            </tr>
            <tr>
                <td style="width: 300px; vertical-align: top; padding-right: 20px;"> 
                    <table style="width: 100%;"><tr><td>
                        <cwc:ResourceLabel ID="lblInitDate" runat="server" ResourceName="Labels" VariableName="DESDE" />
                        <br />
                        <cwc:DateTimePicker ID="dpInitDate" runat="server" TimeMode="Start" IsValidEmpty="false" Mode="Date" />
                    </td><td style="text-align: right;">
                        <cwc:ResourceLabel ID="lblEndDate" runat="server" ResourceName="Labels" VariableName="HASTA" />
                        <br />
                        <cwc:DateTimePicker ID="dpEndDate" runat="server" TimeMode="End" IsValidEmpty="false" Mode="Date" />
                    </td></tr></table>
                </td>
            </tr>
            <tr>
                <td style="width: 300px; vertical-align: top; padding-right: 20px;">        
                    <cwc:ResourceLabel ID="lblDistancia" runat="server" ResourceName="Labels" VariableName="DISTANCIA" />
                    <br />
                    <asp:TextBox runat="server" ID="txtDistancia" Width="80px" CssClass="LogicTextbox" Value="200"></asp:TextBox>
                </td>
            </tr>
        </table>
</asp:Content>
