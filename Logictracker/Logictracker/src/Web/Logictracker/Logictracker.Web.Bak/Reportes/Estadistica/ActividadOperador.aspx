<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActividadOperador.aspx.cs" MasterPageFile="~/MasterPages/ReportGridPage.master" Inherits="Logictracker.Reportes.Estadistica.ReportesActividadOperador" Title="Actividad por Operador" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">  
        <table width="100%" style="font-size: x-small">
            <tr valign="top" align="left">
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AutoPostBack="True" AddAllItem="true" />
                    <br />
                    <br />
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">
                        <contenttemplate>
                            <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                            <br />
                            <cwc:PlantaDropDownList id="ddlBase" runat="server" Width="200px" ParentControls="ddlDistrito" AddAllItem="true" />
                        </contenttemplate>
                        <triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lnkEmpleados" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="OPERADORES" ListControlTargetID="lbEmpleados" />
                            <br />
                    <asp:UpdatePanel ID="upEmpleados" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:EmpleadoListBox ID="lbEmpleados" runat="server" AutoPostBack="false" Width="200px" SelectionMode="Multiple" ParentControls="ddlBase" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblInicio" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" Width="100px" Mode="Date" TimeMode="Start" IsValidEmpty="false" />
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFin" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" Width="100px" Mode="Date" TimeMode="End" IsValidEmpty="false" />
                    <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" StartControlID="dpDesde" EndControlID="dpHasta" MaxRange="31" />
                </td>
                <td>
                    <cwc:ResourceLabel ID="lblKm" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="KM_SUPERADOS" />
                    <br />
                    <c1:C1NumericInput ID="npKm" runat="server" Value="0" MaxValue="99999" MinValue="0" Width="100px" Height="15px" DecimalPlaces="0" />
                </td>
            </tr>
        </table>
</asp:Content>
