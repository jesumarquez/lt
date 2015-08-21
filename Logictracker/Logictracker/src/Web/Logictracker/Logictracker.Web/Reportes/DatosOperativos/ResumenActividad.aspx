<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="ResumenActividad.aspx.cs" Inherits="Logictracker.Web.Reportes.DatosOperativos.ResumenActividad"  %>

<asp:Content ContentPlaceHolderID="Filtros" runat="server" ID="cph">
   
    <table width="100%">
        <tr align="left" style="font-weight: bold" valign="top">
            <td>
                <cwc:ResourceLabel ID="lblLocacion" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <br />
                <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="200px" />
                <br />
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                <br />
                <asp:UpdatePanel ID="upPlanta" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblTransportista" runat="server" ResourceName="Entities" VariableName="PARENTI07" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:TransportistaDropDownList ID="ddlTransportista" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>                        
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <cwc:ResourceLabel ID="lblSubCentroDeCostos" runat="server" ResourceName="Entities" VariableName="PARENTI99" />
                <br />
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:SubCentroDeCostosDropDownList ID="ddlSubCentroDeCostos" runat="server" AddAllItem="true" Width="200px" ParentControls="ddlLocacion,ddlPlanta" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                       </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLinkButton ID="lblMobile" runat="server" ListControlTargetID="lbMobile" ResourceName="Labels" VariableName="VEHICULOS" ForeColor="Black" />
                <br />
                <asp:UpdatePanel ID="upMobile" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cwc:MovilListBox ID="lbMobile" runat="server" Width="200px" Height="95px" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTransportista,ddlSubCentroDeCostos" AutoPostBack="false" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlSubCentroDeCostos" EventName="SelectedIndexChanged" />        
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                <cwc:ResourceLabel ID="lblInitDate" runat="server" ResourceName="Labels" VariableName="DESDE" />
                <br />
                <cwc:DateTimePicker ID="dpInitDate" runat="server" Width="100px" TimeMode="Start" IsValidEmpty="false" Mode="DateTime" />
                <br />
                <cwc:ResourceLabel ID="lblEndDate" runat="server" ResourceName="Labels" VariableName="HASTA" />
                <br />
                <cwc:DateTimePicker ID="dpEndDate" runat="server" Width="100px" TimeMode="End" IsValidEmpty="false" Mode="DateTime" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="DetalleInferior" runat="Server">
</asp:Content>
