<%@ Page Language="C#" AutoEventWireup="true" Inherits="Logictracker.Operacion.Consola.ConsolaEntregas" Codebehind="ConsolaEntregas.aspx.cs" %>

<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Consola de Eventos</title>
    <script type="text/javascript">
    function  getDimensions(){
        var winWidth ,winHeight ;
        var d=document;
        if (typeof window.innerWidth!='undefined') {
            winWidth = window.innerWidth;
            winHeight = window.innerHeight;
        } else {
            if (d.documentElement && 
                typeof d.documentElement.clientWidth!='undefined' && 
                d.documentElement.clientWidth!=0) {
                winWidth = d.documentElement.clientWidth;
                winHeight = d.documentElement.clientHeight;
             } else {
                if (d.body && 
                    typeof d.body.clientWidth!='undefined') {
                    winWidth = d.body.clientWidth;
                    winHeight = d.body.clientHeight;
                }
            }
        }
        return {width: winWidth, height: winHeight};
    }
    function Resize()
    {
        var size = getDimensions();
        var div = $get('progress');
        div.style.width = size.width + "px";
        div.style.height = size.height + "px";
        
        var div = $get('consola_content');
        div.style.height = (size.height - 80) + "px";    
        div.style.overflow = 'auto';    
    }
    </script>

