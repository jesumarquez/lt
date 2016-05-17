angular
    .module('logictracker.dummy.service', [])
    .factory('DummyService', ['$resource', DummyService]);

function DummyService($resource) {
    return $resource('/api/dummy/:dummyId');
}