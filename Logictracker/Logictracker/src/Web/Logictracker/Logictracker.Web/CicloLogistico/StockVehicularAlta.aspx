<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.StockVehicularAlta" Codebehind="StockVehicularAlta.aspx.cs" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="300px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="80%" AutoPostBack="True" OnInitialBinding="cbEmpresa_PreBind" OnSelectedIndexChanged="FiltrosOnSelectedIndexChanged" />
                                  
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI89" />
                    <asp:UpdatePanel ID="upZona" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:ZonaDropDownList ID="cbZona" runat="server" Width="80%" ParentControls="cbEmpresa" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" Width="80%" ParentControls="cbEmpresa" OnSelectedIndexChanged="FiltrosOnSelectedIndexChanged"  />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>

                </cwc:AbmTitledPanel> 
            </td>
            <td style="width: 50%; vertical-align: top;">       
                <cwc:TitledPanel ID="panelTopRight" runat="server" TitleVariableName="PAR_VEHICULOS" TitleResourceName="Menu" Height="300px">
                         
                    <table width="100%">
                        <tr>
                            <td align="center" width="50%">
                                <cwc:ResourceLabel ID="lblNoAsignados" runat="server" VariableName="NO_ASIGNADOS" ResourceName="Labels" Font-Bold="true" />
                            </td>
                            <td align="center" width="50%">
                                <cwc:ResourceLabel ID="lblAsignados" runat="server" VariableName="ASIGNADOS" ResourceName="Labels" Font-Bold="true" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" width="50%">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:ListBox runat="server" ID="lstNoAsignados" Width="90%" Height="200px" SelectionMode="Multiple" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                                        <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="center" width="50%">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:ListBox runat="server" ID="lstAsignados" Width="90%" Height="200px" SelectionMode="Multiple" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa"/>
                                        <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <cwc:ResourceButton runat="server" ID="btnAgregar" VariableName="BUTTON_ADD" ResourceName="Controls" OnClick="BtnAgregarOnClick" />
                            </td>
                            <td align="center">
                                <cwc:ResourceButton runat="server" ID="btnEliminar" VariableName="BUTTON_DELETE" ResourceName="Controls" OnClick="BtnEliminarOnClick" />
                            </td>
                        </tr>
                    </table>

                </cwc:TitledPanel>
            </td>
        </tr>
    </table>    
</asp:Content>
