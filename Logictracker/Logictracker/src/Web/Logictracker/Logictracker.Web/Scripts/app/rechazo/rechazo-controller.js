angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'EntitiesService', RechazoController]);

function RechazoController($scope, EntitiesService) {
    $scope.distritoSelected = {};
    $scope.baseSelected = {};
    $scope.departamentoSelected = {};

    $scope.distritoDS = EntitiesService.distrito.items.query({}, function () {
        $scope.distritoSelected = $scope.distritoDS[0];
        $scope.ddlDistrito.select(0);
    });

    $scope.baseDS = {}

    $scope.departamentoDS = {}

    $scope.$watch("distritoSelected", function (newValue, oldValue) {
        if (newValue) $scope.baseDS = EntitiesService.distrito.bases.query(
            { distritoId: $scope.distritoSelected.Key }
            , function () {
                $scope.baseSelected = $scope.baseDS[0];
                $scope.ddlBase.select(0);
            });
    });

    $scope.$watch("baseSelected", function (newValue, oldValue) {
        if (newValue)
            $scope.departamentoDS = EntitiesService.distrito.departamento.query(
                {
                    distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key
                }, function () {
                    $scope.departamentoSelected = {}
                }
                );
    });

}
