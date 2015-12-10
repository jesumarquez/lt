angular.module('logictracker.common.directives', [])

.directive('ltDdlDistrito', function () {

    function DistritoController($scope, $filter, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.items(onDistritoDSLoad, onFail);

        function onDistritoDSLoad(e) {
            if (e.type === "read" && e.response) {
                try {
                    if ($scope.defaultValue) {
                        $scope.model = $filter("filter")(e.response, { Key: $scope.defaultValue }, true)[0];
                        if ($scope.model) return;
                    }
                } catch (ex) {
                    onFail(ex);
                }
                
                $scope.model = e.response[0];
            }
        };

        function onFail(e)
        {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            defaultValue: "=ltDefaultValue"
        },
        controller: ['$scope', '$filter', 'EntitiesService', DistritoController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltDdlBase', function () {

    function BaseController($scope, $filter, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.bases(onBaseDSLoad, onFail);

        $scope.$watch("dependentOn", onDistritoSelected);

        function onBaseDSLoad(e) {
            if (e.type === "read" && e.response) {
                try {
                    if ($scope.defaultValue) {
                        $scope.model = $filter("filter")(e.response, { Key: $scope.defaultValue }, true)[0];
                        if ($scope.model) return;
                    }
                } catch (ex) {
                    onFail(ex);
                }

                $scope.model = e.response[0];
            }
        };

        function onDistritoSelected(newValue, oldValue) {
            if (newValue !== oldValue) {
                $scope.dataSource.read({ distritoId: $scope.dependentOn.Key });
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            defaultValue: "=ltDefaultValue",
            dependentOn: "=ltDependentOn"
        },
        controller: ['$scope', '$filter', 'EntitiesService', BaseController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltMsDepartamento', function () {

    function DepartamentoController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.departamento(onDSLoad, onFail);

        $scope.$watch("dependsOn", onSelected);

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = [];
            }
        };

        function onSelected(newValue, oldValue) {
            if (newValue != null && newValue !== oldValue) {
                $scope.dataSource.read({ distritoId: $scope.distrito.Key, baseId: $scope.dependsOn.Key });                
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            dependsOn: "=ltDependsOnBase",
            distrito: "=ltDataDistrito"
        },
        controller: ['$scope', 'EntitiesService', DepartamentoController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltMsEstado', function () {

    function EstadoController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.ticketrechazo.estados(null, onFail);

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel"
        },
        controller: ['$scope', 'EntitiesService', EstadoController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltDdlEstado', function () {

    function EstadoController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.ticketrechazo.estados(onLoad, onFail);
        function onLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = e.response[0];
            }
        }

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel"
        },
        controller: ['$scope', 'EntitiesService', EstadoController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltMsCentroDeCostos', function () {

    function CentroDeCostosController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.centroDeCostos(onDSLoad, onFail);

        $scope.$watchGroup(["dependsOnDepartamento", "dependsOnBase"], onSelected);
  
        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = [];
            }
        };

        function onSelected(newValue, oldValue) {
           if (newValue[0] !== undefined && newValue[0].length > 0 && newValue != null && newValue !== oldValue)
                $scope.dataSource.read({
                    distritoId: $scope.distrito.Key,
                    baseId: $scope.dependsOnBase.Key,
                    departamentoId: $scope.dependsOnDepartamento.map(function (o) { return o.Key; })
                });
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            dependsOnBase: "=ltDependsOnBase",
            dependsOnDepartamento: "=ltDependsOnDepartamento",
            distrito: "=ltDataDistrito"
        },
        controller: ['$scope', 'EntitiesService', CentroDeCostosController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltMsMotivo', function () {

    function MotivoController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.ticketrechazo.motivos(null, onFail);

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel"
        },
        controller: ['$scope', 'EntitiesService', MotivoController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltDdlMotivo', function () {

    function MotivoController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.ticketrechazo.motivos(onLoad, onFail);

        function onLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = e.response[0];
            }
        }

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel"
        },
        controller: ['$scope', 'EntitiesService', MotivoController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltMsTransportista', function () {

    function TransportistaController($scope, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.transportista(onDSLoad, onFail);

        $scope.$watch("dependsOn", onSelected);

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = [];
            }
        };

        function onSelected(newValue, oldValue) {
            if (newValue != null && newValue !== oldValue) {
                $scope.dataSource.read({ distritoId: $scope.distrito.Key, baseId: $scope.dependsOn.Key });
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            dependsOn: "=ltDependsOnBase",
            distrito: "=ltDataDistrito"
        },
        controller: ['$scope', 'EntitiesService', TransportistaController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})

.directive('ltAcDistribucion', function(){
    function DistribucionController($scope, EntitiesService){
        $scope.dataSource = EntitiesService.distrito.distribuciones.models({ distritoId: $scope.distrito.Key, baseId: $scope.dependsOn.Key }, null, $scope.onFail);
        $scope.$watch("dependsOn", onSelected);

        function onSelected(newValue, oldValue) {
            if (newValue != null && newValue !== oldValue) {
                $scope.dataSource.read({ distritoId: $scope.distrito.Key, baseId: $scope.dependsOn.Key });
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope:{
            model: '=ltNgModel',
            dependsOn: "=ltDependsOnBase",
            distrito: "=ltDataDistrito",
            kTemplate: "=ltTemplate"
        },
        controller: ['$scope', 'EntitiesService', DistribucionController],
        template: [
            '<input class="form-control k-textbox" ',
                'kendo-auto-complete ',
                'k-ng-model="model" ',
                'k-data-source="dataSource" ',
                'k-data-text-field="\'Codigo\'" ',
                'k-filter="\'contains\'" ',
                'k-min-length="3" ',
                'k-template="kTemplate"/> ',
        ].join('')
    }
});