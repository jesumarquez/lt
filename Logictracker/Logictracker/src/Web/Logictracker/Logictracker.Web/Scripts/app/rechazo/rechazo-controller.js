angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', '$filter', RechazoController])
    .controller('RechazoItemController', ['$scope', 'EntitiesService', RechazoItemController])
    .controller('RechazoEditItemController', ['$scope', 'EntitiesService', RechazoEditItemController])
    .controller("RechazoEstadisticasController", ["$scope", "EntitiesService", RechazoEstadisticasController]);


function RechazoController($scope, EntitiesService, $filter) {

    $scope.UserData = EntitiesService.resources.userData.get();

    $scope.distritoSelected = {};

    $scope.baseDS = [];
    $scope.baseSelected = {};

    $scope.departamentoDS = [];
    $scope.departamentoSelected = [];

    $scope.centroDeCostosDS = [];
    $scope.centroDeCostosSelected = [];

    $scope.transportistaSelected = [];
    $scope.transportistaDS = [];

    $scope.distritoDS = EntitiesService.distrito.items(onDistritoDSLoad, onFail);

    $scope.desde = new Date();
    $scope.hasta = new Date();

    $scope.estadoSelected = {};
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(function (e) { $scope.estadoSelected = e.response[0]; },
        onFail);

    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(function (e) { $scope.motivoSelected = e.response[0]; },
        onFail);

    $scope.baseDS = EntitiesService.distrito.bases(onBaseDSLoad, onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento(onDepartamentoDSLoad, onFail);

    $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos(onCentroDeCostosDSLoad, onFail);

    $scope.transportistaDS = EntitiesService.distrito.transportista(ontransportistaDSLoad, onFail);

    $scope.$watch("distritoSelected", onDistritoSelected);

    $scope.$watch("basesDS", onBasesDSChange);

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$watchGroup(["departamentoSelected", "baseSelected"],
        onDepartamentoAndBaseChange);

    function onDistritoDSLoad(e) {
        if (e.type === "read" && e.response) {
            try {
                if ($scope.UserData.DistritoSelected) {
                    $scope.distritoSelected = $filter("filter")(e.response, { Key: $scope.UserData.DistritoSelected }, true)[0];
                    if ($scope.distritoSelected) return;
                }
            } catch (ex) { }
            $scope.distritoSelected = e.response[0];
        }
    };

    function onBaseDSLoad(e) {
        if (e.type === "read" && e.response) {
            try {
                if ($scope.UserData.BaseSelected) {
                    $scope.baseSelected = $filter("filter")(e.response, { Key: $scope.UserData.BaseSelected }, true)[0];
                    if ($scope.baseSelected) return;
                }
            } catch (ex) { }
            $scope.baseSelected = e.response[0];
        }

    }

    function onFail(error) {
        $scope.notify.show(error.errorThrown, "error");
    };

    function onDistritoSelected(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.baseDS.read({ distritoId: $scope.distritoSelected.Key });
        }
    };

    function onBasesDSChange(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoSelected = [];
            $scope.centroDeCostosSelected = [];
        }
    };

    function onDepartamentoDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.departamentoSelected = [];
        }
    }

    function onCentroDeCostosDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.centroDeCostosSelected = [];
        }
    }

    function ontransportistaDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.transportistaSelected = [];
        }
    }

    function onBaseSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {

            $scope.UserData.DistritoSelected = $scope.distritoSelected.Key;
            $scope.UserData.BaseSelected = $scope.baseSelected.Key;

            $scope.UserData.$save();

            $scope.departamentoDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key
            });

            $scope.transportistaDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key
            });


        }
    };

    function onDepartamentoAndBaseChange(newValue, oldValue) {
        if (newValue[0] !== undefined && newValue[0].length > 0 && newValue != null && newValue !== oldValue)
            $scope.centroDeCostosDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key,
                departamentoId: $scope.departamentoSelected.map(function (o) { return o.Key; })
            });
    }

    function onRechazosDSLoad() {

    }


    $scope.gridOptions = {
        sortable: true,
        groupable: true,
        pageable: {
            pageSize: 20
        },
        columns:
        [
        { field: "TicketRechazoId", title: "Ticket" },
        { field: "FechaHora", title: "Fecha Hora", format: "{0:G}", sortable: true },
        { field: "ClienteId", title: "Cod. Cliente" },
        { field: "Cliente", title: "Cliente" },
        { field: "SupVenDesc", title: "Sup. Venta" },
        { field: "SupRutDesc", title: "Sup. Ruta" },
        { field: "Estado", title: "Estado" },
        { field: "Territorio", title: "Territorio" },
        { field: "Motivo", title: "Motivo" },
        { field: "Bultos", title: "Bultos" },
        { template: "<a href='\\#' class='link' ng-click='onEdit(dataItem.TicketRechazoId)'>Editar</a>", title: "", width: "100px" }
        ]
    }

    $scope.onNuevo = function () {
        $scope.operacion = "A";
        $scope.ticketItem = undefined;
        $scope.rechazoWin.refresh({ url: "Item?op=A" }).open();
    };

    $scope.onEdit = function (id) {
        $scope.operacion = "E";
        $scope.ticketItemId = id;
        $scope.rechazoWin.refresh({ url: "EditItem" }).open();
    }
    
    $scope.onBuscar = function () {

        var filterLIst = [];

        if ($scope.distritoSelected != undefined)
            filterLIst.push({ field: "Empresa.Id", operator: "eq", value: $scope.distritoSelected.Key });

        if ($scope.baseSelected != undefined)
            filterLIst.push({ field: "Linea.Id", operator: "eq", value: $scope.baseSelected.Key });

        if ($scope.estadoSelected != undefined && $scope.estadoSelected.Key !== -1)
            filterLIst.push({ field: "UltimoEstado", operator: "eq", value: $scope.estadoSelected.Key });

        if ($scope.motivoSelected != undefined && $scope.motivoSelected.Key !== -1)
            filterLIst.push({ field: "Motivo", operator: "eq", value: $scope.motivoSelected.Key });

        if ($scope.desde != undefined) {
            var fDesde = new Date($scope.desde);
            fDesde.setHours(0, 0, 0, 0);
            filterLIst.push({ field: "FechaHora", operator: "gte", value: fDesde });
        }

        if ($scope.hasta != undefined) {
            var fHasta = new Date($scope.hasta);
            fHasta.setHours(23, 59, 0, 0);
            filterLIst.push({ field: "FechaHora", operator: "lte", value: fHasta });
        }


        var filters = {
            logic: "and",
            filters: filterLIst
        };

        $scope.rechazosDS = EntitiesService.ticketrechazo.items(filters, onRechazosDSLoad, onFail);

    };

    $scope.onRefreshWindow = function () {
       $scope.rechazoWin.center();
    }
}

