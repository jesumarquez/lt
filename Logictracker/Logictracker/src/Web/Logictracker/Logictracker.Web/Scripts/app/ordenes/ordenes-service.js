﻿angular
    .module('logictracker.ordenes.service', [])
    .factory('OrdenesService', ['$resource', OrdenesService]);

function OrdenesService($resource) {
    var _service = {
        items: getOrdenesItems,
        ordenDetalles: getOrdenDetalles,
        ordenes: $resource("/api/distrito/:distritoId/base/:baseId/ordenes", { distritoId: "@distritoId", baseId: "@baseId" })
    }

    function getOrdenesItems(filters, onEnd, onFail) {

        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/ordenes/datasource";
                    }
                }
            },
            //data: [{ "Id": 1137, "Empresa": "Petrobras", "IdEmpresa": 91, "BaseId": 2476, "Empleado": null, "IdEmpleado": 0, "Transportista": "ALL ROAD SA", "IdTransportista": 321, "PuntoEntrega": "67 - TRASLUX S.A.    ", "IdPuntoEntrega": 646198, "CodigoPuntoEntrega": "490", "CodigoPedido": "2959048", "FechaAlta": "2016-02-15T14:18:16", "FechaPedido": "2016-02-08T03:00:00", "FechaEntrega": null, "InicioVentana": null, "FinVentana": null, "PuntoEntregaLatitud": -34.125461, "PuntoEntregaLongitud": -59.081656 }, { "Id": 1138, "Empresa": "Petrobras", "IdEmpresa": 91, "BaseId": 2476, "Empleado": null, "IdEmpleado": 0, "Transportista": "ALL ROAD SA", "IdTransportista": 321, "PuntoEntrega": "28 - ZAGABRIA S.A.    ", "IdPuntoEntrega": 646159, "CodigoPuntoEntrega": "2359", "CodigoPedido": "2959384", "FechaAlta": "2016-02-15T14:18:16", "FechaPedido": "2016-02-08T03:00:00", "FechaEntrega": null, "InicioVentana": null, "FinVentana": null, "PuntoEntregaLatitud": -33.6966, "PuntoEntregaLongitud": -59.6816 }],
            schema: {
                total: "Total",
                data: "Data",
                errors: "Errors",
                parse: function (data) {
                    $.each(data.Data, function (i, val) {
                        val.FechaAlta = kendo.parseDate(val.FechaAlta);
                        val.FechaPedido = kendo.parseDate(val.FechaPedido);
                        val.FechaEntrega = kendo.parseDate(val.FechaEntrega);
                       });
                    return data;
                },
            },
            pageSize: 25,
            filter: filters,
            serverGrouping: false,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: false,            
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getOrdenDetalles(orderId, list, onEnd, onFail)
    {
        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    traditional: true,
                    dataType: "json",
                    url: function (op) {
                        return "/api/ordenes/" + orderId;
                    },
                    data: { insumos: list }
                }
            },
            schema:
        {
            model: {
                id: "Id",
                fields: {
                    "Id": { type: "number", editable: false },
                    "OrderId": { type: "number", editable: false },
                    "Insumo": { type: "string", editable: false },
                    "Cantidad": { type: "number", editable: false },
                    "Ajuste": { type: "number", editable: true },
                    "Cuaderna": { type: "number", editable: true },
                    "CocheId": { type: "number", editable: true }
                }
            },
        },
            pageSize: 10,
            serverGrouping: false,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: false,
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    }
    
    function getItems(data_, onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: "/api/distrito/" + data_.distritoId + "/base/" + data_.baseId + "/ordenes/"
                }
            },
            schema: {
                total: function (response) {
                    return response.length;
                },
                parse: function (data) {
                    $.each(data, function (i, item) {
                        item.FechaAlta = kendo.parseDate(item.FechaAlta);
                        item.FechaPedido = kendo.parseDate(item.FechaPedido);
                        item.FechaEntrega = kendo.parseDate(item.FechaEntrega);
                    });
                    return data;
                }
            },
            pageSize: 25,
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    }

    return _service;

    //return $resource("/api/distrito/:distritoId/base/:baseId/ordenes/:id",
    //    { distritoId: "@distritoId", baseId: "@baseId", id: "@id" },
    //    { query: { method: 'GET',  isArray: true }
    //});
}




