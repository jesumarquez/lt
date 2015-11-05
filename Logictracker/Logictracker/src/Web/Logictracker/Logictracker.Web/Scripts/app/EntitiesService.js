angular
    .module("logictracker.entities.service", [])
    .factory("EntitiesService", ["$resource", EntitiesService]);

function EntitiesService($resource) {
    var service = {
        distrito: {
            items: getDistritosDS,
            bases: getBasesDS,
            departamento: $resource("/api/distrito/:distritoId/base/:baseId/departamento/items", { distritoId: "@distritoId", baseId: "@baseId" }),
            centroDeCostos: $resource("/api/distrito/:distritoId/base/:baseId/centrodecostos/items", { distritoId: "@distritoId", baseId: "@baseId", deptoId: "@deptoId" }),
            transportista: $resource("/api/distrito/:distritoId/base/:baseId/transportista/items", { distritoId: "@distritoId", baseId: "@baseId" }),
        }
    };

    function getDistritosDS() {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: "/api/distrito/items"
                }
            }
        });

        return ds;
    };


    function getBasesDS() {
        var ds = new kendo.data.DataSource({
            transport: {
                read: {
                    dataType: "json",
                    url: function (op) {
                        return op.distritoId === undefined || op.distritoId === ""
		                    	? ""
		                    	: "/api/distrito/" + op.distritoId + "/base/items";
                    }
                }
            }
        });

        return ds;
    };


    return service;
}
