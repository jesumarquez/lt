<%@ Page Title="Cercania a Puntos de Interes" Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" Inherits="Logictracker.Reportes.DatosOperativos.MobilePois" Codebehind="MobilePois.aspx.cs" %>

<%@ Register src="~/App_Controls/Pickers/NumberPicker.ascx" tagname="NumberPicker" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" Runat="Server">

        <table width="100%">
            <tr>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" Font-Bold="true" VariableName="PARENTI01" />
                    <br />
                    <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="175px" AddAllItem="true" />
                </td>
                <td valign="top">
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
                            <cwc:TipoReferenciaGeograficaDropDownList ID="ddlTipoDomicilio" runat="server" Width="175px" AddAllItem="true"  ParentControls="ddlBase" ShowAllTypes="true" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                        <cwc:ResourceLinkButton ID="lnkDomicilios" runat="server" Font-Bold="true" ResourceName="Menu" VariableName="PAR_POI" ListControlTargetID="lbPOIs" />
                            <br />
                    <asp:UpdatePanel ID="upDomicilio" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                        <ContentTemplate>
                            <cwc:POIsListBox ID="lbPOIs" runat="server" Width="225px" AutoPostBack="false" SelectionMode="Multiple" ParentControls="ddlTipoDomicilio" />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlBase" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlTipoDomicilio" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
                <td valign="top">
                    <cwc:ResourceLabel ID="lblDistancia" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DISTANCIA" />
                    <br />
                    <uc1:NumberPicker ID="npDistancia" runat="server" IsValidEmpty="false" Mask="9999" MaximumValue="9999" Number="500" Width="75" />
                </td>
            </tr>
        </table>

</asp:Content>