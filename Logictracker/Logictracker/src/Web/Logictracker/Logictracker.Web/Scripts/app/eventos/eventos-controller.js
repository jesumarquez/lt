angular.module('logictracker.eventos')
.controller('EventosController', ['EntitiesService', 'ErrorHelper', EventosController]);

function EventosController(EntitiesService, ErrorHelper) {
    var self = this;
    this.distritoSelected = null;

    this.UserData = EntitiesService.resources.userData.get();
    this.UserData.$promise.then(function () {
        if (self.UserData.EmpleadoId === 0) {
            ErrorHelper.onFail(self.notify, { errorThrown: "Usuario sin empleado asociado" });
        }
    });
}
