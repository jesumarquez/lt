﻿angular.module('logictracker.common.directives', [])

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
        $scope.ds = EntitiesService.distrito.transportista(onDSLoad, onFail);

        $scope.$watch("dependsOn", onSelected);
  
        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = [];
            }
        };

        function onSelected(newValue, oldValue) {
            if (newValue != null && newValue !== oldValue) {
                $scope.ds.read({ distritoId: $scope.distrito.Key, baseId: $scope.dependsOn.Key });
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    var link = function (scope, element, attrs)
    {
        //if (scope.required !== undefined && scope.required) element.attr("required", true);
    }

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            dependsOn: "=ltDependsOnBase",
            distrito: "=ltDataDistrito",
        },
        controller: ['$scope', 'EntitiesService', TransportistaController],
        template: [
			'<input class="form-control" kendo-multi-select ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="ds" ',
		        'k-ng-model="model" ',
                'k-auto-bind="false" >',
			'</input>'
        ].join(''),
        link: link
    };
})

.directive('ltCbTransportista', function () {

    function TransportistaController($scope, EntitiesService) {
        $scope.ds = EntitiesService.distrito.transportista(onDSLoad, onFail);

        $scope.ds.read({ distritoId: $scope.distrito.Key, baseId: $scope.base.Key });

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = [];
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
            distrito: "=ltDataDistrito",
            base: "=ltDataBase",
        },
        controller: ['$scope', 'EntitiesService', TransportistaController],
        template: [
			'<input class="form-control" kendo-combo-box ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="ds" ',
		        'k-ng-model="model" ',
                'required ',
                'k-auto-bind="false" >',
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
})

.directive('ltAcPuntoEntrega', function () {
    function PuntoEntregaController($scope, EntitiesService) {

        $scope.dataSource = EntitiesService.distrito.puntoEntrega({ distritoId: $scope.distrito.Key, baseId: $scope.base.Key }, null, $scope.onFail);

        $scope.$watch("dependsOn", onSelected);

        function onSelected(newValue, oldValue) {
            if (newValue != null && newValue !== oldValue) {

                $scope.model = [];

                if (newValue.length == 0) {
                    $scope.dataSource = EntitiesService.distrito.puntoEntrega({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key
                    }, null, $scope.onFail);
                    return;
                }

                $scope.dataSource = EntitiesService.distrito.puntoEntrega({
                    distritoId: $scope.distrito.Key,
                    baseId: $scope.base.Key,
                    distribucionId: $scope.dependsOn[0] !== undefined ? $scope.dependsOn[0].Id : null,
                }, null, $scope.onFail);
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: '=ltNgModel',
            dependsOn: "=ltDependsOnDistribucion",
            distrito: "=ltDataDistrito",
            base: "=ltDataBase",
            kTemplate: "=ltTemplate"
        },
        controller: ['$scope', 'EntitiesService', PuntoEntregaController],
        template: [
            '<input class="form-control k-textbox" ',
                'kendo-auto-complete ',
                'k-ng-model="model" ',
                'k-data-source="dataSource" ',
                'k-data-text-field="\'Codigo\'" ',
                'k-filter="\'contains\'" ',
                'k-min-length="3" ',
                'required ',
                'k-template="kTemplate"/> ',
        ].join('')
    }
})

.directive('ltCbVendedor', function () {

    var controller = function ($scope, EntitiesService) {
        
        $scope.ds = EntitiesService.ticketrechazo.empleado(onDSLoad, onFail);

        $scope.$watch("dependsOn", onSelected);

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.model = e.response[0];
            }
        };

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }

        function onSelected(newValue, oldValue) {

            if (newValue !== oldValue) {

                if (newValue.length == 0) {
                    $scope.clienteSelected = "";
                    $scope.ds.data([]);
                    $scope.ds.read();
                    return;
                }

                if ($scope.dependsOn[0] !== undefined) {

                    $scope.cliente = $scope.dependsOn[0].ClienteDesc;

                    var responsable = ($scope.dependsOn[0] !== undefined && $scope.dependsOn[0].ResponsableId != 0);

                    // Si el punto de entrega no tiene responsable por el momento en el combo de vendedores se van a listar
                    // todos los empleados que tengan asignado el tipo de empleado "V".
                    $scope.ds.read({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key,
                        tipoEmpleadoCodigo: !responsable ? $scope.codigo : null,
                        empleadoId: responsable ? $scope.dependsOn[0].ResponsableId : null,
                    });
                }
            }
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            distrito: "=ltDataDistrito",
            base: "=ltDataBase",
            cliente: "=ltDataCliente",
            codigo: "@ltDataCodigoVendedor",
            dependsOn: "=ltDependsOnPuntoEntrega",
        },
        controller: ['$scope', 'EntitiesService', controller],
        template: [
			'<input class="form-control" kendo-combo-box ',
				'k-data-text-field="\'Descripcion\'" ',
		        'k-data-value-field="\'EmpleadoId\'" ',
		        'k-data-source="ds" ',
		        'k-ng-model="model" ',
                'required >',
			'</input>'
        ].join('')
    };
});