function RechazoItemController($scope, EntitiesService) {


    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(onMotivoDSLoad, $scope.onFail);
    // El motivo es editable solo si es un alta
    $scope.motivoRO = function () { return !isNew(); };

    $scope.estadoSelected = {};
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(onEstadoDSLoad, $scope.onFail);
    $scope.estadoRO = true;

    $scope.clienteSelected = "";
    $scope.clienteRO = false;
    //$scope.clienteDS = EntitiesService.distrito.clientes.models({ distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key }, null, $scope.onFail); // [{codigo:"12"},{codigo:"33"}];

    $scope.puntoEntregaSelected = {};
    $scope.puntoEntregaRO = true;
    $scope.puntoEntregaDS = EntitiesService.distrito.puntoEntrega({ distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key }, onPuntoEntregaDSLoad, $scope.onFail);

    $scope.distribucionDS = EntitiesService.distrito.distribuciones.models({ distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key }, null, $scope.onFail);
    $scope.distribucionSelected = {};
    $scope.distribucionRO = true;

    $scope.supervisorRutaSelected = {};
    $scope.supervisorRutaRO = true;
    $scope.supervisorRutaDS = EntitiesService.ticketrechazo.empleadoReporta(onSupervisorRutaDSLoad, onFail);

    $scope.supervisorVentasSelected = {};
    $scope.supervisorVentasRO = true;
    $scope.supervisorVentasDS = EntitiesService.ticketrechazo.empleadoReporta(onSupervisorVentasDSLoad, onFail);

    $scope.vendedorSelected = {};
    $scope.vendedorRO = true;
    $scope.vendedorDS = EntitiesService.ticketrechazo.empleado(onVendedorDSLoad, onFail);

    $scope.territorio = "";
    $scope.territorioRO = true;

    $scope.enHorarioSelected = {};
    $scope.enHorarioRO = true;

    $scope.movimientosDS = {};

    //$scope.$watch("clienteSelected", onClienteSelected);
    $scope.$watch("vendedorSelected", onVendedorSelected);
    $scope.$watch("distribucionSelected", onDistribucionSelected);
    $scope.$watch("puntoEntregaSelected", onPuntoEntregaSelected);
    $scope.$watch("supervisorVentasSelected", onSupervisorVentasSelected);

    $scope.enHorarioDS = [
        { Key: true, Value: "Si" },
        { Key: false, Value: "No" },
    ];

    function onVendedorSelected(newValue, oldValue) {

        if (newValue !== undefined && newValue !== oldValue) {

            $scope.supervisorVentasDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key,
                empleadoId: $scope.vendedorSelected.Key
            });
        }
        else {
            $scope.supervisorVentasDS.data([]);
            $scope.supervisorVentasDS.read();
        }
    };

    function onDistribucionSelected(newValue, oldValue) {

        $scope.puntoEntregaSelected = [];

        if (newValue != null && newValue !== oldValue) {
            $scope.puntoEntregaDS = EntitiesService.distrito.puntoEntrega({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key,
                distribucionId: $scope.distribucionSelected[0] !== undefined ? $scope.distribucionSelected[0].Id : null,
            }, null, $scope.onFail);
        }
    };

    function onPuntoEntregaSelected(newValue, oldValue) {

        if (newValue !== oldValue) {

            if (newValue.length == 0) {
                $scope.clienteSelected = "";
                $scope.vendedorDS.data([]);
                $scope.vendedorDS.read();
                return;
            }

            if ($scope.puntoEntregaSelected[0] !== undefined) {

                $scope.clienteSelected = $scope.puntoEntregaSelected[0].ClienteCodigo;

                var responsable = ($scope.puntoEntregaSelected[0] !== undefined && $scope.puntoEntregaSelected[0].ResponsableId != 0);

                // Si el punto de entrega no tiene responsable por el momento en el combo de vendedores se van a listar
                // todos los empleados que tengan asignado el tipo de empleado "V".
                $scope.vendedorDS.read({
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key,
                    tipoEmpleadoCodigo: !responsable ? "V" : null,
                    empleadoId: responsable ? $scope.puntoEntregaSelected[0].ResponsableId : null,
                });
            }
        }
    };

    function onSupervisorVentasSelected(newValue, oldValue) {

        if (newValue !== undefined && newValue !== oldValue) {
            $scope.supervisorRutaDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key,
                empleadoId: $scope.supervisorVentasSelected.Key
            });
        }
        else
        {
            $scope.supervisorRutaDS.data([]);
            $scope.supervisorRutaDS.read();
        }
    };

    function onSupervisorVentasDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.supervisorVentasSelected = e.response[0];
        }
    }

    function onSupervisorRutaDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.supervisorRutaSelected = e.response[0];
        }
    }

    function onVendedorDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.vendedorSelected = e.response[0];
        }
    }

    function onMotivoDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.motivoSelected = e.response[0];
        }
    }

    function onEstadoDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.estadoSelected = e.response[0];
        }
    }

    function onPuntoEntregaDSLoad(e) {
        if (e.type === "read" && e.response) {
        }
    }

    function isNew() { return $scope.operation === "A"; }

    $scope.tDistribucion = kendo.template($("#tDistribucion").html());
    $scope.tCliente = kendo.template($("#tCliente").html());
    $scope.tPuntoEntrega = kendo.template($("#tPuntoEntrega").html());


    $scope.gridDetalleOptions = {
        columns:
        [
            { field: "TicketRechazoDetalleId", title: "Id" },
            { field: "FechaHora", title: "Fecha Hora" },
            { field: "UsuarioNombre", title: "Usuario" },
            { field: "Estado", title: "Estado" },
            { field: "Observacion", title: "Observacion" },
        ]
    };


    $scope.onClose = function () {
        $scope.rechazoWin.close();
    };


    $scope.onSave = function (event) {
        event.preventDefault();

        if ($scope.validator.validate()) {

            var ticketRechazo = {
                DistritoId: $scope.distritoSelected.Key,
                LineaId: $scope.baseSelected.Key,
                ClienteId: $scope.puntoEntregaSelected != null && $scope.puntoEntregaSelected[0] !== undefined ? $scope.puntoEntregaSelected[0].ClienteId : "",
                SupVenDesc: $scope.supervisorVentasSelected != null ? $scope.supervisorVentasSelected.Key : "",
                SupRutDesc: $scope.supervisorRutaSelected != null ? $scope.supervisorRutaSelected.Key : "",
                Estado: $scope.estadoSelected.Key,
                Motivo: $scope.motivoSelected.Key,
                Territorio: $scope.territorio,
                VendedorId: $scope.vendedorSelected.Key,
                SupVenId: $scope.supervisorVentasSelected.Key,
                SupRutId: $scope.supervisorRutaSelected.Key,
                Observacion: $scope.observacion,
                EnHorario: $scope.enHorarioSelected.Key,
            };

            EntitiesService.resources.ticketRechazo.save(ticketRechazo);

            $scope.rechazoWin.close();
        }
    };

    function onFail(error) {
        $scope.notify.show(error.errorThrown, "error");
    };
}

