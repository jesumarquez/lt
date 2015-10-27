<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" CodeFile="MobilePoisHistoric.aspx.cs" Inherits="Logictracker.Reportes.Estadistica.MobilePoisHistoric" Title="Untitled Page" %>
<%@ Register src="~/App_Controls/Pickers/NumberPicker.ascx" tagname="NumberPicker" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">

        <table width="100%">
            <tr>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" Font-Bold="true" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" />
                <br />
                    <asp:UpdatePanel ID="upBase" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblBase" runat="server" ResourceName="Entities" Font-Bold="true" VariableName="PARENTI02" />
                            <br />
                            <cwc:PlantaDropDownList ID="ddlBase" runat="server" Width="175px" ParentControls="ddlDistrito" AddAllItem="true"  />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <asp:UpdatePanel runat="server" ID="upTipoDomicilio" UpdateMode="Conditional">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="lblTipoDomicilio" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI10" />
                            <br />
                            <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoDomicilio" runat="server" Width="200px" AddAllItem="true"  ParentControls="ddlDistrito,ddlBase" ShowAllTypes="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
   
                    <asp:UpdatePanel ID="upDomicilio" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Menu" VariableName="PAR_POI" />                           
                            <br />
                            <cwc:ReferenciaGeograficaDropDownList ID="ddlGeoRef" runat="server" Width="200px" ParentControls="ddlDistrito,ddlBase,ddlTipoDomicilio" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoDomicilio" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblDistancia" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DISTANCIA" />
                    <br />
                    <uc1:NumberPicker ID="npDistancia" runat="server" IsValidEmpty="false" Mask="9999" MaximumValue="9999" Number="500" Width="125" />
                </td>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    <br />
                    <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" Width="125" TimeMode="Now" />
                <br />
                    <cwc:ResourceLabel ID="lblFecha" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    <br />
                    <cwc:DateTimePicker ID="dpHasta" runat="server" IsValidEmpty="false" Width="125" TimeMode="Now" />
                    <cwc:DateTimeRangeValidator ID="dtrValidator" runat="server" StartControlID="dpDesde" EndControlID="dpHasta" MaxRange="12:00:00" MinRange="00:01:00" />
                </td>
            </tr>
        </table>
    
</asp:Content>

