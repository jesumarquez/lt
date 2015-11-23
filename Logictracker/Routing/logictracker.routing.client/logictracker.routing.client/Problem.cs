using System;
using System.Collections.Generic;
using System.Configuration;

namespace Logictracker.Routing.Client
{
    public class Problem
    {
        public ProblemType ProblemType { get; set; }
        public List<Vehicle> Vehicles { get; set; } 
        public List<VehicleType> VehicleTypes { get; set; }
        public List<Service> Services { get; set; }
        public List<Shipment> Shipments { get; set; }
        public List<Route> InitialRoutes { get; set; }
        public List<Solution> Solutions { get; set; } 
        
        public Problem()
        {
            VehicleTypes = new List<VehicleType>();   
            Vehicles = new List<Vehicle>();
        }
        
        public void AddVehicleType(string vehicleTypeName, int index, string value)
        {
            var vehicleType = new VehicleType(vehicleTypeName);
            vehicleType.AddCapacityDimension(index, value);
            VehicleTypes.Add(vehicleType);
        }
    }
}
