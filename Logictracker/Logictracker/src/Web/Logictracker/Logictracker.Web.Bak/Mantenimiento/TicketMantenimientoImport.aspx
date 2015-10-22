<%@ Page Language="C#" MasterPageFile="~/MasterPages/ImportPage.master" AutoEventWireup="true" CodeFile="TicketMantenimientoImport.aspx.cs" Inherits="Logictracker.Mantenimiento.TicketMantenimientoImport"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentImport" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
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