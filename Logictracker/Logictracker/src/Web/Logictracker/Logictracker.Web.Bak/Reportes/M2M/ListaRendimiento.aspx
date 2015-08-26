<%@ Page Language="C#" MasterPageFile="~/MasterPages/ReportGridPage.master" AutoEventWireup="true" EnableEventValidation="false" CodeFile="ListaRendimiento.aspx.cs" Inherits="Logictracker.Reportes.M2M.ListaRendimiento" Title="Listado de Rendimiento" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Filtros" runat="Server">
    <%--FILTROS--%>
    <table width="100%">
        <tr>
            <td>
                <table width="100%" border="0">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="lblLocacion" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                            <br />
                            <cwc:LocacionDropDownList ID="ddlLocacion" runat="server" Width="160px" AddAllItem="false" />
                            <br/>
                            <br/>
                            <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                            <br />
                            <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                                <contenttemplate>
                                    <cwc:PlantaDropDownList id="ddlPlanta" runat="server" Width="160px" ParentControls="ddlLocacion" AddAllItem="true" />
                                </contenttemplate>
                                <triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                </triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td valign="top">
                            <cwc:ResourceLinkButton ID="lnkTransportistas" runat="server" ListControlTargetID="ddlTransportista" Font-Bold="true" ForeColor="Black" ResourceName="Entities" VariableName="PARENTI07" />
                            <br />
                            <asp:UpdatePanel runat="server" ID="upTransportistas" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <cwc:TransportistasListBox ID="ddlTransportista" runat="server" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta" Width="175px" AutoPostBack="false" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>                                          
                        <td valign="top">
                            <cwc:ResourceLinkButton ID="lnkVehiculos" runat="server" ListControlTargetID="ddlVehiculo" Font-Bold="true" ForeColor="Black" ResourceName="Labels" VariableName="VEHICULOS" />
                            <br />
                            <asp:UpdatePanel runat="server" ID="upVehiculo" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <cwc:MovilListBox ID="ddlVehiculo" runat="server" SelectionMode="Multiple" ParentControls="ddlLocacion,ddlPlanta,ddlTransportista" Width="175px" AutoPostBack="false" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlLocacion" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="ddlTransportista" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>      
                        <td>
                            <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                            <br />
                            <cwc:DateTimePicker ID="dpDesde" runat="server" IsValidEmpty="false" TimeMode="Start" Width="100px" />
                            <br/>
                            <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                            <br />
                            <cwc:DateTimePicker ID="dpHasta" runat="server" TimeMode="End" IsValidEmpty="false" Width="100px" />
                            <br />
                            <cwc:ResourceCheckBox ID="chkControlaCiclo" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="CONTROLAR_EN_VIAJES" />
                        </td>
                    </tr>
                </table>
            </td>           
        </tr>        
    </table>
</asp:Content>
