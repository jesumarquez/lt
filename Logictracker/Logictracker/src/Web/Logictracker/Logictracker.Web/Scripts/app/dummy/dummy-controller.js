angular
    .module('logictracker.dummy.controller', [])
    .controller('DummyController', ['$scope', DummyController]);

function DummyController($scope) {
    this.data = "Text dummy";
}