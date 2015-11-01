angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'RechazoService', RechazoController]);

function RechazoController($scope, RechazoService) {
    $scope.distritoSelected = {};
    $scope.baseSelected = {};
    $scope.departamentoSelected = {};

    $scope.distritoDS = new kendo.data.DataSource({
        transport: {
            read: {
                dataType: "json",
                url: _baseUrl + "api/Distrito/Items"
            },
            error: function(e) {
                $scope.notify.show(e.status, "error");
            }
        }
    });

    $scope.baseDS = new kendo.data.DataSource({
        transport: {
            read: {
                dataType: "json",
                url: function() {
                    return _baseUrl + "api/Distrito/" + $scope.distritoSelected.Key + "/Base/Items";
                }
            }
        },
        error: function(e) {
            $scope.notify.show(e.errorThrown, "error");
        }

    });

    $scope.departamentoDS = new kendo.data.DataSource({
        transport: {
            read: {
                dataType: "json",
                url: function () {
                    return _baseUrl + "api/Distrito/" + $scope.distritoSelected.Key + "/Base/" + $scope.baseSelected.Key + "/Departamento/Items";
                }
            }
        },
        error: function (e) {
            $scope.notify.show(e.errorThrown, "error");
        }

    });

    $scope.$watch("distritoSelected", function(newValue, oldValue) {
        $scope.baseDS.read();
    });
    
    $scope.$watch("baseSelected", function (newValue, oldValue) {
        $scope.departamentoDS.read();
        $scope.departamentoSelected = {};
    });

    $scope.distritoDefault = function(e) {
        var s = e.sender;
        s.select(0);
        $scope.distritoSelected = s.dataSource.at(0);
    }
    $scope.baseDefault = function (e) {
        var s = e.sender;
        s.select(0);
        $scope.baseSelected = s.dataSource.at(0);
    }
}
