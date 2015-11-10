<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.Distribucion.TipoCicloLogisticoAlta" Codebehind="TipoCicloLogisticoAlta.aspx.cs" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">   
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="300px">
                    
                    <cwc:ResourceLabel ID="lblParenti01" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="80%" AutoPostBack="True" OnInitialBinding="cbEmpresa_PreBind" OnSelectedIndexChanged="cbEmpresaOnSelectedIndexChanged" />
                                  
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="80%" MaxLength="255" />
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="80%" MaxLength="255" />

                </cwc:AbmTitledPanel> 
            </td>
            <td style="width: 50%; vertical-align: top;">       
                <cwc:TitledPanel ID="panelTopRight" runat="server" TitleVariableName="PAR_ESTADO_LOGISTICO" TitleResourceName="Menu" Height="300px">
                         
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
