using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LogicTracker.App.Web.Api.Controllers;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace LogicTracker.App.Web.Api.Models
{
    public class MessageType
    {
        public string Code { get; set; }
        public int type { get; set; }
        public string Description { get; set; }
    }

    public class Profile
    {
        public int MobileId { get; set; }
        public List<MessageType> Messages { get; set; }
    }

    public class Job
    {
        public string ClientName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string EndDate { get; set; }
        public int Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int OrderNumber { get; set; }
        public int Quantity;
        public string StartDate { get; set; }  
        public int State { get; set; }  
        public float Value;
        public float Volumen;
        public float Weight;
    }

    public class Route
    {
        public String Code { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public Job[] Jobs { get; set; }
    }

    public class Location
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class RouteState
    {
        public int RouteId { get; set; }
        public string RouteCode { get; set; }
        public int RouteStatus { get; set; }
        public JobState[] JobStates { get; set; }
    }

    public class JobState
    {
        public long JobId { get; set; }
        public short JobStatus { get; set; }
        public int MessageId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class RouteEvent
    {
        public string RouteCommand { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class RouteList
    {
        public int CompanyId { get; set; }
        public DateTime DateTime { get; set; }
        public int LineId { get; set; }
        public RouteItem[] RouteItems { get; set; }
    }

    public class RouteItem
    {
        public string Code { get; set; }
        public int DeliveriesNumber { get; set; }
        public int Id { get; set; }
        public string Places { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Status { get; set; }
    }

    public class Message
    {
        public long JobId { get; set; }
        public DateTime DateTime { get; set; }
        public MessageType MessageType { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}