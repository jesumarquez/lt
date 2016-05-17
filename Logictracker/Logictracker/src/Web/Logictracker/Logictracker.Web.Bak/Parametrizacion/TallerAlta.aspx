<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" CodeFile="TallerAlta.aspx.cs" Inherits="Logictracker.Parametrizacion.ParametrizacionTallerAlta" Title="Talleres" %>

<%@ Register Src="~/App_Controls/SelectGeoRefference.ascx" TagName="SelectGeoRefference" TagPrefix="uc1" %>
<%@ Register Src="~/App_Controls/EditEntityGeoRef.ascx" TagName="EditEntityGeoRef" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <cwc:AbmTitledPanel ID="panelTopLeft" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels">
                    
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="NAME" ></cwc:ResourceLabel>
                    <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                    
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="CODE" ></cwc:ResourceLabel>
                    <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
                    
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="TELEFONO" ></cwc:ResourceLabel>
                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="32" Width="98%" />
                    
                </cwc:AbmTitledPanel>
            </td>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA DER--%>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <cwc:TitledPanel ID="TitledPanel2" runat="server" Title="Referencia Geografica">
                    <asp:Panel ID="panex" runat="server" SkinID="FilterPanel">
                        <asp:CheckBox ID="chkExistente" runat="server" /> <cwc:ResourceLabel ID="lblSelecc" runat="server" ResourceName="Labels" VariableName="SELECT_EXISTENT_GEOREF" />
                    </asp:Panel>
                    <br />
                    <asp:Panel ID="panSelectGeoRef" runat="server" style="display: none;">
                        <uc1:SelectGeoRefference ID="SelectGeoRef1" runat="server" ParentControls="ddlPlanta" Height="200px" />
                        <br /><br /><br />
                    </asp:Panel>
                    <asp:Panel ID="panNewGeoRef" runat="server">
                        <uc1:EditEntityGeoRef ID="EditEntityGeoRef1" runat="server" ParentControls="ddlPlanta" />
                    </asp:Panel>
                </cwc:TitledPanel>
            </td>
        </tr>
    </table>
</asp:Content>