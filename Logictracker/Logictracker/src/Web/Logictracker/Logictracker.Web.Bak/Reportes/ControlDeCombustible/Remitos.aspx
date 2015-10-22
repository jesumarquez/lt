<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" 
    CodeFile="Remitos.aspx.cs" Inherits="Logictracker.Reportes.ControlDeCombustible.ControlDeCombustible_Remitos" Title="Remitos" %>  

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" Width="75px" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocation" runat="server" Width="200px" AddAllItem="true" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblBase" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" Width="75px" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList runat="server" ID="ddlPlanta" AddAllItem="true" Width="200px" ParentControls="ddlLocation" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocation" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="90px" Mode="Date" TimeMode="Start" />
                </td>
                <td> 
                    <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="90px" Mode="Date" TimeMode="End" />
                </td>
                <td>
                </td>
            </tr>                
        </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleSuperior" runat="Server">
<div>
<br />                               
<cwc:ResourceLabel ID="lblTitle" runat="server" Font-Bold="true" Font-Size="10" 
                ResourceName="Entities" VariableName="PARENTI37" Visible="false" />
<br />
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="DetalleInferior" runat="Server">
    <table  id="tblTotal"  runat="server" width="100%" style="font-size: x-small; border-color: Black; border-style: solid; border-width: 1px" visible="false" >
        <tr>
            <td>
                <asp:Label  ID="lblTotal" runat="server" Font-Size="8"  Font-Bold="true" />
            </td>
        </tr> 
    </table>
    <br />
    <br />
    <div style="text-align: center">        
        <cwc:ResourceLabel ID="lblMobiles" runat="server" Font-Bold="true" Font-Size="10"
             Visible="false" ResourceName="Labels" VariableName="REMITOS_DETALLE"/>
        <br />
        <iframe id="ifMobiles" runat="server" width="100%" src="Remitos/RemitosPorTanque.aspx?IsPrinting=false"
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
                                          ResourceName="Labels" VariableName="REMITOS_DETALLE" />
                <br />
                <iframe id="ifMobilesPrint" visible="false" runat="server" width="100%" height="100%"
                src="Remitos/RemitosPorTanque.aspx?IsPrinting=true" frameborder="0" scrolling="no" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
