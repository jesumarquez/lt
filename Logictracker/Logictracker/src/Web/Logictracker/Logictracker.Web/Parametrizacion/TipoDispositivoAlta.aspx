<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.ParametrizacionTipoDispositivoAlta" Codebehind="TipoDispositivoAlta.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.DropDownCheckLists" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" Runat="Server">

<cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES" >
    <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_TIPO_DISPOSITIVO" TitleResourceName="Labels" Height="100px">
        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="MODELO" />
        <asp:TextBox ID="txtModelo" runat="server" Width="750px" />
        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="FABRICANTE" />
        <asp:TextBox ID="txtFabricante" runat="server" Width="750px" />
        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="COLA_COMANDOS" />
        <asp:TextBox ID="txtColaComando" runat="server" Width="750px" />
        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="FIRMWARE" />
        <asp:UpdatePanel ID="upFirmware" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cwc:FirmwareDropDownList ID="ddlFirmware" runat="server" Width="750px" OnInitialBinding="DdlFirmwareInitialBinding" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Labels" VariableName="CONFIGURACION" />
        <asp:UpdatePanel ID="upConfiguracion" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cwc:ConfiguracionDispositivoDropDownCheckList ID="ddlConfiguracion" runat="server" Width="750px" AutoPostBack="false" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <cwc:ResourceLabel ID="ResourceLabel5" runat="server" ResourceName="Entities" VariableName="PARENTI32" />
        <asp:UpdatePanel ID="upPadre" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <cwc:TipoDispositivoDropDownList ID="ddlPadre" runat="server" Width="750px" OnInitialBinding="DdlPadreInitialBinding" AddSinAsignar="true" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTitledPanel>
    
</cwc:AbmTabPanel>
<cwc:AbmTabPanel ID="abmTabDetalles" runat="server" Title="Detalles" >
    <style type="text/css">
    #parametros input[type=text]
    {
        border: solid 1px #999999;
        padding: 3px;
        margin: 1px;
    }
    </style>
    <div id="parametros">
    <asp:UpdatePanel ID="updDetalles" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <C1:C1GridView ID="grid" runat="server" SkinID="ListGridNoGroupNoPage" DataKeyNames="Id" OnRowDataBound="grid_RowDataBound" OnRowCommand="grid_RowCommand">
                <Columns>
                
                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="NAME" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:TextBox ID="txtNombre" runat="server" Width="95%"></asp:TextBox></ItemTemplate>
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="DESCRIPCION" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:TextBox ID="txtDescripcion" runat="server" Width="95%"></asp:TextBox></ItemTemplate>
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="TIPO_DATO" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:TextBox ID="txtTipoDato" runat="server" Width="95%"></asp:TextBox></ItemTemplate>
                        <ItemStyle Width="60px" />
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="CONSUMIDOR" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:TextBox ID="txtConsumidor" runat="server" Width="95%"></asp:TextBox></ItemTemplate>
                        <ItemStyle Width="60px" />
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR_INICIAL" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:TextBox ID="txtValorInicial" runat="server" Width="95%"></asp:TextBox></ItemTemplate>
                        <ItemStyle Width="100px" />
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="EDITABLE" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:CheckBox ID="chkEditable" runat="server"></asp:CheckBox></ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                    </c1h:C1ResourceTemplateColumn>

                    <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="REQUIRES_RESET" SortExpression="Valor" AllowGroup="false" AllowMove="false" AllowSizing="false">
                        <ItemTemplate><asp:CheckBox ID="chkReboot" runat="server"></asp:CheckBox></ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Center" />
                    </c1h:C1ResourceTemplateColumn>
                    
                    <c1:C1TemplateField>
                        <ItemTemplate>
                            <cwc:ResourceLinkButton ID="lnkBorrar" runat="server" CommandName="Delete" ResourceName="Controls" VariableName="BUTTON_DELETE" />
                        </ItemTemplate>
                        <ItemStyle Width="40px" HorizontalAlign="Right" />
                    </c1:C1TemplateField>

                </Columns>
            </C1:C1GridView> 
            <div class="Grid_Header">
                <cwc:ResourceLinkButton ID="btNewParam" runat="server" OnClick="btNewParam_Click" ResourceName="Controls" VariableName="BUTTON_ADD_PARAMETER" />
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="grid" />
        </Triggers>
    </asp:UpdatePanel>
    </div>
    </cwc:AbmTabPanel>
</asp:Content>

