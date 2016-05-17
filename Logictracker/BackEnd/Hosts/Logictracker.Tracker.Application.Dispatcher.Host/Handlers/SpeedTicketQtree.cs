using System;
using System.Collections.Concurrent;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Utils;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{
    class SpeedTicketQtree : ChainHandler, IMessageHandler<Position>
    {
        private readonly VehicleManager _vehicleManager;

        public HandleResults HandleMessage(Position message)
        {
            var state = _vehicleManager.GetVehicleState(message.DeviceId,message.GeoPoints.First().Date);

            if (state == null) return HandleResults.BreakSuccess;

            message.GeoPoints.ForEach(p => state.Process(p)); 
            
            _vehicleManager.UpdateVehicleState(state);

            return HandleResults.BreakSuccess;
        }

        public SpeedTicketQtree(ChainHandler nextHandler) : base(nextHandler)
        {
            _vehicleManager = new VehicleManager(new DAOFactory());
        }

        public override HandleResults HandleMessage(IMessage message)
        {
            try
            {

                if (message == null) throw new ArgumentNullException("message");

                var p = message as Position;

                return p != null ? HandleMessage(p) : HandleResults.BreakSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return HandleResults.SilentlyDiscarded;
            }
        }

        class VehicleManager
        {
            private readonly IDAOFactory _daoFactory;
            readonly ConcurrentDictionary<int,VehicleState> _vehicleStates = new ConcurrentDictionary<int, VehicleState>();

            public VehicleManager(IDAOFactory daoFactory)
            {
                _daoFactory = daoFactory;
            }

            public VehicleState GetVehicleState(int deviceId, DateTime dateTime)
            {
                return _vehicleStates.GetOrAdd(deviceId, i =>
                {
                    var coche = _daoFactory.CocheDAO.FindMobileByDevice(deviceId);
                    
                    if (coche == null)
                        return null;
                    
                    var device =_daoFactory.DispositivoDAO.FindById(deviceId);
                    
                    var position = _daoFactory.LogPosicionDAO.GetFirstPositionNewerThanDate(coche.Id,dateTime,1) ??
                                   new LogPosicion(new GPSPoint {Date = dateTime}, coche);

                    return VehicleState.Create(position,device);
                });
            }

            public void UpdateVehicleState(VehicleState state)
            {
                if (state == null) throw new ArgumentNullException("state");
                _vehicleStates.AddOrUpdate(state.DeviceId, state, (i, vehicleState) => vehicleState);
            }

        }

    }
}
