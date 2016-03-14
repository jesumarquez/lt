﻿angular
    .module("logictracker.ordenes.controller", ["kendo.directives", "ngAnimate", 'openlayers-directive'])
    .controller("OrdenesController", ["$scope", "$log", "EntitiesService", "OrdenesService", "UserDataInfo", OrdenesController])
    .controller('OrdenesAsignarController', ["$scope", "$log", "EntitiesService", "OrdenesService", OrdenesAsignarController]);

function OrdenesController($scope, $log, EntitiesService, OrdenesService, UserDataInfo) {

    $scope.UserData = UserDataInfo.get($scope, $scope);

    $scope.order = {
        StartDateTime: new Date()
    };

    $scope.order.StartDateTime.setDate($scope.order.StartDateTime.getDate() + 1);

    $scope.ordenesGridOptions = {
        sortable: true,
        groupable: true,
        scrollable: false,
        selectable: "multiple",
        pageable: {
            refresh: true,
            pageSizes: true,
            info: true
        },
        dataBound: function () {
            this.expandRow(this.tbody.find("tr.k-master-row").first());
        },
        columns:
        [
            { field: "Empresa", title: "Empresa" },
            { field: "Transportista", title: "Transportista" },
            { field: "CodigoPuntoEntrega", title: "Codigo Entrega" },
            { field: "PuntoEntrega", title: "Razon Social" },
            { field: "CodigoPedido", title: "Codigo Pedido" },
            { field: "FechaPedido", title: "Pedido", format: "{0: dd/MM HH:mm}" },
        ],
        detailTemplate: '<order-detail lt-ng-order-id="dataItem.Id" lt-ng-selected-list="productsSelected"/>',
    }

    $scope.productsSelected = new kendo.data.ObservableArray([]);

    $scope.distritoSelected = {};
    $scope.baseSelected = {};
    $scope.transportistaSelected = [];

    $scope.puntoEntregaSelected = {};
    $scope.tPuntoEntrega = kendo.template($("#tPuntoEntrega").html());

    $scope.desde = new Date();
    $scope.hasta = new Date();
    $scope.olMap = {
        center: {
            lat: -34.603722,
            lon: -58.381592,
            zoom: 10
        }
    };
    $scope.orderSelected = {};

    function onFail(error) {
        if (error.errorThrown)
            $scope.notify.show(error.errorThrown, "error");
        else
            $scope.notify.show(error.statusText, "error");
    };

    $scope.onerror = function (error) {
        $scope.notify.show(error.statusText, "error");
    }

    $scope.onNuevo = function () {
        $scope.rechazoWin.refresh({ url: "Item?op=A" });
        $scope.rechazoWin.center();
        $scope.rechazoWin.open();
    };

    $scope.onBuscar = function () {
        var filterList = [];

        if ($scope.distritoSelected != undefined)
            filterList.push({ field: "Empresa.Id", operator: "eq", value: $scope.distritoSelected.Key });

        if ($scope.baseSelected != undefined)
            filterList.push({ field: "Linea.Id", operator: "eq", value: $scope.baseSelected.Key });

        if ($scope.puntoEntregaSelected != undefined && $scope.puntoEntregaSelected.length > 0)
            filterList.push({ field: "PuntoEntrega.Id", operator: "eq", value: $scope.puntoEntregaSelected[0].PuntoEntregaId });

        if ($scope.transportistaSelected.length > 0) {
            var transportistaFilter = $scope.transportistaSelected.map(function (e) { return { field: "Transportista.Id", operator: "eq", value: e.Key }; });
            filterList.push({ logic: "or", filters: transportistaFilter });
        }

        var msOffset = new Date().getTimezoneOffset() * 60000;

        if ($scope.desde != undefined) {
            var fDesde = new Date($scope.desde);
            fDesde.setHours(0, 0, 0, 0);
            filterList.push({ field: "FechaAlta", operator: "gte", value: new Date(fDesde.getTime() + msOffset) });
        }

        if ($scope.hasta != undefined) {
            var fHasta = new Date($scope.hasta);
            fHasta.setHours(23, 59, 59, 999);
            filterList.push({ field: "FechaAlta", operator: "lte", value: new Date(fHasta.getTime() + msOffset) });
        }

        var filters = {
            logic: "and",
            filters: filterList
        };

        $scope.Orders = OrdenesService.items(filters, onOrdersDSLoad, onFail);
        function onOrdersDSLoad() {
            //$scope.olMap.markers = $scope.Orders.map(function (o) {
            //    return {
            //        name: o.Id,
            //        lat: o.LatitudPuntoEntrega,
            //        lon: o.LongitudPuntoEntrega,
            //        label: {
            //            message: '<span>' + o.PuntoEntrega + '</span>',
            //            show: false,
            //            showOnMouseOver: true
            //        },
            //        dataItem: o,
            //        onClick: function (event, properties) {
            //            $scope.orderSelected = properties.dataItem;
            //            $("#markerModal").modal('toggle');
            //        }
            //    }
            //});
        }
    };

    $scope.disabledButton = false;

    $scope.programOrders = function (order) {

        $scope.disabledButton = true;

        var selectOrders = [];
        $scope.ordenesGrid.select().each(function (index, row) {
            selectOrders.push($scope.ordenesGrid.dataItem(row));
        });

        $scope.newOrder = new OrdenesService.ordenes();
        $scope.newOrder.OrderList = selectOrders;
        $scope.newOrder.IdVehicle = order.Vehicle.Id;
        $scope.newOrder.StartDateTime = order.StartDateTime;
        $scope.newOrder.LogisticsCycleType = order.LogisticsCycleType.Key;
        $scope.newOrder.$save(
            { distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key },
            function () {
                $scope.onBuscar();
                $('#myModal').modal('hide');
                $scope.disabledButton = false;
            },
            onFail
        );
    };

}

