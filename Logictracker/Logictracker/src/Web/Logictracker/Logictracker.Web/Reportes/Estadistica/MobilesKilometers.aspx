<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.EstadisticaMobilesKilometers" Title="Reporte de Kilometros" Codebehind="MobilesKilometers.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">

        <table width="100%" style="font-size: x-small">
            <tr align="left">
                <td valign="top">
                    <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlLocacion" AddAllItem="true" runat="server" Width="200px" />
                    <br />
                    <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                    <br />
                    <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" AddAllItem="true" Width="200px" AutoPostBack="true" ParentControls="ddlLocacion" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" AddAllItem="true" runat="server" ParentControls="ddlPlanta" Width="200px"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <cwc:ResourceLabel ID="lblTransportista" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI07" />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                           <cwc:TransportistaDropDownList ID="ddlTransportista" AddAllItem="true" runat="server" ParentControls="ddlPlanta" Width="200px"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel> 
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkMoviles" runat="server" Font-Bold="true" ForeColor="Black" ListControlTargetID="lbMobiles" ResourceName="Labels" VariableName="VEHICULOS" />
                    <br />
                    <asp:UpdatePanel ID="upMovil" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbMobiles" runat="server" Width="250px" Height="90px" SelectionMode="Multiple" ParentControls="ddlTipoVehiculo,ddlTransportista" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="false"  />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lbMobiles" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top" align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="DateTime" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="DateTime" />
                    <cwc:BaloonTip ID="baloontip" runat="server" Text="El período máximo para este reporte es de 31 días incluyendo el día de la fecha final." />
                    <cwc:DateTimeRangeValidator ID="dpValidator" runat="server" EndControlID="dpHasta" MaxRange="31" StartControlID="dpDesde" />
                    <br/>
                    <cwc:ResourceCheckBox ID="chkEnCiclo" runat="server" ResourceName="Labels" VariableName="FILTRAR_EN_RUTA" Font-Bold="True" />
                </td>      
            </tr>
        </table>
</asp:Content>
