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

    $scope.distritoDS = EntitiesService.distrito.items.query(
            {},
            onDistritoDSLoad,
            function () { },
            onFail);


    $scope.$watch("distritoSelected", onDistritoSelected);

    $scope.$watch("basesDS", onBasesDSChange);

    $scope.$watch("baseSelected", onBaseSelected);

    $scope.$watchGroup(["departamentoSelected", "baseSelected"], onDepartamentoAndBaseChange);

    function onDistritoDSLoad() {
        $scope.distritoSelected = $scope.distritoDS[0];
    };

    function onFail(error) {
        $scope.notify.show(error.statusText, "error");
    };

    function onDistritoSelected(newValue, oldValue) {
        if (newValue !== oldValue)
            $scope.baseDS = EntitiesService.distrito.bases.query(
         { distritoId: $scope.distritoSelected.Key }
         , function () {
             $scope.baseSelected = $scope.baseDS[0];
         }, function (error) {
             $scope.notify.show(error.statusText, "error");
         });
    };

    function onBasesDSChange(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoSelected = [];
            $scope.centroDeCostosSelected = [];
        }
    };

    function onBaseSelected(newValue, oldValue) {
        if (newValue !== oldValue) {
            $scope.departamentoDS = EntitiesService.distrito.departamento.query(
                {
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.departamentoSelected = [];

                }, function (error) {
                    $scope.notify.show(error.statusText, "error");

                }
            );
            $scope.transportistaDS = EntitiesService.distrito.transportista.query(
                {
                    distritoId: $scope.distritoSelected.Key,
                    baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.transportistaSelected = [];

                }, function (error) {
                    $scope.notify.show(error.statusText, "error");

                }
            );
        }
    };

    function onDepartamentoAndBaseChange(newValue, oldValue) {
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
    };
}
