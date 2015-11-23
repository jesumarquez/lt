<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" 
    CodeFile="mobilesTickets.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.Documentos_mobilesTickets" Title="Reporte de Tickets por Movil" ValidateRequest="false" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.CheckBoxs" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">
    <style type="text/css">
        .column { width:100px; text-align: center; border-left: solid 1px black; }
        .header { border-top: solid 1px black; margin-top: 5px; }
        .datatable {width: 100%; padding: 0px; border-spacing: 0px; text-align: center; }
    </style>

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
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server"  ResourceName="Entities" VariableName="PARENTI17" Font-Bold="true" />
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
                                        VariableName="VEHICULOS" Font-Bold="true" ListControlTargetID="lbMobiles"/>
                            <br />
                    <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobiles" runat="server" Width="250px" Height="90px" SelectionMode="Multiple"
                                 UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false" ParentControls="ddlTipoVehiculo" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lbMobiles" EventName="SelectedIndexChanged" />
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
                    <br />
                    <cwc:ResourceCheckBox ID="chkProgramado" runat="server" ResourceName="Labels" VariableName="TIME_PROGRAMMED" Checked="true" />
                    <cwc:ResourceCheckBox ID="chkManual" runat="server" ResourceName="Labels" VariableName="TIME_MANUAL" Checked="true" />
                    <cwc:ResourceCheckBox ID="chkAuto" runat="server" ResourceName="Labels" VariableName="TIME_AUTOMATIC" Checked="true" />
                </td>      
            </tr>
        </table>
</asp:Content>
