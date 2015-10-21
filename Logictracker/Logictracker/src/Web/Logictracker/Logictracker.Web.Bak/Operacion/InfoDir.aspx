﻿ <%@ Page Language="C#" AutoEventWireup="true" CodeFile="InfoDir.aspx.cs" Inherits="Logictracker.Operacion.OperacionInfoDir" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
<style type="text/css">
    body{
        background-color: White;
    }
    </style>
    <form id="form1" runat="server">
    <div>
        <table>
        <tr>
        <td>
            <asp:Image ID="imgTipo" runat="server" /></td>
        <td>
            <div style="font-size: 9px; color: #CCCCCC">
                <asp:Label ID="lblTipo" runat="server" Text=""></asp:Label>
            </div>
            <div style="font-size: 11px; font-weight: bold;">
                <asp:Label ID="lblDescripcion" runat="server" Text=""></asp:Label>
            </div>
            <div style="font-size: 9px; color: #666666">
                <cwc:ResourceLabel ID="lblTitleLatitud" runat="server" ResourceName="Labels" VariableName="LATITUD" />
                <asp:Label ID="lblLatitud" runat="server" Text=""></asp:Label>
                <br />
                <cwc:ResourceLabel ID="lblTitleLongitud" runat="server" ResourceName="Labels" VariableName="LONGITUD" />
                <asp:Label ID="lblLongitud" runat="server" Text=""></asp:Label>
            </div>
        </td>
        </tr>
        </table>
        <hr />
        <div style="font-size: 9px; color: #CCCCCC">
        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="VEHICULOS_CERCANOS" />
        </div>
        <div style="height: 100px; overflow: auto; border: solid 1px #cccccc;">
            
            <asp:GridView ID="gridMov" runat="server" AutoGenerateColumns="false" 
                Width="100%" ShowHeader="false" GridLines="None" 
                onrowdatabound="gridMov_RowDataBound">
            <Columns>
                <asp:BoundField DataField="TipoVehiculo" ItemStyle-Font-Size="X-Small" />
                <asp:BoundField DataField="Interno" ItemStyle-Font-Bold="true" />
                <asp:BoundField DataField="Distancia" DataFormatString="{0:f2}m" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" />
            </Columns>
            </asp:GridView>
        </div>
    </div>
    </form>
</body>
</html>
