using System;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.QuadTree.Data;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{
    class VehicleState
    {
        public int DeviceId { get; private set; }
        private IVehicleState _vhState;
        private readonly byte[] _excesos = new byte[17];
        private readonly byte[] _infraccion = new byte[17];
        private readonly string _qtreeKey;
        private ulong nextMsgId = 1;
        private Repository QtreeRepository { get { return QtreeInstanceManager.Get(_qtreeKey); } }

        internal ulong NextMsgId
        {
            get { return nextMsgId++; }
        }

        private VehicleState(LastPosition position, Dispositivo dispositivo)
        {
            DeviceId = dispositivo.Id;

            for (var i = 0; i < 16; i++)
            {
                _infraccion[i] = (byte)dispositivo.GetInfraccionNivel(i);
                _excesos[i] = (byte)dispositivo.GetExcesoNivel(i);
            }

            _infraccion[16] = (byte)dispositivo.GetInfraccionNivel(-1);
            _excesos[16] = (byte)dispositivo.GetExcesoNivel(-1);

            _qtreeKey = dispositivo.GetQtreeType() + "|" + dispositivo.GetQtreeFile();

            _vhState = new VechicleNormal(position);
        }


        internal static VehicleState Create(LogPosicion position, Dispositivo dispositivo)
        {
            return new VehicleState(LastPosition.Create(position), dispositivo);
        }
        
        public void Process(GPSPoint gpsPoint)
        {
            _vhState.Evaluate(this, gpsPoint);
        }

        private void SetState(IVehicleState newState)
        {
            _vhState = newState;
        }

        #region estado
        interface IVehicleState
        {
            void Evaluate(VehicleState vehicleState, GPSPoint point);
        }
        #endregion

        #region Normal
        private class VechicleNormal : IVehicleState
        {
            protected LastPosition LastPosition;

            public VechicleNormal(LastPosition lastPosition)
            {
                LastPosition = lastPosition;
            }

            public virtual void Evaluate(VehicleState vehicleState, GPSPoint point)
            {
                // Actualizo la ultima posicion
                if (point.Date > LastPosition.Fecha)
                    LastPosition = LastPosition.Create(point);
                else
                    return;


                var qLevel = vehicleState.QtreeRepository.GetPositionClass(point.Lat, point.Lon);

                var velocidadExceso = vehicleState._infraccion[qLevel];

                
                if (point.Velocidad > velocidadExceso)
                {
      //              Console.WriteLine("[0] velo: {0}  exce:{1}  qlevel:{2}", point.Velocidad, velocidadExceso, qLevel);

                    // Inicia el exceso de velocidad
                    var estadoExceso = new VehicleEnInfraccion(LastPosition, velocidadExceso);
                    vehicleState.SetState(estadoExceso);
                    return;
                }
                // Sigue en estado normal
                vehicleState.SetState(this);
            }
        }
        #endregion

        #region Infraccion
        private class VehicleEnInfraccion : VechicleNormal
        {
            private readonly LastPosition _inicioExceso;
            private int _velocidadInfraccion;
            private int _velocidadAlcanzada;

            public override void Evaluate(VehicleState vehicleState, GPSPoint point)
            {
                if (point.Date >= LastPosition.Fecha)
                    LastPosition = LastPosition.Create(point);
                else
                {
                    if (point.Velocidad >= LastPosition.Velocidad)
                    {
                        
                    }

                    Console.WriteLine("Posicion descartada {0}  vs {1}", point.Date,LastPosition.Fecha);
                    return;
                }
              
                var qLevel = vehicleState.QtreeRepository.GetPositionClass(point.Lat, point.Lon);

                _velocidadInfraccion = Math.Min(_velocidadInfraccion, vehicleState._infraccion[qLevel]);
                _velocidadAlcanzada = Math.Max(_velocidadAlcanzada, point.Velocidad);

                if (_velocidadInfraccion > point.Velocidad)
                {
                    // Fin de infracccion
                    var normal = new VechicleNormal(LastPosition);
                    OnFinInfraccion(vehicleState);
                    vehicleState.SetState(normal);
                    return;
                }

                // Continua en Infraccion
                vehicleState.SetState(this);
            }

            private void OnFinInfraccion(VehicleState vehicleState)
            {

                var duracion = LastPosition.Fecha.Subtract(_inicioExceso.Fecha);

                if (duracion.TotalSeconds < 15)
                    return;

                var strMsg = string.Format
                    (": Permitida {0} - Alcanzada {1} - Duracion: {2}", 
                    Convert.ToInt32(_velocidadInfraccion), 
                    Convert.ToInt32(_velocidadAlcanzada), 
                    duracion );
                
                var speedTicket = new SpeedingTicket(vehicleState.DeviceId,
                    vehicleState.NextMsgId ,
                    _inicioExceso.ToGpsPoint(),
                    LastPosition.ToGpsPoint(),
                    _velocidadAlcanzada, _velocidadInfraccion,
                    string.Empty);

                var dao = new DAOFactory();

                var dispositivo = dao.DispositivoDAO.FindById(vehicleState.DeviceId);
                var coche = dao.CocheDAO.FindMobileByDevice(vehicleState.DeviceId);
                Empleado chofer = null;
                Zona zona = null;

                var messageSaver = new MessageSaver(dao);

                var evento = messageSaver.Save(speedTicket,
                    MessageCode.SpeedingTicket.GetMessageCode(),
                    dispositivo,
                    coche, 
                    chofer, 
                    _inicioExceso.Fecha,
                    _inicioExceso.ToGpsPoint(),
                    LastPosition.ToGpsPoint(), 
                    strMsg, _velocidadInfraccion, 
                    _velocidadAlcanzada, 
                    null, zona);

                var infraccion = new Infraccion
                {
                    Vehiculo = coche,
                    Alcanzado = _velocidadAlcanzada,
                    CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
                    Empleado = evento.Chofer,
                    Fecha = _inicioExceso.Fecha,
                    Latitud = _inicioExceso.Latitud,
                    Longitud =_inicioExceso.Longitud,
                    FechaFin = LastPosition.Fecha,
                    LatitudFin = LastPosition.Latitud,
                    LongitudFin = LastPosition.Longitud,
                    Permitido = _velocidadInfraccion,
                    Zona = zona,
                    FechaAlta = DateTime.UtcNow
                };

                dao.InfraccionDAO.SaveOrUpdate(infraccion);
                
                Console.WriteLine(strMsg);
            }

            //private HandleResults ProcessVelocidadExcedidaEvent(SpeedingTicket velocidadExcedida)
            //{
            //    var texto = GetVelocidadExcedidaText(velocidadExcedida);
            //    var chofer = GetChofer(velocidadExcedida.GetRiderId());
            //    var velocidadPermitida = Convert.ToInt32(velocidadExcedida.SpeedLimit);
            //    var velocidadAlcanzada = Convert.ToInt32(velocidadExcedida.SpeedReached);
            //    var estado = GeocercaManager.CalcularEstadoVehiculo(Coche, inicio, DaoFactory);
            //    var zona = estado != null && estado.ZonaManejo != null && estado.ZonaManejo.ZonaManejo > 0
            //            ? DaoFactory.ZonaDAO.FindById(estado.ZonaManejo.ZonaManejo) : null;

            //    var evento = MessageSaver.Save(velocidadExcedida, MessageCode.SpeedingTicket.GetMessageCode(), Dispositivo, Coche, chofer, inicio.Date, inicio, velocidadExcedida.EndPoint, texto, velocidadPermitida, velocidadAlcanzada, null, zona);

            //    var infraccion = new Infraccion
            //    {
            //        Vehiculo = Coche,
            //        Alcanzado = velocidadAlcanzada,
            //        CodigoInfraccion = Infraccion.Codigos.ExcesoVelocidad,
            //        Empleado = evento.Chofer,
            //        Fecha = inicio.Date,
            //        Latitud = inicio.Lat,
            //        Longitud = inicio.Lon,
            //        FechaFin = velocidadExcedida.EndPoint.Date,
            //        LatitudFin = velocidadExcedida.EndPoint.Lat,
            //        LongitudFin = velocidadExcedida.EndPoint.Lon,
            //        Permitido = velocidadPermitida,
            //        Zona = zona,
            //        FechaAlta = DateTime.UtcNow
            //    };

            //    DaoFactory.InfraccionDAO.Save(infraccion);

            //    return HandleResults.Success;
            //}

            public VehicleEnInfraccion(LastPosition lastPosition, byte velocidadInfraccion)
                : base(lastPosition)
            {
                _inicioExceso = LastPosition.Create(lastPosition);
                _velocidadAlcanzada = lastPosition.Velocidad;
                _velocidadInfraccion = velocidadInfraccion;
            }
        }
        #endregion
    }
}
