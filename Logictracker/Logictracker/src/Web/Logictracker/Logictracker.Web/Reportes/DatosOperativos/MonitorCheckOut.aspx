<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGraphPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.MonitorCheckOut" Title="MonitorCheckOut" Codebehind="MonitorCheckOut.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">

        <table width="100%" style="font-size: x-small">
            <tr align="left">
                <td valign="top">
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
                <td valign="top" align="left">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="100" TimeMode="Start" Mode="DateTime" />
                    <br />
                    <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="100" TimeMode="End" Mode="DateTime" />
                    <cwc:DateTimeRangeValidator ID="dpValidator" runat="server" EndControlID="dpHasta" MaxRange="1" StartControlID="dpDesde" />
                </td>
                <td valign="top" align="left">
                    <cwc:ResourceLabel ID="lblPeriodo" runat="server" ResourceName="Labels" VariableName="PERIODO" Font-Bold="True" />
                    <br />
                    <asp:RadioButton ID="rbtn15" runat="server" Text="15" Checked="true" GroupName="periodo" ValidationGroup="periodo" />
                    <br />
                    <asp:RadioButton ID="rbtn30" runat="server" Text="30" Checked="false" GroupName="periodo" ValidationGroup="periodo" />
                    <br />
                    <asp:RadioButton ID="rbtn60" runat="server" Text="60" Checked="false" GroupName="periodo" ValidationGroup="periodo" />
                </td>
            </tr>
        </table>
</asp:Content>
