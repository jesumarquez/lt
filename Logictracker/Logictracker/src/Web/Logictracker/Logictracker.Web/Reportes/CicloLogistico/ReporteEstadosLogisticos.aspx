<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Reportes.CicloLogistico.ReporteEstadosLogisticos" Codebehind="ReporteEstadosLogisticos.aspx.cs" %>

<%@ Register Assembly="C1.Web.UI.Controls.3" Namespace="C1.Web.UI.Controls.C1Gauge" TagPrefix="c1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentFiltros" runat="server">
    <asp:UpdatePanel ID="pnlUpd" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        <br />
                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        <br />
                        <asp:UpdatePanel ID="upPlantas" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList id="cbPlanta" runat="server" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblTipoCiclo" runat="server" ResourceName="Entities" VariableName="PARTICK09" />
                        <br />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoCicloLogisticoDropDownList id="cbTipoCiclo" runat="server" Width="200px" ParentControls="cbPlanta" AddAllItem="false" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" />
                        <br />
                        <cwc:DateTimePicker ID="dtDesde" runat="server" Mode="DateTime" IsValidEmpty="false" TimeMode="Start" />
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" />
                        <br />
                        <cwc:DateTimePicker ID="dtHasta" runat="server" Mode="DateTime" IsValidEmpty="false" TimeMode="End" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
    
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentReport">
    <asp:UpdatePanel ID="CenterPanel" runat="server">
        <ContentTemplate>

            <table width="100%" border="0">
                <tr>
                    <td align="center" valign="top"> 
                        <c1:C1GridView ID="gridViajes" runat="server" OnRowDataBound="GridViajesOnRowDataBound" AutoGenerateColumns="false" Width="100%" Visible="true" SkinID="ListGridNoGroupNoPage" >
                            <Columns>
                                <c1:C1TemplateField>
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblBase" runat="server" />
                                    </ItemTemplate>                                            
                                </c1:C1TemplateField>
                                <c1:C1TemplateField>
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblFecha" runat="server" />
                                    </ItemTemplate>                                            
                                </c1:C1TemplateField>
                                <c1:C1TemplateField>
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblViaje" runat="server" />
                                    </ItemTemplate>                                            
                                </c1:C1TemplateField>
                                <c1:C1TemplateField>
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblVehiculo" runat="server" />
                                    </ItemTemplate>                                            
                                </c1:C1TemplateField>
                                <c1:C1TemplateField>
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="lblTransportista" runat="server" />
                                    </ItemTemplate>                                            
                                </c1:C1TemplateField>
                            </Columns>
                        </c1:C1GridView>
                    </td>
                </tr>
            </table>                    

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>    
    
            

