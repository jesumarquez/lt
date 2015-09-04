angular.module('logictracker', [])
.controller('DummyController', ['$scope', DummyController]);

function DummyController($scope) {
    $scope.data = "Text dummy";
}