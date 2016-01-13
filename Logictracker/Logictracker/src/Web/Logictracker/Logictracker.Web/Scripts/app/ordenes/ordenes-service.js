angular
    .module('logictracker.ordenes.service', [])
    .factory('OrdenesService', ['$resource', OrdenesService]);

function OrdenesService($resource) {
    var _service = {
        items: getItems,
        list: $resource("/api/distrito/:distritoId/base/:baseId/ordenes/:id",
            { distritoId: "@distritoId", baseId: "@baseId", id: "@id" },
            {
                query: { method: 'GET', isArray: true }
            })
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




