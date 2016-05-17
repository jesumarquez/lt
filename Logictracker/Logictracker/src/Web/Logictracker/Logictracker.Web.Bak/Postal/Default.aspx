<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Logictracker.Postal.Postal_Default" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Logictracker.Configuration" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Correo Privado - Servicio de Mensajer&iacute;a Argentino - Log&iacute;stica,
        Mensajer&iacute;a</title>
    
    <link href="style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
    body
    {
        font-family: Verdana, Arial, Sans-Serif;
    }
    </style>
</head>
<body>

    <script src="<%=Config.Map.GoogleMapsKey %>" type="text/javascript"></script>

    <script type="text/javascript" src="OpenLayers.js"></script>

    <script type="text/javascript">
				var map;

				var displayProjection = new OpenLayers.Projection("EPSG:4326");
				var projection= new OpenLayers.Projection("EPSG:900913");
				var maxExtent = new OpenLayers.Bounds(-20037508/2,-20037508/2,-20037508/4,-20037508/8);
				var mark;

				function init()
				{
				    <% if(Route == null) { %>
				    
				        alert('No se encuentra el numero de Pieza');
				    	return;		
				    <% } else { %>
				    
				    <% if(!Route.Latitud.HasValue) { %>
				        alert('La Pieza ingresada no ha sido rendida aun');
				    	return;	
				    <% } else { %>
				
					map = new OpenLayers.Map('divmapa',
				            {
				                //'numZoomLevels': 1,
				                //'minZoomLevel': 16,
				                'units': 'm',
				                'controls': [],
				                'displayProjection': displayProjection,
				                'projection': projection,
				                'maxExtent': maxExtent
				            });
				            
				    var gStr = new OpenLayers.Layer.Google('Calles', {minZoomLevel: 1, 'sphericalMercator': true});	
			        var gHyb = new OpenLayers.Layer.Google('Híbrido', {type: G_HYBRID_MAP, minZoomLevel: 1, 'sphericalMercator': true});
			        var gSat = new OpenLayers.Layer.Google('Satelital', {type: G_SATELLITE_MAP, minZoomLevel: 1, 'sphericalMercator': true});

					mark = new OpenLayers.Layer.Markers("Icono");

					map.addLayers(new Array(gStr, gHyb, gSat, mark));
					
					

                    var center = new OpenLayers.LonLat(<%=Route.Longitud.Value.ToString(CultureInfo.InvariantCulture) %>,<%=Route.Latitud.Value.ToString(CultureInfo.InvariantCulture) %>).transform(displayProjection, projection);
                    
				    map.setCenter(center, 15);
				    
				    var codigo = '<%=Request.QueryString["i"] %>';
				    var image = 'Image.ashx?i=<%=Request.QueryString["i"] %>';
				    var fecha = '<%=Route.FechaFoto.HasValue ? Route.FechaFoto.Value.ToString("dd/MM/yyyy HH:mm"):"" %>';
				    
				    var size = new OpenLayers.Size(20, 34);
					var offset = new OpenLayers.Pixel(-10, -33);
					var icon = new OpenLayers.Icon('pinpoint2.png', size, offset);

					var feature = new OpenLayers.Feature(mark, center, {'icon': icon});
					feature.closeBox = true;
					feature.popupClass = OpenLayers.Class(OpenLayers.Popup.FramedCloud, {'autoSize': true, 'maxSize': new OpenLayers.Size(500,400), 'minSize': new OpenLayers.Size(100,100) });

					feature.data.popupContentHTML = '<table style="width: 100%;font-size: 10px; text-align: center;"><tr><td>';
					feature.data.popupContentHTML += '<b>' + codigo + '</b>';
					
					feature.data.popupContentHTML += '</td></tr><tr><td>';
					
					feature.data.popupContentHTML += '<a href="' + image + '" target="_blank"><img src="' + image + '" width="100" border="0" /></a>';
					
					feature.data.popupContentHTML += '</td></tr><tr><td>';
					
					feature.data.popupContentHTML += fecha + '<br/>';
					
					feature.data.popupContentHTML += '(<%=Route.Latitud.Value.ToString(CultureInfo.InvariantCulture) %>, <%=Route.Longitud.Value.ToString(CultureInfo.InvariantCulture) %>)';
									
					feature.data.popupContentHTML += '</td></tr></table>';
					feature.data.overflow = "hidden";

					var marker = feature.createMarker();

					var markerClick = function (evt) {
						if (this.popup == null) {
							this.popup = this.createPopup(this.closeBox);
							map.addPopup(this.popup);
							this.popup.show();
						} else {
							this.popup.toggle();
						}
						currentPopup = this.popup;
						elPopup = this.popup;
						OpenLayers.Event.stop(evt);
					};
					marker.events.register("mousedown", feature, markerClick);

					mark.addMarker(marker);

				    
				    <% } } %>
				    
				}
			
			

    </script>

    <form id="form1" runat="server">
    <div>
    
        <div id="divmapa" style="width: 600px; height: 400px; border: solid: 1px black; margin: auto;">
        </div>
        
        <script type="text/javascript">
            init();
        </script>
        
    </div>
    </form>
</body>
</html>
