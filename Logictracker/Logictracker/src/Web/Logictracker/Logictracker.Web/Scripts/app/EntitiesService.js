angular
    .module("logictracker.entities.service", [])
    .factory("EntitiesService", ["$resource", EntitiesService]);

function EntitiesService($resource) {
    return {
        distrito: {
            items: $resource("/api/distrito/items"),
            bases: $resource("/api/distrito/:distritoId/base/items", { distritoId: '@distritoId' }),
            departamento: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: '@distritoId', baseId: '@baseId' })
        }
    }
}
