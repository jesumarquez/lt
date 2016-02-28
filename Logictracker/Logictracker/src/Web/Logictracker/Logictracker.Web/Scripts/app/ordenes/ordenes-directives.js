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
.directive('orderDetail', OrderDetailDirective);

function OrderDetailDirective() {
    var controller = function ($scope, OrdenesService) {
        var vm = this;

        vm.gridOptions = {
            selectable: "multiple",
            scrollable: false,
            sortable: true,
            columns: [
                { field: "Insumo", title: "Producto", width: "160px" },
                { field: "Cantidad", title: "Litros" },
                { template: "<input type='checkbox' ng-click='productos.onSelected(dataItem)'/>" }
            ]
        };

        vm.ds = OrdenesService.ordenDetalles(vm.orderId, [], null, onFail);

        vm.onSelected = function (data) {
            var index = vm.selectedList.indexOf(data);

            if (index > -1)
                vm.selectedList.splice(index, 1);
            else
                vm.selectedList.push(data);

            console.log(vm.selectedList);
            if (vm.selectedList.length > 0) {
                console.log(vm.selectedList[vm.selectedList.length - 1].Id);
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
