angular.module('logictracker.orders.service', [])
    .factory('OrdersService', ['$resource', OrdersService]);

function OrdersService($resource) {
    return $resource('api/orders/:id');
}

