using System;
using System.Collections.Concurrent;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{
    class SpeedTicketQtree : ChainHandler, IMessageHandler<Position>
    {
        private readonly VehicleManager _vehicleManager;

        public HandleResults HandleMessage(Position message)
        {
            var state = _vehicleManager.GetVehicleState(message.DeviceId);

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
            if (message == null) throw new ArgumentNullException("message");

            var p = message as Position;

          

            return p != null ? HandleMessage(p) : HandleResults.BreakSuccess;
        }

        class VehicleManager
        {
            private readonly IDAOFactory _daoFactory;
            readonly ConcurrentDictionary<int,VehicleState> vehicleStates = new ConcurrentDictionary<int, VehicleState>();

            public VehicleManager(IDAOFactory daoFactory)
            {
                _daoFactory = daoFactory;
            }

            public VehicleState GetVehicleState(int deviceId)
            {
                return vehicleStates.GetOrAdd(deviceId, i =>
                {
                    var coche = _daoFactory.CocheDAO.FindMobileByDevice(deviceId);
                    if (coche == null)
                        return null;
                    var position = _daoFactory.LogPosicionDAO.GetLastVehiclePosition(coche);
                    return VehicleState.Create(position);
                });
            }

            public VehicleState UpdateVehicleState( VehicleState state)
            {
                if (state == null) throw new ArgumentNullException("state");
                return vehicleStates.AddOrUpdate(state.DeviceId, state, (i, vehicleState) => vehicleState);
            }
        }

    }
}
