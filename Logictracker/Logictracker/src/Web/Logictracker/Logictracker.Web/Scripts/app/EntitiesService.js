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
            transportista: {
                models: getTransportistaDS,
                empleados: getEmpleadosDS,
            },
            clientes: {
                models: getClientes
            },
            distribuciones: {
                models: getDistribuciones
            },
            puntoEntrega: getPuntoEntrega,
            tipoCicloLogistico: getTipoCicloLogisticoDS
        },
        resources: {
            bases: $resource("/api/distrito/:distritoId/base/items", { distritoId: "@distritoId" }),
            departamentos: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            centroDeCostos: $resource("/api/distrito/:distritoId/base/:baseId/centrodecostos/items", { distritoId: "@distritoId", baseId: "@baseId", deptoId: "@deptoId" }),
            transportista: $resource("/api/distrito/:distritoId/base/:baseId/transportista/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            puntoEntrega: $resource("/api/distrito/:distritoId/base/:baseId/cliente/:clienteId/PuntoEntrega/items", { distritoId: "@distritoId", baseId: "@baseId", clienteId: "@clienteId" }),
            empleadoReporta: $resource("/api/distrito/:distritoId/base/:baseId/empleado/:empleadoId/reporta/models"),
            empleadoByTipo: $resource("/api/distrito/:distritoId/base/:baseId/tipoEmpleadoCodigo/:tipoEmpleadoCodigo/models"),
            empleado: $resource("/api/distrito/:distritoId/base/:baseId/empleado/:empleadoId/model"),
            ticketRechazo: $resource("/api/ticketrechazo/item/:id", { id: "@id" }, { "update": { method: "PUT" } }),
            userData: $resource("/api/UserData"),
            parametros: $resource("/api/Distrito/:distritoId/parametros/items", { distritoId: "@distritoId" }),
            estadisticasPorRol: $resource("/api/ticketrechazo/distrito/:distritoId/base/:baseId/estadisticas/rol", { distritoId: "@distritoId", baseId: "@baseId" }),
            cantidadPorEstado: $resource("/api/ticketrechazo/distrito/:distritoId/base/:baseId/estadisticas/estado", { distritoId: "@distritoId", baseId: "@baseId" }),
            cantidadTicketPorHora: $resource("/api/ticketrechazo/distrito/:distritoId/base/:baseId/estadisticas/ticketporhora", { distritoId: "@distritoId", baseId: "@baseId" }),
            chofer: $resource("/api/ticketrechazo/distrito/:distritoId/base/:baseId/transportista/:transportistaId/tipoEmpleadoCodigo/:tipoEmpleadoCodigo/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            tipoCicloLogistico: $resource("/api/distrito/:distritoId/tipociclologistico/items", { distritoId: "@distritoId" })
        },
        ticketrechazo: {
            estados: getEstados,
            motivos: getMotivos,
            empleadoReporta: getEmpleadoReportaDS,
            empleado: getEmpleadosDS,
            items: getRechazoItems,
            nextEstado: getNextEstado,
            promedioPorVendedor: getPromedioPorVendedor,
            promedioPorEstado: getPromeioPorEstado,
            ticketPorHora: getCantidadTicketPorHora
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
                        op.data.empleadoId !== undefined && op.data.empleadoId !== "")
                        {
                            if (op.data.tipoEmpleadoCodigo !== undefined) {
                                getData(_service.resources.empleadoByTipo,
                                   op,
                                   { distritoId: op.data.distritoId, baseId: op.data.baseId, tipoEmpleadoCodigo: op.data.tipoEmpleadoCodigo });
                            }
                            else {
                                getData(_service.resources.empleadoReporta,
                                    op,
                                    { distritoId: op.data.distritoId, baseId: op.data.baseId, empleadoId: op.data.empleadoId });
                        }
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

    function getEmpleadosDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "" &&
                        op.data.baseId !== undefined && op.data.baseId !== "") {

                        if (op.data.tipoEmpleadoCodigo != null && op.data.tipoEmpleadoCodigo !== "") {
                            // No se filtra por base
                            getData(_service.resources.empleadoByTipo,
                                op,
                                {   
                                    distritoId: op.data.distritoId, 
                                    baseId: -1, 
                                    tipoEmpleadoCodigo: op.data.tipoEmpleadoCodigo , 
                                    transportistaId: (op.data.transportistaId) ? op.data.transportistaId : -1
                                });
                        }
                        else if (op.data.empleadoId != null && op.data.empleadoId !== "") {
                            // No se filtra por base
                            getData(_service.resources.empleado,
                                op,
                            {
                                distritoId: op.data.distritoId,
                                baseId: -1,
                                empleadoId: op.data.empleadoId,
                                transportistaId: (op.data.transportistaId) ? op.data.transportistaId : -1
                            });
                        }
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

    var dictMappingGroup = [];
    dictMappingGroup["UltimoEstado"] = "Estado";
    dictMappingGroup["Estado"] = "UltimoEstado";
    dictMappingGroup["MotivoDesc"] = "Motivo";
    dictMappingGroup["Motivo"] = "MotivoDesc";
    dictMappingGroup["EntregaCodigo"] = "Entrega.Id";
    dictMappingGroup["Entrega.Id"] = "EntregaCodigo";
    dictMappingGroup["VendedorDesc"] = "Vendedor.Id";
    dictMappingGroup["Vendedor.Id"] = "VendedorDesc";
    dictMappingGroup["SupVenDesc"] = "SupervisorVenta.Id";
    dictMappingGroup["SupervisorVenta.Id"] = "SupVenDesc";
    dictMappingGroup["SupRutDesc"] = "SupervisorRuta.Id";
    dictMappingGroup["SupervisorRuta.Id"] = "SupRutDesc";
    dictMappingGroup["ChoferDesc"] = "Chofer.Id";
    dictMappingGroup["Chofer.Id"]= "ChoferDesc";
    

    function mappingGroupFields(e)
    {
        $.each(e.sender._group, function (i, val) {
            if (dictMappingGroup[val.field] !== undefined)
                val.field = dictMappingGroup[val.field];
        });
    }
    
    function reverseMappingGroupFields(val) {
        if (val.Items !== undefined && val.Items.length > 0)
        { 
            $.each(val.Items, function (i, val) {
                reverseMappingGroupFields(val);
            });
        }

        if (val.Member !== undefined && dictMappingGroup[val.Member] !== undefined)
            val.Member = dictMappingGroup[val.Member];
            
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
                    }             
                }
            },
            //requestStart: mappingGroupFields,
            //requestEnd: mappingGroupFields,
            schema: {
                total: "Total",
                data: "Data",
                errors: "Errors",
                parse: function (data) {
                    $.each(data.Data, function (i, val) {
                        val.FechaHora = kendo.parseDate(val.FechaHora);
                        val.FechaHoraEstado = kendo.parseDate(val.FechaHoraEstado);
                    //$.each(data.Data, function (i, val) {
                    //    $.each(val.Items, function (i, item) {
                    //        item.FechaHora = kendo.parseDate(item.FechaHora);
                    //        item.FechaHoraEstado = kendo.parseDate(item.FechaHoraEstado);
                    //    });
                        //reverseMappingGroupFields(val);
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
            group: {
                field: "Estado",
                dir: "asc"
            },
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getNextEstado(data_, onEnd, onFail) {
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

    function getPromedioPorVendedor(filters, onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/ticketrechazo/estadisticas/promedio/porvendedor";
                    },
                },
            },
            schema: {
                total: "Total",
                data: "Data",
                errors: "Errors"
            },
            pageSize: 25,
            filter: filters,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    }

    function getPromeioPorEstado(filters, onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/ticketrechazo/estadisticas/promedio/porestado";
                    },
                },
            },
            schema: {
                total: "Total",
                data: "Data",
                errors: "Errors"
            },
            pageSize: 8,
            filter: filters,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    }

    function getCantidadTicketPorHora(filters, onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            type: "aspnetmvc-ajax",
            transport: {
                read: {
                    type: "GET",
                    dataType: "json",
                    url: function (op) {
                        return "/api/ticketrechazo/estadisticas/promedio/porestado";
                    },
                },
            },
            schema: {
                total: "Total",
                data: "Data",
                errors: "Errors"
            },
            pageSize: 8,
            filter: filters,
            serverFiltering: true,
            serverSorting: false,
            serverPaging: true
        });

        if (angular.isFunction(onEnd))
            ds.bind("requestEnd", onEnd);
        if (angular.isFunction(onFail))
            ds.bind("error", onFail);

        return ds;
    };

    function getTipoCicloLogisticoDS(onEnd, onFail) {
        var ds = new kendo.data.DataSource({
            transport: {
                read: function (op) {
                    if (op.data.distritoId !== undefined && op.data.distritoId !== "") {
                        getData(_service.resources.tipoCicloLogistico, op, { distritoId: op.data.distritoId });
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
    return _service;
}
