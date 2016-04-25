﻿angular
    .module("logictracker.ordenes.controller", ["kendo.directives", "ngAnimate", 'openlayers-directive', "vrp"])
    .controller("OrdenesController", ["$scope", "$log", "EntitiesService", "OrdenesService", "UserDataInfo", OrdenesController])
    .controller('OrdenesAsignarController', ["$scope", "$log", "EntitiesService", "OrdenesService", "$filter", OrdenesAsignarController])
    .controller('OrdenesAsignarAutoController', [
        "$scope",
        "$timeout",
        "$interval",
        "EntitiesService",
        "vrpService",
        "Servicio",
        "Coordenada",
        "Locacion",
        "Ventana",
        "Vehiculo",
        "Problema",
        "TipoVehiculo",
        "Costo",
        "OrdenesService",
        OrdenesAsignarAutoController]);

function OrdenesController($scope, $log, EntitiesService, OrdenesService, UserDataInfo) {

    $scope.accessor = {};

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
            { field: "CodigoPuntoEntrega", title: "Código Entrega" },
            { field: "PuntoEntrega", title: "Razón Social" },
            { field: "CodigoPedido", title: "Código Pedido" },
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
        function onOrdersDSLoad(e) {
            if (e.type === "read" && e.response) {
                $scope.olMap.markers = e.response.Data.map(function (o) {
                    return {
                        name: o.Id,
                        lat: o.PuntoEntregaLatitud,
                        lon: o.PuntoEntregaLongitud,
                        label: {
                            message: '<span>' + o.PuntoEntrega + '</span>',
                            show: false,
                            showOnMouseOver: true
                        },
                        dataItem: o,
                        onClick: function (event, properties) {
                            $scope.orderSelected = properties.dataItem;
                            $("#markerModal").modal('toggle');
                        }
                    }
                });
            }
        }
    };

    $scope.disabledButton = true;

    $scope.getOrden = function(id){
        var orden = $scope.Orders.data().find(function (item) {
            return item.Id === id;
        });
        return orden;
    }
}

function OrdenesAsignarController($scope, $log, EntitiesService, OrdenesService, $filter) {

    $scope.vehicleTypeSelected = {};
    $scope.ds = new kendo.data.DataSource({
        data: $scope.productsSelected,
        change: onDataChanged
    });
    $scope.productosGridOptions =
    {
        columns: [
            { field: "Id", hidden: true },
            { field: "ClienteDescripcion", title: "Cliente", width: "15em" },
            { field: "ClienteLocalidad", title: "Localidad", width: "10em" },
            { field: "OrderId", title: "Pedido", width: "10em" },
            { field: "Insumo", title: "Producto" },
            { field: "Cantidad", title: "Litros", width: "10em" },
            { field: "Cuaderna", title: "Cuaderna", editor: cuadernaEditor, template: "{{ getCuadernaDesc(dataItem) }}" },
            { field: "Ajuste", title: "Ajuste", width: "10em" },
            { field: "Ajuste", title: "Total", template: "{{ dataItem.Cantidad + dataItem.Ajuste }}" },
        ],
        editable: {
            update: true,
            destroy: false
        }
    };
    $scope.cuadernasDs = {};
    $scope.cuadernasGridOptions =
    {
        columns:
             [
                 { field: "Orden", title: "" },
                 { field: "Descripcion", title: "" },
                 { field: "Capacidad", title: "Capacidad" },
                 { field: "Seleccionados", title: "N°" },
                 { field: "Asignado", title: "Asignado" },
                 { field: "Asignado", title: "Disponible", template: "<span ng-class='semaforo(dataItem)'>{{ dataItem.Capacidad - (dataItem.Asignado?dataItem.Asignado:0)}}</span>" },
             ],
    };

    $scope.semaforo = function (dataItem) {
        if (dataItem.Orden === 0) return "";
        return dataItem.Capacidad - (dataItem.Asignado ? dataItem.Asignado : 0) < 0 ? "tex-red" : "";
    }

    $scope.$watch("vehicleTypeSelected", onvehicleTypeSelected);

    function cuadernaEditor(container, options) {
        var l = $("<input kendo-drop-down-list required k-data-text-field=\"'Descripcion'\" k-data-value-field=\"'Orden'\" k-data-source=\"cuadernasDs\" data-bind=\"value:" + options.field + '"/>');
        l.appendTo(container);
    }

    function onvehicleTypeSelected(newValue, oldValue) {
        if (newValue != null && newValue !== oldValue) {
            $scope.cuadernasDs = new kendo.data.DataSource({
                data: newValue.Contenedores
            });

            cleanEditableProducts();
        }
    }

    function cleanEditableProducts() {
        // Limpio si hay algo ya editado
        $scope.productsSelected.forEach(
            function (o) {
                o.set('Cuaderna', 0);
                o.set('Ajuste', 0);
            });
    }

    function onDataChanged(evt) {
        if (evt.action === "itemchange" || evt.action === "remove") {
            var data = $scope.cuadernasDs.data();

            var disabledProgramar = true;

            data.forEach(function (cuaderna) {
                var items = $scope.ds.data();
                var asignado = 0;
                var seleccionados = 0;

                items.forEach(function (item) {
                    if(item.Cuaderna === cuaderna.Orden){
                        asignado += item.Cantidad + item.Ajuste;
                        seleccionados += 1;
                    }

                    if (item.Cuaderna != 0) disabledProgramar = false;
                });                

                cuaderna.set('Asignado', asignado);
                cuaderna.set('Seleccionados', seleccionados);
            });

            $scope.$parent.disabledButton = disabledProgramar;
        }
    }


    $scope.getCuadernaDesc = function (data) {
        return $filter('filter') ($scope.cuadernasDs.data(), { Orden : data.Cuaderna  })[0].Descripcion;
    }

    function onFail(error) {
        try {
            $scope.$parent.disabledButton = false;

            if (error.data.ExceptionMessage) {
                $scope.modalNotify.show(error.data.ExceptionMessage, "error");
                return;
            }
        } catch (x) {
        }
        $scope.modalNotify.show(error.errorThrown, "error");
    };

    $scope.ok = function () {
       
        if ($scope.$parent.disabledButton) return;

        $scope.$parent.disabledButton = true;

        var selectOrders = [];

        $scope.newOrder = new OrdenesService.ordenes();
        $scope.newOrder.OrderList = selectOrders;

        // Obtener solo los productos que tienen cuaderna seleccionada
        var productsSelectedAssigned = new Array();
        $scope.productsSelected.forEach(function (item)
        { if (item.Cuaderna > 0) productsSelectedAssigned.push(item.toJSON()); });

        $scope.newOrder.OrderDetailList = productsSelectedAssigned;
        $scope.newOrder.IdVehicle = $scope.$parent.order.Vehicle.Key;
        $scope.newOrder.IdVehicleType = $scope.vehicleTypeSelected.Id;
        $scope.newOrder.StartDateTime = $scope.$parent.order.StartDateTime;
        $scope.newOrder.LogisticsCycleType = $scope.$parent.order.LogisticsCycleType.Key;
        $scope.newOrder.$save(
            { distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key },
            function (value) {
                onSuccess();
            },
            function (error) { onFail(error); }
        );
    }

    function onSuccess()
    {
        $('#myModal').modal('hide');

        if ($scope.accessor.invoke)
            $scope.accessor.invoke();
            
        $scope.onBuscar();
    }

    $scope.cancel = function () {
        $log.debug("cancel");
        //cleanEditableProducts();
        //$uibModalInstance.dismiss();
    }

    $scope.clean = function () {
        cleanEditableProducts();
    }

}

