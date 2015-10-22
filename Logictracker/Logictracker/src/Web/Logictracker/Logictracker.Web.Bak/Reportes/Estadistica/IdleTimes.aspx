<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPages/ReportGraphPage.master"  CodeFile="IdleTimes.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.ReportesEstadisticaIdleTimes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
        <table width="100%">
            <tr>
                <td>
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="true" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="200px" AddAllItem="true" /> 
                    <br />
                    <br />
                    <cwc:ResourceLabel ID="lblFechaDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker id="dtpDate" runat="server" Width="90px" IsValidEmpty="false" Mode="Date" TimeMode="Start" /> 
                    <br />
                    <br />
                    <cwc:ResourceCheckBox ID="chkUndefined" Font-Bold="true" runat="server" Checked="false" ResourceName="Labels" VariableName="INCLUYE_INDEFINIDO" />  
                </td>
                <td>
                    <cwc:ResourceLinkButton ID="lblBase" runat="server" Font-Bold="true" VariableName="PARENTI02" ResourceName="Entities" ListControlTargetID="lbBase"/>  
                    <asp:UpdatePanel runat="server" ID="upBase" UpdateMode="Conditional">  
                        <ContentTemplate>
                            <cwc:PlantaListBox ID="lbBase" runat="server" Width="300px" Height="100px" AutoPostBack="false" ParentControls="ddlDistrito" SelectionMode="Multiple" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
</asp:Content>