function RechazoEditItemController($scope, EntitiesService) {

    $scope.ticketItem = EntitiesService.resources.ticketRechazo.get({ id: $scope.ticketItemId });
    $scope.gridDetalleOptions = {
        columns:
        [
            { field: "TicketRechazoDetalleId", title: "Id" },
            { field: "FechaHora", title: "Fecha Hora" },
            { field: "UsuarioNombre", title: "Usuario" },
            { field: "Estado", title: "Estado" },
            { field: "Observacion", title: "Observacion" },
        ]
    };

    $scope.estadoSelected = {};
    $scope.estadoDS = EntitiesService.ticketrechazo.nextEstado({ ticketId: $scope.ticketItemId }, onEstadoDS, onFail)

    function onEstadoDS(e) {
        if (e.type === "read" && e.response) {
            $scope.estadoSelected = e.response[0];
        }
    }

    function onFail(error) {
        $scope.notify.show(error.errorThrown, "error");
    };

    $scope.onClose = function () {
        $scope.rechazoWin.close();
    };

    $scope.onSave = function (event) {
        event.preventDefault();

        if ($scope.validator.validate()) {

            var ticketRechazo = {
                TicketRechazoId: $scope.ticketItemId,
                Estado: $scope.estadoSelected.Key,
                Observacion: $scope.ticketItem.Observacion
            };

            EntitiesService.resources.ticketRechazo.update({ id: $scope.ticketItemId }, ticketRechazo);

            $scope.rechazoWin.close();
        }
    };
}

function RechazoEstadisticasController($scope, EntitiesService) {
    $scope.gridOptions = {
        columns: [
        { field: "Vendedor", title: "Vendedores" },
        { field: "Promedio", title: "Promedio (min)" },
        { field: "Estado", title: "Estado" },
        { field: "Pendiente", title: "Pend" },
        { field: "Pendiente14", title: "Pend > 14" }]
    };
    $scope.statsDS = [{
        Vendedor: "Giacomo Guillizani"
    }];

    $scope.averageScale = { min: 0, max: 100 };
    $scope.averageOL = 50;
    $scope.averageVendedores = 25;

    $scope.screenResolution = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/js/rechazos/screen_resolution.json",
                dataType: "json"
            }
        },
        filter: {
            field: "year",
            operator: "eq",
            value: 2009
        }
    });
}
