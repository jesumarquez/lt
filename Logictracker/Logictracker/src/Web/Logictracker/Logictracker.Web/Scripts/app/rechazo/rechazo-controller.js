angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', RechazoController]);

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

    $scope.estadoSelected = {};

    $scope.desde = new Date();
    $scope.hasta = new Date();


    $scope.estadoDS = EntitiesService.ticketrechazo.estados.query(null,
        function() { $scope.estadoSelected = $scope.estadoDS[0]; },
        $scope.onerror);
    
    $scope.distritoDS = EntitiesService.distrito.items.query({}, function () {
        $scope.distritoSelected = $scope.distritoDS[0];
    }, $scope.onerror);


    $scope.$watch("distritoSelected", function (newValue, oldValue) {
        if (newValue !== oldValue)
            $scope.baseDS = EntitiesService.distrito.bases.query(
         { distritoId: $scope.distritoSelected.Key }
         , function () {
             $scope.baseSelected = $scope.baseDS[0];
         },$scope.onerror);
    });

    $scope.$watch("basesDS", function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoSelected = [];
            $scope.centroDeCostosSelected = [];
        }
    });

    $scope.$watch("baseSelected", function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoDS = EntitiesService.distrito.departamento.query(
                {
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.departamentoSelected = [];

                },$scope.onerror
            );
            $scope.transportistaDS = EntitiesService.distrito.transportista.query(
                {
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.transportistaSelected = [];

                }, $scope.onerror
            );
        }
    });

    $scope.$watchGroup(["departamentoSelected", "baseSelected"], function (newValue, oldValue) {
        if (newValue !== oldValue)
            $scope.centroDeCostosDS = EntitiesService.distrito.centroDeCostos.query(
            {
                distritoId: $scope.distritoSelected.Key,
                baseId: $scope.baseSelected.Key,
                deptoId: $scope.departamentoSelected.map(function (o) {
                    return o.Key;
                })
            }, function () {
                $scope.centroDeCostosSelected = [];
            }, $scope.onerror);
    });

    $scope.onerror = function(error) {
        $scope.notify.show(error.statusText, "error");
    }

}
