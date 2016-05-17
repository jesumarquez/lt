<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="PreasignacionViajeVehiculoAlta.aspx.cs" Inherits="Logictracker.CicloLogistico.Distribucion.PreasignacionViajeVehiculoAlta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="server">
    
    <style type="text/css">
        li{list-style: none; padding: 0px; margin: 0px;}
        ul{padding: 0px;margin: 0px;}
    </style>
    
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="120px">
                    
                    <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="false" Width="98%" />
                    
                    <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                    <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" ParentControls="cbEmpresa" AddAllItem="True" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                    <asp:UpdatePanel ID="upTransportista" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TransportistaDropDownList ID="cbTransportista" AddAllItem="true" runat="server" Width="98%" ParentControls="cbEmpresa,cbLinea" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                </cwc:AbmTitledPanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="pnlTopRight" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="120px">
                    
                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                    <asp:UpdatePanel ID="updMovil" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="cbVehiculo" runat="server" Width="50%" ParentControls="cbEmpresa,cbLinea,cbTransportista" TabIndex="20" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbTransportista" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE_DISTRIBUCION" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="50%" MaxLength="32" />
                    
                </cwc:AbmTitledPanel>
            </td>
        </tr>
    </table>

</asp:Content>
