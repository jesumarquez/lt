<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ZonaAlta" Codebehind="ZonaAlta.aspx.cs" %>

<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownLists.ReferenciasGeograficas" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top" width="50%">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="75%" />
                        
                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02" Width="100px" />
                        <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="75%" ParentControls="cbEmpresa" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        
                        <cwc:ResourceLabel id="ResourceLabel5" runat="server" ResourceName="Entities" VariableName="PARENTI93" />
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                            <ContentTemplate>
                                <cwc:TipoZonaDropDownList id="cbTipoZona" runat="server" Width="75%" ParentControls="cbLinea" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>    
                        
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                        <asp:TextBox ID="txtDescripcion" runat="server" Width="70%" />
                        
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="CODE" />
                        <asp:TextBox ID="txtCodigo" runat="server" Width="70%" />
                        
                        <cwc:ResourceLabel ID="lblPrioridad" runat="server" ResourceName="Labels" VariableName="PRIORIDAD" />
                        <asp:TextBox ID="txtPrioridad" runat="server" Width="70%" />
                        
                    </cwc:AbmTitledPanel>
                </td>
                <td valign="top" width="50%" rowspan="2">
                    <cwc:TitledPanel runat="server" TitleVariableName="PARENTI89" TitleResourceName="Entities" Height="130px">
                        <asp:UpdatePanel runat="server" ID="updMonitor" UpdateMode="Conditional">
                            <ContentTemplate>
                                <mon:Monitor ID="Monitor" runat="server" Width="600px" Height="510px" />                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </cwc:TitledPanel>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <cwc:TitledPanel runat="server" TitleVariableName="PAR_POI" TitleResourceName="Menu">
                        
                        <table width="100%" border="0">
                            <tr>
                                <td colspan="2" align="left">
                                    <cwc:ResourceLabel ID="lblTipoReferencias" runat="server" ResourceName="Entities" VariableName="PARENTI10" />
                                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                                        <ContentTemplate>
                                            <cwc:TipoReferenciaGeograficaDropDownList runat="server" ID="cbTipoReferencia" AddAllItem="True" ParentControls="cbEmpresa,cbLinea" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                            <asp:AsyncPostBackTrigger ControlID="cbLinea" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="left">
                                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                    <asp:TextBox runat="server" ID="txtGeoRef" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <cwc:ResourceButton runat="server" ID="btnBuscar" VariableName="BUTTON_SEARCH" ResourceName="Controls" OnClick="BtnBuscarOnClick" />
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="NO_ASIGNADAS" />
                                </td>
                                <td align="center">
                                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="ASIGNADAS" />
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
                                    <cwc:ResourceButton runat="server" ID="btnAgregar" VariableName="BUTTON_ADD" ResourceName="Controls" OnClick="BtnAgregarOnClick" />
                                </td>
                                <td align="center">
                                    <cwc:ResourceButton runat="server" ID="btnEliminar" VariableName="BUTTON_DELETE" ResourceName="Controls" OnClick="BtnEliminarOnClick" />
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
    </cwc:AbmTabPanel>
    
</asp:Content>
