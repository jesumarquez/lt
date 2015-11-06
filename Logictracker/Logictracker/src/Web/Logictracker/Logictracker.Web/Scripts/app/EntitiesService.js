angular
    .module("logictracker.entities.service", [])
    .factory("EntitiesService", ["$resource", "$http", EntitiesService]);

function EntitiesService($resource, $http) {
    var _service = {
        distrito: {
            items: getDistritosDS,
            bases: getBasesDS,
            departamento: getDepartamentosDS,
            centroDeCostos: $resource("/api/distrito/:distritoId/base/:baseId/centrodecostos/items", { distritoId: "@distritoId", baseId: "@baseId", deptoId: "@deptoId" }),
            transportista: $resource("/api/distrito/:distritoId/base/:baseId/transportista/items", { distritoId: "@distritoId", baseId: "@baseId" }),
        },
        resources: {
            bases: $resource("/api/distrito/:distritoId/base/items", { distritoId: "@distritoId" }),
            departamentos: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: "@distritoId", baseId: "@baseId" })
        }
    };

    function getDistritosDS() {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/distrito/items"
                }
            }
        });

        return ds;
    };


    function getBasesDS() {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "") {
                        getData(_service.resources.bases, op, { distritoId: op.data.distritoId });
                    }
                    else {
                        op.error();
                    }
                }
            }
        });

        return ds;
    };

    function getDepartamentosDS() {
        var ds = new kendo.data.DataSource({
            transport: {
                dataType: "jsonp",
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "") {
                        
                        getData(_service.resources.departamentos,
                            op,
                            { distritoId: op.data.distritoId, baseId : op.data.baseId });
                    }
                    else {
                        //op.success([{Key: -1, Value: 'Todos'}]);
                        op.error();
                    }
                }
            }
        });

        return ds;
    };


    function getData(res, option, params) {
        res.query(params,
        function (data) {
            //if (data.lenght == 0)
            //    option.error();
            //else
                option.success(data);
        },
        function (error) {
            option.error(error);
        });
    };

    return _service;
}
