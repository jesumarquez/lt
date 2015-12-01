angular.module('logictracker.rechazo.directives', [])
.directive('ltRechazoEstado', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list ' +
                    'k-data-source="estadoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="estadoSelected" />'
    };
})
.directive("ltRechazoMotivo", function () {
    return {
        restrict: "E",
        template: '<input class="form-control" kendo-drop-down-list ' +
                    'k-data-source="motivoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model=\"motivoSelected\" ' +
                   '/>'

    };
})
.directive("ltPuntoEntrega", function () {
    return {
        restrict: "E",
        template: '<input class="form-control" kendo-drop-down-list ' +
                    'k-data-source="" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model=\"puntoEntregaSelected\" ' +
                   '/>'

    };
})