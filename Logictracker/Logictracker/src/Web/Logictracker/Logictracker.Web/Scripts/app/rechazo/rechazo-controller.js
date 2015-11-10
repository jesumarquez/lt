angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', RechazoController])
    .controller('RechazoItemController',['$scope', 'EntitiesService', RechazoItemController]);



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

    $scope.distritoDS = EntitiesService.distrito.items();
    $scope.distritoDS.bind("requestEnd", onDistritoDSLoad);
    $scope.distritoDS.bind("error", onFail);
    $scope.estadoSelected = {};

    $scope.desde = new Date();
    $scope.hasta = new Date();


    $scope.estadoDS = EntitiesService.ticketrechazo.estados.query(null,
        function () { $scope.estadoSelected = $scope.estadoDS[0]; },
        $scope.onerror);


    $scope.baseDS = EntitiesService.distrito.bases();
    $scope.baseDS.bind("requestEnd", onBaseDSLoad);
    $scope.baseDS.bind("error", onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento();
    $scope.departamentoDS.bind("requestStart", function (e) {
        console.log("request started");
    });
    $scope.departamentoDS.bind("requestEnd", onDepartamentoDSLoad);
    $scope.departamentoDS.bind("error", onFail);

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
    function onBaseDSLoad(e){
        if (e.type === "read" && e.response) {
            $scope.baseSelected = e.response[0];
        }
    }

    function onFail(error) {
        $scope.notify.show(error.errorThrown, "error");
    };

    function onDistritoSelected(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.baseDS.read({ distritoId: $scope.distritoSelected.Key })
        }
    };

    function onBasesDSChange(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoSelected =[];
            $scope.centroDeCostosSelected =[];
        }
    };

    function onDepartamentoDSLoad(e) {
        if (e.type === "read" && e.response) {
            $scope.departamentoSelected = [];
        }
    }

    function onBaseSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {
            $scope.departamentoDS.read({
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key
            });
               
            $scope.transportistaDS = EntitiesService.distrito.transportista.query(
                {
                        distritoId: $scope.distritoSelected.Key,
                        baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.transportistaSelected =[];

                }, function (error) {
                    $scope.notify.show(error.statusText, "error");

            }
            );
    }
    };

    function onDepartamentoAndBaseChange(newValue, oldValue) {
        if (newValue[0] !== undefined && newValue[0].length > 0 && newValue != null && newValue !== oldValue)
            $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos.query(
            {
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key,
                    deptoId: $scope.departamentoSelected.map(function (o) {
                        return o.Key;
            })
            }, function () {
                $scope.centroDeCostosSelected =[];
            }, function (error) {
                $scope.notify.show(error.statusText, "error");
    });

    $scope.onerror = function (error) {
        $scope.notify.show(error.statusText, "error");
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
        $scope.rechazoWin.refresh({ url: "Item?op=A" });
        $scope.rechazoWin.center();
        $scope.rechazoWin.open();
    };

    $scope.onEdit = function(id) {
        $scope.rechazoWin.refresh({ url: "Item?op=E&id=" + id });   
        $scope.rechazoWin.center();
        $scope.rechazoWin.open();
    }

    $scope.onBuscar = function() {

    };

}

function RechazoItemController($scope, EntitiesService) {
    $scope.mensaje = "Soy en numero 4";
}