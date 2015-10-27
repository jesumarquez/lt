<%@ Page Language="C#" MasterPageFile="~/MasterPages/ImportPage.master" AutoEventWireup="true" CodeFile="ConsumoImport.aspx.cs" Inherits="Logictracker.Mantenimiento.ConsumoImport"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentImport" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnl" runat="server" style="background-color: #E7E7E7">
                <table style="width:100%">
                    <tr>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                        </td>
                        <td>
                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="175px" />
                        </td>
                        <td>
                            <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI02" />
                        </td>
                        <td>
                            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="175px" ParentControls="cbEmpresa" AddNoneItem="true" AddAllItem="true" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        
            <asp:Panel ID="panelProgress" runat="server" Visible="false" CssClass="filterpanel" style="background-color: #E7E7E7">
                <table style="width:100%">
                    <tr>
                        <td style="width:250px; vertical-align: top;">
                            <asp:Label ID="lblDirs" runat="server" />
                            <br />
                            <asp:Literal ID="litProgress" runat="server" />
                        </td>
                        <td>
                            <div style="border: solid 1px black; height: 300px; overflow: auto;">
                                <asp:Label ID="lblResult" runat="server" />
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>