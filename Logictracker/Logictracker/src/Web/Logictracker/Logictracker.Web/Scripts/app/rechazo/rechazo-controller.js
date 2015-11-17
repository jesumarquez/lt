angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', RechazoController])
    .controller('RechazoItemController', ['$scope', 'EntitiesService', RechazoItemController]);

function RechazoController($scope, EntitiesService) {

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
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(function () { $scope.estadoSelected = $scope.estadoDS[0]; },
        onFail);

    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(function () { $scope.motivoSelected = $scope.motivoDS[0]; },
        onFail);

    $scope.baseDS = EntitiesService.distrito.bases(onBaseDSLoad, onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento(onDepartamentoDSLoad, onFail);

    $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos(onCentroDeCostosDSLoad,onFail);
    
    $scope.transportistaDS = EntitiesService.distrito.transportista(ontransportistaDSLoad,onFail);

    $scope.$watch("distritoSelected", onDistritoSelected);

    $scope.$watch("basesDS", onBasesDSChange);

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$watchGroup(["departamentoSelected", "baseSelected"],
        onDepartamentoAndBaseChange);

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

    $scope.rechazosDS = [];

    $scope.gridOptions = {
        columns:
        [
        { field: "Id", title: "Ticket" },
        { field: "FechaHora", title: "Fecha Hora" },
        { field: "ClienteCod", title: "Cod. Cliente" },
        { field: "ClienteDesc", title: "Cliente" },
        { field: "SupVenDesc", title: "Sup. Venta" },
        { field: "SupRutDesc", title: "Sup. Ruta" },
        { field: "UltEstado", title: "Estado" },
        { field: "Territorio", title: "Territorio" },
        { field: "Motivo", title: "Motivo" },
        { field: "Bultos", title: "Bultos" },
        ]
    }

    $scope.onNuevo = function () {
        $scope.operacion = "A";
        $scope.rechazoWin.refresh({ url: "Item?op=A" }).open().center();
    };

    $scope.onEdit = function (id) {
        $scope.operacion = "E";
        $scope.rechazoWin.refresh({ url: "Item?op=E&id=" + id }).open().center();
    }

    $scope.onBuscar = function () {
    };

}

function RechazoItemController($scope, EntitiesService) {

    // A = alta , M = modification
    //$scope.operation = "A";


    $scope.motivoSelected = {};
    $scope.motivoDS = EntitiesService.ticketrechazo.motivos(function () { $scope.motivoSelected = $scope.motivoDS[0]; }, $scope.onFail);
    // El motivo es editable solo si es un alta
    $scope.motivoRO = function () { return !isNew(); };


    $scope.estadoSelected = {};
    $scope.estadoDS = EntitiesService.ticketrechazo.estados(function () { $scope.estadoSelected = $scope.estadoDS[0]; }, $scope.onFail);
    $scope.estadoRO = true;

    
    $scope.entregaSelected = {};
    $scope.entregaRO = true;

    $scope.clienteSelected = {};
    $scope.clienteRO = true;
    $scope.clienteDS = EntitiesService.distrito.clientes.models({distritoId:$scope.distritoSelected.Key,baseId:$scope.baseSelected.Key},null,$scope.onFail); // [{codigo:"12"},{codigo:"33"}];

    $scope.distribucionDS = EntitiesService.distrito.distribuciones.models({distritoId:$scope.distritoSelected.Key,baseId:$scope.baseSelected.Key},null,$scope.onFail);
    $scope.distribucionSeñected = {};
    $scope.distribucionRO = true;

    $scope.supervisorRutaSelected = {};
    $scope.supervisorRutaRO = true;

    $scope.supervisorVentasSelected = {};
    $scope.supervisorVentas = true;

    $scope.territorio = "";
    $scope.territorioRO = true;

    $scope.enHorarioSelected = {};
    $scope.enHorarioRO = true;

    $scope.movimientosDS = {};

    function isNew() { return $scope.operation === "A"; }

    $scope.tDistribucion = kendo.template($("#tDistribucion").html());
}

