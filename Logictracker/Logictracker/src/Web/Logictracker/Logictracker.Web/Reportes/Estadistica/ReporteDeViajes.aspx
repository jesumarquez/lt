<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Reportes.Estadistica.ReporteDeViajes" Codebehind="ReporteDeViajes.aspx.cs" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register assembly="C1.Web.C1WebChart.2" namespace="C1.Web.C1WebChart" tagprefix="C1WebChart" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Monitor de viajes</title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/default.css"/>
    <style type="text/css">
        body
        {
            overflow: hidden;
            margin: 0px;
            padding: 0px;
            border: 0px none;
            font-family: Verdana, Arial, Sans-Serif;
            font-size: 11px;
        }
        html, body
        {
            height: 100%;
        }
        form
        {
            width: 100%;
            height: 100%;
        }
    </style>
</head>
<body id="monitor">
        <form id="form1" runat="server">
            <div>
                <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
                
                <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<a href='../../'><div class='logo_online'> </div></a>" tabPosition="top"></cc1:LayoutRegion>
                <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="300" split="false" minSize="300" maxSize="300" collapsible="true" title="Filtros"></cc1:LayoutRegion>
                <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" runat="server"></cc1:LayoutManager>
                <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
                
                <asp:Panel ID="WestPanel" runat="server">
                    <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="520px">
                        <cc1:TabPanel ID="TabPanel4" runat="server" HeaderText="<img src=\'../../Operacion/LorryGreen.png\' alt=\'Vehiculos\' title=\'Vehiculos\' />" >
                            <HeaderTemplate>
                                <img alt="\'Vehiculos\'" src="Operacion/LorryGreen.png/'" 
                                    title="\'Vehiculos\'" />
                            </HeaderTemplate>
                            <ContentTemplate>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                    <ContentTemplate>                                    
                                    <%--Empresa--%>
                                        <div class="header">
                                            <cwc:ResourceLabel ID="lblEmpresa" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                        </div>
                                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="100%" OnSelectedIndexChanged="cbLocacion_SelectedIndexChanged" />
                                    <%--Linea--%>
                                        <div class="header">
                                            <cwc:ResourceLabel ID="lblLinea" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                        </div>
                                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" OnSelectedIndexChanged="cbPlanta_SelectedIndexChanged" AddAllItem="true" />
                                    <%--Tipo Vehiculo--%>
                                        <asp:Panel ID="PanelVehiculo" runat="server" CssClass="header">
                                            <cwc:ResourceLabel ID="lblVehiculo" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                        </asp:Panel>
                                        <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" OnSelectedIndexChanged="cbTipoVehiculo_SelectedIndexChanged" ParentControls="cbLinea" />
                                    <%--Vehiculo--%>
                                        <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="PanelVehiculo" ListControlId="cbVehiculo"  />
                                        <cwc:MovilListBox ID="cbVehiculo" runat="server" SelectionMode="Multiple" ParentControls="cbLinea,cbTipoVehiculo" Width="100%" Height="250px" AutoPostBack="True" OnSelectedIndexChanged="cbVehiculo_SelectedIndexChanged" UseOptionGroup="true" />
                                    <%--Fecha--%>
                                        <div class="header">
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td align="left">
                                                        <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="FECHA" ResourceName="Labels" />
                                                    </td>
                                                    <td align="left">
                                                        <cwc:DateTimePicker ID="dtFecha" runat="server" Mode="Date" TimeMode="Start" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="header">
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td align="left">
                                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="DESDE" ResourceName="Labels" />
                                                    </td>
                                                    <td align="left">
                                                        <c1:C1NumericInput ID="npHoraDesde" runat="server" Width="40px" Height="12px" NullText="0" ShowNullText="true" MinValue="0" MaxValue="23" Value="0" Increment="1" DecimalPlaces="0" />
                                                    </td>
                                                    <td align="left">
                                                        <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="HASTA" ResourceName="Labels" />
                                                    </td>
                                                    <td align="left">
                                                        <c1:C1NumericInput ID="npHoraHasta" runat="server" Width="40px" Height="12px" NullText="0" ShowNullText="true" MinValue="0" MaxValue="23" Value="0" Increment="1" DecimalPlaces="0" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td align="center">
                                                        <cwc:ResourceButton ID="btnBuscar" runat="server" VariableName="BUTTON_SEARCH" ResourceName="Controls"  OnClick="BtnBuscar_OnClick" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </ContentTemplate>                         
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </cc1:TabPanel>

                    </cc1:TabContainer>
                </asp:Panel>
                
                <asp:Panel ID="CenterPanel" runat="server" style="height: 100%; overflow: auto;">
                    <asp:UpdatePanel ID="updContent" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <br /><br /><br />
                            <table width="100%" border="0">
                                <tr>
                                    <td align="center" colspan="2">
                                        <table width="95%" border="0" id="tblContent" runat="server">
                                            <tr>
                                                <td align="center" width="10%">
                                                    &nbsp;
                                                </td>
                                                <td align="center" width="90%">
                                                    <div id="dias" runat="server" style="position: relative; height:20px;" ></div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" width="35%">
                                        <br /><br /><br /><br /><br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <table width="95%" border="2" id="tblReferencias" runat="server" visible="false">
                                            <tr>
                                                <td align="center">
                                                    <table width="100%">
                                                        <tr>
                                                            <td align="center" colspan="2" class="header">
                                                                <cwc:ResourceLabel ID="lblTit" runat="server" VariableName="REFERENCIAS" ResourceName="Labels" Font-Bold="true" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left" >
                                                                <cwc:ResourceLabel ID="ResourceLabel4" runat="server" VariableName="DETENCIONES_MAYOR_8" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center" width="40px" >
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #FFFFFF">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="DETENCIONES_MAYOR_1" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #EBE478">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="DETENCIONES_MOTOR_ENCENDIDO" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #78A2E2">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="DETENCIONES" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #CCCCCC">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" VariableName="EN_MOVIMIENTO_MAYOR_120" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #E27878">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel9" runat="server" VariableName="EN_MOVIMIENTO_MAYOR_80" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #FF8000">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>       
                                                        <tr>
                                                            <td align="left">
                                                                <cwc:ResourceLabel ID="ResourceLabel10" runat="server" VariableName="EN_MOVIMIENTO" ResourceName="Labels" />
                                                            </td>
                                                            <td align="center">
                                                                <table width="100%" border="1">
                                                                    <tr>
                                                                        <td style="background-color: #78E27D">&nbsp;</td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr> 
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td align="center" width="65%">
                                        <br /><br /><br /><br /><br />
                                        <table width="300px" border="0" runat="server"  visible="false" id="divVehiculo">
                                            <tr>
                                                <td align="center" valign="middle" style="height:30px; background-color:#EEEEEE; font-weight:bold;">
                                                    
                                                </td>
                                            </tr>
                                        </table>
                                        <C1WebChart:C1WebChart ID="C1WebChart1" runat="server" Height="200px" Width="300px" ImageRenderMethod="HttpHandler" Visible="false">
            <Serializer Value="&lt;?xml version=&quot;1.0&quot;?&gt;
