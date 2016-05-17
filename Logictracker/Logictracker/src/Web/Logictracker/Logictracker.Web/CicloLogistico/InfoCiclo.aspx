 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.CicloLogistico.InfoCiclo" Codebehind="InfoCiclo.aspx.cs" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <style type="text/css">
        body { background-color: White; }
    </style>
    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <asp:UpdatePanel ID="updInfoMovil" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div>
                    <table>
                        <tr>
                            <td rowspan="2" style="text-align: center; width: 40px;">
                                <img id="imgIcono" runat="server" alt=""/>
                            </td>
                            <td>
                                <asp:Label ID="lblCodigo" runat="server" ForeColor="#808080" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblDescripcion" runat="server" Font-Bold="True" />
                            </td>
                        </tr>
                    </table>
                    <asp:Panel Id="panelTab" runat="server" style="text-align: right;" Visible="true">
                        <asp:LinkButton ID="btInfo" runat="server" CssClass="tabbutton" Enabled="false" onclick="BtInfoClick">Info</asp:LinkButton>
                        <asp:LinkButton ID="btPedidos" runat="server" CssClass="tabbutton" onclick="BtPedidosClick">Pedido</asp:LinkButton>
                    </asp:Panel>
                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                        <asp:View ID="View1" runat="server">
                            <table style="margin-top: 5px;">
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel19" runat="server"  ResourceName="Labels" VariableName="TIME_PROGRAMMED" Font-Bold="True" Font-Size="7"/>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblProgramado" runat="server" Font-Size="7" />
                                    </td>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel16" runat="server"  ResourceName="Labels" VariableName="TIME_MANUAL" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblManual" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel17" runat="server"  ResourceName="Labels" VariableName="ENTRADA" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblEntrada" runat="server" Font-Size="7" />
                                    </td>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel18" runat="server"  ResourceName="Labels" VariableName="SALIDA" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblSalida" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel22" runat="server"  ResourceName="Labels" VariableName="ESTADO" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td colspan="3">
                                        <asp:Label ID="lblEstado" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="4">
                                        <cwc:ResourceLinkButton ID="lnkEditarPunto" runat="server" ResourceName="Labels" VariableName="EDITAR_PUNTO_ENTREGA" Font-Size="7" />
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                        <asp:View ID="View2" runat="server">
                            <table style="margin-top: 5px;">
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server"  ResourceName="Labels" VariableName="BULTOS" Font-Bold="True" Font-Size="7"/>
                                    </td>
                                    <td>
                                        <asp:Label ID="lblBultos" runat="server" Font-Size="7" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server"  ResourceName="Labels" VariableName="PESO" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblPeso" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel5" runat="server"  ResourceName="Labels" VariableName="VOLUMEN" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblVolumen" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server"  ResourceName="Labels" VariableName="VALOR" Font-Bold="True" Font-Size="7" />
                                    </td>
                                    <td>
                                        <asp:Label ID="lblValor" runat="server" Font-Size="7" />
                                    </td>
                                </tr>
                            </table>
                        </asp:View>
                    </asp:MultiView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>