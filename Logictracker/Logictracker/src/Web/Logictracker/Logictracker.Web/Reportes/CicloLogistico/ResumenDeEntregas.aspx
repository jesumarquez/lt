<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CicloLogistico.ResumenDeEntregas" Codebehind="ResumenDeEntregas.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="server">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="130px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbTransportista" />
                <br />
                <cwc:TransportistasListBox ID="lbTransportista" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblDepartamento" runat="server" ResourceName="Entities" VariableName="PARENTI04" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbDepartamento" />
                <br />
                <cwc:DepartamentoListBox ID="lbDepartamento" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI37" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbCentroDeCostos" />
                <br />
                <cwc:CentroDeCostosListBox ID="lbCentroDeCostos" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblSubCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI99" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbSubCentroDeCostos" />
                <br />
                <cwc:SubCentroDeCostosListBox ID="lbSubCentroDeCostos" runat="server" Width="130px" AutoPostBack="true" ParentControls="ddlLocacion,ddlPlanta,lbDepartamento,lbCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI03" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbVehiculo" />
                <br />
                <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="130px" ParentControls="ddlLocacion,ddlPlanta,lbTransportista,lbDepartamento,lbCentroDeCostos,lbSubCentroDeCostos" SelectionMode="Multiple" />
            </td>
            <td valign="top" align="left">
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="Start" />
                <br />
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="true" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="Date" TimeMode="End" />
                <br />
                <cwc:ResourceCheckBox runat="server" ID="chkPendientes" ResourceName="Labels" VariableName="VER_PENDIENTES" Font-Bold="True" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FiltrosAvanzados" runat="server" />
<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="server" />
<asp:Content ID="Content4" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <table id="tbl_totales" runat="server" cellpadding="5" class="abmpanel" style="width: 30%" visible="false">  
        <tr>
            <td class="panelheader" colspan="3">
                <cwc:ResourceLabel ID="lblResTotales" runat="server" ResourceName="Labels" VariableName="TOTALES" Font-Bold="true" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                <cwc:ResourceLabel ID="rlblTotal" runat="server" ResourceName="Labels" VariableName="TOTAL" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblPorc" runat="server" Text="%" />
            </td>
        </tr>
        <tr>
            <td>
                <cwc:ResourceLabel ID="rlblEntregas" runat="server" ResourceName="Labels" VariableName="ENTREGAS" Font-Bold="True" />
            </td>
            <td>
                <asp:Label ID="lblTotal" runat="server" />
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr style="background-color: chartreuse">
            <td>
                <cwc:ResourceLabel ID="rlblRealizadas" runat="server" ResourceName="Labels" VariableName="REALIZADOS" />
            </td>
            <td>
                <asp:Label ID="lblCantRealizadas" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcRealizadas" runat="server" />
            </td>
        </tr>
        <tr style="background-color: #008000">
            <td>
                <cwc:ResourceLabel ID="rlblCompletadas" runat="server" ResourceName="Labels" VariableName="COMPLETADOS" />
            </td>
            <td>
                <asp:Label ID="lblCantCompletadas" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcCompletadas" runat="server" />
            </td>
        </tr>
        <tr style="background-color: #FFFF00">
            <td>
                <cwc:ResourceLabel ID="rlblVisitadas" runat="server" ResourceName="Labels" VariableName="VISITADOS" />
            </td>
            <td>
                <asp:Label ID="lblCantVisitadas" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcVisitadas" runat="server" />
            </td>
        </tr>
        <tr style="background-color: #00A2E8">
            <td>
                <cwc:ResourceLabel ID="rlblEnSitio" runat="server" ResourceName="Labels" VariableName="EN_SITIO" />
            </td>
            <td>
                <asp:Label ID="lblCantEnSitio" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcEnSitio" runat="server" />
            </td>
        </tr>
        <tr style="background-color: gray">
            <td>
                <cwc:ResourceLabel ID="rlblEnZona" runat="server" ResourceName="Labels" VariableName="EN_ZONA" />
            </td>
            <td>
                <asp:Label ID="lblCantEnZona" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcEnZona" runat="server" />
            </td>
        </tr>
        <tr style="background-color: #FF0000">
            <td>
                <cwc:ResourceLabel ID="rlblNoCompletadas" runat="server" ResourceName="Labels" VariableName="NO_COMPLETADOS" />
            </td>
            <td>
                <asp:Label ID="lblCantNoCompletadas" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcNoCompletadas" runat="server" />
            </td>
        </tr>
        <tr style="background-color: #FF4500">
            <td>
                <cwc:ResourceLabel ID="rlblNoVisitadas" runat="server" ResourceName="Labels" VariableName="NO_VISITADOS" />
            </td>
            <td>
                <asp:Label ID="lblCantNoVisitadas" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblPorcNoVisitadas" runat="server" />
            </td>
        </tr>
    </table>
    
    <asp:UpdatePanel ID="upCharts" runat="server">   
        <ContentTemplate>
            <table>
                <tr align="center">
                    <td>
                        <div id="divChart" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
   </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="DetalleSuperiorPrint" runat="server" />
<asp:Content ID="Content6" ContentPlaceHolderID="DetalleInferiorPrint" runat="server" />