function OrdenesAsignarController($scope, $log, EntitiesService, OrdenesService) {

    $scope.vehicleTypeSelected = {};

    $scope.ds = new kendo.data.DataSource({
        data: $scope.productsSelected,
        schema:
        {
            model: {
                id: "Id",
                fields: {
                    "Id": { type: "number", editable: false },
                    "OrderId": { type: "number", editable: false },
                    "Insumo": { type: "string", editable: false },
                    "Cantidad": { type: "number", editable: false },
                    "Ajuste": { type: "number", editable: true },
                    "Cuaderna": { type: "number", editable: true },
                    "CocheId": { type: "number", editable: true }
                }
            },
        },
        change: onDataChanged
    });

    $scope.$watch("vehicleTypeSelected", onvehicleTypeSelected);



    function onvehicleTypeSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {
            $scope.cuadernasDs = new kendo.data.DataSource({
                data: newValue.Contenedores
            });
            // Deberia limpiar 
        }
    }


    function sumCuaderna(cuaderna) {
        var l = 0;
        var n = 0;
        angular.forEach($scope.ds.data(), function (value, key) {
            if (value.Cuaderna === cuaderna) {
                l += value.Cantidad + value.Ajuste;
                n += 1;
            }
        });
        return { asignados: l, cantidad: n }
    }

    function onDataChanged(evt) {
        if (evt.action === "itemchange") {
            var data = $scope.cuadernasDs.data();
            angular.forEach(data, function (value, key) {
                var s = sumCuaderna(value.Orden);

                value.Seleccionados = s.cantidad;
                value.Asignado = s.asignados;
                value.Total = value.Capacidad - s.asignados;
            });
        }
    }

    $scope.noEdit = function (container, options) {
        var l = $("<span/>");
        l.text(options.model[options.field]);
        l.appendTo(container);
    }


    $scope.noEditTotal = function (container, options) {
        var l = $("<span/>");
        l.text(options.model["Cantidad"] + options.model["Ajuste"]);
        l.appendTo(container);
    }

    $scope.cuadernaEditor = function (container, options) {
        var l = $("<input kendo-drop-down-list required k-data-text-field=\"'Orden'\" k-data-value-field=\"'Orden'\" k-data-source=\"cuadernasDs\" data-bind=\"value:" + options.field + '"/>');
        l.appendTo(container);
    }

    $scope.productosGridOptions =
    {
        columns: [
            { field: "Id", hidden: true },
            { field: "OrderId", title: "Pedido", editor: $scope.noEdit, width: "10em" },
            { field: "Insumo", title: "Producto", editor: $scope.noEdit },
            { field: "Cantidad", title: "Litros", editor: $scope.noEdit, width: "10em" },
            { field: "Cuaderna", title: "Cuaderna", editor: $scope.cuadernaEditor },
            { field: "Ajuste", title: "Ajuste", width: "10em" },
            { field: "Ajuste", title: "Total", template: "#= data.Cantidad + data.Ajuste #", editor: $scope.noEditTotal },
        ],
        editable: {
            update: true,
            destroy: false
        }
    };

    $scope.cuadernasGridOptions =
    {
        columns:
     [
         { field: "Orden", title: "" },
         { field: "Descripcion", title: "" },
         { field: "Capacidad", title: "Capacidad" },
         { field: "Seleccionados", title: "N°" },
         { field: "Asignado", title: "Asignado" },
         { field: "Disponible", title: "Disponible", template: "#= data.Capacidad - (data.Asignado?data.Asignado:0) #" },
     ],
    };

    $scope.ok = function () {
        $log.debug("ok");
        //$uibModalInstance.close();

        var selectOrders = [];
        //$scope.ordenesGrid.select().each(function (index, row) {
        //    selectOrders.push($scope.ordenesGrid.dataItem(row));
        //});

        $scope.newOrder = new OrdenesService.ordenes();
        $scope.newOrder.OrderList = selectOrders;
        $scope.newOrder.OrderDetailList = $scope.productsSelected.toJSON();
        $scope.newOrder.IdVehicle = $scope.$parent.order.Vehicle.Key;
        $scope.newOrder.IdVehicleType = $scope.vehicleTypeSelected.Id;
        $scope.newOrder.StartDateTime = $scope.$parent.order.StartDateTime;
        $scope.newOrder.LogisticsCycleType = $scope.$parent.order.LogisticsCycleType.Key;
        $scope.newOrder.$save(
            { distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key },
            function () {
                //$scope.onBuscar();
                //$('#myModal').modal('hide');
                //$scope.disabledButton = false;
            },
            function () {}
                //onFail
        );
            }

    $scope.cancel = function () {
        $log.debug("cancel");
        //$uibModalInstance.dismiss();
    }


}