</head>
<body id="monitor">
    <form id="form1" runat="server">
    <div>
        <cwc:CustomScriptManager ID="CustomScriptManager1" runat="server" />
        <asp:UpdatePanel ID="updSerialized" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:HiddenField ID="hidSerialized" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <cc1:LayoutRegion ID="rgCenter" TargetControlID="CenterPanel" runat="server" initialSize="200" minSize="200" maxSize="500" title="<a href='../../'><div class='logo_consola'> </div></a>" tabPosition="top" />
        <cc1:LayoutRegion ID="rgWest" TargetControlID="WestPanel" runat="server" initialSize="300" split="false" minSize="300" maxSize="300" collapsible="true" title="Filtros" />
        <cc1:LayoutRegion ID="rgSouth" TargetControlID="SouthPanel" runat="server" initialSize="20" split="false" />
        <cc1:LayoutManager ID="rgManager" TargetControlId="pnlManager" Center="rgCenter" West="rgWest" East="rgEast" South="rgSouth" runat="server" />
        
        <asp:Panel ID="pnlManager" runat="server"></asp:Panel>
        
        <asp:Panel ID="WestPanel" runat="server">
            <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="520px" >
                <cc1:TabPanel ID="TabPanel1" runat="server" HeaderText="Vehiculos" >
                    <ContentTemplate>
                        <asp:UpdatePanel ID="updFiltros" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                <div class="header">
                                    <cwc:ResourceLabel ID="ResourceLabel3" runat="server" VariableName="PARENTI01" ResourceName="Entities" />
                                </div>
                                <cwc:LocacionDropDownList ID="cbLocacion" runat="server" AutoPostBack="True" Width="100%" AddAllItem="false" />
                                
                                <div class="header">
                                    <cwc:ResourceLabel ID="ResourceLabel1" runat="server" VariableName="PARENTI02" ResourceName="Entities" />
                                </div>
                                <cwc:PlantaDropDownList ID="cbPlanta" runat="server" AutoPostBack="true" Width="100%" ParentControls="cbLocacion" OnSelectedIndexChanged="CbPlantaSelectedIndexChanged" AddAllItem="true" />
                                
                                <asp:Panel ID="PanelVehiculo" runat="server" CssClass="header">
                                    <cwc:ResourceLabel ID="ResourceLabel2" runat="server" VariableName="PARENTI03" ResourceName="Entities" />
                                </asp:Panel>
                                <cwc:TipoDeVehiculoDropDownList ID="cbTipoVehiculo" runat="server" AutoPostBack="true" Width="100%" AddAllItem="true" ParentControls="cbPlanta" OnSelectedIndexChanged="CbTipoVehiculoSelectedIndexChanged" />
                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="updVehiculos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>
                                <cwc:MovilListBox ID="cbVehiculo" runat="server" SelectionMode="Multiple" Height="350px" UseOptionGroup="true" ParentControls="cbPlanta,cbTipoVehiculo" HideWithNoDevice="True" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="CbVehiculoSelectedIndexChanged" />  
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="cbLocacion" EventName="SelectedindexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbPlanta" EventName="SelectedindexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="cbTipoVehiculo" EventName="SelectedindexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </ContentTemplate>
                </cc1:TabPanel>
            </cc1:TabContainer>
        </asp:Panel>
        
        <asp:Panel ID="CenterPanel" runat="server">
            <div class="header">
                <table style="width: 100%; font-weight: bold;">
                    <tbody>
                        <tr>
                            <td style="width: 100px; align: left;">Fecha</td>
                            <td style="width: 90px; align: left;">Vehículo</td>
                            <td style="width: 120px; align: left;">Ruta</td>
                            <td style="width: 80px; align: left;">Entrega</td>
                            <td style="width: 80px; align: left;">Estado</td>
                            <td style="width: 80px; align: left;">Próxima</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div id="consola_content">
                <div id="consola_entregas"></div>
            </div>
        </asp:Panel>
        <asp:Panel ID="SouthPanel" runat="server" CssClass="bar" style="background-color: #666666; height: 20px;padding-top: 0px;">
            <div id='clock' style='width: auto;color: White;float: right; font-size: 14px; font-weight: bold; padding-right: 10px;padding-left: 10px; background-color: #666666; border-left: solid 1px #999999; border-right: solid 1px #999999;'>00:00</div>
            <div id="div_mensajes"></div>
        </asp:Panel>
    </div>
    <asp:UpdateProgress ID="uppProgress" runat="server" DisplayAfter="0">
        <ProgressTemplate>
            <div id="progress" class="progress">
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <script type="text/javascript">
            $addHandler(window, 'resize', Resize);
            Resize();
    </script>
   
    <script type="text/javascript">
    /* BEGIN MESSAGES FUNCTIONS */
    var lastId=0;
    function setStatus(texto) {document.getElementById('div_mensajes').innerHTML = texto;}
    function refresh(auto)
    {
        setStatus("Actualizando Informaci&oacute;n...");
        CallServer((auto?'AUTO:':'') + lastId + ':' + document.getElementById('<% Response.Write(hidSerialized.ClientID); %>').value,'');
    }
    function addMessages(msg, auto, maxId)
    {
        var max = <% Response.Write(MaxResults); %>;
        var messagePlace = document.getElementById('consola_entregas');
        if(!auto) messagePlace.innerHTML = '';
        
        if(!msg || msg.length == 0) 
        {
            setStatus("No se encontraron nuevos mensajes");
            delete msg;
            return;
        }  
        
        if(!auto)
        {
            setStatus("Listo");
            messagePlace.innerHTML = msg;
        }
        else
        {
            setStatus(msg.length+" nuevos mensajes");
            var rows = messagePlace.getElementsByTagName('table');
            var count = rows.length + msg.length;
            var topulse = new Array();
            
            for(var i = max; i < count; i++) try{ messagePlace.removeChild(messagePlace.childNodes[messagePlace.childNodes.length - 1]); }catch(e){}
            
            var div = document.createElement('div');
            for(var i = 0; i < msg.length; i++)
            {
                var rowData = msg[i];
                div.innerHTML = rowData;
                var elem = div.getElementsByTagName('table')[0];
                topulse.push(elem);
                
                var tr = null;
                
                for(var j = 0; j < rows.length; j++)
                {
                    if(rows[j].getAttribute("o") < elem.getAttribute("o"))
                    {
                        tr = rows[j];
                        break;
                    }
                }
                
                if(tr != null) messagePlace.insertBefore(elem, tr);
                else messagePlace.appendChild(elem);
            }  
            
            try { colorfade(topulse); } catch (e) { }                 
        }
        lastId = maxId;

        delete msg;
    }    
    
    var frame=50;
    function colorfade(elems) 
    {        
        if(frame>0) 
        {
            var per = 100 - (frame * 2);
            var op =  per/100;
            
            for(var i = 0; i < elems.length; i++)
            {
                var element = elems[i];
                element.style['opacity'] = op;	
	            element.style['-moz-opacity'] = op;	
	            element.style['filter'] = 'alpha(opacity='+per+')';
	        }
            frame--;
            setTimeout(colorfade,20, elems);	
        }
        else
        {
            for(var i = 0; i < elems.length; i++)
            {
                var element = elems[i];
                element.style['opacity'] = null;	
	            element.style['-moz-opacity'] = null;
	            element.style['filter'] = null;
	        }
            frame=50;
        }   
    }
    /* END MESSAGES FUNCTIONS */
    
    /* BEGIN EVENT FUNCTIONS */
    function ShowEvents(){ set_eventPanelDisplay(false); }
    function set_eventPanelDisplay(show) 
    { 
        $get('panelPopup').style.display = show ? '' : 'none'; 
    }
    function s(id){showEvent(id);}
    function showEvent(id)
    { 
        if(cancelShow)
        {
            cancelShow = false;
            return;
        }
        $get('ifrPopupDetail').src = '../InfoEvent.aspx?evt=' + id; 
        set_eventPanelDisplay(true); 
    }
    /* END EVENT FUNCTIONS */
    
    /* BEGIN FOTO FUNCTIONS */
    var cancelShow = false;
    function showFoto(id)
    {
        cancelShow = true;
        window.open('../../Common/Pictures/Default.aspx?e='+id, "Fotos_"+id, "width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no");
    }
    /* END FOTO FUNCTIONS */
    
    /* BEGIN CLOCK FUNCTIONS */
    var serverTime = new Date();
    var clockInterval = 10;
    function startClock(datetime)
    {
        serverTime.setTime(datetime.getTime()+5000);
        setInterval(clockTick, clockInterval * 1000);
    }
    function clockTick()
    {
        serverTime.setTime(serverTime.getTime()+(clockInterval * 1000));
        $get('clock').innerHTML = PadLeft(serverTime.getHours(), 2,'0') + '<blink>:</blink>' + PadLeft(serverTime.getMinutes(), 2, '0');
    }
    function PadLeft(str, len, ch)
    {
        var str2 = str+'';
        while(str2.length < len) str2 = ch + str2;
        return str2;
    }
    /* END CLOCK FUNCTIONS */
    
    setInterval(refresh, <% Response.Write(TimerInterval); %> * 1000, true);

    </script>
    
    </form>
</body>
</html>
 