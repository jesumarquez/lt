angular.module('logictracker.ordenes.directives', ['angular.filter'])
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
.directive('orderDetail', OrderDetailDirective)
.directive('summaryProductsSelected', SummaryProductsSelected);

function OrderDetailDirective() {
    var controller = function ($scope, OrdenesService) {
        var vm = this;

        vm.gridOptions = {
            scrollable: false,
            sortable: true,
            columns: [
                { field: "Insumo", title: "Producto", width: "160px" },
                { field: "Cantidad", title: "Litros" },
                { template: "<input type='checkbox' ng-model='dataItem.checked' ng-change='productos.onSelected(dataItem)'/>" }
            ]
        };

        vm.ds = OrdenesService.ordenDetalles(vm.orderId, [], onDSLoad, onFail);

        vm.onSelected = function (data) {
            var index = -1;

            var found = $.grep(vm.selectedList, function (item) {
                return item.Id === data.Id;
            });

            if (found.length > 0) {
                index = vm.selectedList.indexOf(found[0]);
            }

            if (index > -1) {
                vm.selectedList.splice(index, 1);
            }
            else {
                vm.selectedList.push(data);
            }
        };

        $scope.$on('onClearProductsSelected', function (event, args) {
            vm.ds.read();
        });

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $.each(vm.selectedList, function (index, item) {
                    $.each(e.response, function (dsIndex, dsItem) {
                        if (dsItem.Id === item.Id) {
                            dsItem.checked = true;
                        }
                    });
                });
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        };
    };

    return {
        restrict: 'E',
        scope: {
            orderId: "=ltNgOrderId",
            selectedList: "=ltNgSelectedList"
        },
        controller: ['$scope', 'OrdenesService', controller],
        controllerAs: 'productos',
        bindToController: true,
        template: [
            '<div kendo-grid ',
            'k-options="productos.gridOptions" ',
            'k-data-source="productos.ds"></div>'
        ].join('')
    };

}

function SummaryProductsSelected() {
    var controller = function ($scope) {
        var vm = this;

        vm.totalByProduct = function (items) {
            var total = 0;
            $.each(items, function (i, item) {
                total += item.Cantidad;
            });
            return total;
        };

        vm.clearSelection = function () {
            vm.selectedList.splice(0, vm.selectedList.length);
            $scope.$parent.$broadcast('onClearProductsSelected', {});
        };
    };

    return {
        restrict: 'E',
        scope: {
            selectedList: "=ltNgSelectedList"
        },
        controller: ['$scope', controller],
        controllerAs: 'summary',
        bindToController: true,
        template: [
            '<div class="SummaryContainer" ng-show="summary.selectedList.length >0">',
                '<span class="SummaryContainer-clear" ng-click="summary.clearSelection()">x</span>',
                '<ul>',
                    '<li class="SummaryItem" ng-repeat="(key, value) in summary.selectedList | groupBy: \'Insumo\'">',
                        '{{key}} ({{value.length}}): {{summary.totalByProduct(value)}}',
                    '</li>',
                '</ul>',
            '</div>'
        ].join('')
    };

}
