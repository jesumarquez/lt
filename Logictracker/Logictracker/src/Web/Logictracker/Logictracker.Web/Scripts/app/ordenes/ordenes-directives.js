angular.module('logictracker.ordenes.directives', ['angular.filter'])
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

    var controller = ['$scope', '$log', function ($scope, $log) {

        $scope.$log = $log;

        $scope.checked = false;

        $scope.$watch('checked', function (newValue, oldValue) {
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
                { field: "EstadoDescripcion", title: "Estado"},
                { template: "<input type='checkbox' ng-model='dataItem.checked' ng-change='productos.onSelected(dataItem)'/>" }
            ]
        };

        vm.ds = OrdenesService.ordenDetalles(vm.orderId, [], onDSLoad, onFail);

        vm.onSelected = function (data) {
            var index = vm.selectedList.toJSON().findIndex(function (item) {
                return item.Id === data.Id;
            }, data);

            data.NumRuta = undefined;
            data.OrdenRuta = undefined;

            if (index > -1) {
                vm.selectedList.splice(index, 1);
                data.Ajuste = 0;
                data.Cuaderna = 0;
            }
            else {
                vm.selectedList.push(data);                
            }            
        };

        $scope.$watch(function () { return vm.orderId; }, onOrderChange)
        $scope.$watchCollection(function () { return vm.selectedList; }, onSelectedListChange);

        function onOrderChange(newValue, oldValue) {
            if (newValue !== oldValue) {
                vm.ds = OrdenesService.ordenDetalles(vm.orderId, [], onDSLoad, onFail);
            }
        };

        function onSelectedListChange(newValue, oldValue) {
            if (newValue !== oldValue) {
                updateSelected(vm.selectedList, vm.ds.data());
            }
        };

        function updateSelected(source, target) {
            $.each(target, function (dsIndex, dsItem) {
                var found = $.grep(source, function (item) {
                    return item.Id === dsItem.Id;
                });
                var checked = found.length > 0;
                if (dsItem.checked !== checked) {
                    dsItem.checked = checked;
                }
            });
        };

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                updateSelected(vm.selectedList, e.response);
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
        };
    };

    return {
        restrict: 'E',
        scope: {
            selectedList: "=ltNgSelectedList",
            accessor: "="
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
        ].join(''),
        link: link
    };

    function link(scope, element, attrs, controller) {
        if (controller.accessor) {
            controller.accessor.invoke = function () {                
                if (controller)
                    controller.clearSelection();
            };
        }
    }

}


//No Funciona
//ltCbCuadernaEditor
(function () {

    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;
            vm.ds = new kendo.data.DataSource({
                data: vm.dependsOn.Contenedores
            });

            $scope.$watch(function () { return vm.dependsOn; }, onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = [];
                }
            };

            function onSelected(newValue, oldValue) {
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }
        };

        return {
            restrict: 'E',
            scope: {
                model: "=ltNgModel",
                dependsOn: "=ltDependsOn",
                dataBind: "=ltDataBind"
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'cuadernaEditor',
            bindToController: true,
            template: [
                '<input kendo-drop-down-list ',
                    'k-data-text-field="\'Orden\'" ',
                    'k-data-value-field="\'Orden\'" ',
                    'k-data-source="cuadernaEditor.ds" ',
                    'k-ng-model="cuadernaEditor.model" ',
                    'data-bind="value:\'Cuaderna\'" ',
                    'k-auto-bind="true" >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltCbCuadernaEditor', directive);
})();
