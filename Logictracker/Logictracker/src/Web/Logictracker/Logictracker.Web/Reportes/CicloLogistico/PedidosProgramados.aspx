<%@ Page Language="C#" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="PedidosProgramados.aspx.cs" Inherits="Logictracker.Reportes.CicloLogistico.PedidosProgramados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <style type="text/css">
    #tabla_programacion
    {
        width: 100%; 
        background-color: #CCC; 
        border-spacing: 1px;
    }
    .Grid_Header
    {
        text-align: center;
        font-size: 9px;
        padding: 3px;
    }
    .Grid_Alternating_Item
    {
        cursor: default;
    }
    .hora
    {
        font-size: 10px;
    }    
    #tabla_programacion tr.pendiente
    {
    }
    #tabla_programacion tr.generado
    {
        background-color: #e7e7e7;
        color: #999;
    }
    #tabla_programacion td.text
    {

    }
    #tabla_programacion td.button
    {
        font-size: 9px;
        text-align: center;
        color: #999;
    }
    #tabla_programacion td.center
    {
        text-align: center;
    }
    #tabla_programacion td.number
    {
        text-align: right;
    }
    #tabla_programacion td.cantidad
    {
        background-color: #CCFFCC;
    }
    #tabla_programacion td.cantidadtotal
    {
        background-color: #BBEEBB;
        font-weight: bold;
    }
    #tabla_programacion td.total
    {
        background-color: #e7e7e7;
        font-weight: bold;
    }
    .separator td, .separator th
    {
        border-top: solid 2px #CCC;
    }
    .footer th
    {
        font-size: 9px;
        background-color: #e7e7e7; 
        text-align: right;
    }
    .footer td
    {
        background-color: #f0f0f0; 
    }
    .footer td.total_ok
    {
        background-color: #CCFFCC; 
    }
    .footer td.total_mal
    {
        background-color: #FFCCCC; 
    }
    .limited_size_small
    {
        width: 50px;
    }
    .limited_size_medium
    {
        width: 80px;
        overflow: hidden;
    }
    .popup
    {
        position: absolute;
        border: solid 1px #ccc;
        background-color: #f0f0f0;
        z-index: 9999999;
        padding: 3px;
    }
    .clickable
    {
        cursor: pointer;
    }
    .clickable:hover
    {
        color: #999;
        font-weight: bold;
    }
    #btShowGraph
    {
        cursor: pointer;
        padding: 2px;
        width: 120px;
        border: solid 1px #c3c3c3;
        border-top: none;
        background-color: #e7e7e7;
        font-size: 10px;
        font-weight: bold;
        color: #999;
        text-align: center;
        position: relative;
        bottom: 1px;
        margin-bottom: 2px;
    }
    #btShowGraph:hover
    {
        color: #c39636;
    }
    #divGrafico
    {
        display: none;
    }
    </style>
    <script type="text/javascript">
    var currentPopup = null;
    function showpopup(elem)
    {   
        for(var i = 0; i < elem.childNodes.length; i++)
        {
            var node = elem.childNodes[i];;
            if(!node.getAttribute) continue;
            var att = node.getAttribute('estado')
            if(att != null && att.Value == '1') return;
            if(att != null && att.Value == '0') break;
        }
        var child = null;
        for(var i = 0; i < elem.parentNode.childNodes.length; i++)
            if((child = elem.parentNode.childNodes[i]).className == 'popup') break;
        
        var show = child.style.display == 'none';
        
        child.style.display = show ? 'block' : 'none';
        
        if(currentPopup != null && currentPopup != child) 
            currentPopup.style.display = 'none';
            
        currentPopup = show ? child : null;
    }
    function showGraph()
    {
        var divProgramacion = document.getElementById('divProgramacion');
        var divGrafico = document.getElementById('divGrafico');
        var btShowGraph = document.getElementById('btShowGraph');
        
        var show = divGrafico.style.display == 'none';        
        divProgramacion.style.display = show ? 'none' : 'block';
        divGrafico.style.display = show ? 'block' : 'none';
        btShowGraph.innerHTML = show 
            ? "<asp:Literal ID="litTabProg" runat="server" />" 
            : "<asp:Literal ID="litTabGraph" runat="server" />";
    }
    </script>
    
    <%--TOOLBAR--%>
    <cwc:ToolBar ID="ToolBar1" runat="server" ResourceName="Menu" VariableName="CLOG_PROGRAMACION" />
       
    <%--FILTROS--%>
    <div class="filterpanel">
        <asp:UpdatePanel ID="updFilters" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>   
       
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <%--EMPRESA--%>
                                        <cwc:ResourceLabel ID="lblEmpresa" runat="server" ResourceName="Entities" VariableName="PARENTI01"/>
                                        <br />
                                        <cwc:LocacionDropDownList ID="cbEmpresa" runat="server" Width="200px" AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <%--LINEA--%>
                                        <cwc:ResourceLabel ID="lblLinea" runat="server" ResourceName="Entities" VariableName="PARENTI02"/>
                                        <br />
                                        <cwc:PlantaDropDownList ID="cbLinea" runat="server" Width="200px" ParentControls="cbEmpresa" AutoPostBack="true" />
                                    </td>
                                    <td>
                                        <%--BOCA DE CARGA--%>
                                        <cwc:ResourceLabel ID="lblBocaDeCarga" runat="server" ResourceName="Entities" VariableName="BOCADECARGA"/>
                                        <br />
                                        <cwc:BocaDeCargaDropDownList ID="cbBocaDeCarga" runat="server" Width="200px" ParentControls="cbLinea" />
                                    </td>                                    
                                </tr>
                                <tr>
                                    <td>
                                        <%--PRODUCTO--%>
                                        <cwc:ResourceLabel ID="ResourceLabel2" runat="server" ResourceName="Entities" VariableName="PARENTI63"/>
                                        <br />
                                        <cwc:ProductoDropDownList ID="cbProducto" runat="server" Width="200px" ParentControls="cbLinea" AddAllItem="true" />
                                    </td>
                                    <td>
                                        <%--DESDE--%>
                                        <cwc:ResourceLabel ID="lblDia" runat="server" ResourceName="Labels" VariableName="DIA"/>
                                        <br />
                                        <cwc:DateTimePicker ID="dtDia" runat="server" Mode="Date" TimeMode="Start" IsValidEmpty="true" />
                                    </td>
                                    <td rowspan="2" style="color: #999999; width: 100px; text-align: right; vertical-align: bottom;">
                                        <cwc:ResourceButton ID="btBuscar" runat="server" ResourceName="Controls" VariableName="BUTTON_SEARCH" OnClick="BtBuscarClick" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    
    <%--PROGRESSLABEL--%>
    <cwc:ProgressLabel ID="ProgressLabel1" runat="server" />
    
    <%--ERRORLABEL--%>
    <cwc:InfoLabel ID="infoLabel1" runat="server" />
         
    <%--RESULTADO--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
    <ContentTemplate> 
    
    <asp:Panel ID="panelResultado" runat="server" Visible="false">
        
    <div id="btShowGraph" onclick="showGraph();">
    Grafico
    </div>
    <div id="divProgramacion">
        <asp:Repeater ID="repPedidos" runat="server" OnItemDataBound="RepPedidosItemDataBound" >
            <HeaderTemplate>
                <table id="tabla_programacion">
                    <tr class="Grid_Header">
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblPedido" runat="server" ResourceName="Labels" VariableName="CODE" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblHoraCarga" runat="server" ResourceName="Labels" VariableName="HORA_CARGA" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblEnObra" runat="server" ResourceName="Labels" VariableName="FECHA_EN_OBRA" /></th>
                        <th class="limited_size_medium"><cwc:ResourceLabel ID="lblCliente" runat="server" ResourceName="Entities" VariableName="CLIENT" /></th>
                        <th class="limited_size_medium"><cwc:ResourceLabel ID="lblPuntoEntrega" runat="server" ResourceName="Entities" VariableName="PARENTI44" /></th>
                        <th class="limited_size_medium"><cwc:ResourceLabel ID="lblContacto" runat="server" ResourceName="Labels" VariableName="CONTACTO" /></th>
                        <th class="limited_size_medium"><cwc:ResourceLabel ID="lblObservacion" runat="server" ResourceName="Labels" VariableName="OBSERVACION" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblBomba" runat="server" ResourceName="Labels" VariableName="NUMERO_BOMBA" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblEsMinimixer" runat="server" ResourceName="Labels" VariableName="MULTIPLES_REMITOS" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblProducto" runat="server" ResourceName="Labels" VariableName="PRODUCTO" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblCantidad" runat="server" ResourceName="Labels" VariableName="CANTIDAD_PEDIDO" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblTiempoCiclo" runat="server" ResourceName="Labels" VariableName="PEDIDO_TIEMPO_CICLO" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblFrecuencia" runat="server" ResourceName="Labels" VariableName="PEDIDO_FRECUENCIA_ENTREGA" /></th>
                        <th class="limited_size_small"><cwc:ResourceLabel ID="lblMovilesNecesarios" runat="server" ResourceName="Labels" VariableName="PROGRAMACION_MOVILES_NECESARIOS" /></th>
                        <asp:Literal ID="litHoras" runat="server" />
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class='Grid_Alternating_Item <asp:Literal ID="litStyle" runat="server" />'>
                    <td class="clickable">
                        <asp:LinkButton ID="lblPedido" runat="server" OnClick="LblPedido_OnClick" ForeColor="Black" />
                        <asp:HiddenField ID="hidId" runat="server" />
                    </td>
                    <td class="number"><asp:Label ID="lblHoraCarga" runat="server" /></td>
                    <td class="number"><asp:Label ID="lblEnObra" runat="server" /></td>
                    <td class="text"><asp:Label ID="lblCliente" runat="server" /></td>
                    <td class="text"><asp:Label ID="lblPuntoEntrega" runat="server" /></td>
                    <td class="text"><asp:Label ID="lblContacto" runat="server" /></td>
                    <td class="text"><asp:Label ID="lblObservacion" runat="server" /></td>
                    <td class="text center"><asp:Label ID="lblBomba" runat="server" /></td>
                    <td class="text center"><asp:Label ID="lblEsMinimixer" runat="server" /></td>
                    <td class="text center"><asp:Label ID="lblProducto" runat="server" /></td>
                    <td class="number cantidad"><asp:Label ID="lblCantidad" runat="server" /></td>
                    <td class="number"><asp:Label ID="lblTiempoCiclo" runat="server" /></td>
                    <td class="number"><asp:Label ID="lblFrecuencia" runat="server" /></td>
                    <td class="number"><asp:Label ID="lblMovilesNecesarios" runat="server" /></td>
                    <asp:Literal ID="litHoras" runat="server" />                    
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr class="Grid_Alternating_Item footer separator">                    
                    <th colspan="10"><cwc:ResourceLabel ID="lblCantidadTotal" runat="server" ResourceName="Labels" VariableName="TOTAL" /></th>
                    <td class="number cantidadtotal"><asp:Label ID="lblCantidad" runat="server" /></td>
                    <td class="number total"><asp:Label ID="lblTiempoCiclo" runat="server" /></tdclass>
                    <th colspan="2">
                    <cwc:ResourceLabel ID="lblVolumenHora" runat="server" ResourceName="Labels" VariableName="PROGRAMACION_VOLUMEN_HORA" /></th>
                    <asp:Literal ID="litHoras" runat="server" />
                </tr>
                <tr class="Grid_Alternating_Item footer">
                    <td colspan="12"></td>
                    <th colspan="2">
                    <cwc:ResourceLabel ID="lblMixersHora" runat="server" ResourceName="Labels" VariableName="PROGRAMACION_MIXERS_HORA" /></th>
                    <asp:Literal ID="litHoras2" runat="server" />
                </tr>
                <tr class="Grid_Alternating_Item footer">
                    <td colspan="12"></td>
                    <th colspan="2">
                    <cwc:ResourceLabel ID="lblMixersEquivalentes" runat="server" ResourceName="Labels" VariableName="PROGRAMACION_MIXERS_EQUIVALENTES" /></th>
                    <asp:Literal ID="litHoras3" runat="server" />
                </tr>
                </table>  
            </FooterTemplate>
        </asp:Repeater> 
        
        </div>
        
        <div id="divGrafico" style="display: none;">
            <table width="100%">
                <tr align="center">
                    <td>
                        <div id="divGraph" runat="server">
                            <asp:Literal ID="litGraph" runat="server" />
                        </div>
                        <script type="text/javascript" src="../FusionCharts/FusionCharts.js"></script>   
                    </td>
                </tr>
            </table>    
        </div>       
        
        <cwc:SizeHiddenField ID="sizeField" runat="server" />
                
        </asp:Panel>        
      </ContentTemplate>
      <Triggers>
      <asp:AsyncPostBackTrigger ControlID="btBuscar" EventName="Click" />
      </Triggers>
    </asp:UpdatePanel>
    
</asp:Content>

