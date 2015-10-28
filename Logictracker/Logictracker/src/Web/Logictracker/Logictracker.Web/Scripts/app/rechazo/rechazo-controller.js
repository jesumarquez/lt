angular
    .module('logictracker.rechazo.controller', [])
    .controller('RechazoController', ['$scope', 'RechazoService', RechazoController]);

function RechazoController($scope, RechazoService) {
    this.data = "Rechazo";
    this.list = RechazoService.query();
}