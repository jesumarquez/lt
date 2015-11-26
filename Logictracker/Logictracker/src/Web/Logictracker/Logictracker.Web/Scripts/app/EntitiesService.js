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
            transportista: getTransportistaDS,
            clientes: {
                models: getClientes
            },
            distribuciones: {
                models: getDistribuciones
            },
            puntoEntrega: getPuntoEntrega            
        },
        resources: {
            bases: $resource("/api/distrito/:distritoId/base/items", { distritoId: "@distritoId" }),
            departamentos: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            centroDeCostos: $resource("/api/distrito/:distritoId/base/:baseId/centrodecostos/items", { distritoId: "@distritoId", baseId: "@baseId", deptoId: "@deptoId" }),
            transportista: $resource("/api/distrito/:distritoId/base/:baseId/transportista/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            puntoEntrega: $resource("/api/distrito/:distritoId/base/:baseId/cliente/:clienteId/PuntoEntrega/items", { distritoId: "@distritoId", baseId: "@baseId", clienteId: "@clienteId" }),
            empleadoReporta: $resource("/api/distrito/:distritoId/base/:baseId/empleado/:empleadoId/reporta/items"),
            ticketRechazo: $resource("/api/ticketrechazo/item/:id", { id: "@id" }, { "update": { method: "PUT" } }),
            userData: $resource("/api/UserData")
        },
        ticketrechazo: {
            estados: getEstados,
            motivos: getMotivos,
            empleadoReporta: getEmpleadoReportaDS,
            items: getRechazoItems,
            nextEstado:  getNextEstado
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

    function getPuntoEntrega(data_, onEnd, onFail) {
        
        var url = data_.distribucionId != null ? "/api/distrito/" + data_.distritoId + "/base/" + data_.baseId + "/viajeDistribucion/" + data_.distribucionId + "/PuntoEntrega/items" :
            "/api/distrito/" + data_.distritoId + "/base/" + data_.baseId + "/PuntoEntrega/items";

        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return url;
                    },

                },

            },
            schema: {
                data: "Data"
            },
            serverFiltering: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);
        return ds;

    }

    function getEmpleadoReportaDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "" &&
                        op.data.empleadoId !== undefined && op.data.empleadoId !== "") {

                        getData(_service.resources.empleadoReporta,
                            op,
                            { distritoId: op.data.distritoId, baseId: op.data.baseId, empleadoId: op.data.empleadoId });

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

    function getClientes(data_, onEnd, onFail) {

        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/distrito/" + data_.distritoId + "/base/" + data_.baseId + "/cliente/models";
                    },

                },

            },
            schema: {
                data: "Data"
            },
            serverFiltering: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);
        return ds;
    }



    function getDistribuciones(data_, onEnd, onFail) {

        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/distrito/" + data_.distritoId + "/base/" + data_.baseId + "/distribucion/models";
                    },

                },

            },
            schema: {
                data: "Data"
            },
            serverFiltering: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);
        return ds;
    }

    function getRechazoItems(filters, onEnd, onFail) {

        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/ticketrechazo/datasource";
                    },
                },
            },
            schema: {
                total: "total",
                data: "Data",
                parse: function (data) {
                    $.each(data.Data, function (i, val) {
                        val.FechaHora = kendo.parseDate(val.FechaHora);
                    });
                    return data;
                }
            },
            filter: filters,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: true,
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getNextEstado(data_, onEnd, onFail){
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: "/api/ticketrechazo/" + data_.ticketId + "/estado/next/",
                }
            }
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    }

    return _service;
}
