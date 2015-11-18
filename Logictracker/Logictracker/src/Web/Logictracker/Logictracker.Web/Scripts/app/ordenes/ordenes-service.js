angular
    .module('logictracker.ordenes.service', [])
    .factory('OrdenesService', ['$resource', OrdenesService]);

function OrdenesService($resource) {
    return $resource("/api/distrito/:distritoId/base/:baseId/ordenes/:id",
        { distritoId: "@distritoId", baseId: "@baseId", id: "@id" },
        { query: { method: 'GET',  isArray: true }
    });
}




