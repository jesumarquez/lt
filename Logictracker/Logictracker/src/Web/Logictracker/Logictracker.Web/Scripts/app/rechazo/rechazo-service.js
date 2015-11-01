angular
    .module('logictracker.rechazo.service', [])
    .factory('RechazoService', ['$resource', RechazoService]);

function RechazoService($resource) {
    return $resource('/api/rechazo/:rechazoId');
}


