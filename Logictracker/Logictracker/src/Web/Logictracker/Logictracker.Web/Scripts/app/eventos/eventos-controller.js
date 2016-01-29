angular.module('logictracker.eventos')
.controller('EventosController', ['$scope', 'UserDataInfo', EventosController]);

function EventosController($scope, UserDataInfo) {
    var vm = this;
    vm.distritoSelected = null;
    vm.baseSelected = null;
    vm.tipoCocheSelected = null;
    vm.tipoMensajeSelected = null;
    vm.desde = null;
    vm.hasta = null;

    vm.UserData = UserDataInfo.get($scope, vm);
}


