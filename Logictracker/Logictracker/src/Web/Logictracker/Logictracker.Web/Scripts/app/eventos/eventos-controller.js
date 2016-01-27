angular.module('logictracker.eventos')
.controller('EventosController', ['$scope', 'EntitiesService', EventosController]);

function EventosController($scope, EntitiesService) {
    $scope.name = 'logictracker.eventos.EventosController';
}
