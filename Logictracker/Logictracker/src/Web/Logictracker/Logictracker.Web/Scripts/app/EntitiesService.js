angular
    .module("logictracker.entities.service", [])
    .factory("EntitiesService", ["$resource", "$http", EntitiesService]);

function EntitiesService($resource, $http) {
    var _service = {
        distrito: {
            items: getDistritosDS,
            bases: getBasesDS,
            departamento: getDepartamentosDS,
            centroDeCostos: getCentroDeCostosDS,
            transportista: getTransportistaDS
        },
        resources: {
            bases: $resource("/api/distrito/:distritoId/base/items", { distritoId: "@distritoId" }),
            departamentos: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            centroDeCostos: $resource("/api/distrito/:distritoId/base/:baseId/centrodecostos/items", { distritoId: "@distritoId", baseId: "@baseId", deptoId: "@deptoId" }),
            transportista: $resource("/api/distrito/:distritoId/base/:baseId/transportista/items", { distritoId: "@distritoId", baseId: "@baseId" }),
        },
        ticketrechazo: {
            estados: getEstados, //$resource("/api/ticketrechazo/estado/items"),
            motivos: getMotivos, // $resource("/api/ticketrechazo/motivo/items")
        }
    };

    function getDistritosDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/distrito/items"
                }
            }
        });
        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };


    function getBasesDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "") {
                        getData(_service.resources.bases, op, { distritoId: op.data.distritoId });
                    }
                    else {
                        op.success([]);
                    }
                }
            }
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getDepartamentosDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                dataType: "jsonp",
                type: "GET",
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "") {

                        getData(_service.resources.departamentos,
                            op,
                            { distritoId: op.data.distritoId, baseId: op.data.baseId });

                    }
                    else {
                        op.success([]);
                    }
                }
            }
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getCentroDeCostosDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "" &&
                        op.data.departamentoId !== undefined && op.data.departamentoId !== "") {

                        getData(_service.resources.centroDeCostos,
                            op,
                            { distritoId: op.data.distritoId, baseId: op.data.baseId, departamentoId: op.data.departamentoId });

                    }
                    else {
                        op.success([]);
                    }
                }
            }
        });
        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getTransportistaDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "") {

                        getData(_service.resources.transportista,
                            op,
                            { distritoId: op.data.distritoId, baseId: op.data.baseId });

                    }
                    else {
                        op.success([]);
                    }
                }
            }
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);
        return ds;
    };

    function getData(res, option, params) {
        res.query(params,
            function (data) {
                if (data === undefined || data == null)
                    option.error();
                else
                    option.success(data);
            },
            function (error) {
                option.error(error);
            }
        );
    };

    function getMotivos(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/ticketrechazo/motivo/items"
                }
            }
        });
        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getEstados(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/ticketrechazo/estado/items"
                }
            },

        });
        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);
        return ds;
    };

    return _service;
}
