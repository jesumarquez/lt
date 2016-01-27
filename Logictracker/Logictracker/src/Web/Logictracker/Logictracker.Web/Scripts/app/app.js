angular.module('logictracker', [
    "kendo.directives", 
    "ngResource",
    "logictracker.entities.service",
    "logictracker.dummy.service",
    "logictracker.dummy.controller",
    "logictracker.orders.service",
    "logictracker.orders.controller",
    "logictracker.rechazo.service",
    "logictracker.rechazo.controller",
    "logictracker.rechazo.directives",
    "logictracker.ordenes.service",
    "logictracker.ordenes.controller",
    "logictracker.ordenes.directives",
    "logictracker.common.directives",
    "logictracker.eventos"]);
angular.module('logictracker.eventos', []);
