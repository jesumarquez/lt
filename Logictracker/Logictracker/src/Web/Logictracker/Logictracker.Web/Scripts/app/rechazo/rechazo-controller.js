angular
    .module('logictracker.rechazo.controller', ['kendo.directives'])
    .controller('RechazoController', ['$scope', 'RechazoService', RechazoController]);

function RechazoController($scope, RechazoService) {
    $scope.distritoselected = {};

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
                    return _baseUrl + "api/Distrito/" + $scope.distritoselected.Key + "/Base/Items";
                }
            }
        },
        error: function(e) {
            $scope.notify.show(e.errorThrown, "error");
        }

    });

    $scope.$watch("distritoselected", function(newValue, oldValue) {
        $scope.baseDS.read();
    });
}
