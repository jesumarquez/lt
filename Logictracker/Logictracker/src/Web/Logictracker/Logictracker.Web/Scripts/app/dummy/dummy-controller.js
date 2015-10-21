angular
    .module('logictracker.dummy.controller', [])
    .controller('DummyController', ['$scope', 'DummyService', DummyController]);

function DummyController($scope, DummyService) {
    this.data = "Text dummy";
    this.list = DummyService.query();
}