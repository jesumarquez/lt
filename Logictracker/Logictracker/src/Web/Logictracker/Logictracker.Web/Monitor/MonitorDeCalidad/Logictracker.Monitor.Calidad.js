function clearLayers() {
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.PosicionesReportadas);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.PosicionesDescartadas);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.Recorrido);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.RecorridoDescartado);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.Eventos);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.EventosDescartados);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.EventosDuracion);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.EventosDuracionDescartados);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.PuntosDeInteres);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.Geocercas);
    Logictracker.Monitor.Mapa.clearLayer(Logictracker.Monitor.Layers.Qtree);
}
var color = { normal: "#3399CC", encolada: "#FFCC66", descartada: "#FF66FF", infraccion: "#FF6666" };
function processPositions(data, center) {
    baseData = data;
    if(data.positions.length > 0) {
        var points = [];
        var last = null;
        var distAccum = 0;
        var idxDescartada = 0;
        var pointsDes = null;
        var isEnqueued = false;
        

        $.each(data.positions, function(i, item) {
            var enqueued = wasEnqueued(item);

            if (enqueued != isEnqueued) {
                if (points.length > 1) {
                    addLine("r"+i, points, (isEnqueued ? color.encolada : color.normal), Logictracker.Monitor.Layers.Recorrido);
                }
                points = new Array();
                if (last) points.pushPoint(last);

                isEnqueued = enqueued;
            }
            points.pushPoint(item);

            if (last && data.descartadas.length > idxDescartada) {
                pointsDes = new Array();
                pointsDes.pushPoint(last);
                while (idxDescartada < data.descartadas.length && data.descartadas[idxDescartada].date > last.date && data.descartadas[idxDescartada].date < item.date) {
                    var descartada = data.descartadas[idxDescartada];
                    addMarker("d" + idxDescartada, descartada, Logictracker.Monitor.Layers.PosicionesDescartadas, gPPDescartada(descartada));
                    pointsDes.pushPoint(descartada);
                    idxDescartada++;
                }
                if (pointsDes.length > 1) {
                    pointsDes.pushPoint(item);
                    addLine("rd" + idxDescartada, pointsDes, color.descartada, Logictracker.Monitor.Layers.RecorridoDescartado);
                }
            }

            var distance = last != null ? LoxodromicaConAltitud(last.lat, last.lon, 0, item.lat, item.lon, 0) : 0;
            distAccum += distance;
            addMarker(i, item, Logictracker.Monitor.Layers.PosicionesReportadas, gPP(item, last, distance, distAccum), Logictracker.Monitor.Images.Posicion, 8);

            last = item;
        });
        pointsDes = new Array();
        pointsDes.pushPoint(last);
        for(;idxDescartada < data.descartadas.length; idxDescartada++)
        {
            var descartada = data.descartadas[idxDescartada];

            addMarker("d" + idxDescartada, descartada, Logictracker.Monitor.Layers.PosicionesDescartadas, gPPDescartada(descartada));
            pointsDes.pushPoint(descartada);
        }
        if (pointsDes.length > 1) {
            addLine("rd" + idxDescartada, pointsDes, color.descartada, Logictracker.Monitor.Layers.RecorridoDescartado);
        }

        if (points.length > 1) {
            addLine("r-1", points, isEnqueued ? color.encolada : color.normal, Logictracker.Monitor.Layers.Recorrido);
        }

        if (data.positions.length > 0) {
            var primera = data.positions[0];
            addMarker("salida", primera, Logictracker.Monitor.Layers.Eventos, gFP(primera),
                Logictracker.Monitor.Images.Salida, new OL.S(32, 32), new OL.PX(-16, -30));

            if (center) {
                Logictracker.Monitor.Mapa.setCenter(new OL.LL(primera.lon, primera.lat));
                Logictracker.Monitor.Mapa.setDefaultCenter(primera.lon, primera.lat);
            }
        }
        if (data.positions.length > 0) {
            addMarker("llegada", data.positions[data.positions.length - 1], Logictracker.Monitor.Layers.Eventos,
                gFP(data.positions[data.positions.length - 1]), Logictracker.Monitor.Images.Llegada, new OL.S(32, 32), new OL.PX(-16, -30));
        }
    }
}

function processQualityMessages(data) {
    baseData.quality = data.quality;
    baseData.infractions = data.infractions;
    baseData.quality_descartados = data.quality_descartados;
    
    $.each(data.quality, function(i, item) {
        if (!hasValidLatLon(item)) return;
        addMarker("q" + i, item, Logictracker.Monitor.Layers.Eventos, gMSP(item.id), item.icon, new OL.S(24, 24), new OL.PX(-12, -12));
        //if (hasDuration(item)) addMessageWithDuration(item, 'e', '#FF0000', Logictracker.Monitor.Layers.EventosDuracion);
    });
    $.each(data.quality_descartados, function(i, item) {
        if (!hasValidLatLon(item)) return;
        addMarker("qd" + i, item, Logictracker.Monitor.Layers.EventosDescartados, gMSPDescartado(item.id), item.icon, new OL.S(24, 24), new OL.PX(-12, -12));
        //if (hasDuration(item)) addMessageWithDuration(item, 'ed', '#000000', Logictracker.Monitor.Layers.EventosDuracionDescartados);
    });

    $.each(data.infractions, function(i, item) {
        if (!hasValidLatLon(item)) return;
        addMessageWithDuration(item, 'e', color.infraccion, Logictracker.Monitor.Layers.EventosDuracion);
    });
    
}

