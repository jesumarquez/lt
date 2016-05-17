<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DistribucionGlobalDespachos.aspx.cs" Inherits="Logictracker.CicloLogistico.DistribucionGlobalDespachos" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
<%@ Register TagPrefix="mon" Namespace="Logictracker.Web.Monitor" Assembly="Logictracker.Web.Monitor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="~/App_Styles/openlayers.css"/>
    <style>
	    .pnlResumen {
		    height: 200px;margin:10px 10px;
		    overflow-y: hidden;
	    }
	    .pnlResumen:hover {
		     overflow-y:scroll;
	    }
    </style>
    <script type="text/javascript">
        function getEntregaP(id)
        {
            return "<iframe width=\"350\" height=\"210\" style=\"border:none;\" src=\"InfoEntrega.aspx?id=" + id + "\" />";
        }
    </script>
</head>
<body id="monitor">
    <form id="form1" runat="server">
    
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <cwc:ResourceLayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" tabPosition="top" ResourceName="Menu" VariableName="DISTRIBUCION_GLOBAL_ENTREGAS"></cwc:ResourceLayoutRegion>
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="260" split="false" minSize="260" maxSize="260" collapsible="true" title="Filtros"></cc1:LayoutRegion>
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" runat="server"></cc1:LayoutManager>
        
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server"  >
             <asp:UpdatePanel ID="updTabCompleto" runat="server" RenderMode="Inline" ChildrenAsTriggers="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="panelFiltros" DefaultButton="btnSearch">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <table id="tbFilters">
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" AutoPostBack="True" Width="100%" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:PlantaDropDownList ID="cbLinea" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa" AddAllItem="true" OnSelectedIndexChanged="CbLineaSelectedIndexChanged" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel5" runat="server" VariableName="PARENTI04" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:DepartamentoDropDownList ID="cbDepartamento" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbEmpresa,cbLinea" AddAllItem="true"  />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel8" runat="server" VariableName="PARENTI37" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:CentroDeCostosDropDownList ID="cbCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLinea,cbDepartamento" AddAllItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel6" runat="server" VariableName="PARENTI99" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:SubCentroDeCostosDropDownList ID="cbSubCentroDeCostos" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLinea,cbDepartamento,cbCentroDeCostos" AddAllItem="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            <div class="header">
                                                <cwc:ResourceLabel ID="ResourceLabel7" runat="server" VariableName="PARENTI07" ResourceName="Entities" />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:TransportistaDropDownList ID="cbTransportista" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbLinea" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th valign="top">
                                            <div class="header">
                                                <cwc:ResourceLabel ID="lblVehiculo" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                                <cwc:SelectAllExtender ID="selVehiculo" runat="server" AutoPostBack="true" TargetControlId="lblVehiculo" ListControlId="cbVehiculo"  />
                                            </div>
                                        </th>
                                        <td>
                                            <cwc:MovilListBox ID="cbVehiculo" runat="server" ParentControls="cbLinea,cbTransportista,cbDepartamento,cbCentroDeCostos,cbSubCentroDeCostos" Width="100%" AutoPostBack="True" SelectionMode="Multiple" HideWithNoDevice="True" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="header">
                                            <cwc:ResourceLabel ID="lblFecha" runat="server" ResourceName="Labels" VariableName="FECHA" />
                                        </td>
                                        <td>
                                            <cwc:DateTimePicker ID="dtFecha" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="False" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <cwc:ResourceButton ID="btnSearch" runat="server" CssClass="LogicButton_Big" ToolTip="Buscar" Width="75px" OnClick="BtnSearchClick" ResourceName="Controls" VariableName="BUTTON_SEARCH" />                
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <br/>
                                            <asp:UpdatePanel ID="upMonitorEstado" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <cwc:ResourceLinkButton ID="lnkMonitorEstado" runat="server" Visible="false" CssClass="LogicLinkButton" OnClick="LnkMonitorEstado" ResourceName="Labels" VariableName="VER_MONITOR_ESTADO" />
                                                </ContentTemplate>
                                                <Triggers>
                                                    <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                                                </Triggers>
                                            </asp:UpdatePanel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2">
                                            <br/>
                                            <a href="../">
                                                <div class="Logo"></div>
                                            </a>
                                            <br/><br/>
                                        </td>
                                    </tr>
                                </table>                                
                            </ContentTemplate>                         
                        </asp:UpdatePanel>
                    </asp:Panel>
                    <div class="pnlResumen">
                    <asp:UpdatePanel ID="pnlResumen" runat="server" UpdateMode="Conditional" Visible="True">
                        <ContentTemplate>                                                                                      
                        </ContentTemplate>                        
                    </asp:UpdatePanel>    
                    </div>                                                                            
                    
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server" >
            <mon:Monitor ID="monitorPuntos" runat="server" Width="800px" Height="500px" />
        </asp:Panel>
        
        <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
            <ProgressTemplate>
                <div id="progress" class="progress"></div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </form>
</body>
</html>
