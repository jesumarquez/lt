 <%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.InfoMovil" Codebehind="InfoMovil.aspx.cs" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Ajax" Assembly="Logictracker.Web.CustomWebControls" %> <%@ Register TagPrefix="cwc" Namespace="Logictracker.Web.CustomWebControls.Buttons" Assembly="Logictracker.Web.CustomWebControls" %>

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
            .infoMovil_label
            {
                font-size: 9px; color: #CCCCCC;
            }
            .infoMovil_info
            {
                font-size: 10px; color: #666666;
            }
        </style>
        <form id="form1" runat="server">
            <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
            <asp:UpdatePanel ID="updInfoMovil" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <asp:Timer ID="timerGeocoder" runat="server" Enabled="true" Interval="100" OnTick="TimerGeocoderTick"></asp:Timer>
                    <div>
                        <table style="width: 100%">
                            <tr>
                                <td style="width: 40px;">
                                    <div>
                                        <asp:Image ID="imgDirection" runat="server" Width="16px" Height="16px" style="position: absolute; left: 0px;" />
                                        <asp:Image ID="imgTipo" runat="server" />
                                    </div>
                                </td>            
                                <td>
                                    <div style="font-size: 9px; color: #CCCCCC;">
                                        <asp:Label ID="lblTipo" runat="server" Text="" />
                                    </div>
                                    <div style="font-size: 12px; font-weight: bold;">
                                        <asp:Label ID="lblInterno" runat="server" Text="" />
                                    </div>
                                </td>
                                <td align="right" valign="bottom">                
                                    <div style="font-size: 9px; color: #CCCCCC;">
                                        <asp:Label ID="lblOdometro" runat="server" Text="" />
                                    </div>
                                    <div style="font-size: 11px; font-weight: bold;">
                                        <asp:Label ID="lblPatente" runat="server" Text="" />
                                    </div>
                                    <div style="font-size: 11px; font-weight: bold;">
                                        <asp:Label ID="lblTransportista" runat="server" Text="" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" valign="bottom"></td>
                            </tr>
                        </table>
                        <asp:Panel Id="panelTab" runat="server" style="text-align: right;" Visible="false">
                            <asp:LinkButton ID="btInfo" runat="server" CssClass="tabbutton" Enabled="false" onclick="BtInfoClick">Info</asp:LinkButton>
                            <asp:LinkButton ID="btEstado" runat="server" CssClass="tabbutton" onclick="BtEstadoClick">Estado</asp:LinkButton>
                            <asp:LinkButton ID="btRuta" runat="server" CssClass="tabbutton" onclick="BtRutaClick">Ruta</asp:LinkButton>
                        </asp:Panel>
                        <hr />
                        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                            <asp:View ID="View1" runat="server">
                                <div style="padding-left: 5px;">
                                    <div style="font-size: 9px; color: #CCCCCC">
                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" ResourceName="Entities" VariableName="PARENTI09" />:
                                        <span style="font-size: 10px; color: #666666">
                                            <asp:Label ID="lblChofer" runat="server" Text=""></asp:Label>
                                        </span>
                                    </div>
                                    <div style="height: 6px"></div>
                                    <div id="divUltimoLogin" style="font-size: 9px; color: #CCCCCC" runat="server">
                                        <cwc:ResourceLabel ID="lblRfid" runat="server" ResourceName="Labels" VariableName="ULTIMO_LOGIN" />:
                                        <span style="font-size: 10px; color: #666666">
                                            <asp:Label ID="lblLastRfid" runat="server" />
                                        </span>
                                    </div>
                                    <div style="height: 6px"></div>
                                    <div style="font-size: 9px; color: #CCCCCC">
                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Labels" VariableName="ULTIMA_POSICION" />
                                        <span style="font-size: 10px; color: #666666">
                                            <asp:Label ID="lblFechaPosicion" runat="server" Text=""></asp:Label>
                                            <br />
                                            <div style="padding-top: 5px; padding-bottom: 5px">
                                                <asp:Label ID="lblPosicion" runat="server" Text=""></asp:Label>
                                            </div>                
                                        </span>
                                    </div>
                                    <div>
                                        <span style="font-size: 9px; color: #CCCCCC">
                                            <cwc:ResourceLabel ID="lblTitVelocidad" runat="server" ResourceName="Labels" VariableName="VELOCIDAD" />
                                        </span>
                                        <asp:Label ID="lblVelocidad" runat="server" Text="0"></asp:Label> <cwc:ResourceLabel ID="lblKmH" runat="server" ResourceName="Labels" VariableName="KM_POR_HORA" />
                                    </div>
                                    <div style="height: 6px"></div>
                                    <div id="divClientes" style="font-size: 9px; color: #CCCCCC" runat="server">
                                        <cwc:ResourceLabel ID="lblTitleClientes" runat="server" ResourceName="Labels" VariableName="WORKING_FOR" />:
                                        <span style="font-size: 10px; color: #666666">
                                            <asp:Label ID="lblClientes" runat="server" Text=""></asp:Label>
                                        </span>
                                    </div>
                                    <div style="font-size: 9px; color: #CCCCCC; padding-top: 5px;">
                                        <cwc:ResourceLabel ID="ResourceLabel4" runat="server" ResourceName="Labels" VariableName="VEHICULOS_CERCANOS" />
                                        <span id="spanVerCercanos" runat="server">
                                            ( <cwc:ResourceLinkButton ID="btVerCercanos" runat="server" ResourceName="Controls" VariableName="BUTTON_VER" OnClick="BtVerCercanosClick" /> )
                                        </span>
                                    </div>
                                    <div id="divCercanos" runat="server" style="border: solid 1px #cccccc;"  Visible="false">
                                        <asp:GridView ID="gridMov" runat="server" AutoGenerateColumns="false" Width="100%" ShowHeader="false" GridLines="None">
                                            <Columns>
                                                <asp:BoundField DataField="Interno" ItemStyle-Font-Bold="true" />
                                                <asp:BoundField DataField="DistanciaKm" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:0.00}" ItemStyle-Font-Bold="true" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div style="height: 16px"></div>
                                    <div>
                                        <cwc:ResourceLinkButton ID="lnkHistorico" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_HISTORICO" />
                                    </div>
                                    <div>
                                        <cwc:ResourceLinkButton ID="lnkCalidad" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_CALIDAD" />
                                    </div>
                                </div>
                            </asp:View>
                            <asp:View ID="View2" runat="server">
                                <table style="width: 100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <div style="font-size: 9px; color: #CCCCCC">
                                                <cwc:ResourceLabel ID="ResourceLabel9" runat="server" ResourceName="Entities" VariableName="TICKET" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblTicket" runat="server" Text="" />
                                                </span>
                                            </div>
                                        </td>
                                        <td>
                                            <div style="font-size: 9px; color: #CCCCCC">
                                                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" ResourceName="Entities" VariableName="PARENTI18" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblCliente" runat="server" Text="" />
                                                </span>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <div style="height: 6px"></div>
                                <div style="font-size: 9px; color: #CCCCCC">
                                        <cwc:ResourceLabel ID="ResourceLabel7" runat="server" ResourceName="Labels" VariableName="DATE" />
                                        <span style="font-size: 10px; color: #666666">
                                            <asp:Label ID="lblFecha" runat="server" Text=""></asp:Label>
                                        </span>
                                    </div>
                                <div style="height: 6px"></div>
                                <c1:C1GridView ID="grid"  runat="server" OnRowDataBound="GridRowDataBound" SkinID="SmallGrid">
                                    <Columns>
                                        <c1:C1TemplateField HeaderText="Estado" />
                                        <c1:C1TemplateField HeaderText="Prog." >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField HeaderText="Manual" >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />    
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField HeaderText="Auto" >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </c1:C1TemplateField>
                                    </Columns>
                                </c1:C1GridView>
                                <div style="height: 6px"></div>
                                <asp:Label ID="lblDetalles" runat="server" Text="" />
                            </asp:View>
                            <asp:View ID="View3" runat="server">
                                <table style="width: 100%" cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <div style="font-size: 9px; color: #CCCCCC">
                                                <cwc:ResourceLabel ID="lblTitViaje" runat="server" ResourceName="Entities" VariableName="OPETICK03" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblViaje" runat="server" />
                                                </span>
                                                <cwc:ResourceLabel ID="lblTitEntregas" runat="server" ResourceName="Labels" VariableName="ENTREGAS" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblEntregas" runat="server" />
                                                </span>
                                                <cwc:ResourceLabel ID="lblTitRealizadas" runat="server" ResourceName="Labels" VariableName="REALIZADAS" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblRealizadas" runat="server" />
                                                </span>
                                                <asp:Label ID="lblTitPorc" runat="server" Text="%" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblPorc" runat="server" />
                                                </span>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="font-size: 9px; color: #CCCCCC">
                                                <cwc:ResourceLabel ID="lblTitAlta" runat="server" ResourceName="Labels" VariableName="ALTA" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblAlta" runat="server" />
                                                </span>
                                                <cwc:ResourceLabel ID="lblTitInicio" runat="server" ResourceName="Labels" VariableName="INICIO" Font-Bold="True" />
                                                <span style="font-size: 10px; color: #666666">
                                                    <asp:Label ID="lblInicio" runat="server" />
                                                </span>
                                                <cwc:ResourceLinkButton ID="lnkMonitorCiclo" runat="server" ResourceName="Labels" VariableName="POSICIONAR" OnClick="LnkMonitorCicloOnClick" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <div style="height: 6px"></div>
                                <c1:C1GridView ID="gridRuta" runat="server" OnRowDataBound="GridRutaRowDataBound" SkinID="SmallGrid">
                                    <Columns>
                                        <c1:C1TemplateField HeaderText="Cliente" />
                                        <c1:C1TemplateField HeaderText="Pto. Entr." />
                                        <c1:C1TemplateField HeaderText="Entrega" />
                                        <c1:C1TemplateField HeaderText="Ent." >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField HeaderText="Man." >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />    
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField HeaderText="Sal." >
                                            <ItemStyle Width="40px" HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                        </c1:C1TemplateField>
                                        <c1:C1TemplateField HeaderText="Estado" />
                                    </Columns>
                                </c1:C1GridView>
                            </asp:View>
                        </asp:MultiView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </form>
    </body>
</html>
