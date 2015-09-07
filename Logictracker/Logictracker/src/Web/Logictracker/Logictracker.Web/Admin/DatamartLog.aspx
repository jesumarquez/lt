<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Admin.DatamartLog" Codebehind="DatamartLog.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblDatamart" runat="server" ResourceName="Datamart" VariableName="Datamart" Font-Bold="true" />
                <br />
                <cwc:DataMartDropDownList ID="ddlDatamart" runat="server" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpDesde" runat="server" TimeMode="Start" Mode="DateTime" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dtpHasta" runat="server" TimeMode="Now" Mode="DateTime" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" runat="Server">
   
           
                    <table style="width: 100%;color: #999999; text-align:right">
                        <tr>                 
                            <td style="text-align: right;">
                                <asp:Panel ID="panelBuscar" runat="server">
                                    Buscar <asp:TextBox ID="txtBuscar" runat="server" Width="200px" AutoPostBack="true"></asp:TextBox>
                                    <div style="width: 20px; height: 20px; float: right; padding-top: 5px; text-align:center; position: relative; right: 24px; cursor: pointer;" onclick="var t = $get('<%= txtBuscar.ClientID %>'); t.value = ''; <%= Page.ClientScript.GetPostBackEventReference(txtBuscar, "") %>;">X</div>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
             
       
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="Server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="Server">
</asp:Content>
