 <%@ Page Language="C#" AutoEventWireup="True" Inherits="Logictracker.Monitor.MonitorDeCalidad.MonitorCalidad" Codebehind="monitorDeCalidad.aspx.cs" %>

<%@ Import Namespace="Logictracker.Culture"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>
<%@ Register src="../../Operacion/Qtree/AutoGenConfig.ascx" tagname="AutoGenConfig" tagprefix="uc" %>
<%@ Register src="../../Operacion/Qtree/LevelSelector.ascx" tagname="LevelSelector" tagprefix="uc" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Monitor de Calidad</title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
</head>
<body id="monitor">    
    <script type="text/javascript" src="../../App_Scripts/jquery.min.js"></script>
    <script type="text/javascript" src="../../App_Scripts/Logictracker.Monitor.js"></script>
    <script type="text/javascript" src="Logictracker.Monitor.Calidad.js"></script>
    <script type="text/javascript">
        function gFP(item) {
            var fecha = formatDate(new Date(item.date));
            return "<table width=\"150px\"><tr><td style=\"font-weight: bold;\">" + fecha + "</td></tr></table>";
        }

        function gPP(item,last,distance,accDistance) {
            var fecha = formatDate(new Date(item.date));
            var aUltima = formatTimeSpan(last ? Math.abs((last.date - item.date) /1000) : 0);
            var diffCurso = (last ? Math.abs(item.course - last.course) : 0);
            return "<table width=\"250px\"><tr><td style=\"font-weight: bold;\">" + fecha +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "VELOCIDAD") %></u>: " + item.speed +
            "km/h</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "CURSO") %></u>: " + GetCourseDescription(item.course) + " (" + (Math.round(item.course)) +
            ")</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "TIEMPO_ULTIMA_POSICION") %></u>: " + aUltima +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "DIFERENCIA_CURSO_ULTIMA") %></u>: " + (Math.round(diffCurso)) +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "DISTANCIA_ULTIMA_POSICION") %></u>: " + (Math.round(distance*100)/100) +
            "m</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "DISTANCIA_ACUMULADA") %></u>: " + (Math.round((accDistance/1000)*100)/100) +
            "km</td></tr></table>"
            <% if(Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin)
               { 
               %>
            +"<a href=\"javascript:$get('<%=hiddenEliminar.ClientID%>').value = '"+(item.historical?"H:":"P:")+item.id+"';"
            +"<% Response.Write(ClientScript.GetPostBackEventReference(botonEliminar, ""));%>\">Borrar</a>"
            
            <%
               }%>
               ;
        }
        
        function gPPDescartada(item) {
            var fecha = formatDate(new Date(item.date));
            return "<table width=\"250px\"><tr><td style=\"font-weight: bold;\">" + fecha +
            "</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "VELOCIDAD") %></u>: " + item.speed +
            "km/h</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "CURSO") %></u>: " + GetCourseDescription(item.course) + " (" + (Math.round(item.course)) +
            ")/h</td></tr><tr><td><u><%= CultureManager.GetString("Labels", "MOTIVO_DESCARTE") %></u>: " + GetMotivoDescarte(item.motivo) + 
            "</td></tr></table>";
        }
        
        function gPOIP(a) {
            return "<table width=\"200px\"><tr><td style=\"font-weight: bold;\">" + a + "</td></tr></table>";
        }

        function gMSP(id) {
            return "<table width=\"250px\"><tr><td style=\"font-weight: bold;\">" +
            "<iframe width=\"400px\" height=\"90px\" style=\"border:none;\" src=\"../popup/MobileEventPopUp.aspx?id=" + id + "\" ></iframe>"
            + "</td></tr><tr><td>"
            <% if(Usuario.AccessLevel >= Logictracker.Types.BusinessObjects.Usuario.NivelAcceso.SysAdmin)
               { 
               %>
            +"<a href=\"javascript:$get('<%=hiddenEliminarEvento.ClientID%>').value = '"+id+"';"
            +"<% Response.Write(ClientScript.GetPostBackEventReference(botonEliminarEvento, ""));%>\">  Borrar</a>"
            
            <%
               }%>
            +"</td></tr></table>";
        }
        
        function gMSPDescartado(id) {
            return "<table width=\"250px\"><tr><td style=\"font-weight: bold;\">" +
            "<iframe width=\"400px\" height=\"90px\" style=\"border:none;\" src=\"../popup/MobileEventPopUp.aspx?id=" + id + "\" ></iframe>"
            +"</td></tr></table>";
        }
    </script>
    <form id="form1" runat="server">
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
               
        <asp:Panel ID="pnlManager" runat="server" />
        
        <cwc:ResourceLayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" ResourceName="Menu" VariableName="OPE_MON_CALIDAD" />
        
        <cwc:ResourceLayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="250" collapsible="true" split="false"
            ResourceName="Labels" VariableName="FILTROS" />
        
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" West="rgWest" Center="rgCenter" runat="server" />
        
        <asp:Panel ID="CenterPanel" runat="server">
            <div style="z-index: 99999999; width: 100%; position: absolute;">
                <%--PROGRESSLABEL--%>
                <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
            </div>
            
            <%--ERRORLABEL--%>
            <cwc:InfoLabel ID="infoLabel1" runat="server" />
            
            <mon:Monitor ID="Monitor" runat="server" />
        </asp:Panel>
        
        <asp:HiddenField ID="hiddenEliminar" runat="server" />
        <asp:Button ID="botonEliminar" runat="server" style="display:none;" OnClick="botonEliminar_Click" />
        
        <asp:HiddenField ID="hiddenEliminarEvento" runat="server" />
        <asp:Button ID="botonEliminarEvento" runat="server" style="display:none;" OnClick="botonEliminarEvento_Click" />
        
        <asp:Panel ID="WestPanel" runat="server" CssClass="filters" Height="100%" ScrollBars="Auto">
            <div class="x-panel-body">
            <table id="tblFiltros" style="font-size: x-small;width: 100%; border-spacing: 0px; padding: 0px;">
                <tr>
                    <td style="width: 95px" class="header">
                        <cwc:ResourceLabel ID="lblDistrito" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI01" />
                    </td>
                    <td>
                        <cwc:LocacionDropDownList ID="ddlDistrito" runat="server" Width="100%" />
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblPlanta" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI02" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upBase" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:PlantaDropDownList ID="ddlPlanta" runat="server" Width="100%" AutoPostBack="true" ParentControls="ddlDistrito" AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblResponsable" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="RESPONSABLE" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upResponsable" runat="server" RenderMode="Inline" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:EmpleadoDropDownList ID="ddlEmpleado" runat="server" AddAllItem="true" Width="100%" AutoPostBack="true" ParentControls="ddlDistrito,ddlPlanta" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI17" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upTipoVehiculo" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cwc:TipoDeVehiculoDropDownList ID="ddlTipoVehiculo" runat="server" Width="100%" ParentControls="ddlPlanta"
                                    AddAllItem="true" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" Font-Bold="true" ResourceName="Entities" VariableName="PARENTI03" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upMovil" runat="server">
                            <ContentTemplate>
                                <cwc:MovilDropDownList ID="ddlMovil" runat="server" Width="100%" ParentControls="ddlDistrito,ddlPlanta,ddlTipoVehiculo,ddlEmpleado" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlDistrito" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlTipoVehiculo" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlEmpleado" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblDesde" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="DESDE" />
                    </td>
                    <td>
                        <cwc:DateTimePicker runat="server" ID="dtDesde" Mode="DateTime" IsValidEmpty="False"></cwc:DateTimePicker>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblHasta" runat="server" Font-Bold="true" ResourceName="Labels" VariableName="HASTA" />
                    </td>
                    <td>
                        <cwc:DateTimePicker runat="server" ID="dtHasta" Mode="DateTime" IsValidEmpty="False"></cwc:DateTimePicker>
                        <cwc:DateTimeRangeValidator ID="dtvalidator" runat="server" StartControlID="dtDesde" EndControlID="dtHasta" MaxRange="23:59"></cwc:DateTimeRangeValidator>
                    </td>
                </tr>
                <tr>
                    <td class="header">
                        <cwc:ResourceLabel ID="lblPuntosDeInteres" runat="server" ResourceName="Entities" VariableName="PARENTI05" Font-Bold="true" />
                        <cwc:SelectAllExtender ID="SelectAllExtender1" runat="server" ListControlId="lbPuntosDeInteres" TargetControlId="lblPuntosDeInteres" />
                    </td>
                    <td>
                        <asp:UpdatePanel ID="upPuntosDeInteres" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cwc:TipoReferenciaGeograficaListBox ID="lbPuntosDeInteres" runat="server" Width="140px" ParentControls="ddlPlanta"
                                    SelectionMode="Multiple" AutoPostBack="false" Monitor="True" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlPlanta" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
            
            </div>
            
            <div class="x-panel-body" style="text-align: center; padding-top: 10px;">
            
                <asp:CheckBox ID="chkQtree" runat="server" Text="Mostrar Qtree" Visible="true" />
                
                <br />
                <br />

                <asp:UpdatePanel ID="pnlQtree" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
                    <ContentTemplate>
                        <table width="100%" id="tblVersion" runat="server">
                            <tr>
                                <td align="left">
                                    <cwc:ResourceLabel ID="lbl1" runat="server" VariableName="ARCHIVO" ResourceName="Labels" />: <asp:Label ID="lblArchivo" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <cwc:ResourceLabel ID="lbl2" runat="server" VariableName="VERSION_SERVER" ResourceName="Labels" />: <asp:Label ID="lblVersionServer" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <cwc:ResourceLabel ID="lbl3" runat="server" VariableName="VERSION_EQUIPO" ResourceName="Labels" />: <asp:Label ID="lblVersionEquipo" runat="server" />
                                </td>
                            </tr>
                        </table>

                        <br />

                        <table width="100%" id="tblEditar" runat="server">
                            <tr>
                                <td align="center">
                                    <cwc:ResourceLabel ID="lblEditarTramo" runat="server" VariableName="EDITAR_TRAMO" ResourceName="Labels" />
                                </td>
                                <td align="left">
                                    <uc:LevelSelector ID="lvlSel" runat="server" />
                                </td>
                                <td align="left">
                                    <asp:Button ID="btnGenerar" runat="server" OnClick="btnGenerarOnClick" CssClass="LogicButton_Big" Text="Editar Qtree" Width="100%" style="padding: 5px;" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <br />
                <br />

                <asp:UpdatePanel runat="server" ID="pnlLnk">
                    <ContentTemplate>
                        <cwc:ResourceLinkButton ID="lnkReporteDeEventos" runat="server" VariableName="IR_REPORTE_DE_EVENTOS" ResourceName="Labels" OnClick="LnkReporteDeEventosClick" />      
                    </ContentTemplate>
                </asp:UpdatePanel>
                      
                <br/>
                <br/>
                
                <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="btnSearch_Click" ResourceName="Controls" VariableName="BUTTON_SEARCH" />
                
            </div>
            
            <a href="../../Home.aspx">
                <div class="Logo"></div>
            </a>
        </asp:Panel>
        
        <%--HACK PARA QUE NO HAGA POSTBACK--%>
        <asp:UpdatePanel ID="upQualityMonitor" runat="server" UpdateMode="Conditional">
            <ContentTemplate/>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="botonEliminar" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="botonEliminarEvento" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
                    
        <script type="text/javascript">        
            window.onresize = function() {
                $get('<%=WestPanel.ClientID%>').style.height = getDimensions().height - 27 + 'px';
            };

            window.onresize();
        </script>
    </form>
</body>
</html>
