<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.ControlDeCombustible.ControlDeCombustible_Despachos" Title="Despachos" Codebehind="Despachos.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>   

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">  
        <table width="100%" >
            <tr>
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AutoPostBack="true" ParentControls="ddlLocacion" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                       <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" ParentControls="ddlPlanta" Width="200px" AddAllItem="true"/>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel> 
              </td>
              <td>
                    <cwc:ResourceLinkButton ID="lnkMovil" runat="server" Font-Bold="true" ResourceName="Labels"
                                VariableName="VEHICULOS" ListControlTargetID="lbMobiles" />
                    <br />
                    <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobiles" runat="server" Width="250px" Height="90px" SelectionMode="Multiple" 
                                UseOptionGroup="true" OptionGroupProperty="Estado" ParentControls="ddlTipoVehiculo" AutoPostBack="false" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
              </td>
              <td>
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="Start" Width="125px" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Mode="DateTime" TimeMode="End" Width="125px" />
                </td>
            </tr>                
        </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
        <div style="text-align: center">                        
                    <cwc:ResourceLabel ID="lblTitle" runat="server" Font-Bold="true" Font-Size="10"
                                         ResourceName="Entities" VariableName="PARENTI37" Visible="false" />
        </div>
        <br />
        <table  id="tblTotal"  runat="server" width="100%" style="font-size: x-small; 
                border-color: Black; border-style: solid; border-width: 1px" visible="false" >
            <tr>
                <td>
                    <asp:Label  ID="lblTotal" runat="server" Font-Size="8"  Font-Bold="true" />
                </td>
            </tr>
        </table>
        <br />
        <div style="text-align: center">        
          <cwc:ResourceLabel ID="lblMobiles" runat="server" Font-Bold="true" Font-Size="10" Visible="false"
                                              ResourceName="Labels" VariableName="DESPACHOS_DETALLE" />
                        <br />
          <iframe id="ifMobiles" runat="server" width="100%" src="Despachos/DespachosPorMovil.aspx?IsPrinting=false"
                            visible="false" style="border-style: none" height="500px" />
       </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="DetalleInferiorPrint" runat="server">
    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div style="text-align: center; width: auto;">
                <br />
                <br />   
                <asp:Label  ID="lblTotalPrint" runat="server" Font-Size="8" Font-Bold="true" />
                <br />
            </div>
            <br />
            <div style="text-align: center; width: auto; page-break-before:always;">
                <cwc:ResourceLabel ID="lblDetallePrint" runat="server" Font-Bold="true" Font-Size="10" Visible="false"
                                          ResourceName="Labels" VariableName="DESPACHOS_DETALLE" />
                <br />
                <iframe id="ifMobilesPrint" visible="false" runat="server" width="100%" height="100%"
                src="Despachos/DespachosPorMovil.aspx?IsPrinting=true" frameborder="0" scrolling="no" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
