angular
    .module('logictracker.ordenes.controller', ['kendo.directives'])
    .controller('OrdenesController', ['$scope', 'EntitiesService', 'OrdenesService', OrdenesController]);

function OrdenesController($scope, EntitiesService, OrdenesService) {
    //$scope.mydata = "Seleccione los filtros necesarios y haga click en Buscar...";

    $scope.ordenesGridOptions = {
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
            { template: '<input type="checkbox" ng-model="dataItem.Selected">', width: "10px" },
            { field: "Empresa", title: "Empresa"},
            { field: "Empleado", title: "Empleado"},
            { field: "Transportista", title: "Transportista"},
            { field: "PuntoEntrega", title: "Entrega"},
            { field: "CodigoPedido", title: "Codigo"},
            { field: "FechaAlta", title: "Registrado", format: "{0: dd/MM HH:mm}" },
            { field: "FechaPedido", title: "Pedido", format: "{0: dd/MM HH:mm}" },
            { field: "FechaEntrega", title: "Entrega", format: "{0: dd/MM HH:mm}" },
            { field: "InicioVentana", title: "Inicio"},
            { field: "FinVentana", title: "Fin"}
        ]
    }


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
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(function () { $scope.estadoSelected = $scope.estadoDS[0]; },onFail);

    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(function () { $scope.motivoSelected = $scope.motivoDS[0]; },onFail);

    $scope.baseDS = EntitiesService.distrito.bases(onBaseDSLoad, onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento(onDepartamentoDSLoad, onFail);

    $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos(onCentroDeCostosDSLoad,onFail);
    
    $scope.transportistaDS = EntitiesService.distrito.transportista.models(ontransportistaDSLoad,onFail);

    $scope.$watch("distritoSelected", onDistritoSelected);

    $scope.$watch("basesDS", onBasesDSChange);

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$watchGroup(["departamentoSelected", "baseSelected"],onDepartamentoAndBaseChange);

    function onDistritoDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.distritoSelected = e.response[0];
        }
    };

    function onBaseDSLoad(e) {
        if (e.type === "read" && e.response) {
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

    $scope.onNuevo = function () {
        $scope.operacion = "A";
        $scope.rechazoWin.refresh({ url: "Item?op=A" }).open().center();
    };

        $scope.onerror = function (error) {
            $scope.notify.show(error.statusText, "error");
        }
    
        $scope.onNuevo = function () {
            $scope.rechazoWin.refresh({ url: "Item?op=A" });
            $scope.rechazoWin.center();
            $scope.rechazoWin.open();
        };

        $scope.onBuscar = function () {
            //$scope.mydata = "Cargando...";

            //var transportista = {};
            //if ($scope.transportistaSelected.length > 0)
            //    transportista = $scope.transportistaSelected[0].Key;
            //else
            //    transportista = -1;
            //alert(transportista);
            //$scope.OrderList = OrdenesService.list.query({
            //        distritoId: $scope.distritoSelected.Key,
            //        baseId: $scope.baseSelected.Key
            //    }, function (s, o) {
            //    $scope.mydata = $scope.OrderList.length + " registros";
            //});

            $scope.Orders = OrdenesService.items({ distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key }, null, $scope.onerror);
        };

        $scope.programOrders = function (order) {
            $scope.newOrder = new OrdenesService.list();
            $scope.newOrder.OrderList = $scope.Orders.data();
            $scope.newOrder.RouteCode = order.RouteCode;
            $scope.newOrder.Vehicle = order.IdVehicle;
            $scope.newOrder.StartDateTime = order.StartDateTime;
            $scope.newOrder.LogisticsCycleType = order.LogisticsCycleType.Key;
            $scope.newOrder.$save(
                { customerId: $scope.distritoSelected.Key },
                function () {
                    $scope.onBuscar();
                },
                onFail
            );
        };
    }



