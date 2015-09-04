angular.module('logictracker', [])
.controller('DummyController', ['$scope', DummyController]);

function DummyController($scope) {
    this.data = "Text dummy";
}