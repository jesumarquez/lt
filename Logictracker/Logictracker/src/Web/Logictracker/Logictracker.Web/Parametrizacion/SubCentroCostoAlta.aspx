<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.SubCentroCostoAlta" Codebehind="SubCentroCostoAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table style="width: 100%; border-spacing: 10px;">
        <tr>
            <td style="width: 50%; vertical-align: top;">
                <asp:UpdatePanel ID="upCentros" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:AbmTitledPanel ID="panSubCentros" runat="server" TitleResourceName="Labels" TitleVariableName="DATOS_SUBCENTRO">
                                
                                <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                                <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="98%" OnInitialBinding="DdlEmpresaPreBind" />
                                
                                <cwc:ResourceLabel ID="lblParenti02" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                                <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="98%" OnInitialBinding="CbLineaPreBind" ParentControls="ddlEmpresa" AddAllItem="true" />
                                
                                <cwc:ResourceLabel ID="lblParenti37" runat="server" ResourceName="Entities" VariableName="PARENTI37" />
                                <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" Width="98%" AddAllItem =true  OnInitialBinding="CbCentroDeCostosPreBind" ParentControls="ddlEmpresa,cbLinea" />
                                
                                <cwc:ResourceLabel ID="lblCodigo" runat="server" ResourceName="Labels" VariableName="CODE" />
                                <asp:TextBox ID="txtCodigo" runat="server" Width="98%" MaxLength="32" />
                                
                                <cwc:ResourceLabel ID="lblDescripcion" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                                <asp:TextBox ID="txtDescripcion" runat="server" Width="98%" MaxLength="32" />
                                
                                <cwc:ResourceLabel ID="lblObjetivo" runat="server" ResourceName="Labels" VariableName="OBJETIVO" />
                                <asp:TextBox ID="txtObjetivo" runat="server" />
                                
                        </cwc:AbmTitledPanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td style="width: 50%; vertical-align: top;">
            </td>
        </tr>
    </table>
</asp:Content>
