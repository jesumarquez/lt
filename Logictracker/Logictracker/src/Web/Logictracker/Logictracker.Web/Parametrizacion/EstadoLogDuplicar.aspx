<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="EstadoLogDuplicar.aspx.cs"
    Inherits="Logictracker.Parametrizacion.Parametrizacion_EstadoLogDuplicar" Title="Estados Logisticos" %>
<%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table width="100%">
        <tr align="center">
            <td>
                <%--TITLEBAR--%>
                <cwc:TitleBar ID="TitleBar1" runat="server" ResourceName="Menu" VariableName="PAR_ESTADOLOG" />
    
                <%--PROGRESSLABEL--%>
                <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
                <%--ERRORLABEL--%>
                <cwc:InfoLabel ID="infoLabel1" runat="server" />
                <table cellpadding="5">
                    <tr align="center">
                        <td align="left">
                            <cwc:ResourceLabel ID="lblPlanta" runat="server" ResourceName="Entities" VariableName="PARENTI02" Font-Bold="true" />
                            <br />
                            <asp:UpdatePanel ID="upplanta" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                <ContentTemplate>
                                    <cwc:PlantaListBox ID="lbPlanta" runat="server" Width="250px" SelectionMode="Multiple" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lbPlanta" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                        </td>
                        <td align="left" valign="bottom">
                            <cwc:ResourceButton ID="btnDuplicate" runat="server" Width="150px" OnClick="btnDuplicate_Click" ResourceName="Controls"
                                VariableName="BUTTON_DUPLICATE" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
