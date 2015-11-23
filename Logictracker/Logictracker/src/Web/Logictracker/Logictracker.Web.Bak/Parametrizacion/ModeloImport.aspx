<%@ Page Language="C#" MasterPageFile="~/MasterPages/ImportPage.master" AutoEventWireup="true" CodeFile="ModeloImport.aspx.cs" Inherits="Logictracker.Parametrizacion.ModeloImport"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentImport" Runat="Server">
    <table border="0">
        <tr>
            <td align="left">
                <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01" />
                <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AddAllItem="true" Width="60%" />        
            </td>
            <td align="left">
                <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                <asp:UpdatePanel ID="upLinea" runat="server" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <cwc:PlantaDropDownList ID="cbLinea" AddAllItem="true" runat="server" Width="60%" ParentControls="cbEmpresa" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cbEmpresa" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>        
            </td>
        </tr>
    </table>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="panelProgress" runat="server" Visible="false" CssClass="filterpanel" style="background-color: #E7E7E7">
                <table style="width:100%">
                    <tr>
                        <td style="width:250px; vertical-align: top;">
                            <asp:Label ID="lblDirs" runat="server"></asp:Label>
                            <br />
                            <asp:Literal ID="litProgress" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <div style="border: solid 1px black; height: 300px; overflow: auto;">
                                <asp:Label ID="lblResult" runat="server"></asp:Label>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>