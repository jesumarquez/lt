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

        function onFail(e) {
            $scope.$emit('errorEvent', e);
        }
    };

    return {
        restrict: 'E',
        scope: {
            model: "=ltNgModel",
            defaultValue: "=ltDefaultValue"
        },
        replace: true,
        controller: ['$scope', '$filter', 'EntitiesService', DistritoController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" name="filter-distrito" /> ',
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
		        'k-ng-model="model" name="filter-base"/> ',

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
		        'k-ng-model="model" /> ',

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
		        'k-ng-model="model" />',

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
		        'k-ng-model="model" /> ',

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
		        'k-ng-model="model" /> ',

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
		        'k-ng-model="model" /> ',

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
		        'k-ng-model="model" /> ',

        ].join('')
    };
})

.directive('ltMsTransportista', function () {

    function TransportistaController($scope, EntitiesService) {
        $scope.ds = EntitiesService.distrito.transportista.models(onDSLoad, onFail);

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

    var link = function (scope, element, attrs) {
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
                'k-auto-bind="false" name="filter-transportista" >',
			'</input>'
        ].join(''),
        link: link
    };
})

.directive('ltCbTransportista', function () {

    function TransportistaController($scope, EntitiesService) {
        $scope.ds = EntitiesService.distrito.transportista.models(onDSLoad, onFail);

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

.directive('ltAcDistribucion', function () {
    function DistribucionController($scope, EntitiesService) {
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
        scope: {
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

                if ($scope.distribucion == undefined || $scope.distribucion.length == 0) {
                    $scope.dataSource = EntitiesService.distrito.puntoEntrega({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key
                    }, null, $scope.onFail);
                    return;
                }

                $scope.dataSource = EntitiesService.distrito.puntoEntrega({
                    distritoId: $scope.distrito.Key,
                    baseId: $scope.base.Key,
                    distribucionId: $scope.distribucion[0] !== undefined ? $scope.distribucion[0].Id : null,
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
            dependsOn: "=ltDependsOn",
            distribucion: "=ltDataDistribucion",
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
                'k-template="kTemplate" name="filter-entrega"/> ',
        ].join('')
    }
})

.directive('ltCbChofer', function () {

    function ChoferController($scope, EntitiesService) {
        $scope.ds = EntitiesService.distrito.transportista.empleados(onDSLoad, onFail);

        $scope.$watch("dependsOn", onSelected);

        function onSelected(newValue, oldValue) {
            if (newValue !== oldValue) {
                if (newValue.length == 0) {
                    $scope.ds.data([]);
                    $scope.ds.read();
                    return;
                }
                if ($scope.dependsOn !== undefined) {

                    $scope.ds.read({
                        distritoId: $scope.distrito.Key,
                        baseId: $scope.base.Key,
                        transportistaId: $scope.dependsOn.Key,
                        tipoEmpleadoCodigo: $scope.codigo
                    });
                }
            }
        }

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
            dependsOn: "=ltDependsOnTransportista",
            codigo: "@ltDataCodigoChofer",
        },
        controller: ['$scope', 'EntitiesService', ChoferController],
        template: [
            '<input class="form-control" kendo-combo-box ',
                'k-data-text-field="\'Descripcion\'" ',
                'k-data-value-field="\'EmpleadoId\'" ',
                'k-data-source="ds" ',
                'k-ng-model="model" ',
                'required ',
                'k-auto-bind="false" >',
            '</input>'
        ].join('')
    };
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
})

.directive('ltDdlTipoCicloLogistico', function () {

    function TipoCicloLogisticoController($scope, $filter, EntitiesService) {
        $scope.dataSource = EntitiesService.distrito.tipoCicloLogistico(onDSLoad, onFail);

        $scope.$watch("dependentOn", onSelected);

        function onDSLoad(e) {
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

        function onSelected(newValue, oldValue) {
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
        controller: ['$scope', '$filter', 'EntitiesService', TipoCicloLogisticoController],
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" > ',
			'</input>'
        ].join('')
    };
})
.directive('ltDdlCoche', function () {

    function CocheController($scope, $filter, EntitiesService) {
        vm = this;
        vm.ds = EntitiesService.distrito.coche(onDSLoad, onFail);

        $scope.$watch("vm.dependsOn", onSelected);

        function onDSLoad(e) {
            if (e.type === "read" && e.response) {
                vm.model = e.response[0];
            }
        };

        function onSelected(newValue, oldValue) {
            if (vm.distrito != undefined && vm.dependsOn != undefined && newValue !== undefined && newValue !== oldValue) {
                if (vm.tipoCoche != undefined)
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key, tipoCocheId: vm.tipoCoche.Id });
                else
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key });
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
            dependsOn: "=ltDependsOn",
            distrito: "=ltDataDistrito",
            base: "=ltDataBase",
            tipoCoche: "=ltDataTipoCoche"
        },
        //replace: true,
        controller: ['$scope', '$filter', 'EntitiesService', CocheController],
        controllerAs: 'vm',
        bindToController: true,
        template: [
			'<input class="form-control" kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="vm.ds" ',
		        'k-ng-model="vm.model" /> '
        ].join('')
    };
});


/// ltCbSupervisorVenta
(function () {

    var directive = function () {

        var controller = function ($scope, EntitiesService) {

            var vm = this;

            vm.ds = EntitiesService.ticketrechazo.empleadoReporta(onDSLoad, onFail);

            $scope.$watch("vm.dependsOn", onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = e.response[0];
                }
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }

            function onSelected(newValue, oldValue) {

                if (newValue !== undefined && newValue !== oldValue) {

                    if (newValue && (newValue.baseId !== vm.base.Key)) {
                        vm.notifyShow()("El vendedor  " + newValue.Descripcion + " pertenece a otra base", "warning");
                    }

                    vm.ds.read({
                        distritoId: vm.distrito.Key,
                        baseId: vm.base.Key,
                        empleadoId: vm.dependsOn.EmpleadoId
                    });
                }
                else {
                    vm.ds.data([]);
                    vm.ds.read();
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
            controllerAs: 'vm',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-combo-box ',
                    'k-data-text-field="\'Descripcion\'" ',
                    'k-data-value-field="\'EmpleadoId\'" ',
                    'k-data-source="vm.ds" ',
                    'k-ng-model="vm.model" ',
                    'required >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltCbSupervisorVenta', directive);

}());

/// ltCbSupervisorRuta
(function () {

    var directive = function () {

        var controller = function ($scope, EntitiesService) {

            var vm = this;

            vm.ds = EntitiesService.ticketrechazo.empleadoReporta(onDSLoad, onFail);
            vm.supervisorRutasRead = false;

            $scope.$watch("vm.dependsOn", onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    if (e.response.length == 0)
                        if (vm.supervisorRutasRead) {
                            vm.ds.read({
                                distritoId: vm.distrito.Key,
                                baseId: vm.base.Key,
                                empleadoId: vm.dependsOn.EmpleadoId,
                                tipoEmpleadoCodigo: vm.codigo
                            });
                        }

                    vm.supervisorRutasRead = false;
                    vm.model = e.response[0];
                }
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }

            function onSelected(newValue, oldValue) {

                if (newValue !== undefined && newValue !== oldValue) {
                    vm.supervisorRutasRead = true;

                    vm.ds.read({
                        distritoId: vm.distrito.Key,
                        baseId: vm.base.Key,
                        empleadoId: vm.dependsOn.EmpleadoId
                    });
                }
                else {
                    vm.ds.data([]);
                    vm.ds.read();
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
            controllerAs: 'vm',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-combo-box ',
                    'k-data-text-field="\'Descripcion\'" ',
                    'k-data-value-field="\'EmpleadoId\'" ',
                    'k-data-source="vm.ds" ',
                    'k-ng-model="vm.model" ',
                    'required >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltCbSupervisorRuta', directive);

}());

// ltDdlInsumo
(function () {

    var directive = function () {

        var controller = function ($scope, EntitiesService) {

            var vm = this;
            vm.ds = EntitiesService.distrito.insumo(onDSLoad, onFail);

            $scope.$watch("vm.dependsOn", onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = e.response[0];
                }
            };

            function onFail(e) {
                $scope.$emit('errorEvent', e);
            }

            function onSelected(newValue, oldValue) {

                if (newValue !== undefined && newValue !== oldValue) {
                    vm.ds.read({
                        distritoId: vm.distrito.Key,
                        baseId: vm.base.Key,
                    });
                }
                else {
                    vm.ds.data([]);
                    vm.ds.read();
                }
            }
        };

        return {
            restrict: 'E',
            scope: {
                model: "=ltNgModel",
                distrito: "=ltDataDistrito",
                base: "=ltDataBase",
                dependsOn: "=ltDependsOn"
            },
            controller: ['$scope', 'EntitiesService', controller],
            controllerAs: 'vm',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-multi-select ',
                    'k-data-text-field="\'Value\'" ',
                    'k-data-value-field="\'Key\'" ',
                    'k-data-source="vm.ds" ',
                    'k-ng-model="vm.model" ',
                    'required >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltMsInsumo', directive);

}());



/// ltDdlTipoCoche
(function () {
    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;

            vm.ds = EntitiesService.distrito.tipoCoche(onDSLoad, onFail);

            $scope.$watch(function () { return vm.base; }, onBaseChange);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = e.response[0];
                }
            };

            function onBaseChange(newValue, oldValue) {
                if (newValue != null && newValue !== oldValue) {
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key });
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
                base: "=ltDataBase",
                distrito: "=ltDataDistrito"
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'tipoCoche',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-drop-down-list ',
                    'k-data-text-field="\'Descripcion\'" ',
                    'k-data-value-field="\'Id\'" ',
                    'k-data-source="tipoCoche.ds" ',
                    'k-ng-model="tipoCoche.model" > ',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltDdlTipoCoche', directive);
})();

/// ltDdlTipoMensaje
(function () {
    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;

            vm.ds = EntitiesService.distrito.tipoMensaje(onDSLoad, onFail);

            $scope.$watch(function () { return vm.base; }, onBaseChange);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = e.response[0];
                }
            };

            function onBaseChange(newValue, oldValue) {
                if (newValue != null && newValue !== oldValue) {
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key });
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
                base: "=ltDataBase",
                distrito: "=ltDataDistrito"
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'tipoMensaje',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-drop-down-list ',
                    'k-data-text-field="\'Value\'" ',
                    'k-data-value-field="\'Key\'" ',
                    'k-data-source="tipoMensaje.ds" ',
                    'k-ng-model="tipoMensaje.model" ',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltDdlTipoMensaje', directive);
})();

//ltMsCoche
(function () {

    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;
            vm.ds = EntitiesService.distrito.coche(onDSLoad, onFail);

            $scope.$watch(function () { return vm.base; }, onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = [];
                }
            };

            function onSelected(newValue, oldValue) {
                if (newValue != null && newValue !== oldValue) {
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key, excludeNone: true });
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
                base: "=ltDataBase",
                distrito: "=ltDataDistrito",
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'coche',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-multi-select ',
                    'k-data-text-field="\'Value\'" ',
                    'k-data-value-field="\'Key\'" ',
                    'k-data-source="coche.ds" ',
                    'k-ng-model="coche.model" ',
                    'k-auto-bind="false" >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltMsCoche', directive);


})();