&lt;Chart2DPropBag Version=&quot;2.0.20103.20251&quot;&gt;
  &lt;StyleCollection&gt;
    &lt;NamedStyle Name=&quot;Area&quot; ParentName=&quot;Area.default&quot; StyleData=&quot;Border=Solid,ControlDark,1;Rounding=10 10 10 10;&quot; /&gt;
    &lt;NamedStyle Name=&quot;LabelStyleDefault.default&quot; ParentName=&quot;Control&quot; StyleData=&quot;BackColor=Transparent;Border=None,Transparent,1;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Control&quot; ParentName=&quot;Control.default&quot; StyleData=&quot;BackColor=Window;Border=Solid,ControlDark,0;&quot; /&gt;
    &lt;NamedStyle Name=&quot;AxisY2&quot; ParentName=&quot;Area&quot; StyleData=&quot;AlignHorz=Far;AlignVert=Center;Rotation=Rotate90;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Header&quot; ParentName=&quot;Control&quot; StyleData=&quot;Border=None,Transparent,1;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Footer&quot; ParentName=&quot;Control&quot; StyleData=&quot;Border=None,Transparent,1;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Area.default&quot; ParentName=&quot;Control&quot; StyleData=&quot;AlignVert=Top;Border=None,Transparent,1;&quot; /&gt;
    &lt;NamedStyle Name=&quot;AxisY&quot; ParentName=&quot;Area&quot; StyleData=&quot;AlignHorz=Near;AlignVert=Center;ForeColor=ControlDarkDark;Rotation=Rotate270;&quot; /&gt;
    &lt;NamedStyle Name=&quot;AxisX&quot; ParentName=&quot;Area&quot; StyleData=&quot;AlignHorz=Center;AlignVert=Bottom;ForeColor=ControlDarkDark;Rotation=Rotate0;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Legend&quot; ParentName=&quot;Legend.default&quot; StyleData=&quot;AlignHorz=Center;AlignVert=Top;&quot; /&gt;
    &lt;NamedStyle Name=&quot;LabelStyleDefault&quot; ParentName=&quot;LabelStyleDefault.default&quot; /&gt;
    &lt;NamedStyle Name=&quot;PlotArea&quot; ParentName=&quot;Area&quot; StyleData=&quot;BackColor=Window;BackColor2=;Border=None,ControlText,1;GradientStyle=None;HatchStyle=None;Opaque=True;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Legend.default&quot; ParentName=&quot;Control&quot; StyleData=&quot;AlignVert=Top;Border=None,Transparent,1;Wrap=False;&quot; /&gt;
    &lt;NamedStyle Name=&quot;Control.default&quot; ParentName=&quot;&quot; StyleData=&quot;BackColor=Control;Border=None,Transparent,1;ForeColor=ControlText;&quot; /&gt;
  &lt;/StyleCollection&gt;
  &lt;ChartGroupsCollection&gt;
    &lt;ChartGroup Name=&quot;Group0&quot; ChartType=&quot;Pie&quot;&gt;
      &lt;DataSerializer DefaultSet=&quot;True&quot;&gt;
        &lt;DataSeriesCollection&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#FFFFFF&quot; /&gt;
            &lt;SymbolStyle Color=&quot;LightPink&quot; OutlineColor=&quot;DeepPink&quot; Shape=&quot;Box&quot; /&gt;
            &lt;X&gt;1&lt;/X&gt;
            &lt;Y&gt;50&lt;/Y&gt;
            &lt;DataTypes&gt;Single;Double;Double;Double;Double&lt;/DataTypes&gt;
            &lt;FillStyle HatchStyle=&quot;Cross&quot; /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#EBE478&quot; /&gt;
            &lt;SymbolStyle Color=&quot;LightBlue&quot; OutlineColor=&quot;DarkBlue&quot; Shape=&quot;Dot&quot; /&gt;
            &lt;X&gt;0&lt;/X&gt;
            &lt;Y&gt;20&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#78A2E2&quot; /&gt;
            &lt;SymbolStyle Color=&quot;LightBlue&quot; OutlineColor=&quot;DarkBlue&quot; Shape=&quot;Dot&quot; /&gt;
            &lt;X&gt;0&lt;/X&gt;
            &lt;Y&gt;10&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#CCCCCC&quot; /&gt;
            &lt;SymbolStyle Color=&quot;Cornsilk&quot; Shape=&quot;Tri&quot; /&gt;
            &lt;X&gt;1&lt;/X&gt;
            &lt;Y&gt;30&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#E27878&quot; /&gt;
            &lt;SymbolStyle Color=&quot;Crimson&quot; Shape=&quot;Diamond&quot; /&gt;
            &lt;SeriesLabel /&gt;
            &lt;X&gt;1&lt;/X&gt;
            &lt;Y&gt;0&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#FF8000&quot; /&gt;
            &lt;SymbolStyle Color=&quot;Cyan&quot; Shape=&quot;InvertedTri&quot; /&gt;
            &lt;X&gt;1&lt;/X&gt;
            &lt;Y&gt;0&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
          &lt;DataSeriesSerializer Offset=&quot;15&quot;&gt;
            &lt;LineStyle Color=&quot;#78E27D&quot; /&gt;
            &lt;SymbolStyle Color=&quot;DarkBlue&quot; Shape=&quot;Star&quot; /&gt;
            &lt;SeriesLabel /&gt;
            &lt;X&gt;1&lt;/X&gt;
            &lt;Y&gt;0&lt;/Y&gt;
            &lt;DataTypes&gt;Double&lt;/DataTypes&gt;
            &lt;FillStyle /&gt;
            &lt;Histogram /&gt;
          &lt;/DataSeriesSerializer&gt;
        &lt;/DataSeriesCollection&gt;
        &lt;Highlight /&gt;
      &lt;/DataSerializer&gt;
    &lt;/ChartGroup&gt;
    &lt;ChartGroup Name=&quot;Group2&quot;&gt;
      &lt;DataSerializer&gt;
        &lt;Highlight /&gt;
      &lt;/DataSerializer&gt;
    &lt;/ChartGroup&gt;
  &lt;/ChartGroupsCollection&gt;
  &lt;Header Compass=&quot;East&quot; Visible=&quot;False&quot;&gt;
    &lt;Text&gt;Header text&lt;/Text&gt;
  &lt;/Header&gt;
  &lt;Footer Compass=&quot;East&quot; Visible=&quot;False&quot;&gt;
    &lt;Text&gt;Footer text&lt;/Text&gt;
  &lt;/Footer&gt;
  &lt;Legend Compass=&quot;East&quot; Visible=&quot;False&quot; /&gt;
  &lt;ChartArea LocationDefault=&quot;-1, -1&quot; SizeDefault=&quot;-1, -1&quot; Depth=&quot;20&quot; Rotation=&quot;45&quot; Elevation=&quot;45&quot; PlotLocation=&quot;-1, -1&quot; PlotSize=&quot;-1, -1&quot;&gt;
    &lt;Margin /&gt;
  &lt;/ChartArea&gt;
  &lt;Axes&gt;
    &lt;Axis Max=&quot;5&quot; Min=&quot;1&quot; UnitMajor=&quot;1&quot; UnitMinor=&quot;0.5&quot; AutoMajor=&quot;True&quot; AutoMinor=&quot;True&quot; AutoMax=&quot;True&quot; AutoMin=&quot;True&quot; Compass=&quot;South&quot;&gt;
      &lt;Text&gt;Axis X&lt;/Text&gt;
      &lt;GridMajor Visible=&quot;True&quot; Spacing=&quot;1&quot; /&gt;
    &lt;/Axis&gt;
    &lt;Axis Max=&quot;25&quot; Min=&quot;5&quot; UnitMajor=&quot;5&quot; UnitMinor=&quot;2.5&quot; AutoMajor=&quot;True&quot; AutoMinor=&quot;True&quot; AutoMax=&quot;True&quot; AutoMin=&quot;True&quot; Compass=&quot;West&quot;&gt;
      &lt;Text&gt;Axis Y&lt;/Text&gt;
      &lt;GridMajor Visible=&quot;True&quot; Spacing=&quot;5&quot; /&gt;
    &lt;/Axis&gt;
    &lt;Axis Max=&quot;0&quot; Min=&quot;0&quot; UnitMajor=&quot;0&quot; UnitMinor=&quot;0&quot; AutoMajor=&quot;True&quot; AutoMinor=&quot;True&quot; AutoMax=&quot;True&quot; AutoMin=&quot;True&quot; Compass=&quot;East&quot; /&gt;
  &lt;/Axes&gt;
  &lt;AutoLabelArrangement /&gt;
  &lt;VisualEffectsData&gt;45,1,0.6,0.1,0.5,0.9,0,0,0.15,0,0,1,0.5,-25,0,0,0,1,64,1;Group0=45,1,0.6,0.1,0.5,0.9,0,0,0.15,0,0,1,0.5,-25,0,0,0,1,64,1;Group1=45,1,0.6,0.1,0.5,0.9,0,0,0.15,0,0,1,0.5,-25,0,0,0,1,64,1&lt;/VisualEffectsData&gt;
&lt;/Chart2DPropBag&gt;"></Serializer>
        </C1WebChart:C1WebChart>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td align="center">
                                        <cwc:ResourceLinkButton ID="lnkHistorico" runat="server" ResourceName="Labels" VariableName="VER_MONITOR_HISTORICO" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                            <div style="display:none;">
                                <asp:Button ID="btDummy" runat="server" OnClick="DivRoute_OnClick" />
                                <asp:HiddenField ID="hidden" runat="server" />
                                <asp:HiddenField ID="hidDesde" runat="server" />
                                <asp:HiddenField ID="hidHasta" runat="server" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </div>
        
            <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">                
                <ProgressTemplate>
                    <div id="progress" class="progress" />
                </ProgressTemplate>
            </asp:UpdateProgress>
            
        </form>
        <script type="text/javascript">
            function docb(arg, desde, hasta)
            {
                document.getElementById("<%=hidden.ClientID%>").value = arg;
                document.getElementById("<%=hidDesde.ClientID%>").value = desde;
                document.getElementById("<%=hidHasta.ClientID%>").value = hasta;
                <%=ClientScript.GetPostBackEventReference(btDummy, "")%>;
            }            
        </script>
    </body>
</html>
 