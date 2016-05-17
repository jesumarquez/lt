function SimRecorrido(div)
{
    this.paused = true;
    var i = Function.createDelegate(this, this.init);
    var f = Function.createDelegate(this, this.fail);
    google.earth.createInstance(div, i, f);   
}
SimRecorrido.prototype.fail = function(errorCode) {};
SimRecorrido.prototype.init = function(instance)
{
    this.ge = instance;
    this.ge.getWindow().setVisibility(true);
    this.ge.getLayerRoot().enableLayerById(this.ge.LAYER_BUILDINGS, true);
    this.ge.getLayerRoot().enableLayerById(this.ge.LAYER_ROADS, true); 
    //Los edificios grises tapan todo el recorrido, asi que por ahora no van
    //this.ge.getLayerRoot().enableLayerById(this.ge.LAYER_BUILDINGS_LOW_RESOLUTION, true); 
    this.ge.getLayerRoot().enableLayerById(this.ge.LAYER_TERRAIN, true); 
    
    this.ge.getNavigationControl().setVisibility(this.ge.VISIBILITY_SHOW);
    
    this.geHelpers = new GEHelpers(this.ge);
    this.path = [];
    this.placemarks = {};
    this.on_init();
};
/**
 * Move the camera to the given location, staring straight down, and unhighlight
 * all items in the left directions list
 * @param {google.maps.LatLng} loc The location to fly the camera to
 */
SimRecorrido.prototype.flyToLatLng = function(lat, lon) 
{
  var la = this.ge.createLookAt('');
  la.set(lat, lon,
      10, // altitude
      this.ge.ALTITUDE_RELATIVE_TO_GROUND,
      90, // heading
      0, // tilt
      200 // range (inverse of zoom)
      );
  this.ge.getView().setAbstractView(la);
};

SimRecorrido.prototype.start = function(opt_cb)
{
    if (!this.simulator) 
    {
        var c = Function.createDelegate(this, function() { this.simulator.start(); if (opt_cb) opt_cb(); });
        this.reset(c);
    }
    else { this.simulator.start(); if (opt_cb) opt_cb(); }
    this.paused = false;
    this._updatePlayIndicator();
};        
SimRecorrido.prototype.reset = function(opt_cb)
{
    if (this.simulator) this.simulator.destroy();
    
    var t = Function.createDelegate(this, this.onTick);
    // create a DDSimulator object for the current DS_path array
    // on the DS_ge Earth instance
    this.simulator = new DDSimulator(this.ge, this.path, {on_tick: t});
    this.simulator.initUI(opt_cb);
    this.paused = true;
    this._updatePlayIndicator();
    this.resetEvents();
};
SimRecorrido.prototype.slower = function()
{
    if (this.simulator && this.simulator.options.speed > 0.125) 
    {
        this.simulator.options.speed /= 2.0;
        this._updateSpeedIndicator();
    }
};    
SimRecorrido.prototype.faster = function()
{
    if (this.simulator && this.simulator.options.speed < 32.0) 
    {
        this.simulator.options.speed *= 2.0;
        this._updateSpeedIndicator();
    }
};
SimRecorrido.prototype.playPause = function()
{
    //if (!this.simulator) return;
    if(!this.paused)
    {
        this.simulator.stop();
        this.paused = true;
    }
    else 
    {
        this.start();        
    }
    this._updatePlayIndicator();
};
SimRecorrido.prototype.onTick = function()
{
    this._updateRouteIndicators();
    this.showEvents();
    
};
SimRecorrido.prototype.showEvents = function(events)
{
    var curtime = this.getCurrentTime();
    for(var i = this.eventIndex; i < this.events.length; i++)
    {
        var event = this.events[i];
        if(event.time > curtime) break;
        this.addPlacemark('event' + event.id, event.lat, event.lon, event.name + '<br/>'+event.time.format('dd/MM/yyyy HH:mm:ss'), event.icon);
        
        if(!this.balloon) this.balloon = this.ge.createFeatureBalloon('currentEvent');
        this.balloon.setFeature(this.placemarks['event' + event.id]);
        this.ge.setBalloon(this.balloon);
        setTimeout(Function.createDelegate(this, function(){this.ge.setBalloon(null);}), 5000);
        this.eventIndex++;
    }
};
;
SimRecorrido.prototype.resetEvents = function()
{
    this.ge.setBalloon(null);
    if(!this.events) return;
    for(var i = 0; i < this.eventIndex; i++)
    {
        var event = this.events[i];
        this.geHelpers.removeFeature('event' + event.id);
    }
    this.eventIndex = 0;
    this.showEvents();
};
SimRecorrido.prototype.addEvents = function(events)
{
    this.resetEvents();
    this.events = events;
    this.eventIndex = 0;
};

