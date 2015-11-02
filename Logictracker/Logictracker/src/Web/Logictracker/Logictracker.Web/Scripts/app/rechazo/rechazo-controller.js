angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', RechazoController]);

function RechazoController($scope, EntitiesService) {

    $scope.distritoSelected = {};

    $scope.baseDS = [];
    $scope.baseSelected = {};


    $scope.departamentoSelected = [];
    $scope.centroDeCostosSelected = [];

    $scope.departamentoDS = [];
    $scope.centroDeCostosDS = [];

    $scope.distritoDS = EntitiesService.distrito.items.query({}, function () {
        $scope.distritoSelected = $scope.distritoDS[0];
    }, function () { }
        , function (error) {
            $scope.notify.show(error.statusText, "error");
        });


    $scope.$watch("distritoSelected", function (newValue, oldValue) {
        if (newValue !== oldValue)
            $scope.baseDS = EntitiesService.distrito.bases.query(
         { distritoId: $scope.distritoSelected.Key }
         , function () {
             $scope.baseSelected = $scope.baseDS[0];
         }, function (error) {
             $scope.notify.show(error.statusText, "error");
         });
    });

    $scope.$watch("basesDS", function (newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoSelected = [];
            $scope.centroDeCostosSelected = [];
        }
    });
    
    $scope.$watch("baseSelected", function (newValue, oldValue) {
        if (newValue !== oldValue)
            $scope.departamentoDS = EntitiesService.distrito.departamento.query(
                {
                    distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.departamentoSelected = [];
                }, function (error) {
                    $scope.notify.show(error.statusText, "error");

                }
                );
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
            }, function (error) {
                $scope.notify.show(error.statusText, "error");
            });
    });
}
