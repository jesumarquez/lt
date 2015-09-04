 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Monitor.MonitorDeEntidades.OperacionInfoEntidad" Codebehind="InfoEntidad.aspx.cs" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>    
</head>
<body>  
<style type="text/css">
    body
    {
        background-color: White;
    }
    .tabbutton
    {
        padding: 3px;
    }
    .infoEntidad_label
    {
        font-size: 9px; color: #CCCCCC;
    }
    .infoEntidad_info
    {
        font-size: 10px; color: #666666;
    }
    </style>
    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <asp:UpdatePanel ID="updInfoEntidad" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <div>
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 40px;">
                                <div>
                                    <asp:Image ID="imgTipo" runat="server" />
                                </div>
                            </td>
                            <td>
                                <div style="font-size: 9px; color: #CCCCCC;">
                                    <asp:Label ID="lblTipo" runat="server" Text="" />
                                </div>
                                <div style="font-size: 12px; font-weight: bold;">
                                    <asp:Label ID="lblDescripcion" runat="server" Text="" />
                                </div>
                            </td>                            
                        </tr>
                    </table>
                    <hr />
                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                        <asp:View ID="View1" runat="server">
                            <div style="padding-left: 5px;">
                                <div style="font-size: 9px; color: #CCCCCC">
                                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="ULTIMO_REPORTE" />
                                    <span style="font-size: 10px; color: #666666">
                                        <asp:Label ID="lblFechaPosicion" runat="server" Text="" />
                                        <br />
                                        <div style="padding-top: 5px; padding-bottom: 5px">
                                            <asp:Label ID="lblPosicion" runat="server" Text="" />
                                        </div>                
                                    </span>
                                </div>
                                <div style="height: 6px" />
                                <div id="divValor" style="font-size: 9px; color: #CCCCCC" runat="server">
                                    <asp:Label ID="lblValor" runat="server" Text="" style="font-size: 10px; color: #666666" />
                                </div>  
                                <br />
                                <div>
                                    <cwc:ResourceLinkButton ID="lnkMonitorSubEntidades" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_SUBENTIDADES" OnClick="LnkSubEntidadesOnClick" />
                                </div>                              
                            </div>
                        </asp:View>
                    </asp:MultiView>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