function processGeocercas(data) {
    baseData.geocercas = data.geocercas;

    if (data.geocercas.length > 0) {

        $.each(data.geocercas, function(i, item) {
            if (hasValidLatLon(item)) {
                addMarker("g" + item.id, item, Logictracker.Monitor.Layers.PuntosDeInteres, gPOIP(item.name), item.icon, new OL.S(24, 24), new OL.PX(-12, -12));
            }
            if (item.points.length > 0) {
                var baseStyle = { strokeColor: item.color, fillColor: item.color, fillOpacity: 0.5 };
                var geom;
                if (item.radio > 0) {
                    var point = new OL.P(item.points[0].lon, item.points[0].lat);
                    var style = Logictracker.Monitor.Mapa.GCS(item.points[0].lon, item.points[0].lat, item.radio, baseStyle, i == 0);
                    geom = new OL.V(point, null, style);
                    Logictracker.Monitor.Mapa.aGC("m" + item.id, geom, Logictracker.Monitor.Layers.Geocercas);
                } else {
                    var points = [];
                    $.each(item.points, function(j, punto) {
                        points.pushPoint(punto);
                    });
                    geom = points.toPolygonVector(baseStyle);
                    Logictracker.Monitor.Mapa.aG("m" + item.id, geom, Logictracker.Monitor.Layers.Geocercas);
                }
            }
        });

    }
}

function processQtree(data) {
    baseData.qtree = data.qtree;

    if (data.qtree.length > 0) {
        $.each(data.qtree, function(i, item) {
        Logictracker.Monitor.Mapa.aGC("t" + item.id, createBox(item), Logictracker.Monitor.Layers.Qtree);
        });

    }
}

var baseData = {};
var parameters = null;
var currentAjaxRequest = null;
function CallForData(qs, center) {
    parameters = qs;
    AbortAjaxRequest();
    hideError();
    showProgress("Cargando posiciones...");
    currentAjaxRequest = $.getJSON("?op=p&qs=" + qs, function(data) {
        try {

            clearLayers();
            if (data.positions.length == 0) {
                hideProgress();
                showInfo("No se encontraron posiciones");
            }
            else if (data.positions.length > 7500) {
                hideProgress();
                showError("Hay demasiadas posiciones (" + data.positions.length + ")");
                return;
            }
            processPositions(data, center);
            showProgress("Cargando eventos...");
            AbortAjaxRequest();
            currentAjaxRequest = $.getJSON("?op=q&qs=" + qs, function(data2) {
                try {
                    hideProgress();
                    if (data2.quality.length > 1000) {
                        hideProgress();
                        showError("Hay demasiados eventos de calidad (" + data2.quality.length + ")");
                        return;
                    }
                    if (data2.infractions.length > 1000) {
                        hideProgress();
                        showError("Hay demasiadas infracciones (" + data2.infractions.length + ")");
                        return;
                    }
                    if (data2.quality_descartados.length > 1000) {
                        hideProgress();
                        showError("Hay demasiados eventos de calidad descartados (" + data2.quality_descartados.length + ")");
                        return;
                    }
                    processQualityMessages(data2);
                    showProgress("Cargando referencias geográficas...");
                    AbortAjaxRequest();
                    currentAjaxRequest = $.getJSON("?op=g&qs=" + qs, function (data3) {
                        try {
                            if (data3.geocercas.length > 1000) {
                                hideProgress();
                                showError("Hay demasiados eventos de geocercas (" + data3.geocercas.length + ")");
                                return;
                            }
                            
                            processGeocercas(data3);

                            if ($('#'+Logictracker.Monitor.QtreeCheck).is(':checked')) {
                                showProgress("Cargando qtree...");
                                AbortAjaxRequest();
                                currentAjaxRequest = $.getJSON("?op=t&qs=" + qs, function (data4) {
                                    processQtree(data4);
                                    hideProgress();
                                }).fail(function (jqxhr, textStatus, error) {
                                    var err = textStatus + ", " + error;
                                    showError(err); hideProgress();

                                });
                            }
                            else {
                                hideProgress();
                            }
                        }
                        catch (ex) { showError(ex); hideProgress(); }
                    })
                    .fail(function (jqxhr, textStatus, error) {
                        var err = textStatus + ", " + error;
                        showError(err); hideProgress();

                    });
                }
                catch (ex) { showError(ex); hideProgress(); }
            })
            .fail(function (jqxhr, textStatus, error) {
                var err = textStatus + ", " + error;
                showError(err); hideProgress();

            });
        }
        catch (ex) { showError(ex); hideProgress(); }
    })
    .fail(function (jqxhr, textStatus, error) {
        var err = textStatus + ", " + error;
        showError(err); hideProgress();

    });
}
function AbortAjaxRequest() {
    if (currentAjaxRequest) currentAjaxRequest.abort();
    currentAjaxRequest = null;
}
function CallForQualityMessages() {
    AbortAjaxRequest();
    currentAjaxRequest = $.getJSON("?op=q&qs=" + parameters, processQualityMessages);
}
function showInfo(text) {
    $(Logictracker.Monitor.Info).addClass("infolabel").css("display", "block").text(text).show();
}
function hideInfo() {
    $(Logictracker.Monitor.Info).text("").hide();
}
function showError(text) {
    $(Logictracker.Monitor.Info).addClass("errorlabel").css("display", "block").text(text).show();
}
function hideError() {
    hideInfo();
}
function showProgress(text) {
    $(Logictracker.Monitor.Progress + " .progresslabel").text(text);
    $(Logictracker.Monitor.Progress).show();
}
function hideProgress() {
    $(Logictracker.Monitor.Progress + " .progresslabel").text("Espere por favor...");
    $(Logictracker.Monitor.Progress).hide();
}