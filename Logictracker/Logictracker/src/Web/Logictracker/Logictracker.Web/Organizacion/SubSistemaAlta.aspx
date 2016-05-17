 <%@ Page Language="C#" MasterPageFile="~/MasterPages/AbmPage.master" AutoEventWireup="true"
    Inherits="Logictracker.Organizacion.AltaSistema" Title="Sub Sistemas" Codebehind="SubSistemaAlta.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="AbmContent" runat="Server">
    <table id="tbTipoPunto" style="width: 90%; margin: auto; margin-top: 30px;" cellpadding="5">
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <%--COLUMNA IZQ--%>
                <cwc:AbmTitledPanel ID="panGeneral" runat="server" TitleVariableName="DATOS_GENERALES"
                    TitleResourceName="Labels" Height="180px">
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Labels" VariableName="DESCRIPCION" />
                    <cwc:MenuResourcesDropDownList ID="ddlMenuResource" runat="server" Width="100%" FunctionMode="false" />
                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="URL" />
                    <asp:TextBox ID="txtURL" runat="server" Width="100%" MaxLength="255" TabIndex="3" />
                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" ResourceName="Labels" VariableName="ORDER" />
                    <c1:C1NumericInput ID="npOrden" runat="server" SmartInputMode="true" Width="120px"
                        Height="17px" MaxValue="128" DecimalPlaces="0" MinValue="0" />                   
                    <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="ACTIVO" />
                    <asp:CheckBox ID="chkEnabled" runat="server" Checked="True" />
                </cwc:AbmTitledPanel>
            </td>
            <td style="vertical-align: top; width: 50%;">
            </td>
        </tr>
    </table>
</asp:Content>
