<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportPage.master" AutoEventWireup="true" Inherits="Logictracker.Web.Reportes.CicloLogistico.ProyeccionStock" Codebehind="ProyeccionStock.aspx.cs" %>

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
                        <cwc:ResourceLabel ID="lblZona" runat="server" ResourceName="Entities" VariableName="PARENTI89" />
                        <br />
                        <asp:UpdatePanel ID="upZonas" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:ZonaDropDownList id="cbZona" runat="server" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                    <td>
                        <cwc:ResourceLabel ID="lblTipoVehiculo" runat="server" ResourceName="Entities" VariableName="PARENTI17" />
                        <br />
                        <asp:UpdatePanel ID="upTipoVehiculos" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoDeVehiculoDropDownList id="cbTipoVehiculo" runat="server" Width="200px" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="FiltersSelectedIndexChanged" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
    
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentReport">
    <asp:UpdatePanel ID="CenterPanel" runat="server">
        <ContentTemplate>
            <asp:UpdatePanel ID="pnlUpdate" runat="server">
                <ContentTemplate>

                    <table width="100%" border="0">
                        <tr>
                            <td align="center" valign="top"> 
                                <c1:C1GridView ID="gridStock" runat="server" OnRowDataBound="GridStockOnRowDataBound" AutoGenerateColumns="false" Width="100%" Visible="true" SkinID="ListGridNoGroupNoPage" >
                                    <Columns>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblZona" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblTipoVehiculo" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia1" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia2" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>                                        
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate >
                                                <asp:Label ID="lblDia3" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia4" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia5" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia6" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia7" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia8" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia9" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <asp:Label ID="lblDia10" runat="server" />
                                            </ItemTemplate>                                            
                                        </c1:C1TemplateField>
                                    </Columns>
                                </c1:C1GridView>
                            </td>
                        </tr>
                    </table>

                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>    
    
            

