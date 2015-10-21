 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Web.CicloLogistico.InfoDetencion" Codebehind="InfoDetencion.aspx.cs" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

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
    <div>
        <table width="100%" border="0">
            <tr>
                <td align="left">
                    <asp:Label ID="lblFecha" runat="server"  />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Label ID="lblEvento" runat="server" Font-Bold="True" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:Label ID="lblLatLong" runat="server"  />
                </td>
                
            </tr>
            <tr>
                <td align="left">
                    <C1:c1GridView ID="gridEntregas" runat="server" OnRowDataBound="GridEntregasRowDataBound" SkinID="SmallGrid" >
                        <Columns>
                            <C1:C1TemplateField HeaderText="Orden" >
                                <ItemStyle HorizontalAlign="Right" />
                            </C1:C1TemplateField>
                            <C1:C1TemplateField HeaderText="Entrega" />
                            <C1:C1TemplateField HeaderText="Pto. Entrega" />
                            <C1:C1TemplateField HeaderText="Descripción" />
                            <C1:C1TemplateField HeaderText="Estado" />
                            <c1:C1CheckBoxField HeaderText="Asociar" >
                                <ItemStyle HorizontalAlign="Center" />
                            </C1:C1CheckBoxField>
                        </Columns>
                    </C1:c1GridView>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cwc:ResourceLabel ID="lblNombre" runat="server" VariableName="NOMBRE" ResourceName="Labels" />:
                    <asp:TextBox ID="txtNombre" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cwc:ResourceLabel ID="lblRadio" runat="server" VariableName="RADIO" ResourceName="Labels" />:
                    <asp:TextBox ID="txtRadio" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cwc:ResourceButton ID="btnAsociar" runat="server" VariableName="ASOCIAR_PUNTOS" ResourceName="Labels" OnClick="BtnAsociarClick" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    <cwc:ResourceLabel ID="lblAsociados" runat="server" VariableName="ASOCIACION_EXITOSA" ResourceName="Labels" Visible="False" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
