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
        </div>
    </form>
</body>
</html>