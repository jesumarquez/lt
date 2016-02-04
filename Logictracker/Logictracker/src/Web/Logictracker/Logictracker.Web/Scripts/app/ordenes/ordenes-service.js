angular
    .module('logictracker.ordenes.service', [])
    .factory('OrdenesService', ['$resource', OrdenesService]);

function OrdenesService($resource) {
    var _service = {
        items: getOrdenesItems,
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




