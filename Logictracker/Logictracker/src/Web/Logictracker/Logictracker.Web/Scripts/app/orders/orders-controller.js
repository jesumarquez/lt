angular
    .module('logictracker.orders.controller', [])
    .controller('OrdersController', ['$scope', 'OrdersService', OrdersController]);

function OrdersController($scope, OrdersService) {
    this.data = "Orders controller";
    this.OrderList = OrdersService.query();

    $scope.programOrders = function (orders) {
        $scope.newOrder = new OrdersService();
        $scope.newOrder.OrderList = orders.OrderList;
        $scope.newOrder.RouteCode = orders.RouteCode;
        $scope.newOrder.Vehicle = orders.IdVehicle;
        $scope.newOrder.StartDateTime = orders.StartDateTime;
        $scope.newOrder.LogisticsCycleType = orders.LogisticsCycleType;
        $scope.newOrder.$save(function () { });
    };
}
