'use strict';

angular
    .module('vrp', ["ngResource"])
    .factory('vrpService',
    ['$resource', '$log',
        function ($resource, $log, Problema, VrpSolucion) {
            var baseUrl = "http://190.111.252.242:8089";
            var service = {};

            service.getExample = function() {
                var res = $resource(baseUrl + "/example/route");
                return res.get().$promise.then(
                    function(result){
                        return Problema.buid(result);
                    }
                );
            };
            
            service.newRoute = function (problema) {
                var res = $resource(baseUrl + "/route" );
                return res.save(problema).$promise.then(
                    function (result) {
                        return VrpSolucion.buid(result);
                    }
                );
            }
            
            service.getRoute = function (uid)
            {
                var res = $resource(baseUrl + "/route/" + uid  );
                return res.get().$promise.then(
                    function (result) {
                        return VrpSolucion.buid(result);
                    }
                );
            }
        
            return service;
        }])
    .factory('Costo', function() {

        function Costo(fijo, distancia, tiempo) {
            this.fijo = fijo;
            this.distancia = distancia;
            this.tiempo = tiempo;
        };

        Costo.build = function(data) {
            return new Costo(data.fijo, data.distancia, data.tiempo);
        }

        return Costo;
    })
    .factory('TipoVehiculo', function(Costo) {

        function TipoVehiculo(id, capacidad, costo) {
            this.id = id;
            this.capacidad = capacidad;
            this.costo = costo;
        };

        TipoVehiculo.build = function(data) {
            return new TipoVehiculo(data.id, data.capacidad, Costo.build(data.costo));
        }

        return TipoVehiculo;
    })
    .factory('Coordenada', function() {

        function Coordenada(x, y) {
            this.x = x;
            this.y = y;
        };

        Coordenada.build = function(data) {
            return new Coordenada(data.x, data.y);
        }

        return Coordenada;
    })
    .factory('Ventana', function() {

        function Ventana(inicio, fin) {
            this.inicio = inicio;
            this.fin = fin;
        }

        Ventana.build = function(data) {
            return new Ventana(data.inicio, data.fin);
        }

        return Ventana;
    })
    .factory('Servicio', function(Coordenada, Ventana) {
        //new Servicio("serv2",new Coordenada(-34.1,-58.9),20,0, new Ventana(36000,46800));    
        function Servicio(id, coordenada, demanda, duracion, ventana) {
            this.id = id;
            this.coordenada = coordenada;
            this.demanda = demanda;
            this.duracion = duracion;
            this.ventanas = [ventana]
        }

        Servicio.build = function(data) {
            return new Servicio(data.id, Coordenada.build(data.coordenada), data.demanda, data.duracion, Ventana.build(data.ventanas[0]));
        }

        return Servicio;
    })
    .factory('Locacion', function(Coordenada) {
        function Locacion(id, coordenada) {
            this.id = id;
            this.coordenada = coordenada;
        }

        Locacion.build = function(data) {
            return new Locacion(data.id, Coordenada.build(data.coordenada))
        }

        return Locacion;
    })

    .factory('Vehiculo', function(Locacion, Ventana) {
        function Vehiculo(id, tipo_vehiculo_id, locacion, tiempo_programado) {
            this.id = id;
            this.tipo_vehiculo_id = tipo_vehiculo_id;
            this.locacion = locacion;
            this.tiempo_programado = tiempo_programado;
        }

        Vehiculo.build = function(data) {
            return new Vehiculo(data.id, data.tipo_vehiculo_id, Locacion.build(data.locacion), Ventana.build(data.tiempo_programado));
        }

        return Vehiculo;
    })
    .factory('Problema', function(Servicio, TipoVehiculo, Vehiculo) {
        function Problema() {
            this.vehiculos = [];
            this.servicios = [];
            this.tipo_vehiculos = [];
        }

        Problema.prototype.add_servicio = function(servicio) {
            this.servicios.push(servicio);
        }

        Problema.prototype.add_tipo_vehiculo = function(tipo_vehiculo) {
            this.tipo_vehiculos.push(tipo_vehiculo);
        }

        Problema.prototype.add_vehiculo = function(vehiculo) {
            this.vehiculos.push(vehiculo);
        }

        Problema.build = function(data) {
            var rv = new Problema();

            angular.forEach(data.servicios, function(value) { rv.add_servicio(Servicio.buil(value)) })
            angular.forEach(data.tipo_vehiculos, function(value) { rv.add_tipo_vehiculo(TipoVehiculo.buil(value)) })
            angular.forEach(data.vehiculos, function(value) { rv.add_vehiculo(Vehiculo.buil(value)) })

            return rv;
        }

        return Problema;
    })
    .factory('Solucion', function(Ruta, Servicio) {
        function Solucion(costo) {
            this.costo = costo;
            this.rutas = [];
            this.sinAsignar = [];
        }

        Solucion.prototype.add_ruta(ruta)
        {
            this.rutas.push(ruta);
        }

        Solucion.prototype.add_sin_asignar(servicio)
        {
            this.sinAsignar.push(servicio);
        }

        Solucion.build = function(data) {
            var rv = new Solucion(data.costo);
            angular.forEach(data.rutas, function(value, key) { rv.add_ruta(Ruta.build(value)); });
            angular.forEach(data.sinAsignar, function(value, key) { rv.add_sin_asignar(Servicio.build(value)); });
        }

        return Solucion;
    })
    .factory('Ruta', function(Acto) {
        function Ruta(idVehiculo, inicio, fin) {
            this.idVehiculo = idVehiculo;
            this.inicio = inicio;
            this.fin = fin;
            this.actos = [];
        }

        Ruta.prototype.add_acto = function(acto) {
            this.actos.push(acto);
        }

        Ruta.build = function(data) {
            var rv = new Ruta(data.idVehiculo, data.inicio, data.fin);
            angular.forEach(data.actos, function(value) { rv.add_acto(Acto.build(value)); });
            return rv;
        }

        return Ruta;
    })
    .factory('Acto', function() {
        function Acto(tipo, idServicio, inicio, fin) {
            this.tipo = tipo;
            this.idServicio = idServicio;
            this.inicio = inicio;
            this.fin = fin;
        }

        Acto.prototype.build = function(data) {
            return new Acto(data.tipo, data.idServicio, data.inicio, data.fin);
        }

        return Acto;
    })
    .factory('VrpSolucion',function(Problema,Solucion){
        
        function VrpSolucion(uid,status,message)
        {
            this.uid = uid;
            this.status = status;
            this.message = message;
        }
        
        VrpSolucion.buid = function(data)
        {
            var rv = new VrpSolucion(data.uid,data.status,data.message);
            
            rv.problema = Problema.buid(data.problema);
            
            if (data.status == 1)
                rv.solution = Solucion.build(data.solution);
            
            return rv;
        }
        
        VrpSolucion.prototype.isFinalResult = function()
        {
            return this.status == 1;
        }
        
        return VrpSolucion;
    })