<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true"
    CodeFile="Log.aspx.cs" Inherits="Logictracker.Admin.AdminLog" Title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%" cellpadding="5">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblTipo" runat="server" ResourceName="Labels" VariableName="TYPE" Font-Bold="true" />
                <br />
                <cwc:LogTypeDropDownList ID="ddlLogType" runat="server" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblModulo" runat="server" ResourceName="Labels" VariableName="MODULO" Font-Bold="true" />
                <br />
                <cwc:LogModuleDropDownList ID="ddlLogModule" runat="server" AddAllItem="true" />
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblComponente" runat="server" ResourceName="Labels" VariableName="COMPONENTE" Font-Bold="true" />
                <br />
                <cwc:LogComponentDropDownList ID="ddlLogComponent" runat="server" AddAllItem="true" />
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
