angular.module('logictracker.rechazo.directives', [])
.directive('ltRechazoDistrito', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list="ddlDistrito" ' +
                    'k-data-source="distritoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="distritoSelected"/>'
    };
})
.directive('ltRechazoBase', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-drop-down-list="ddlBase" ' +
                    'k-data-source="baseDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="baseSelected" />'
    };
})
.directive('ltRechazoDepartamento', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-multi-select ' +
                    'k-data-source="departamentoDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="departamentoSelected" />'
    };
})
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
.directive('ltRechazoCentroDeCostos', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-multi-select ' +
                    'k-data-source="centroDeCostosDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="centroDeCostosSelected" />'
    };
})
.directive('ltRechazoTransportista', function () {
    return {
        restrict: 'E',
        template: '<input class="form-control" kendo-multi-select ' +
                    'k-data-source="transportistaDS" ' +
                    'k-data-text-field="\'Value\'" ' +
                    'k-data-value-field="\'Key\'" ' +
                    'k-ng-model="transportistaSelected" />'
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