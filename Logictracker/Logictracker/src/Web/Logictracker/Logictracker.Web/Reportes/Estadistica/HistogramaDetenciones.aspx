<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" 
        Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaHistogramaDetenciones" Codebehind="HistogramaDetenciones.aspx.cs" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">     
        <table width="100%" style="font-weight: bold">
            <tr>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" />
                    <br />
                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                    <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" AddAllItem="true" ParentControls="ddlDistrito" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                    <br />
                    <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                        <ContentTemplate>
                            <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="175px" AddAllItem="true" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ResourceName="Entities" VariableName="PARENTI03" ListControlTargetID="lbVehiculos" />
                    <br />
                    <asp:UpdatePanel ID="upVehiculos" runat="server" UpdateMode="Conditional" RenderMode="Inline" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:MovilListBox ID="lbVehiculos" runat="server" Width="175px" Height="95px" SelectionMode="Multiple"
                                 ParentControls="ddlTipoVehiculo" UseOptionGroup="true" OptionGroupProperty="Estado" AutoPostBack="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" Width="120px" Mode="DateTime" IsValidEmpty="false" TimeMode="Start" />
                    <br />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" Width="120px" Mode="DateTime" IsValidEmpty="false" TimeMode="End" />
                    <cwc:DateTimeRangeValidator ID="dtrange" runat="server" MaxRange="31" MinRange="0" StartControlID="dpDesde" EndControlID="dpHasta" />
                </td>
                <td align="left">
                    <cwc:ResourceLabel ID="lblDuracion" runat="server" ResourceName="Labels" VariableName="DURACION" />
                    <br />
                    <c1:C1NumericInput ID="npDuracion" runat="server" Width="70px" Height="15px" Value="30" MinValue="0" MaxValue="999" DecimalPlaces="0" />
                    <br />
                    <cwc:ResourceLabel ID="lblRadio" runat="server" ResourceName="Labels" VariableName="RADIO" />
                    <br />
                    <c1:C1NumericInput ID="npRadio" runat="server" Width="70px" Height="15px" Value="100" MinValue="0" MaxValue="9999" DecimalPlaces="0" />
                    <br />
                    <cwc:ResourceLabel ID="lblResultados" runat="server" ResourceName="Labels" VariableName="RESULTADOS" />
                    <br />
                    <c1:C1NumericInput ID="npResultados" runat="server" Width="70px" Height="15px" Value="10" MinValue="0" MaxValue="99" DecimalPlaces="0" />
                </td>
            </tr>
        </table>
</asp:Content>

