<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.CicloLogistico.Tickets_TicketTimeDifferences" Title="Reporte de Diferencias" Codebehind="TicketTimeDifferences.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">    
        <table width="100%" style="font-size: x-small">
            <tr align="left">
                <td>
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AutoPostBack="true" ParentControls="ddlLocacion" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />   
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" ParentControls="ddlPlanta" AddAllItem="true" Width="200px"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel> 
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkMoviles" runat="server" ResourceName="Labels" 
                                        VariableName="VEHICULOS" Font-Bold="true" ListControlTargetID="lbMobiles" />
                            <br />
                    <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false" >
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobiles" runat="server" Width="250px" Height="90px" AutoPostBack="false" UseOptionGroup="true" OptionGroupProperty="Estado"
                                          SelectionMode="Multiple" ParentControls="ddlTipoVehiculo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top" align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125px" Mode="Date" TimeMode="Start" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125px" Mode="Date" TimeMode="End" />
                </td>          
                <td valign="bottom" align="right">
                    <asp:CheckBox AutoPostBack="true" ID="chkSubtotals" runat="server" Text="Mostrar Totales" 
                          OnCheckedChanged="chkSubtotals_chkChanged"/>
                <br />
                <br />
                </td>
            </tr>
        </table>
    <br />
  </asp:Content>
   
   <asp:Content ID="cntn2" runat="server" ContentPlaceHolderID="DetalleInferior" >
   <br />
   <br />
    <cwc:ResourceLabel ID="lblTotales" runat="server" ResourceName="Labels" 
                    VariableName="TOTALES" Font-Bold="True" Visible="false" />
    <br />
    <br />
    <C1:C1GridView id="grdSubtotals" BorderStyle="Solid" BorderWidth="1px" cBorderColor="Black"
        runat="server" HorizontalAlign="Center" SkinID="GridLines" >
    </C1:C1GridView>
</asp:Content>
