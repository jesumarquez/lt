angular.module('logictracker.rechazo.directives', [])

.directive("ltPuntoEntrega", function () {
    return {
        restrict: "E",
        template: '<input class="form-control" kendo-drop-down-list' +
                    'k-data-source="" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model=\"puntoEntregaSelected\" ' +
                   '/>'

    };
})