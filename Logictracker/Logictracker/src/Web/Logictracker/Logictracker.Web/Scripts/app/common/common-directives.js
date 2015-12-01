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
			'<input kendo-drop-down-list ',
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
			'<input kendo-drop-down-list ',
				'k-data-text-field="\'Value\'" ',
		        'k-data-value-field="\'Key\'" ',
		        'k-data-source="dataSource" ',
		        'k-ng-model="model" ',
			'</input>'
        ].join('')
    };
})