function OrdenesAsignarAutoController(
    $scope,
    $timeout,
    $interval,
    EntitiesService,
    vrpService,
    Servicio,
    Coordenada,
    Locacion,
    Ventana,
    Vehiculo,
    Problema,
    TipoVehiculo,
    Costo,
    OrdenesService)
{
    $scope.tipoCocheSelected = {};
    $scope.asignarDisabledButton = false;

    $scope.sortedProductsGridOptions = {
        columns: [
                { field: "NumRuta", title: "Ruta" },
                { field: "OrdenRuta", title: "Orden" },
                { field: "ClienteDescripcion", title: "Cliente", width: "15em"},
                { field: "ClienteLocalidad", title: "Localidad", width: "10em" },
                { field: "Insumo", title: "Producto" },
                { field: "Cantidad", title: "Litros" },
                { field: "EstadoDescripcion", title: "Estado" }
        ],
        // Template para pintar el row cuando no tiene definido una ruta y un orden
        rowTemplate: '<tr style="#:NumRuta==undefined? \"background-color:\\#ffdad8;\":\"\"#" data-uid="#= uid #">' +
        '<td>#:NumRuta==undefined?\'\':NumRuta#</td><td>#: OrdenRuta==undefined?\'\':OrdenRuta #</td>'+
        '<td>#:ClienteDescripcion #</td><td>#:ClienteLocalidad #</td><td>#:Insumo #</td><td>#:Cantidad #</td>'+
        '<td>#:EstadoDescripcion #</td></tr>'
    };

    $scope.rutear = rutear;
    $scope.sortedProducts = $scope.productsSelected;

    // Array de los productos ya ruteados
    $scope.solution = [];

    var getRoutePromise = null; //promise de getRoute. se usa para cancelar las peticiones a getRoute.

    function rutear() {

        var problema = new Problema();
        var capacidad = sumCapacidadCuadernas();
        var tVeh = new TipoVehiculo($scope.tipoCocheSelected.Id, capacidad, new Costo(300, 1, 1));
        // Se consideran las coordenadas de la base
        var coordVeh = new Coordenada($scope.baseSelected.Latitud, $scope.baseSelected.Longitud);
        var vehiculo = new Vehiculo("V1", $scope.tipoCocheSelected.Id, new Locacion("0", coordVeh), new Ventana(28800, 61200));

        $.each($scope.productsSelected, function (index, item) {

            var orden = $scope.getOrden(item.OrderId);

            var coordSrv = new Coordenada(orden.PuntoEntregaLatitud, orden.PuntoEntregaLongitud);

            var srv = new Servicio(item.Id, coordSrv, item.Cantidad, 0, new Ventana(36000, 46800));

            problema.add_servicio(srv);
        });

        problema.add_vehiculo(vehiculo);
        problema.add_tipo_vehiculo(tVeh);

        var cancelGetRoutePromise = $timeout(cancelGetRoute, 10000, false, getRoutePromise); //cancela el getRoute luego de 10 seg.

        //intenta obtener la ruta
        vrpService.newRoute(problema).then(function (res) {
            
            //valida si hay una solucion
            if (angular.isUndefined(res.status) || res.status !== 1) {
                //intenta 5 veces obtener la ruta cada segundo. 
                getRoutePromise = $interval(getRoute, 1000, 10, false, { svc: vrpService, uid: res.uid, cancelGetRoutePromise: cancelGetRoutePromise });
            }
            else {
                sortProducts(res);
                //cancela la el timeout
                $timeout.cancel(cancelGetRoutePromise);
            }
        });

    }

    //Consula el estado del problema
    function getRoute(data) {

        data.svc.getRoute(data.uid).then(function (res) {
            if (res.status === 1) {
                sortProducts(res);
                $timeout.cancel(data.cancelGetRoutePromise);
            }
        })
    }

    //Cancela la consulta del problema
    function cancelGetRoute(getRoutePromise) {
        if (getRoutePromise !== null) {
            $interval.cancel(getRoutePromise);
        }
        console.log('cancel getroute');
    }

    function sumCapacidadCuadernas() {
        var total = 0;
        if ($scope.tipoCocheSelected !== null && $scope.tipoCocheSelected.Contenedores != null) {
            $scope.tipoCocheSelected.Contenedores.forEach(function (cuaderna) {
                total += cuaderna.Capacidad;
            });
        }
        return total;
    }

    function sortProducts(vrpSolucion) {

        var rutas = vrpSolucion.solution.rutas;

        rutas.forEach(function (ruta, iR) {
            var actos = ruta.actos;
            var iRuta = iR;
            actos.forEach(function (acto, iActo) {

                var p = $scope.sortedProducts.find(function (prod) {
                    return prod.Id == acto.idServicio;
                });

                if (angular.isDefined(p)) {
                    p.OrdenRuta = iActo;
                    p.NumRuta = iRuta;

                    // Por el momento se asigna a todos los productos la cuaderna 1
                    p.Cuaderna = 1;
                }
            });
        })

        // Valida solo los productos que hayan sido ruteados
        var s = $scope.sortedProducts.toJSON().sort(compareRoute);
        s.forEach(function (item) {
            if (angular.isDefined(item.NumRuta) && angular.isDefined(item.OrdenRuta))
                $scope.solution.push(item);
        });

        // Ordena la grilla por Ruta y Orden
        $('#sortedProductsGrid').data('kendoGrid').dataSource.sort([{ field: 'NumRuta', dir: 'asc' }, { field: 'OrdenRuta', dir: 'asc' }]);
    }

    function compareRoute(a, b) {
        if (a.NumRuta > b.NumRuta) {
            return 1;
        }
        if (a.NumRuta < b.NumRuta) {
            return -1;
        }
        if (a.NumRuta === b.NumRuta) {
            if (a.OrdenRuta > b.OrdenRuta) {
                return 1;
            }
            if (a.OrdenRuta < b.OrdenRuta) {
                return -1;
            }
            return 0;
        }
        // a must be equal to b
        return 0;
    }

    $scope.programar = function ()
    {
        var newOrder = new OrdenesService.ordenes();

        newOrder.OrderDetailList = $scope.solution;
        newOrder.IdVehicle = -2; // Ninguno
        newOrder.LogisticsCycleType = -2; // Ninguno
        newOrder.IdVehicleType = $scope.tipoCocheSelected.Id;
        newOrder.StartDateTime = $scope.$parent.order.StartDateTime;
        newOrder.$save(
            { distritoId: $scope.distritoSelected.Key, baseId: $scope.baseSelected.Key },
            function (value) {
                onSuccess();
            },
            function (error) { onFail(error); }
        );
    }

    function onSuccess()
    {
        $scope.solution = [];

        $('#modalAuto').modal('hide');

        if ($scope.accessor.invoke)
            $scope.accessor.invoke();

        $scope.onBuscar();
    }

    function onFail(error)
    {
        if (error.errorThrown)
            $scope.notify.show(error.errorThrown, "error");
        else
            $scope.notify.show(error.statusText, "error");
    }
}