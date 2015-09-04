<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Reportes.Accidentologia.VehicleInfractionsDetails" Codebehind="VehicleInfractionsDetails.aspx.cs" %>

<asp:content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <table width="100%">
        <tr align="left" valign="top">
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                <br />
                <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="150px" AutoPostBack="true" />
                <br />
                <br />
                <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                        <br />
                        <cwc:PlantaDropDownList ID="ddlBase" runat="server" ParentControls="ddlDistrito" AddAllItem="true" Width="150px" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblTransportista" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" ParentControls="ddlBase" Width="175px" SelectionMode="Multiple" AddNoneItem="true" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />
                <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                <br />
                <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:TipoVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" ParentControls="ddlBase" Width="175px" SelectionMode="Multiple" AddNoneItem="true" AddAllItem="true" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                
            </td>
            <td align="left" valign="top">
                <cwc:ResourceLinkButton ID="btnVehiculo" runat="server" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbVehiculo" />
                <asp:UpdatePanel ID="upVehiculo" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="lbVehiculo" runat="server" Width="300px" Height="70px" AutoPostBack="false" ParentControls="ddlBase,ddlTransportista,ddlTipoVehiculo" SelectionMode="Multiple" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" /> 
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td align="left" valign="top">
                <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpDesde" runat="server" Width="80px" TimeMode="Start" Mode="Date" />
                <br />
                <br />
                <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpHasta" runat="server" Width="80px" TimeMode="End" Mode="Date" />
                <cwc:DateTimeRangeValidator MinRange="0" MaxRange="93" StartControlID="dpDesde" EndControlID="dpHasta" runat="server" ID="rangeValid" />
            </td>
            <td valign="top">
                <cwc:ResourceCheckBox ID="chkVerEsquinas" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="VER_ESQUINAS" />
            </td>
        </tr>
    </table>
    </asp:content>
