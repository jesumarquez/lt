<%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true" Inherits="Logictracker.Parametrizacion.PlanAlta" Codebehind="PlanAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <cwc:AbmTabPanel ID="abmTabGeneral" runat="server" ResourceName="Labels" VariableName="DATOS_GENERALES">
        <table style="width: 100%; border-spacing: 10px;">
            <tr>
                <td valign="top">
                    <cwc:AbmTitledPanel ID="panelTop" runat="server" TitleVariableName="DATOS_GENERALES" TitleResourceName="Labels" Height="130px">
                        
                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Labels" VariableName="EMPRESA" />
                        <cwc:EmpresaTelefonicaDropDownList ID="cbEmpresa" runat="server" Width="60%" />
                        
                        <cwc:ResourceLabel ID="lblTipoLinea" runat="server" ResourceName="Labels" VariableName="TIPO_LINEA" />
                        <cwc:TipoLineaTelefonicaDropDownList ID="cbTipoLinea" runat="server" Width="60%" />
                        
                        <cwc:ResourceLabel ID="lblCodigoAbono" runat="server" ResourceName="Labels" VariableName="CODIGO_ABONO" />
                        <asp:TextBox ID="txtCodigoAbono" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblCosto" runat="server" ResourceName="Labels" VariableName="COSTO_MENSUAL" />
                        <asp:TextBox ID="txtCosto" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblAbonoDatos" runat="server" ResourceName="Labels" VariableName="ABONO_DATOS" />
                        <asp:TextBox ID="txtAbonoDatos" runat="server" />
                        
                        <cwc:ResourceLabel ID="lblUnidadMedida" runat="server" ResourceName="Labels" VariableName="UNIDAD_MEDIDA" />
                        <cwc:UnidadMedidaDatosDropDownList ID="cbUnidadMedida" runat="server" Width="60%" />
                        
                    </cwc:AbmTitledPanel>
                </td>
            </tr>
        </table>
    </cwc:AbmTabPanel>
    
</asp:Content>