(function () {

    var directive = function () {

        var controller = function ($scope, EntitiesService) {

            $scope.ds = EntitiesService.ticketrechazo.empleadoReporta(onDSLoad, onFail);

            $scope.$watch("dependsOn", onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    $scope.model = e.response[0];
                }
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }

            function onSelected(newValue, oldValue) {

                if (newValue !== undefined && newValue !== oldValue) {

                    if (newValue && (newValue.baseId !== $scope.base.Key)) {
                        $scope.notifyShow()("El vendedor  " + newValue.Descripcion + " pertenece a otra base", "warning");
                    }

                    $scope.ds.read({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key,
                        empleadoId: $scope.dependsOn.EmpleadoId
                    });
                }
                else {
                    $scope.ds.data([]);
                    $scope.ds.read();
                }
            }
        };

        return {
            restrict: 'E',
            scope: {
                model: "=ltNgModel",
                distrito: "=ltDataDistrito",
                base: "=ltDataBase",
                dependsOn: "=ltDependsOnVendedor",
                notifyShow: "&ltNotifyShow"
            },
            controller: ['$scope', 'EntitiesService', controller],
            template: [
                '<input class="form-control" kendo-combo-box ',
                    'k-data-text-field="\'Descripcion\'" ',
                    'k-data-value-field="\'EmpleadoId\'" ',
                    'k-data-source="ds" ',
                    'k-ng-model="model" ',
                    'required >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltCbSupervisorVenta', directive);

}());

(function () {

    var directive = function () {

        var controller = function ($scope, EntitiesService) {

            $scope.ds = EntitiesService.ticketrechazo.empleadoReporta(onDSLoad, onFail);
            $scope.supervisorRutasRead = false;

            $scope.$watch("dependsOn", onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    if (e.response.length == 0)
                        if ($scope.supervisorRutasRead) {
                            $scope.ds.read({
                                distritoId: $scope.distrito.Key,
                                baseId: $scope.base.Key,
                                empleadoId: $scope.dependsOn.EmpleadoId,
                                tipoEmpleadoCodigo: $scope.codigo
                            });
                        }

                    $scope.supervisorRutasRead = false;
                    $scope.model = e.response[0];
                }
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }

            function onSelected(newValue, oldValue) {

                if (newValue !== undefined && newValue !== oldValue) {
                    $scope.supervisorRutasRead = true;

                    $scope.ds.read({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key,
                        empleadoId: $scope.dependsOn.EmpleadoId
                    });
                }
                else {
                    $scope.ds.data([]);
                    $scope.ds.read();
                }
            }
        };

        return {
            restrict: 'E',
            scope: {
                model: "=ltNgModel",
                distrito: "=ltDataDistrito",
                base: "=ltDataBase",
                dependsOn: "=ltDependsOnSupervisorVentas",
                codigo: "@ltDataCodigoSupervisorRutas"
            },
            controller: ['$scope', 'EntitiesService', controller],
            template: [
                '<input class="form-control" kendo-combo-box ',
                    'k-data-text-field="\'Descripcion\'" ',
                    'k-data-value-field="\'EmpleadoId\'" ',
                    'k-data-source="ds" ',
                    'k-ng-model="model" ',
                    'required >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltCbSupervisorRuta', directive);

}());