//ltMsMensaje
(function () {

    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;
            vm.ds = EntitiesService.distrito.mensaje(onDSLoad, onFail);

            $scope.$watch(function () { return vm.base; }, onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = [];
                }
            };

            function onSelected(newValue, oldValue) {
                if (newValue != null && newValue !== oldValue) {
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key });
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
                base: "=ltDataBase",
                distrito: "=ltDataDistrito",
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'mensaje',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-multi-select ',
                    'k-data-text-field="\'Value\'" ',
                    'k-data-value-field="\'Key\'" ',
                    'k-data-source="mensaje.ds" ',
                    'k-ng-model="mensaje.model" ',
                    'k-auto-bind="false" >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltMsMensaje', directive);


})();

//ltMsEmpleado
(function () {

    var directive = function () {

        var Controller = function ($scope, EntitiesService) {
            var vm = this;
            vm.ds = EntitiesService.distrito.empleados(onDSLoad, onFail);

            $scope.$watch(function () { return vm.base; }, onSelected);

            function onDSLoad(e) {
                if (e.type === "read" && e.response) {
                    vm.model = [];
                }
            };

            function onSelected(newValue, oldValue) {
                if (newValue != null && newValue !== oldValue) {
                    vm.ds.read({ distritoId: vm.distrito.Key, baseId: vm.base.Key });
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
                base: "=ltDataBase",
                distrito: "=ltDataDistrito",
            },
            controller: ['$scope', 'EntitiesService', Controller],
            controllerAs: 'empleado',
            bindToController: true,
            template: [
                '<input class="form-control" kendo-multi-select ',
                    'k-data-text-field="\'Value\'" ',
                    'k-data-value-field="\'Key\'" ',
                    'k-data-source="empleado.ds" ',
                    'k-ng-model="empleado.model" ',
                    'k-auto-bind="false" >',
                '</input>'
            ].join('')
        };
    };

    angular.module('logictracker.common.directives')
        .directive('ltMsEmpleado', directive);


})();

