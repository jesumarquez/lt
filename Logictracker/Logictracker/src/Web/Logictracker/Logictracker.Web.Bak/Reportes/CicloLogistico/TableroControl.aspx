<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/ReportPage.master" AutoEventWireup="true" CodeFile="TableroControl.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.TableroControl" %>

<asp:Content ID="Filtros" runat="server" ContentPlaceHolderID="ContentFiltros">
    <table width="100%" style="font-size: x-small">
        <tr align="left">
            <td>
                <cwc:ResourceLabel ID="lblDistrito" runat="server" ResourceName="Entities" VariableName="PARENTI01" Font-Bold="True" />
                <br />
                <cwc:LocacionDropDownList ID="ddlEmpresa" runat="server" Width="200px" AddAllItem="true" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="True" />
                <br />
                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDepartamento" runat="server" ResourceName="Entities" VariableName="PARENTI04" Font-Bold="True" />
                <br />
                <cwc:DepartamentoDropDownList ID="ddlDepartamento" runat="server" Width="200px" AddAllItem="true" ParentControls="ddlEmpresa,ddlPlanta" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblDesde" runat="server" ResourceName="Labels" VariableName="DESDE" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtDesde" runat="server" />
            </td>
            <td>
                <cwc:ResourceLabel ID="lblHasta" runat="server" ResourceName="Labels" VariableName="HASTA" Font-Bold="True" />
                <br />
                <cwc:DateTimePicker ID="dtHasta" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentReport" runat="server">
    <asp:UpdatePanel ID="pnlUpd" runat="server" UpdateMode="Always">
        <ContentTemplate>
            <table width="100%" border="0">
                <tr>
                    <td align="center" valign="top" width="50%">
                        <cwc:TitledPanel ID="pnlCc" runat="server" TitleVariableName="CENTROS_COSTOS" TitleResourceName="Menu">
                            <table width="100%" border="0">                                    
                                <tr>
                                    <td>
                                        <asp:GridView ID="gridCentrosDeCostos" runat="server" OnRowDataBound="GridCentroDeCostosOnRowDataBound" AutoGenerateColumns="False" Width="100%" AllowSorting="True" >
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lblCentroDeCosto" runat="server" OnClick="LblCentroDeCostoOnClick" ForeColor="Black" Font-Underline="False" Font-Bold="True" Font-Italic="True" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCantSubCentros" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblObjetivo" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRealizado" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPorcRealizado" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>
                        </cwc:TitledPanel>
                    </td>
                    <td align="center" valign="top" width="50%">
                        <cwc:TitledPanel ID="pnlSubcc" runat="server" TitleVariableName="SUBCENTROS_COSTOS" TitleResourceName="Menu">
                            <table width="100%" border="0">
                                <tr>
                                    <td>
                                        <asp:GridView ID="gridSubCentrosDeCostos" SelectionMode="None" runat="server" OnRowDataBound="GridSubCentroDeCostosOnRowDataBound" AutoGenerateColumns="False" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lblSubCentroDeCosto" runat="server" OnClick="LblSubCentroDeCostoOnClick" ForeColor="Black" Font-Underline="False" Font-Bold="True" Font-Italic="True" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCantVehiculos" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblObjetivo" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRealizado" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPorcRealizado" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPorcHistorico" runat="server" />
                                                    </ItemTemplate>                                            
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </td>
                                </tr>
                            </table>   
                        </cwc:TitledPanel>
                    </td>
                </tr>
                <tr>
                    <td width="100%" valign="top" colspan="2">
                        <table width="100%" border="0">
                            <tr>
                                <td align="center">
                                    <cwc:TitledPanel ID="pnlVehiculos" runat="server" TitleVariableName="PAR_VEHICULOS" TitleResourceName="Menu" >
                                        <table width="100%" border="0">
                                            <tr>
                                                <td align="center" valign="top">
                                                    <asp:GridView ID="gridVehiculos" SelectionMode="None" runat="server" OnRowDataBound="GridVehiculosOnRowDataBound" AutoGenerateColumns="False" Width="100%" >
                                                        <Columns>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblVehiculo" runat="server" Font-Bold="True" Font-Italic="True" />
                                                                </ItemTemplate>                                            
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblObjetivo" runat="server" />
                                                                </ItemTemplate>                                            
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblRealizado" runat="server" />
                                                                </ItemTemplate>                                            
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPorcRealizado" runat="server" />
                                                                </ItemTemplate>                                            
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblPorcHistorico" runat="server" />
                                                                </ItemTemplate>                                            
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </cwc:TitledPanel>
                                </td>
                            </tr>
                        </table>                    
                    </td>
                </tr>
            </table>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


