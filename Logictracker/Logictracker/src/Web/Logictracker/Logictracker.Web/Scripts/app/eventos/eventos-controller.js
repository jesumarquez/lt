angular.module('logictracker.eventos')
.controller('EventosController', ['$scope', 'UserDataInfo', 'EntitiesService', EventosController]);

function EventosController($scope, UserDataInfo, EntitiesService) {
    var vm = this;
    vm.distritoSelected = null;
    vm.baseSelected = null;
    vm.tipoCocheSelected = null;
    vm.tipoMensajeSelected = null;
    vm.desde = null;
    vm.hasta = null;
    vm.cocheSelected = [];
    vm.mensajeSelected = [];
    vm.empleadoSelected = [];

    vm.UserData = UserDataInfo.get($scope, vm);

    vm.Search = function () {
        EntitiesService.
    };
}