SimRecorrido.prototype.addPlacemarks = function(pois)
{
    for (var i = 0; i < pois.length; i++)
    {
        var poi = pois[i];       
        this.addPlacemark('poi' + poi.id, poi.lat, poi.lon, poi.name, poi.icon);
    }
};
SimRecorrido.prototype.addPlacemark = function(name, lat, lon, content, icon)
{
    if(!this.placemarks[name])
    {
        var options = {id: name, 'description': content, 'standardIcon': 'red-circle'};
        if(icon) options.icon = icon;
        this.placemarks[name] = this.geHelpers.createPointPlacemark(new google.maps.LatLng(lat, lon), options);
    }
    this.ge.getFeatures().appendChild(this.placemarks[name]); 
};
SimRecorrido.prototype.createLineStringFromPath = function()
{
    var lineStringKml = '<Placemark><LineString><coordinates>\n';
      
    for (var i = 0; i < this.path.length; i++)
        lineStringKml +=    this.path[i].loc.lng().toString() + ',' +
                            this.path[i].loc.lat().toString() + ',10\n';

    lineStringKml += '</coordinates></LineString></Placemark>';
    return lineStringKml;
};
SimRecorrido.prototype.createRoutePlacemarksFromPath = function()
{
    var lineStringKml = '';
    
    var lastColor = null;
    
    var placemarks = new Array();

    for (var i = 0; i < this.path.length; i++)
    {
        if(this.path[i].color != lastColor)
        {
            if(lineStringKml != '') 
            {
                lineStringKml += '</coordinates></LineString></Placemark>';
                var pm = this.ge.parseKml(lineStringKml);
                pm.setStyleSelector(this.geHelpers.createLineStyle({width: 10, color: '88'+lastColor }));
                placemarks.push(pm);
                lineStringKml = '';
            }
            lineStringKml += '<Placemark><LineString><coordinates>\n';
        }
        lineStringKml +=    this.path[i].loc.lng().toString() + ',' +
                            this.path[i].loc.lat().toString() + ',10\n';
        lastColor = this.path[i].color;
    }
    lineStringKml += '</coordinates></LineString></Placemark>';
    var pm = this.ge.parseKml(lineStringKml);
    pm.setStyleSelector(this.geHelpers.createLineStyle({width: 10, color: '88'+lastColor }));
    placemarks.push(pm);

    return placemarks;
};
SimRecorrido.prototype.createRoute = function(positions, iconstart, iconend)
{   
    if(!positions || positions.length == 0) return;
    
    this.path = new Array();
    this.placemarks = {};

    for (var i = 0; i < positions.length; i++)
    {
        var pos = positions[i];
        var loc = new google.maps.LatLng(pos.lat, pos.lon);
        //var next = i < positions.length - 1 ? new google.maps.LatLng(positions[i+1].lat, positions[i+1].lon) : null;
        var distance = pos.speed / pos.duration;
                
        this.path.push({
                    'loc': loc,
                    'step': i,
                    'distance': pos.distance,
                    'speed': pos.speed,
                    'time': pos.time,
                    // this segment's time duration is proportional to its length in
                    // relation to the length of the step
                    'duration': pos.duration,
                    'color': pos.color
                    });
    }
    this.reset();
    this.geHelpers.clearFeatures();   

    // create the route placemark from the LineString KML blob
    /*
    var routeLineString = this.ge.parseKml(this.createLineStringFromPath());
    routeLineString.setTessellate(true);

    var routePlacemark = this.ge.createPlacemark('');
    routePlacemark.setGeometry(routeLineString);
    this.placemarks['route'] = routePlacemark;
    
    
    routePlacemark.setStyleSelector(this.geHelpers.createLineStyle({width: 10, color: '88ff0000'}));
    this.ge.getFeatures().appendChild(routePlacemark);
    */
    var placemarks = this.createRoutePlacemarksFromPath();

    for(var i = 0; i < placemarks.length; i++)
    {
        this.ge.getFeatures().appendChild(placemarks[i]);
    }
    
    var initlat = positions[0].lat;
    var initlon = positions[0].lon;
    var endlat = positions[positions.length - 1].lat;
    var endlon = positions[positions.length - 1].lon;
    // fly to the start of the route
    this.flyToLatLng(initlat, initlon);
    
    this.startDate = positions[0].time;
    
    this.addPlacemark('start', initlat, initlon, 'Inicio de Recorrido <br/>'+positions[0].time.format('dd/MM/yyyy HH:mm:ss'), iconstart);
    
    this.addPlacemark('end', endlat, endlon, 'Fin de Recorrido <br/>'+positions[positions.length - 1].time.format('dd/MM/yyyy HH:mm:ss'), iconend);
    
    this._updateSpeedIndicator();
    this._updatePlayIndicator();
    this._updateRouteIndicators();
};
SimRecorrido.prototype.goTo= function(time)
{
    if(this.path.length <= 1) return;
    this.simulator.goTo(time);
    this.resetEvents();
};
SimRecorrido.prototype.updateSpeedIndicator = function(){};
SimRecorrido.prototype._updateSpeedIndicator = function()
{
    this.updateSpeedIndicator(this.simulator.options.speed);
};
SimRecorrido.prototype.updatePlayIndicator = function(){};
SimRecorrido.prototype._updatePlayIndicator = function()
{
    this.updatePlayIndicator(!this.paused);
};
SimRecorrido.prototype.updateCarSpeedIndicator = function(){};
SimRecorrido.prototype.updateDistanceIndicator = function(){};
SimRecorrido.prototype.updateTimeIndicator = function(){};
SimRecorrido.prototype._updateRouteIndicators = function()
{
    this.updateCarSpeedIndicator(this.simulator.currentSpeed);
    this.updateDistanceIndicator(this.simulator.totalDistance);
    
    
    this.updateTimeIndicator(this.getCurrentTime());
    
};
SimRecorrido.prototype.getCurrentTime = function()
{
    var time = new Date();
    if(this.simulator && this.simulator.totalDuration)
        time.setTime(this.startDate.getTime() + (this.simulator.totalDuration *1000));
    else time = this.startDate;
    return time;
};
SimRecorrido.prototype.on_init = function(){};

