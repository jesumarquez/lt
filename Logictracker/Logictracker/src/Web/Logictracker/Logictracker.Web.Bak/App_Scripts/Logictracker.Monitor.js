function getDimensions() {
    var winWidth, winHeight;
    var d = document;
    if (typeof window.innerWidth != 'undefined') {
        winWidth = window.innerWidth;
        winHeight = window.innerHeight;
    } else {
        if (d.documentElement &&
                typeof d.documentElement.clientWidth != 'undefined' &&
                d.documentElement.clientWidth != 0) {
            winWidth = d.documentElement.clientWidth;
            winHeight = d.documentElement.clientHeight;
        } else {
            if (d.body &&
                    typeof d.body.clientWidth != 'undefined') {
                winWidth = d.body.clientWidth;
                winHeight = d.body.clientHeight;
            }
        }
    }
    return { width: winWidth, height: winHeight };
}

var EarthRadius = 6371000.0;
var Radians = Math.PI / 180;
function LoxodromicaConAltitud (latA, lonA, altA, latB, lonB, altB)
{
    var iniLon = lonA * Radians;
    var finLon = lonB * Radians;
    var iniLat = latA * Radians;
    var finLat = latB * Radians;
    var iniAlt = altA + EarthRadius;
    var finAlt = altB + EarthRadius;

    var xi = iniAlt * Math.cos(iniLat) * Math.sin(iniLon);
    var yi = iniAlt * Math.sin(iniLat);
    var zi = iniAlt * Math.cos(iniLat) * Math.cos(iniLon);
    var xf = finAlt * Math.cos(finLat) * Math.sin(finLon);
    var yf = finAlt * Math.sin(finLat);
    var zf = finAlt * Math.cos(finLat) * Math.cos(finLon);

    return Math.sqrt((xf - xi) * (xf - xi) + (yf - yi) * (yf - yi) + (zf - zi) * (zf - zi));
}
function formatDate(date) {
    return padStr(date.getDate()) + "/" +
          padStr(1 + date.getMonth()) + "/" +
          padStr(date.getFullYear()) + " " +
          padStr(date.getHours()) + ":" +
          padStr(date.getMinutes()) + ":" +
          padStr(date.getSeconds());
}
function formatTimeSpan(seconds) {
    var minutes = parseInt(seconds / 60, 10);
    var hours = parseInt(minutes / 60, 10);
    minutes = minutes - (hours * 60);
    seconds = seconds - (minutes * 60);
    return padStr(hours) + ":" + padStr(minutes) + ":" + padStr(seconds);
}
function padStr(i) { return (i < 10) ? "0" + i : "" + i; }

function GetCourseDescription(course) {
    if ((0.0 <= course && course < 22.5) || (337.5 <= course && course < 360.1)) return "NORTE";
    if (22.5 <= course && course < 67.5) return "NOR-ESTE";
    if (67.5 <= course && course < 112.5) return "ESTE";
    if (112.5 <= course && course < 157.5) return "SUR-ESTE";
    if (157.5 <= course && course < 202.5) return "SUR";
    if (202.5 <= course && course < 247.5) return "SUR-OESTE";
    if (247.5 <= course && course < 292.5) return "OESTE";
    if (292.5 <= course && course < 337.5) return "NOR-OESTE";
    return "INDEFINIDO";
}
function GetMotivoDescarte(motivo) {
    switch (motivo) {
        case 1: return "Sin movil asignado";
        case 2: return "Fecha invalida";
        case 3: return "Fuera del mapa";
        case 4: return "Velocidad invalida";
        case 5: return "Distancia invalida";
        case 6: return "Bajo nivel de señal";
        case 7: return "Excepcion";
        case 8: return "Descartado por Datamart";
        case 9: return "Descarte Manual";
        case 10: return "Dispositivo no asignado";
        case 11: return "Mensaje no encontrado";
        case 12: return "Dentro de inhibidor";
        case 13: return "Posiciones perdidas";
        case 14: return "Duracion Invalida";
        case 17: return "Existe una mejor posición en velocidad 0";            
        default: return "Sin Definir";
    }
}
function wasEnqueued(item) { return item.received - item.date > 5 * 60 * 1000; }
function hasValidLatLon(item) { return Math.abs(item.lat) > 0.0 && Math.abs(item.lon) > 0.0; }
function hasDuration(item) { return item.date && item.enddate;}

Array.prototype.pushPoint = function(item) { this.push(new OL.P(item.lon, item.lat)); };
Array.prototype.toLineVector = function(style) { return new OL.V(new OL.L(this), null, style); };
Array.prototype.toPolygonVector = function(style) { return new OL.V(new OL.PY(new OL.LR(this)), null, style); };

function addMarker(id, item, layer, content, image, size, offset) {
    if (!image) image = Logictracker.Monitor.Images.Posicion;
    if (!size) size = new OL.S(10, 10);
    else if (jQuery.type(size) === "number") { size = new OL.S(size, size); }
    if (!offset) offset = new OL.PX(-size.w/2, -size.h/2);
    var marker = new OL.M(new OL.LL(item.lon, item.lat), new OL.I(image, size, offset));
    Logictracker.Monitor.Mapa.aMP(id, marker, layer, content);
}
function addLine(id, points, color, layer) {
    Logictracker.Monitor.Mapa.aG(id, points.toLineVector({ strokeColor: color, strokeWidth: 4 }), layer);
}
function createBox(item) {
    var points = [];
    points.pushPoint(item);
    points.pushPoint({ lon: item.lon + item.hres, lat: item.lat });
    points.pushPoint({ lon: item.lon + item.hres, lat: item.lat - item.vres });
    points.pushPoint({ lon: item.lon, lat: item.lat - item.vres });
    points.pushPoint(item);

    return points.toPolygonVector({ strokeColor: item.color, fillColor: item.color, fillOpacity: 0.5 });
}

function addMessageWithDuration(item, type, color, layer) {
    var points = [];
    points.pushPoint(item);
    for (var i = 0; i < baseData.positions.length; i++) {
        var pos = baseData.positions[i];
        var date = new Date(pos.date);
        if (date > new Date(item.enddate)) break;
        if (date <= new Date(item.date)) continue;
        points.pushPoint(pos);
    }
    points.pushPoint({ lat: item.endlat, lon: item.endlon });
    addLine(type + "_" + item.id, points, color, layer);
}
