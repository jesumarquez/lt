angular.module('logictracker.ordenes.directives', [])
.directive('ltOrdenesDistrito', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list="ddlDistrito" ' +
                    'k-data-source="distritoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="distritoSelected"/>'
    };
})
.directive('ltOrdenesBase', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list="ddlBase" ' +
                    'k-data-source="baseDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="baseSelected" />'
    };
})
.directive('ltOrdenesDepartamento', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-multi-select ' +
                    'k-data-source="departamentoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="departamentoSelected" />'
    };
})
.directive('ltOrdenesEstado', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list ' +
                    'k-data-source="estadoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="estadoSelected" />'
    };
})
.directive('ltOrdenesChkProducto', function () {

    var controller = ['$scope','$log', function ($scope,$log) {

        $scope.$log = $log;

        $scope.checked = false;

        $scope.$watch('checked', function(newValue,oldValue) {
            $scope.$log.info(newValue);
            // cambiar el estado deacuerdo al check usando data
        });

        function init() {
            // verificar si esta checkeado desde data
        }

        init();
    }];

    return {
        controller: controller,
        retrict: 'E',
        replace: true,
        scope: {
            data: '=',
            checked: '='
        },
        template: '<input class="checkbox" type="checkbox" ng-model="checked" />'
    };
})
;