<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="PuertaAccesoAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.PuertaAccesoAlta" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="lblDatosAsignacion" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="220px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="100%" />
                    
                    <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="100%" MaxLength="200" />
                    
                    <cwc:ResourceLabel ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" />
                    <asp:UpdatePanel ID="updVehiculo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:MovilDropDownList ID="cbVehiculo" runat="server"  Width="100%" ParentControls="cbEmpresa,cbLinea" ShowOnlyAccessControl="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblPuerta" runat="server" ResourceName="Labels" VariableName="PUERTA" />
                    <c1:C1NumericInput ID="npPuerta" runat="server" Height="17px" DecimalPlaces="0" MinValue="1" MaxValue="9999" Width="200px"/>
                    
                    <cwc:ResourceLabel ID="lblZonaAccesoEntrada" runat="server" ResourceName="Labels" VariableName="ENTRA_A" />
                    <asp:UpdatePanel runat="server" ID="upZonaAccesoEntrada" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ZonaAccesoDropDownList runat="server" ID="cbZonaAccesoEntrada" Width="100%" ParentControls="cbEmpresa,cbLinea" AddNoneItem="True" />        
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="lblZonaAccesoSalida" runat="server" ResourceName="Labels" VariableName="SALE_A" />
                    <asp:UpdatePanel runat="server" ID="upZonaAccesoSalida" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ZonaAccesoDropDownList runat="server" ID="cbZonaAccesoSalida" Width="100%" ParentControls="cbEmpresa,cbLinea" AddNoneItem="True" />        
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    
                </cwc:AbmTitledPanel>
            </td>
            <td></td>
        </tr>
    </table>
</asp:Content>
