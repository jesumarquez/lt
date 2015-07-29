using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicTracker.App.Web.Api.Models
{
    public class OrderJob
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public Location Location { get; set; }
        public DateTime Programmed { get; set; }
        public DateTime ProgrammedTo { get; set; }
        public DateTime? ManualDayTime { get; set; }
        public DateTime? EntranceDateTime { get; set; }
        public DateTime? ExitDateTime { get; set; }
        public int State { get; set; }
        public DateTime? ReceptionDateTime { get; set; }
        public DateTime? ReadingConfirmationDateTime { get; set; }
        public Message ConfirmationMessage { get; set; }
        public DateTime? EntranceOrExclusiveManual { get; set; }
        public DateTime? ExitOrExclusiveManual { get; set; }
        public DateTime ManualOrEntrance { get; set; }
        public DateTime ManualOrExit { get; set; }
        public DateTime? GarminEta { get; set; }
        public DateTime? GarminETAInformedAt { get; set; }
        public DateTime? GarminReadInactiveAt { get; set; }
        public DateTime? GarminUnreadInactiveAt { get; set; }
    }

    public class Order
    {
        public String Code { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? RealStartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public OrderJob[] OrderJobs { get; set; }
    }
}