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

    $scope.distritoDS = EntitiesService.distrito.items();
    $scope.distritoDS.bind("requestEnd", onDistritoDSLoad);
    $scope.distritoDS.bind("error", onFail);

    $scope.baseDS = EntitiesService.distrito.bases();
    $scope.baseDS.bind("requestEnd", onBaseDSLoad);
    $scope.baseDS.bind("error", onFail);

    $scope.departamentoDS = EntitiesService.distrito.departamento();
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
        if (newValue !== oldValue) {
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
        if (newValue !== oldValue)
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
    };
}
