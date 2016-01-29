angular
    .module("logictracker.common")
    .factory("ErrorHelper", ErrorHelper)
    .factory("UserDataInfo", ['EntitiesService', 'ErrorHelper', UserDataInfo]);

function ErrorHelper() {
    var helper = {
        onFail : onFail
    };

    function onFail(notify, error) {
        if (!notify) return;
        try {
            if (error.data.ExceptionMessage) {
                notify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) { }

        if (error.errorThrown !== undefined)
            notify.show(error.errorThrown, "error");
        else
            notify.show(error, "error");
    };

    return helper;
}


/*
    Mantiene sincorinzados distrito y base con la session.
*/
function UserDataInfo(EntitiesService, ErrorHelper) {
    var service = {
        get: get
    };

    var self = this;

    function get(scope, controller) {
        self.controller = controller;

        scope.$watch(function () {
            return self.controller.baseSelected;
        }, onBaseSelected);

        self.UserData = EntitiesService.resources.userData.get();
        self.UserData.$promise.then(function () {
            if (self.UserData.EmpleadoId === 0) {
                ErrorHelper.onFail(self.controller.notify, { errorThrown: "Usuario sin empleado asociado" });
            }
        });
        return self.UserData;
    }

    function onBaseSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {

            self.UserData.DistritoSelected = self.controller.distritoSelected.Key;
            self.UserData.BaseSelected = self.controller.baseSelected.Key;

            self.UserData.$save();
        }
    };

    return service;
}
