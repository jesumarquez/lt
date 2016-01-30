angular
    .module('logictracker.ordenes.controller', ['kendo.directives'])
    .controller('OrdenesController', ['$scope', 'EntitiesService', 'OrdenesService', OrdenesController]);

function OrdenesController($scope, EntitiesService, OrdenesService) {
    //$scope.mydata = "Seleccione los filtros necesarios y haga click en Buscar...";
    $scope.UserData = EntitiesService.resources.userData.get();
    $scope.UserData.$promise.then(function () {
        if ($scope.UserData.EmpleadoId === 0) {

            onFail({ errorThrown: "Usuario sin empleado asociado" });
        }
    });

    $scope.order = {
        StartDateTime: new Date()
    };
    $scope.order.StartDateTime.setDate($scope.order.StartDateTime.getDate() + 1);

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

    $scope.desde = new Date();
    $scope.hasta = new Date();

    $scope.estadoSelected = {};
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(function () { $scope.estadoSelected = $scope.estadoDS[0]; },onFail);

    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(function () { $scope.motivoSelected = $scope.motivoDS[0]; },onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento(onDepartamentoDSLoad, onFail);

    $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos(onCentroDeCostosDSLoad,onFail);
    
    $scope.transportistaDS = EntitiesService.distrito.transportista.models(ontransportistaDSLoad,onFail);

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$watchGroup(["departamentoSelected", "baseSelected"],onDepartamentoAndBaseChange);

    function onFail(error) {
        if(error.errorThrown)
            $scope.notify.show(error.errorThrown, "error");
        else
            $scope.notify.show(error.statusText, "error");
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

    //$scope.onNuevo = function () {
    //    $scope.operacion = "A";
    //    $scope.rechazoWin.refresh({ url: "Item?op=A" }).open().center();
    //};

        $scope.onerror = function (error) {
            $scope.notify.show(error.statusText, "error");
        }
    
        $scope.onNuevo = function () {
            $scope.rechazoWin.refresh({ url: "Item?op=A" });
            $scope.rechazoWin.center();
            $scope.rechazoWin.open();
        };

        $scope.onBuscar = function () {
            var filterList = [];

            if ($scope.distritoSelected != undefined)
                filterList.push({ field: "Empresa.Id", operator: "eq", value: $scope.distritoSelected.Key });

            if ($scope.baseSelected != undefined)
                filterList.push({ field: "Linea.Id", operator: "eq", value: $scope.baseSelected.Key });

            if ($scope.transportistaSelected.length > 0) {
                var transportistaFilter = $scope.transportistaSelected.map(function (e) { return { field: "Transportista.Id", operator: "eq", value: e.Key }; });
                filterList.push({ logic: "or", filters: transportistaFilter });
            }

            var filters = {
                logic: "and",
                filters: filterList
            };

            $scope.Orders = OrdenesService.items(filters, null, onFail);
        };

        $scope.programOrders = function (order) {
            $scope.newOrder = new OrdenesService.ordenes();
            $scope.newOrder.OrderList = $scope.Orders.data();
            $scope.newOrder.IdVehicle = order.Vehicle.Key;
            $scope.newOrder.StartDateTime = order.StartDateTime;
            $scope.newOrder.LogisticsCycleType = order.LogisticsCycleType.Key;
            $scope.newOrder.$save(
                { distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key},
                function () {
                    $scope.onBuscar();
                },
                onFail
            );
        };
    }



