angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', '$filter', RechazoController])
    .controller('RechazoItemController', ['$scope', 'EntitiesService', RechazoItemController])
    .controller('RechazoEditItemController', ['$scope', 'EntitiesService', RechazoEditItemController])
    .controller("RechazoEstadisticasController", ["$scope", "EntitiesService", RechazoEstadisticasController]);


function RechazoController($scope, EntitiesService, $filter) {

    $scope.UserData = EntitiesService.resources.userData.get();
    $scope.UserData.$promise.then(function () {
        if ($scope.UserData.EmpleadoId === 0) {

            onFail({ errorThrown: "Usuario sin empleado asociado" });
        }
    });

    $scope.distritoSelected = [];
    $scope.baseSelected = [];
    $scope.departamentoSelected = [];
    $scope.centroDeCostosSelected = [];
    $scope.transportistaSelected = [];
    $scope.estadoSelected = [];
    $scope.motivoSelected = [];

    $scope.desde = new Date();
    $scope.hasta = new Date();

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$on('errorEvent', function (event, data)
    { onFail(data); });

    function onBaseSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {

            $scope.UserData.DistritoSelected = $scope.distritoSelected.Key;
            $scope.UserData.BaseSelected = $scope.baseSelected.Key;

            $scope.UserData.$save();
        }
    };

    $scope.gridOptions = {
        sortable: true,
        groupable: true,
        scrollable: false,
        pageable: {
            refresh: true,
            pageSizes: true,
            info: true
        },
        columns:
        [
        { template: kendo.template($("#rechazo-sem").html()), width: "10px" },
        { field: "FechaHoraEstado", title: "Fecha Hora", format: "{0: dd/MM HH:mm}", sortable: true },
        { field: "MotivoDesc", title: "Motivo", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { field: "Estado", title: "Estado" },
        { field: "Bultos", title: "Bultos", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { field: "EntregaCodigo", title: "Cod. Entrega", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { field: "VendedorDesc", title: "Vendedor" },
        { field: "SupVenDesc", title: "Sup. Venta", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { field: "SupRutDesc", title: "Jefe de Ventas", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { field: "ChoferDesc", title: "Chofer", headerAttributes: { "class": "grid-colVisible" }, attributes: { "class": "grid-colVisible" } },
        { template: "<a href='\\#' class='link' ng-click='onEdit(dataItem.TicketRechazoId)'>Editar</a>", title: "", width: "5em" }

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

        var filterList = [];

        if ($scope.distritoSelected != undefined)
            filterList.push({ field: "Empresa.Id", operator: "eq", value: $scope.distritoSelected.Key });

        if ($scope.baseSelected != undefined)
            filterList.push({ field: "Linea.Id", operator: "eq", value: $scope.baseSelected.Key });

        var msOffset = new Date().getTimezoneOffset() * 60000;

        if ($scope.desde != undefined) {
            var fDesde = new Date($scope.desde);
            fDesde.setHours(0, 0, 0, 0);
            filterList.push({ field: "FechaHoraEstado", operator: "gte", value: new Date(fDesde.getTime() + msOffset) });
        }

        if ($scope.hasta != undefined) {
            var fHasta = new Date($scope.hasta);
            fHasta.setHours(23, 59, 59, 999);
            filterList.push({ field: "FechaHoraEstado", operator: "lte", value: new Date(fHasta.getTime() + msOffset) });
        }

        if ($scope.motivoSelected.length > 0) {
            var motivoFilters = $scope.motivoSelected.map(function (m) { return { field: "Motivo", operator: "eq", value: m.Key }; });
            filterList.push({ logic: "or", filters: motivoFilters });
        }

        if ($scope.estadoSelected.length > 0) {
            var estadoFilters = $scope.estadoSelected.map(function (e) { return { field: "UltimoEstado", operator: "eq", value: e.Key }; });
            filterList.push({ logic: "or", filters: estadoFilters });
        }

        if ($scope.transportistaSelected.length > 0) {
            var transportistaFilter = $scope.transportistaSelected.map(function (e) { return { field: "Transportista.Id", operator: "eq", value: e.Key }; });
            filterList.push({ logic: "or", filters: transportistaFilter });
        }


        var filters = {
            logic: "and",
            filters: filterList
        };

        $scope.rechazosDS = EntitiesService.ticketrechazo.items(filters, null, onFail);

    };

    $scope.onRefreshWindow = function () {
        $scope.rechazoWin.center();
    }

    function onFail(error) {
        if (!$scope.notify) return;
        try {
            if (error.data.ExceptionMessage) {
                $scope.notify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) { }

        if (error.errorThrown !== undefined)
            $scope.notify.show(error.errorThrown, "error");
        else
            $scope.notify.show(error, "error");
    }

}

function RechazoItemController($scope, EntitiesService) {

    $scope.motivoSelected = {};
    $scope.estadoSelected = {};
    $scope.clienteSelected = "";
    $scope.puntoEntregaSelected = {};
    $scope.distribucionSelected = {};
    $scope.transportistaSelected = {};
    $scope.supervisorRutaSelected = {};
    $scope.supervisorVentasSelected = {};
    $scope.vendedorSelected = {};
    $scope.territorio = "";
    $scope.enHorarioSelected = {};
    $scope.movimientosDS = {};
    $scope.choferSelected = {};

    $scope.disabledButton = false;

    $scope.codigoVendedor = "V";
    $scope.codigoSupervisorRutas = "SR";
    $scope.codigoChofer = "Cho";

    $scope.enHorarioDS = [
        { Key: true, Value: "Si" },
        { Key: false, Value: "No" },
    ];

    $scope.notifyShow = function (message, type) {
        $scope.notify.show(message, type);
    };

    
    $scope.tDistribucion = kendo.template($("#tDistribucion").html());
    $scope.tPuntoEntrega = kendo.template($("#tPuntoEntrega").html());

    $scope.parametros = EntitiesService.resources.parametros.query({ distritoId: $scope.distritoSelected.Key }, paramOnLoad);

    function paramOnLoad(e) {
        if (e !== undefined && e.length > 0) {
            angular.forEach(e, function (value) {
                switch (value.Key) {
                    case "CodigoSupervisorRutas":
                        $scope.codigoSupervisorRutas = value.Value;
                        break;
                    case "CodigoVendedor":
                        $scope.codigoVendedor = value.Value;
                        break;
                }
            });
        }
    }

    $scope.gridDetalleOptions = {
        columns:
        [
            { field: "TicketRechazoDetalleId", title: "Id" },
            { field: "FechaHora", title: "Fecha Hora" },
            { field: "EmpleadoDesc", title: "Empleado" },
            { field: "Estado", title: "Estado" },
            { field: "Observacion", title: "Observacion", encoded: false },
        ]
    };

    $scope.onClose = function () {
        $scope.rechazoWin.close();
    };

    $scope.onSave = function (event) {
        event.preventDefault();

        if ($scope.validator.validate()) {

            $scope.disabledButton = true;

            var ticketRechazo = {
                DistritoId: $scope.distritoSelected.Key,
                LineaId: $scope.baseSelected.Key,
                ClienteId: $scope.puntoEntregaSelected != null && $scope.puntoEntregaSelected[0] !== undefined ? $scope.puntoEntregaSelected[0].ClienteId : "",
                Territorio: $scope.territorio,
                Bultos: $scope.bultos,
                VendedorId: $scope.vendedorSelected.EmpleadoId,
                SupRutDesc: $scope.supervisorRutaSelected ? $scope.supervisorRutaSelected.Descripcion : "",
                SupVenDesc: $scope.supervisorVentasSelected ? $scope.supervisorVentasSelected.Descripcion : "",
                SupVenId: $scope.supervisorVentasSelected.EmpleadoId,
                SupRutId: $scope.supervisorRutaSelected.EmpleadoId,
                Motivo: $scope.motivoSelected.Key,
                Estado: $scope.estadoSelected.Key,
                Observacion: $scope.observacion,
                EnHorario: $scope.enHorarioSelected.Key,
                EntregaId: $scope.puntoEntregaSelected[0] ? $scope.puntoEntregaSelected[0].PuntoEntregaId : 0,
                TransportistaId: $scope.transportistaSelected.Key,
                ChoferId: $scope.choferSelected.EmpleadoId,
                ChoferDesc: $scope.choferSelected ? $scope.choferSelected.Descripcion : "",
            };

            EntitiesService.resources.ticketRechazo.save(ticketRechazo).$promise.then(
                function () {

                    $scope.disabledButton = false;
                    $scope.mainGrid.refresh();
                    $scope.rechazoWin.close();
                }, onFail);
        }
    };

    function onFail(error) {
        try {
            $scope.disabledButton = false;
            if (error.data.ExceptionMessage) {
                $scope.notify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) { }
        $scope.notify.show(error.errorThrown, "error");
    };
}

function RechazoEditItemController($scope, EntitiesService) {

    $scope.ticketItem = EntitiesService.resources.ticketRechazo.get({ id: $scope.ticketItemId });
    $scope.gridDetalleOptions = {
        columns:
        [
            { field: "TicketRechazoDetalleId", title: "Id", width: "5em" },
            { field: "FechaHora", title: "Fecha Hora", width: "13em", template: "#: kendo.toString(kendo.parseDate(FechaHora),'G') #" },
            { field: "EmpleadoDesc", title: "Empleado", width: "8em" },
            { field: "Estado", title: "Estado", width: "10em" },
            { field: "Observacion", title: "Observacion" },
        ],
        sortable: {
            mode: "single",
            allowUnsort: false
        },
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

            EntitiesService.resources.ticketRechazo.update({ id: $scope.ticketItemId }, ticketRechazo).$promise.then(
                function () {
                    $scope.rechazosDS.read();
                    $scope.mainGrid.refresh();
                    $scope.rechazoWin.close();
                },
                onFail);


        }
    };
}

function RechazoEstadisticasController($scope, EntitiesService) {

    $scope.UserData = EntitiesService.resources.userData.get({}, function () { });

    $scope.autoRefesh = true;
    $scope.averageScale = {
        min: 0, max: 100, ranges: [{ from: 0, to: 30, color: "green" },
                                   { from: 30, to: 60, color: "yellow" },
                                   { from: 60, to: 100, color: "red" }]
    };
    $scope.chartCantitdadPorEstadoSerieDefault = {
        labels: {
            visible: true,
            position: "insideEnd",
            background:"transparent"
        },        
        type: "pie"
    };
    $scope.distritoSelected = {};
    $scope.baseSelected = {};
    $scope.transportistaSelected = [];
    $scope.opcionesGrillaVendedor = {
        columns: [
            { field: "Usuario", title: "Usuario" },
            { field: "EstadoIngreso", title: "De estado" },
            { field: "EstadoEgreso", title: "A estado" },
            { field: "Intervinio", title: "Intervinio en" },
            { field: "Promedio", title: "Promedio (min)" }
        ],
        
        sortable: {
            mode: "single",
            allowUnsort: false
        },
        pageable: true
    };
    $scope.opcionesGrillaEstados = {
        columns: [
            { field: "Estado", title: "Estado" },
            { field: "Promedio", title: "Promedio (min)", format: "{0:0}" }
        ],
        sortable: {
            mode: "single",
            allowUnsort: false
        },
        pageable: true
    };

    $scope.$watch("baseSelected", onBaseSelected);

    var chartData = [{ "name": "cantidad", "value": 1, "time": "07:00" }, { "name": "cantidad", "value": 2, "time": "08:00" }, { "name": "cantidad", "value": 1, "time": "09:00" }, { "name": "cantidad", "value": 1, "time": "10:00" }, { "name": "cantidad", "value": 4, "time": "11:00" }, { "name": "cantidad", "value": 5, "time": "12:00" }, { "name": "cantidad", "value": 4, "time": "13:00" }, { "name": "cantidad", "value": 4, "time": "14:00" }, { "name": "cantidad", "value": 4, "time": "15:00" }, { "name": "cantidad", "value": 4, "time": "16:00" }, { "name": "cantidad", "value": 4, "time": "17:00" }, { "name": "cantidad", "value": 4, "time": "18:00" }, { "name": "cantidad", "value": 3, "time": "19:00" }];

    $scope.chartDataSource = new kendo.data.DataSource({ data: chartData });

    $(window).bind("resize", function () {

        $("div[kendo-chart]").each(function () {
            $(this).data("kendoChart").refresh();
        });

        $("span[kendo-radialgauge]").each(function () {
            $(this).data("kendoRadialGauge").redraw();
        });
    });

    function onBaseSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {

            $scope.UserData.DistritoSelected = $scope.distritoSelected.Key;
            $scope.UserData.BaseSelected = $scope.baseSelected.Key;

            $scope.UserData.$save();
        }
    };

    function onPromediosPorRolLoad() {
        $scope.promedioVendedor = $scope.promediosPorRol.vendedor;
        $scope.promedioSupervisorVentas = $scope.promediosPorRol.supervisorVentas;
        $scope.promedioJefeVentas = $scope.promediosPorRol.jefeVentas;
    }

    function onFail(error) {
        try {
            if (error.data.ExceptionMessage) {
                $scope.notify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) {
        }
        $scope.notify.show(error.errorThrown, "error");
    };

    $scope.onBuscar = function () {
        $scope.promediosPorRol = EntitiesService.resources.estadisticasPorRol.get({
            distritoId: $scope.distritoSelected.Key,
            baseId: $scope.baseSelected.Key
        }, onPromediosPorRolLoad);

        var filterList = [];

        if ($scope.distritoSelected != undefined)
            filterList.push({ field: "EmpresaId", operator: "eq", value: $scope.distritoSelected.Key });

        if ($scope.baseSelected != undefined)
            filterList.push({ field: "BaseId", operator: "eq", value: $scope.baseSelected.Key });

        if ($scope.transportistaSelected.length > 0) {
            var transportistaFilter = $scope.transportistaSelected.map(function (e) { return { field: "Transportista.Id", operator: "eq", value: e.Key }; });
            filterList.push({ logic: "or", filters: transportistaFilter });
        }

        var filter = {
            logic: "and",
            filters: filterList
        };

        $scope.datosGrillaVendedor = EntitiesService.ticketrechazo.promedioPorVendedor(filter, null, onFail);

        $scope.datosGrillaEstados = EntitiesService.ticketrechazo.promedioPorEstado(filter, null, onFail);

        EntitiesService.resources.cantidadPorEstado.query({
            distritoId: $scope.distritoSelected.Key,
            baseId: $scope.baseSelected.Key
        }, function (data) {
            //debugger;
            $scope.chartCantitdadPorEstado = data;
        });
    };

    $scope.onAutoRefreshClick = function () {
    };
}
