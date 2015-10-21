<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.LocacionAlta" Title="Untitled Page" Codebehind="LocacionAlta.aspx.cs" %>
<%@ Register TagPrefix="c1h" Namespace="Logictracker.Web.Helpers.C1Helpers" Assembly="Logictracker.Web.Helpers" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content2" ContentPlaceHolderID="AbmContent" runat="Server">

    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_GENERALES" Height="115px">
                    
                    <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                    <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="10" />
                    
                    <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                    
                    <cwc:ResourceLabel ID="lblUsoHorario" runat="server" ResourceName="Labels" VariableName="USO_HORARIO" />
                    <cwc:TimeZoneDropDownList ID="ddlTimeZone" runat="server" Width="200px" />
                    
                </cwc:AbmTitledPanel>
            </td>
            <td style="vertical-align: top; width: 50%;">
                <cwc:TitledPanel ID="tpComportamientos" runat="server" TitleResourceName="Labels"
                    TitleVariableName="COMPORTAMIENTOS"  Height="115px"><br />
                    <cwc:ResourceCheckBox ID="chkIdentificaChoferes" runat="server" ResourceName="Labels"
                        VariableName="IDENTIFICA_CHOFERES" /><br /><br />
                    <cwc:ResourceCheckBox ID="chkChoferesPorBase" runat="server" ResourceName="Labels"
                        VariableName="CHOFERES_POR_BASE" /><br /><br />
                    <div>
                    <cwc:ResourceLabel ID="lblFrecuenciaReporte" runat="server" ResourceName="Labels" VariableName="FRECUENCIA_REPORTE_AC" SecureRefference="ASSISTCARGO" />
                    <cwc:SecuredTextBox ID="txtFrecuenciaReporte" runat="server" Width="100px" MaxLength="6" Text="0" SecureRefference="ASSISTCARGO" />
                    </div>
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>
    </cwc:AbmTabPanel>
    <cwc:AbmTabPanel ID="tabParametros" runat="server" Title="Parametros">
        <asp:UpdatePanel ID="updParametros" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
        
        <C1:C1GridView id="gridParametros" runat="server" Width="100%" cssclass="SmallGrid" autogeneratecolumns="False" OnRowDataBound="gridParametros_RowDataBound">
            <Columns>
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="NOMBRE" SortDirection="Ascending" SortExpression="Nombre">
                    <ItemStyle Width="50%" />
                    <ItemTemplate>
                    <asp:TextBox ID="txtNombre" runat="server" Width="100%"></asp:TextBox>
                    </ItemTemplate>
                </c1h:C1ResourceTemplateColumn>
                <c1h:C1ResourceTemplateColumn ResourceName="Labels" VariableName="VALOR">
                    <ItemStyle Width="50%" />
                    <ItemTemplate>
                    <asp:TextBox ID="txtValor" runat="server" Width="100%"></asp:TextBox>
                    </ItemTemplate>
                </c1h:C1ResourceTemplateColumn>
                
            </Columns>
        </C1:C1GridView>
        <div style="text-align: right;">
            <asp:Button  ID="btAddParameter" runat="server" Text="Agregar Parametro" CssClass="LogicButton" OnClick="btAddParameter_Click"/>
        </div>
        </ContentTemplate>
        </asp:UpdatePanel>
    </cwc:AbmTabPanel>
</asp:Content>