var simulador;
function init()
{
    simulador = new SimRecorrido('map3d');
    simulador.updateSpeedIndicator = function(speed) { $get('simulator_speed').innerHTML = 'x'+speed; };
    simulador.updatePlayIndicator = function(playing) { $get('btplay').className = simulador.paused ? 'simulator_button_play': 'simulator_button_pause';};
    simulador.updateCarSpeedIndicator = function(speed) { $get('speed').innerHTML = Math.round(speed); };
    simulador.updateDistanceIndicator = function(distance) { $get('distance').innerHTML = parseInt(distance,10)/1000; };
    simulador.updateTimeIndicator = function(time) 
    { 
        $get('time').innerHTML = formatTime(time); 
        if(simulador.path.length <= 1) return;
        var init = simulador.path[0];
        var end = simulador.path[simulador.path.length - 1];
        var totalDuration = (end.time.getTime() - init.time.getTime()) / 1000;
        var partialDuration = (time.getTime() - init.time.getTime()) / 1000;
        var width = Math.round(100 * (partialDuration / totalDuration));
        $get('timebar').style.width = width + '%';
    };
    simulador.on_init = function() { callServer(); };
    
    $addHandler($get('backtimebar'), 'click', function(e)
    {
//    var s = '';
//        for(w in e)
//        s += w +'+ '+ e[w] + '\n';
//        alert(s);
        if(simulador.path.length <= 1) return;
        var init = simulador.path[0];
        var end = simulador.path[simulador.path.length - 1];
        var totalDuration = (end.time.getTime() - init.time.getTime()) / 1000;
        var partialDuration = totalDuration * (e.offsetX / 200);
        simulador.goTo(partialDuration);
        var time = new Date();
        time.setTime(init.time.getTime() + (partialDuration * 1000));
        simulador.updateTimeIndicator(time);
    });
}
/**
 * Formats a time given in seconds to a human readable format
 * @param {number} s Time in seconds
 * @return {string} A string formatted in hh:mm form representing the given
 *     number of seconds
 */
function formatTime(time) {
  var h = time.getHours();
  var m = time.getMinutes();
  var s = time.getSeconds();
  return ((h < 10) ? ('0' + h) : h) + ':' + ((m < 10) ? ('0' + m) : m)+ ':' + ((s < 10) ? ('0' + s) : s);
}

function getManualMove()
{
    return $get('manualMove').checked;
}
google.load("earth", "1.x");
google.setOnLoadCallback(init);

