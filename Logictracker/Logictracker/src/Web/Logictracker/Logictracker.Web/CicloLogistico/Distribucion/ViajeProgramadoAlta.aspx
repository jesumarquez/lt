<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Distribucion.ViajeProgramadoAlta" Codebehind="ViajeProgramadoAlta.aspx.cs" %>  

<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="175px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="80%" AutoPostBack="True" />

                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                    <asp:UpdatePanel ID="upDepto" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" Width="80%" ParentControls="cbEmpresa" AddAllItem="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                 
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE_DISTRIBUCION" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="80%" MaxLength="32" />

                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="KM" />
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Label ID="txtKm" runat="server" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                            <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                        </Triggers>
                    </asp:UpdatePanel>
                    
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="HORAS" />
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <asp:Label ID="txtHoras" runat="server" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                            <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                        </Triggers>
                    </asp:UpdatePanel>

                </cwc:AbmTitledPanel> 
            </td>
            <td style="width: 50%; vertical-align: top;" rowspan="2">
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="RECORRIDO" TitleResourceName="Labels" Height="320px">
                    
                    <asp:UpdatePanel runat="server" ID="updMonitor" UpdateMode="Conditional">
                        <ContentTemplate>
                            <mon:Monitor ID="monitor" runat="server" Width="600px" Height="510px" />                
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </cwc:AbmTitledPanel>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <cwc:TitledPanel runat="server" TitleVariableName="PTOS_ENTREGA" TitleResourceName="Menu">
                        
                    <table width="100%" border="0">
                        <tr>
                            <td colspan="2" align="left">
                                <cwc:ResourceLabel ID="lblTipoReferencias" runat="server" ResourceName="Entities" VariableName="PARENTI18" />
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <cwc:ClienteDropDownList runat="server" ID="cbCliente" AddAllItem="True" ParentControls="cbEmpresa" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" align="left">
                                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                <asp:TextBox runat="server" ID="txtPunto" />
                                <cwc:ResourceButton runat="server" ID="btnBuscar" CssClass="LogicButton_Big" VariableName="BUTTON_SEARCH" ResourceName="Controls" OnClick="BtnBuscarOnClick" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="NO_ASIGNADAS" />
                            </td>
                            <td align="center">
                                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="ASIGNADAS" />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" width="50%">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:ListBox runat="server" ID="lstNoAsignadas" Width="90%" Height="200px" SelectionMode="Multiple" OnSelectedIndexChanged="LstNoAsignadasOnSelectedIndexChanged" AutoPostBack="True" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnBuscar"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td align="center" width="50%">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                    <ContentTemplate>
                                        <asp:ListBox runat="server" ID="lstAsignadas" Width="90%" Height="200px" SelectionMode="Multiple" OnSelectedIndexChanged="LstAsignadasOnSelectedIndexChanged" AutoPostBack="True" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAgregar"/>
                                        <asp:AsyncPostBackTrigger ControlID="btnEliminar"/>
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <cwc:ResourceButton runat="server" ID="btnAgregar" CssClass="LogicButton_Big" VariableName="BUTTON_ADD" ResourceName="Controls" OnClick="BtnAgregarOnClick" />
                            </td>
                            <td align="center">
                                <cwc:ResourceButton runat="server" ID="btnEliminar" CssClass="LogicButton_Big" VariableName="BUTTON_DELETE" ResourceName="Controls" OnClick="BtnEliminarOnClick" />
                            </td>
                        </tr>
                    </table>
                    <asp:UpdatePanel runat="server" ID="none" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                                
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lstAsignadas" />
                            <asp:AsyncPostBackTrigger ControlID="lstNoAsignadas" />
                        </Triggers>
                    </asp:UpdatePanel>
                        
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>    
</asp:Content>
