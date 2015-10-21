<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="FuncionAlta.aspx.cs"
    Inherits="Logictracker.Organizacion.AltaFuncion" Title="Funciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table id="tbTipoPunto" style="width: 90%; margin: auto;" cellpadding="5">
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA IZQ--%>
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleVariableName="DATOS_GENERALES"
                    TitleResourceName="Labels" Height="180px">
                    <%--    <cwc:TitledPanel ID="panGeneral" runat="server" Title="Datos Generales" Height="180px">--%>
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="SISTEMA" />
                    <asp:UpdatePanel ID="upSistema" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:SubSistemaDropDownList ID="cbSubSistema" runat="server" Width="100%" OnInitialBinding="cbSubSistema_PreBind" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbSubSistema" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <cwc:MenuResourcesDropDownList ID="ddlMenuResource" runat="server" Width="100%" FunctionMode="true" />
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="URL" />
                    <asp:TextBox ID="txtURL" runat="server" Width="100%" MaxLength="255" />
                    <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="REFFERENCE" />
                    <asp:TextBox ID="txtReferencia" runat="server" MaxLength="32" Width="100%" />
                </cwc:AbmTitledPanel>
                <%-- </cwc:TitledPanel>--%>
            </td>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA DER--%>
                <cwc:AbmTitledPanel ID="AbmTitledPanel1" runat="server" TitleVariableName="CONFIGURACION"
                    TitleResourceName="Labels" Height="180px">
                    <%--      <cwc:TitledPanel ID="TitledPanel1" runat="server" Title="Configuración" Height="180px">--%>
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="GRUPO" />
                    <cwc:MenuResourcesDropDownList ID="cbGrupo" runat="server" Width="100%" FunctionMode="false"
                        AddEmptyItem="true" />
                    <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Labels" VariableName="PARAMETERS" />
                    <asp:TextBox ID="txtParametros" runat="server" MaxLength="32" Width="100%" />
                    <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="TYPE" />
                    <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:TiposFuncionDropDownList ID="cbTipo" runat="server" Width="100%" OnInitialBinding="cbTipo_PreBind" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cbTipo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </cwc:AbmTitledPanel>
                <%-- </cwc:TitledPanel>--%>
            </td>
        </tr>
    </table>
</asp:Content>
